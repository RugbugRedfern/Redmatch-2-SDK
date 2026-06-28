using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

#if !REDMATCH
[InitializeOnLoad]
public static class ProjectInit
{
	// yes, you can just change this value. but I recommend you don't.
	// some features of the map may break and not work correctly, depending
	// on the changes I made between versions.
	public const string SDKVersion = "1.1.2";
	private readonly static string[] validVersions = new string[] { "2019.4.17f1", "2019.4.41f1" };
	const string configURL = "https://rugbug.net/redmatch/custom-maps/config.txt";

	public static bool CanBuild { get; private set; } = true;

	static bool requestedConfig;
	static bool configDownloadCompleted;
	static UnityWebRequest configRequest;

	static Dictionary<int, string> predefinedLayers = new Dictionary<int, string>()
	{
		{ 8, "PostProcessing" },
		{ 16, "CollideWithEnvironment" },
		{ 22, "DontCollideWithPlayer" },
		{ 23, "DontCollideWithPlayerOrSelf" }
	};

	static ProjectInit()
	{
		Start();
	}

	static void Start()
	{
		SetPlayerSettings();
		CheckEditorVersion();
		DownloadConfig();
		CheckForPackages();
	}

	static void SetPlayerSettings()
	{
		if(PlayerSettings.colorSpace != ColorSpace.Linear)
		{
			Debug.Log("Setting color space to Linear.");
			PlayerSettings.colorSpace = ColorSpace.Linear;
		}

		SetLayers();
	}

	static void SetLayers()
	{
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

		SerializedProperty layers = tagManager.FindProperty("layers");

		if(layers == null || !layers.isArray)
		{
			Debug.LogWarning("Failed to set up layers.");
			return;
		}

		bool changedLayer = false;

		foreach(var kvp in predefinedLayers)
		{
			SerializedProperty layerSP = layers.GetArrayElementAtIndex(kvp.Key);
			if(layerSP.stringValue != kvp.Value)
			{
				Debug.Log("Setting up layers. Layer " + kvp.Key + " is now called " + kvp.Value);
				layerSP.stringValue = kvp.Value;

				changedLayer = true;
			}
		}

		if(changedLayer)
		{
			tagManager.ApplyModifiedProperties();
		}
	}

	static void CheckEditorVersion()
	{
		string version = Application.unityVersion;

		if(!validVersions.Contains(version))
		{
			string error = $"Invalid version! You need Unity 2019.4.17f1 or 2019.4.41f1 but are running Unity {version}.";
			EditorUtility.DisplayDialog("Invalid Version", error, "OK");
			Debug.LogError(error);
		}
	}

	static void DownloadConfig()
	{
		if(requestedConfig)
		{
			return;
		}

		configRequest = UnityWebRequest.Get(configURL);
		configRequest.SendWebRequest();
		EditorApplication.update += Update_DownloadConfigProcess;

		requestedConfig = true;
	}

	static void Update_DownloadConfigProcess()
	{
		if(!configRequest.isDone || configDownloadCompleted)
		{
			return;
		}

		configDownloadCompleted = true;
		EditorApplication.update -= Update_DownloadConfigProcess;

		if(configRequest.isNetworkError || configRequest.isHttpError)
		{
			Debug.LogError($"Error getting config information: {configRequest.error}");
			return;
		}

		var config = JsonConvert.DeserializeObject<SDKConfig>(configRequest.downloadHandler.text);

		// Major version change
		if(config.currentVersion != SDKVersion)
		{
			CanBuild = false;
			if(EditorUtility.DisplayDialog("Error", $"The installed Redmatch 2 Custom Map SDK version (v{SDKVersion}) is out of date. Download the new version v{config.currentVersion}.", "New Version"))
			{
				Application.OpenURL(config.downloadPage);
			}
			return;
		}
	}

	#region Packages
	static string[] requiredPackages = new string[]
	{
		"com.unity.probuilder@4.4.0",
		"com.unity.postprocessing@3.0.3",
	};

	static ListRequest listRequest;

	static List<string> pendingPackages = new List<string>();

	static AddRequest pendingRequest;

	static void CheckForPackages()
	{
		listRequest = Client.List();
		EditorApplication.update += Update_ListRequestProgress;
	}

	static void Update_ListRequestProgress()
	{
		if(listRequest.IsCompleted)
		{
			if(listRequest.Status == StatusCode.Success)
			{
				EditorApplication.update -= Update_ListRequestProgress;

				List<string> packagesToInstall = requiredPackages.ToList();

				foreach(var package in listRequest.Result)
				{
					// Only retrieve packages that are currently installed in the
					// Project (and are neither Built-In nor already Embedded)
					if(package.isDirectDependency && package.source
						!= PackageSource.BuiltIn && package.source
						!= PackageSource.Embedded)
					{
						if(packagesToInstall.Contains(package.packageId))
						{
							packagesToInstall.Remove(package.packageId);
						}
					}
				}

				if(packagesToInstall.Count > 0)
				{
					InstallPackages(packagesToInstall);
				}
			}
			else
			{
				Debug.LogError("Failed to check installed packages: " + listRequest.Error.message);
			}
		}
	}

	public static void InstallPackages(List<string> packagesToInstall)
	{
		if(pendingPackages.Count > 0 || pendingRequest != null)
		{
			Debug.LogError("Package install already in progress!");
			return;
		}

		EditorApplication.update += Update_PackageInstallProgress;

		pendingPackages = packagesToInstall;
	}

	// I don't think this really matters since when a package is installed
	// the project recompiles and then redoes this whole process. It works,
	// just not how I meant it to.
	static void Update_PackageInstallProgress()
	{
		if(pendingRequest == null)
		{
			if(pendingPackages.Count == 0)
			{
				EditorApplication.update -= Update_PackageInstallProgress;
			}
			else
			{
				pendingRequest = Client.Add(pendingPackages[0]);
				pendingPackages.RemoveAt(0);
			}
		}
		else
		{
			if(pendingRequest.IsCompleted)
			{
				if(pendingRequest.Status == StatusCode.Success)
					Debug.Log("Installed " + pendingRequest.Result.displayName);
				else if(pendingRequest.Status >= StatusCode.Failure)
					Debug.LogError("Failed to install package: " + pendingRequest.Error.message);

				pendingRequest = null;
			}
		}
	}
	#endregion

	[Serializable]
	class SDKConfig
	{
		public string currentVersion;
		public string downloadPage;
	}
}
#endif