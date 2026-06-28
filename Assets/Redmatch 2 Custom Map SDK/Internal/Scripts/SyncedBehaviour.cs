
using System.Collections.Generic;
using UnityEngine;

public abstract class SyncedBehaviour : ActivatorReferencer
{
	public bool sync = true;
	public bool serverOnly = true;

	public override void RunErrorChecks(ref List<string> errors)
	{
		if(!TryGetComponent<MyceliumIdentity>(out var identity))
		{
			errors.Add("A MyceliumIdentity is required on this GameObject.");
		}
	}
}
