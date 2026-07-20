using System;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

#if !REDMATCH
[InitializeOnLoad]
public static class MapSdkUpdater
{
	private const string PACKAGE_URL_BASE = "https://github.com/RugbugRedfern/Redmatch-2-SDK.git?path=/Assets/Redmatch%202%20Custom%20Map%20SDK";
	private const string COMMIT_API_URL = "https://api.github.com/repos/RugbugRedfern/Redmatch-2-SDK/commits/";
	private const string SESSION_CHECK_KEY = "Redmatch.MapSDK.CheckedForUpdates";

	private static UnityWebRequest checkRequest;
	private static AddRequest updateRequest;
	private static string installedHash;
	private static string installedVersion;
	private static string installedRevision;
	private static bool showNoUpdateDialog;

	[Serializable]
	private class GitHubCommitResponse
	{
		public string sha;
	}

	static MapSdkUpdater()
	{
		EditorApplication.delayCall += CheckOnStartup;
	}

	private static void CheckOnStartup()
	{
		if (SessionState.GetBool(SESSION_CHECK_KEY, false))
			return;

		SessionState.SetBool(SESSION_CHECK_KEY, true);
		StartUpdateCheck(false);
	}

	[MenuItem("Redmatch 2/Check for Map SDK Updates", priority = 400)]
	private static void CheckManually()
	{
		StartUpdateCheck(true);
	}

	private static void StartUpdateCheck(bool shouldShowNoUpdateDialog)
	{
		if (checkRequest != null || (updateRequest != null && !updateRequest.IsCompleted))
			return;

		PackageInfo package = GetInstalledPackage(shouldShowNoUpdateDialog);

		if (package == null)
			return;

		installedHash = package.git.hash;
		installedVersion = package.version;
		installedRevision = package.git.revision;
		showNoUpdateDialog = shouldShowNoUpdateDialog;

		if (string.IsNullOrEmpty(installedRevision))
		{
			CleanupCheck();
			return;
		}

		checkRequest = UnityWebRequest.Get(COMMIT_API_URL + UnityWebRequest.EscapeURL(installedRevision));
		checkRequest.timeout = 10;
		checkRequest.SetRequestHeader("Accept", "application/vnd.github+json");
		checkRequest.SendWebRequest();

		EditorApplication.update += Update_CheckRequest;
	}

	private static void Update_CheckRequest()
	{
		if (checkRequest == null || !checkRequest.isDone)
			return;

		EditorApplication.update -= Update_CheckRequest;

		if (checkRequest.isNetworkError || checkRequest.isHttpError)
		{
			string error = checkRequest.error;

			if (showNoUpdateDialog)
				EditorUtility.DisplayDialog("Map SDK Update Check Failed", $"Unity could not check for Map SDK updates.\n\n{error}", "OK");
			else
				Debug.LogError($"Could not check for Redmatch 2 Map SDK updates: {error}");

			CleanupCheck();
			return;
		}

		GitHubCommitResponse response = JsonUtility.FromJson<GitHubCommitResponse>(checkRequest.downloadHandler.text);
		string latestHash = response != null ? response.sha : null;

		if (string.IsNullOrEmpty(latestHash))
		{
			if (showNoUpdateDialog)
				EditorUtility.DisplayDialog("Map SDK Update Check Failed", "GitHub returned an invalid response while checking for updates.", "OK");
			else
				Debug.LogError("GitHub returned an invalid response while checking for Redmatch 2 Map SDK updates.");

			CleanupCheck();
			return;
		}

		bool updateAvailable = !string.Equals(installedHash, latestHash, StringComparison.OrdinalIgnoreCase);
		string currentVersion = installedVersion;
		string currentRevision = installedRevision;
		bool shouldShowNoUpdateDialog = showNoUpdateDialog;

		CleanupCheck();

		if (!updateAvailable)
		{
			if (shouldShowNoUpdateDialog)
				EditorUtility.DisplayDialog("Redmatch 2 Map SDK", $"No new update is available.\n\nInstalled version: {currentVersion}", "OK");

			return;
		}

		bool shouldUpdate = EditorUtility.DisplayDialog("Redmatch 2 Map SDK Update Available", $"A newer Map SDK version is available.\n\nInstalled version: {currentVersion}\nBranch: {currentRevision}\n\nWould you like to update now?", "Update", "Later");

		if (shouldUpdate)
			BeginUpdate(currentRevision);
	}

	private static void BeginUpdate(string revision)
	{
		if (updateRequest != null && !updateRequest.IsCompleted)
			return;

		updateRequest = Client.Add($"{PACKAGE_URL_BASE}#{revision}");
		EditorApplication.update += Update_InstallRequest;
		Debug.Log($"Updating the Redmatch 2 Map SDK from the {revision} branch...");
	}

	private static void Update_InstallRequest()
	{
		if (updateRequest == null || !updateRequest.IsCompleted)
			return;

		EditorApplication.update -= Update_InstallRequest;

		if (updateRequest.Status == StatusCode.Success)
		{
			string version = updateRequest.Result != null ? updateRequest.Result.version : "unknown";

			Debug.Log($"Redmatch 2 Map SDK updated successfully to version {version}.");
			EditorUtility.DisplayDialog("Redmatch 2 Map SDK Updated", $"The Map SDK was updated successfully.\n\nInstalled version: {version}", "OK");
		}
		else
		{
			string error = updateRequest.Error != null ? updateRequest.Error.message : "An unknown Package Manager error occurred.";

			Debug.LogError($"Failed to update the Redmatch 2 Map SDK: {error}");
			EditorUtility.DisplayDialog("Map SDK Update Failed", $"Unity could not update the Redmatch 2 Map SDK.\n\n{error}", "OK");
		}

		updateRequest = null;
	}

	private static PackageInfo GetInstalledPackage(bool showError)
	{
		PackageInfo package = PackageInfo.FindForAssetPath("Packages/com.redmatch.mapsdk/package.json");

		if (package != null && package.source == PackageSource.Git && package.git != null)
			return package;

		if (showError)
		{
			bool takeMeThere = EditorUtility.DisplayDialog("Map SDK Update Unavailable", "This copy of the Redmatch 2 Map SDK was not installed through Git UPM. You will need to manually download the updated SDK from redmatchgame.com/custom-maps", "Take me There", "OK");

			if(takeMeThere)
			{
				Application.OpenURL("https://redmatchgame.com/custom-maps");
			}
		}

		return null;
	}

	private static void CleanupCheck()
	{
		checkRequest?.Dispose();
		checkRequest = null;
		installedHash = null;
		installedVersion = null;
		installedRevision = null;
		showNoUpdateDialog = false;
	}
}
#endif