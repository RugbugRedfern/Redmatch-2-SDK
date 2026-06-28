using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

#if !REDMATCH
[CreateAssetMenu(menuName = "Redmatch 2/Map Config", fileName = "config")]
public class MapConfig : ScriptableObject
{
	public string bundleName;
	[Header("This should match the Scene name and map folder name")]
	public new string name;
	public Texture2D thumbnail;
	public List<Texture2D> screenshots = new List<Texture2D>();
	[Header("For settings that are checkboxes in-game, write True or False. Otherwise write a number.")]
	public MatchSettingOverride[] matchSettingOverrides;

	public string GetMapDirectory()
	{
		string path = AssetDatabase.GetAssetPath(this);
		path = path.Substring(0, path.Length - "config.asset".Length);
		return path;
	}

	public string GetFullMapDirectory()
	{
		return MappingUtils.ConvertLocalPathToGlobalPath(GetMapDirectory());
	}

	public static MapConfig GetCurrent()
	{
		string scenePath = SceneManager.GetActiveScene().path;

		int lastIndex = scenePath.LastIndexOf('/');
		string sceneFolder = scenePath.Substring(0, lastIndex);

		string configPath = sceneFolder + "/config.asset";

		var config = AssetDatabase.LoadAssetAtPath<MapConfig>(configPath);

		if(config == null)
		{
			throw new System.Exception($"Missing config file at {configPath}");
		}

		return config;
	}

	public string GetJSON()
	{
		var json = new UnsanitizedMapConfigData();

		if(matchSettingOverrides != null)
		{
			foreach(var setting in matchSettingOverrides)
			{
				json.MatchSettingOverrides.Add(setting.setting.ToString(), setting.value);
			}
		}

		json.SDKVersion = ProjectInit.SDKVersion;

		return JsonConvert.SerializeObject(json, Formatting.Indented);
	}
}

[System.Serializable]
public class MatchSettingOverride
{
	public CustomMapMatchSetting setting;
	public string value;
}
#endif