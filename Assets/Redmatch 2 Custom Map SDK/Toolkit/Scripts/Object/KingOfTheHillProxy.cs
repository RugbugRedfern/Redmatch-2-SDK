using System.Collections.Generic;
using UnityEngine;

public class KingOfTheHillProxy : ErrorCollectionBehaviour
{
	Color color = new Color(255, 179, 0);

	void OnDrawGizmos()
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = color;
		Gizmos.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
	}

	public override void RunErrorChecks(ref List<string> errors)
	{
		foreach(var proxy in GameObject.FindObjectsOfType<KingOfTheHillProxy>())
		{
			if(proxy == this)
				continue;

			if(proxy.gameObject.name == gameObject.name)
			{
				errors.Add($"2 King of the Hill Proxies cannot have the same name ({gameObject.name})");
			}
		}
	}
}
