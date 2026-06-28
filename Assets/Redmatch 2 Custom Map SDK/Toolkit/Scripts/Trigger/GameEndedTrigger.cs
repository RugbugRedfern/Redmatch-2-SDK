using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEndedTrigger : Trigger
{
	public override string GetTarget()
	{
		return "None.";
	}

#if REDMATCH
	void Awake()
	{
		GameMode_Base.OnGameEnded += OnGameEnded;
	}

	void OnDestroy()
	{
		GameMode_Base.OnGameEnded -= OnGameEnded;
	}

	private void OnGameEnded()
	{
		try
		{
			Activate(null, null);
		}
		catch(Exception ex)
		{
			Debug.LogError("Error with GameEndedTrigger OnGameEnded: " + ex.ToString());
		}
	}
#endif
}
