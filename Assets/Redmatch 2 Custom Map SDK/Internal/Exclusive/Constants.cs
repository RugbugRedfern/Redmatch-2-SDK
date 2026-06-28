using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !REDMATCH
public static class Constants
{
	public static string PlayerPrefsApplicationPathKey { get; private set; } = "applicationPath";
	public static AppId_t AppID = (AppId_t)1280770;
}
#endif