using System.Collections.Generic;
using UnityEngine;

public class UpgradeCabinetProxy : ErrorCollectionBehaviour
{
	[HideInInspector] public int id = -1;

	public override void RunErrorChecks(ref List<string> errors)
	{
		var components = gameObject.GetComponents<MonoBehaviour>();

		if(components.Length > 1)
		{
			errors.Add("Don't attach any other components to an Upgrade Cabinet Proxy. They will be destroyed.");
		}
	}
}