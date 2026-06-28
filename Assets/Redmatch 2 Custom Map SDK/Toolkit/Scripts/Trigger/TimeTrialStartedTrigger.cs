using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Principal;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class TimeTrialStartedTrigger : CooldownTrigger
{
	public override void RunErrorChecks(ref List<string> errors)
	{
		base.RunErrorChecks(ref errors);

		int triggers = gameObject.GetComponents<TimeTrialStartedTrigger>().Length;
		if(triggers > 1)
		{
			errors.Add($"You cannot have {triggers} Time Trial Started Trigger components on the same GameObject. Put them on separate GameObjects.");
		}
	}

	public override string GetTarget()
	{
		return "The player who started the time trial.";
	}

	
// Some code here has been excluded from the SDK.

}
