using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class AssetBundleBuilder
{
#if !REDMATCH
	private const string SdkRuntimeAssembly = "Redmatch.MapSDK.Runtime";
	private const string GameRuntimeAssembly = "Assembly-CSharp";

	private static readonly MethodInfo MonoScriptInitMethod =
		FindMonoScriptInitMethod();

	private class MonoScriptState
	{
		public string AssetPath;
		public MonoScript Script;
		public string ClassName;
		public string Namespace;
		public string AssemblyName;
	}

	public static void BuildAssetBundles(MapConfig config, string buildPath)
	{
		if(!Directory.Exists(buildPath))
			Directory.CreateDirectory(buildPath);

		AssetDatabase.RemoveUnusedAssetBundleNames();

		AssetBundleBuild[] builds = GetAssetBuilds(config.bundleName);

		List<MonoScriptState> changedScripts = new List<MonoScriptState>();

		try
		{
			/*
			 * The SDK scripts compile into Redmatch.MapSDK.Runtime,
			 * but the game and old maps expect the scripts
			 * to serialize as Assembly-CSharp.
			 */
			RedirectSdkScriptsToGameAssembly(changedScripts);

			BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ForceRebuildAssetBundle;

			AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildPath, builds, options, BuildTarget.StandaloneWindows64);

			if(manifest == null)
			{
				throw new InvalidOperationException("Unity failed to build the AssetBundles. Check the console for the underlying build error.");
			}
		}
		finally
		{
			// Always restore the package's actual assembly metadata,
			// including when the bundle build throws an exception.
			RestoreSdkScripts(changedScripts);
		}
	}

	private static MethodInfo FindMonoScriptInitMethod()
	{
		MethodInfo[] methods = typeof(MonoScript).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);

		foreach(MethodInfo method in methods)
		{
			if(method.Name != "Init")
				continue;

			ParameterInfo[] parameters =
				method.GetParameters();

			/*
			 * Newer signature:
			 *
			 * Init(
			 *     string className,
			 *     string namespaceName,
			 *     string assemblyName,
			 *     bool isEditorScript
			 * )
			 */
			if(
				parameters.Length == 4 &&
				parameters[0].ParameterType == typeof(string) &&
				parameters[1].ParameterType == typeof(string) &&
				parameters[2].ParameterType == typeof(string) &&
				parameters[3].ParameterType == typeof(bool)
			)
			{
				return method;
			}

			/*
			 * Older signature:
			 *
			 * Init(
			 *     string scriptContents,
			 *     string className,
			 *     string namespaceName,
			 *     string assemblyName,
			 *     bool isEditorScript
			 * )
			 */
			if(
				parameters.Length == 5 &&
				parameters[0].ParameterType == typeof(string) &&
				parameters[1].ParameterType == typeof(string) &&
				parameters[2].ParameterType == typeof(string) &&
				parameters[3].ParameterType == typeof(string) &&
				parameters[4].ParameterType == typeof(bool)
			)
			{
				return method;
			}
		}

		return null;
	}

	private static void RedirectSdkScriptsToGameAssembly(List<MonoScriptState> changedScripts)
	{
		if(MonoScriptInitMethod == null)
		{
			throw new MissingMethodException("Could not find UnityEditor.MonoScript.Init. The AssetBundle compatibility redirect cannot run in this Unity version.");
		}

		string[] assetPaths = AssetDatabase.GetAllAssetPaths();

		foreach(string assetPath in assetPaths)
		{
			if(!assetPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
				continue;

			MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

			if(script == null)
				continue;

			Type scriptType = script.GetClass();

			/*
			 * Plain helper files and scripts that do not represent
			 * a MonoBehaviour or ScriptableObject can return null.
			 *
			 * Those files do not have a MonoScript component identity
			 * that needs to be redirected.
			 */
			if(scriptType == null)
				continue;

			string assemblyName = scriptType.Assembly.GetName().Name;

			if(assemblyName != SdkRuntimeAssembly)
				continue;

			MonoScriptState state =
				new MonoScriptState
				{
					AssetPath = assetPath,
					Script = script,
					ClassName = scriptType.Name,
					Namespace =
						scriptType.Namespace ??
						string.Empty,
					AssemblyName = assemblyName
				};

			/*
			 * Add the state before changing the MonoScript so the
			 * finally block can attempt restoration if the internal
			 * Unity call throws.
			 */
			changedScripts.Add(state);

			SetMonoScriptIdentity(
				state.Script,
				state.ClassName,
				state.Namespace,
				GameRuntimeAssembly
			);
		}

		if(changedScripts.Count == 0)
		{
			Debug.LogError("No MonoScripts from Redmatch.MapSDK.Runtime were found. The bundle may still serialize SDK components with the package assembly name.");
		}
	}

	private static void RestoreSdkScripts(List<MonoScriptState> changedScripts)
	{
		for(int i = changedScripts.Count - 1; i >= 0; i--)
		{
			MonoScriptState state = changedScripts[i];

			try
			{
				SetMonoScriptIdentity(state.Script, state.ClassName, state.Namespace, state.AssemblyName);
			}
			catch(Exception exception)
			{
				Debug.LogError($"Failed to restore MonoScript metadata for {state.AssetPath}:\n{exception}");
			}
		}
	}

	private static void SetMonoScriptIdentity(MonoScript script, string className, string namespaceName, string assemblyName)
	{
		try
		{
			ParameterInfo[] parameters = MonoScriptInitMethod.GetParameters();

			if(parameters.Length == 4)
			{
				MonoScriptInitMethod.Invoke(
					script,
					new object[]
					{
						className,
						namespaceName,
						assemblyName,
						false
					}
				);
			}
			else if(parameters.Length == 5)
			{
				MonoScriptInitMethod.Invoke(
					script,
					new object[]
					{
						script.text,
						className,
						namespaceName,
						assemblyName,
						false
					}
				);
			}
			else
			{
				throw new MissingMethodException("UnityEditor.MonoScript.Init has an unsupported signature.");
			}
		}
		catch(TargetInvocationException exception)
		{
			if(exception.InnerException != null)
				throw exception.InnerException;

			throw;
		}
	}

	private static AssetBundleBuild[] GetAssetBuilds(string bundleName)
	{
		List<string> targetNames = new List<string>() {
			bundleName,
			bundleName + "_scene",
		};

		List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
		foreach(string targetName in targetNames)
		{
			string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(targetName);

			AssetBundleBuild build = new AssetBundleBuild();
			build.assetBundleName = targetName;
			build.assetNames = assets;

			builds.Add(build);
		}

		return builds.ToArray();
	}

	public static void AssignBundleNames(MapConfig config)
	{
		string[] files = Directory.GetFiles(config.GetFullMapDirectory(), "*", SearchOption.AllDirectories);

		string excludedDirectory = Path.Combine(config.GetFullMapDirectory(), "Exclude");
		if(!Directory.Exists(excludedDirectory))
		{
			Directory.CreateDirectory(excludedDirectory);
			AssetDatabase.Refresh();
		}

		string[] excludedFiles = Directory.GetFiles(excludedDirectory, "*", SearchOption.AllDirectories);

		foreach(string file in files)
		{
			if(excludedFiles.Contains(file))
				continue;

			if(file.EndsWith(".meta"))
				continue;

			string extension = Path.GetExtension(file);
			string fileName = Path.GetFileNameWithoutExtension(file) + extension;
			if(fileName == "config.asset")
				continue;

			string localFilePath = "Assets" + file.Substring(Application.dataPath.Length);

			var assetImporter = AssetImporter.GetAtPath(localFilePath);

			if(extension == ".unity")
				assetImporter.assetBundleName = config.bundleName + "_scene";
			else
				assetImporter.assetBundleName = config.bundleName;
		}
	}
#endif
}