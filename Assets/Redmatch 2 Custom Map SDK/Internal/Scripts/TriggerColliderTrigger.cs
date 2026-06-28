using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerColliderTrigger : CooldownTrigger
{
	protected virtual void Awake()
	{
		gameObject.layer = 16;
	}

	public override void RunErrorChecks(ref List<string> errors)
	{
		base.RunErrorChecks(ref errors);

		if(gameObject.TryGetComponent<DamageableTrigger>(out var damageableTrigger))
		{
			errors.Add("You cannot have a Damageable Trigger on the same GameObject. It needs to be on a separate GameObject because of its layer.");
		}

		var cols = gameObject.GetComponents<Collider>();

		bool success = false;

		foreach(var col in cols)
		{
			if(col.isTrigger)
			{
				success = true;
				break;
			}
		}

		if(!success)
		{
			errors.Add("You need to add a Collider with Is Trigger checked to this GameObject or the trigger will not fire.");
		}

		if(!gameObject.TryGetComponent<Rigidbody>(out var rb))
		{
			errors.Add("For this trigger to properly detect Players you will need to add a Rigidbody component. You can mark it as kinematic to keep it from moving.");
		}
	}
}
