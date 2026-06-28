using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public static class CustomMapTimeTrialValidator
{
	public static bool IsSceneValid(Scene scene, out string error)
	{
		int count = 0;

		foreach(var root in scene.GetRootGameObjects())
		{
			var proxies = root.GetComponentsInChildren<TimeTrialProxy>();

			foreach(var proxy in proxies)
			{
				count++;
			}
		}

		if(count > 1)
		{
			error = "You can only have 1 Time Trial per map.";
			return false;
		}

		error = "";
		return true;
	}
}
