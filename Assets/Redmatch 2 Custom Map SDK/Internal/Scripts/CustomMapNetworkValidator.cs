using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public static class CustomMapNetworkValidator
{
	public static bool IsSceneValid(Scene scene, out string error)
	{
#if REDMATCH
		List<int> ids = new List<int>();

		foreach(var root in scene.GetRootGameObjects())
		{
			var identities = root.GetComponentsInChildren<MyceliumIdentity>();

			foreach(var identity in identities)
			{
				if(identity.NetID < 20000)
				{
					error = $"ID is invalid on MyceliumIdentity {identity.name}: {identity.NetID}";
					return false;
				}

				if(ids.Contains(identity.NetID))
				{
					error = $"Duplicate ID on MyceliumIdentity {identity.name}: {identity.NetID}";
					return false;
				}

				ids.Add(identity.NetID);
			}

			var proxies = root.GetComponentsInChildren<UpgradeCabinetProxy>();

			foreach(var proxy in proxies)
			{
				if(proxy.id < 20000)
				{
					error = $"ID is invalid on UpgradeCabinetProxy {proxy.name}: {proxy.id}";
					return false;
				}

				if(ids.Contains(proxy.id))
				{
					error = $"Duplicate ID on UpgradeCabinetProxy {proxy.name}: {proxy.id}";
					return false;
				}

				ids.Add(proxy.id);
			}
		}
#endif

		error = "";
		return true;
	}
}
