using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CustomMapRequiredComponentsValidator
{
	static Type PostProcessVolumeType = Type.GetType("UnityEngine.Rendering.PostProcessing.PostProcessVolume, Unity.Postprocessing.Runtime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", true);

	static Type[] requiredComponents =
	{
		typeof(KingOfTheHillProxy),
		typeof(Spawnpoint),
		typeof(Camera),
		typeof(AudioListener),
		PostProcessVolumeType,
	};

	public static bool IsSceneValid(Scene scene, out string error)
	{
		Dictionary<Type, int> componentCounts = new Dictionary<Type, int>();
		List<MapInfo> mapInfos = new List<MapInfo>();

		int correctLayerPPCount = 0;

		foreach(var type in requiredComponents)
		{
			componentCounts[type] = 0;
		}

		foreach(var root in scene.GetRootGameObjects())
		{
			foreach(var type in requiredComponents)
			{
				componentCounts[type] += root.GetComponentsInChildren(type).Length;
			}

			foreach(var mapInfo in root.GetComponentsInChildren<MapInfo>())
			{
				mapInfos.Add(mapInfo);
			}

			foreach(var ppVolume in root.GetComponentsInChildren(PostProcessVolumeType, true))
			{
				if(ppVolume.gameObject.layer == 8)
				{
					correctLayerPPCount++;
				}

				if(!ppVolume.gameObject.TryGetComponent(PostProcessVolumeType, out var buddy))
				{
					error = $"PostProcessVolume on GameObject {ppVolume.gameObject.name} is missing a required PostProcessVolumeBuddy component.";
					return false;
				}
			}
		}

		foreach(var type in requiredComponents)
		{
			if(componentCounts[type] == 0)
			{
				error = $"Missing required active component {type}. You should have at least 1 of these in your map on an active GameObject.";
				return false;
			}
		}

		if(mapInfos.Count > 1)
		{
			error = $"Only 1 MapInfo can be present in the scene. This map has {mapInfos.Count}.";
			return false;
		}

		if(mapInfos.Count == 0)
		{
			error = $"Missing required active component MapInfo. You should have at least 1 of these in your map on an active GameObject.";
			return false;
		}

		if(mapInfos[0].worldCamera == null)
		{
			error = "The MapInfo needs to have a world camera specified.";
			return false;
		}

		if(correctLayerPPCount == 0)
		{
			error = $"At least one PostProcessVolume must be on layer 8.";
			return false;
		}

		error = "";
		return true;
	}
}
