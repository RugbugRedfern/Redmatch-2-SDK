using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

#if !REDMATCH
public static class MapSdkUpdater
{
	private const string PACKAGE_URL = "https://github.com/RugbugRedfern/Redmatch-2-SDK.git?path=/Assets/Redmatch%202%20Custom%20Map%20SDK#release";

	private static AddRequest request;
	private static string previousHash;
	private static string previousVersion;

	[MenuItem("Redmatch 2/Update Map SDK", priority = 400)]
	private static void UpdatePackage()
	{
		if (request != null && !request.IsCompleted)
			return;

		PackageInfo installedPackage = PackageInfo.FindForAssembly(typeof(MapSdkUpdater).Assembly);

		if (installedPackage == null || installedPackage.source != PackageSource.Git || installedPackage.git == null)
		{
			EditorUtility.DisplayDialog("Map SDK Update Unavailable", "This copy of the Redmatch 2 Map SDK was not installed through Git UPM.", "OK");
			return;
		}

		previousHash = installedPackage.git.hash;
		previousVersion = installedPackage.version;

		request = Client.Add(PACKAGE_URL);
		EditorApplication.update += Update_MonitorRequest;

		Debug.Log("Checking for a newer Redmatch 2 Map SDK release...");
	}

	private static void Update_MonitorRequest()
	{
		if (request == null || !request.IsCompleted)
			return;

		EditorApplication.update -= Update_MonitorRequest;

		if (request.Status == StatusCode.Success)
		{
			PackageInfo installedPackage = request.Result;
			string installedVersion = installedPackage != null ? installedPackage.version : "unknown";
			string installedHash = installedPackage != null && installedPackage.git != null ? installedPackage.git.hash : string.Empty;

			if (!string.IsNullOrEmpty(previousHash) && previousHash == installedHash)
			{
				EditorUtility.DisplayDialog("Redmatch 2 Map SDK", $"No new update is available.\n\nInstalled version: {installedVersion}", "OK");
			}
			else
			{
				EditorUtility.DisplayDialog("Redmatch 2 Map SDK Updated", $"The Redmatch 2 Map SDK was updated successfully.\n\nPrevious version: {previousVersion}\nInstalled version: {installedVersion}", "OK");
			}
		}
		else
		{
			string errorMessage = request.Error != null ? request.Error.message : "An unknown Package Manager error occurred.";

			EditorUtility.DisplayDialog("Map SDK Update Failed", $"Unity could not update the Redmatch 2 Map SDK.\n\n{errorMessage}", "OK");
		}

		request = null;
		previousHash = null;
		previousVersion = null;
	}
}
#endif