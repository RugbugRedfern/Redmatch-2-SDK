using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if !REDMATCH
public class MappingMenus : MonoBehaviour
{
	[MenuItem("Redmatch 2/Connect to Steam", priority = 100)]
	public static void ConnectToSteam()
	{
		EditorSteamManager.ConnectToSteam();
	}

	[MenuItem("Redmatch 2/Locate Redmatch 2.exe", priority = 101)]
	public static void LocateExe()
	{
		MappingUtils.SetApplicationPath();
	}

	[MenuItem("Redmatch 2/Check For Errors _F3", priority = 299)]
	public static void CheckForErrors()
	{
		MappingUtils.CheckForErrors();
	}

	[MenuItem("Redmatch 2/Build Map _F4", priority = 300)]
	public static void BuildMap()
	{
		MappingUtils.BuildMap(MapConfig.GetCurrent());
	}

	[MenuItem("Redmatch 2/Test Map", priority = 301)]
	public static void TestMap()
	{
		MappingUtils.TestMap(MapConfig.GetCurrent());
	}

	[MenuItem("Redmatch 2/Build and Test Map _F5", priority = 302)]
	public static void BuildAndTestMap()
	{
		MappingUtils.BuildAndTestMap(MapConfig.GetCurrent());
	}

	[MenuItem("Redmatch 2/Upload Map to Workshop", priority = 303)]
	public static void UploadMap()
	{
		MappingUtils.UploadMap(MapConfig.GetCurrent());
	}

	[MenuItem("Redmatch 2/New Map", priority = 50)]
	public static void NewMap()
	{
		MappingUtils.NewMap();
	}
}
#endif