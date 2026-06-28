using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivatorReferencer : ErrorCollectionBehaviour
{
	public virtual IEnumerable<Activator> GetReferences()
	{
		yield break;
	}

#if UNITY_EDITOR
	void OnValidate()
	{
		foreach(var reference in GetReferences())
		{
			if(reference)
			{
				reference.RecacheReferences();
			}
		}
	}
#endif
}