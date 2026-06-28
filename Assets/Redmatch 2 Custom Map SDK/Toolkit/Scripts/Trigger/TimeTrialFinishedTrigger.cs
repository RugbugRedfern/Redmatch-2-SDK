
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Principal;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class TimeTrialFinishedTrigger : CooldownTrigger
{
	[Tooltip("The minimum time for the time trial to be completed in to execute the trigger.")]
	public float minimumTime = 0;
	[Tooltip("The maximum time for the time trial to be completed in to execute the trigger.")]
	public float maximumTime = float.MaxValue;

	public override void RunErrorChecks(ref List<string> errors)
	{
		base.RunErrorChecks(ref errors);

		int triggers = gameObject.GetComponents<TimeTrialFinishedTrigger>().Length;
		if(triggers > 1)
		{
			errors.Add($"You cannot have {triggers} Time Trial Finished Trigger components on the same GameObject. Put them on separate GameObjects.");
		}
	}

	public override string GetTarget()
	{
		return "The player who finished the time trial.";
	}

	
// Some code here has been excluded from the SDK.

}
