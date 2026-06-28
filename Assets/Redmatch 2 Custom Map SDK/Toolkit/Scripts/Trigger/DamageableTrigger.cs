
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class DamageableTrigger : CooldownTrigger
{
	[Tooltip("The minimum amount of damage that must be done in a single hit to execute the trigger.")]
	public short minimumDamage = 0;

	public override void RunErrorChecks(ref List<string> errors)
	{
		base.RunErrorChecks(ref errors);

		if(gameObject.TryGetComponent<TriggerColliderTrigger>(out var triggerColliderTrigger))
		{
			errors.Add("You cannot have a Damageable Trigger on the same GameObject as an Enter, Exit, or Stay trigger. It needs to be on a separate GameObject because of its layer.");
		}

		int damageableTriggerComponentCount = gameObject.GetComponents<DamageableTrigger>().Length;
		if(damageableTriggerComponentCount > 1)
		{
			errors.Add($"You cannot have {damageableTriggerComponentCount} Damageable Trigger components on the same GameObject. Only 1 will be triggered.");
		}

		var cols = gameObject.GetComponents<Collider>();

		bool success = false;

		foreach(var col in cols)
		{
			if(!col.isTrigger)
			{
				success = true;
				break;
			}
		}

		if(!success)
		{
			errors.Add("You need to add a Collider with Is Trigger unchecked to this GameObject or the trigger will not fire.");
		}
	}

	public override string GetTarget()
	{
		return "The Player that shoots this trigger.";
	}

#if REDMATCH
	MyceliumIdentity identity;

	public void Initialize()
	{
		gameObject.layer = 9; // damageable layer

		identity = GetComponent<MyceliumIdentity>();

		identity.RegisterRPC(191, RPC_SyncShot, RPCSecurityLevel.Anyone);
	}

	public DamageResponse TakeDamage(DamagePacket damagePacket)
	{
		if(CheckCooldown())
		{
			if(damagePacket.amount < minimumDamage)
			{
				return DamageResponse.WasShielded;
			}
			else
			{
				MyceliumNetwork.RPC(identity.GetRPCMessage(191).WriteShort(damagePacket.amount), ReliableType.Reliable);
			}	

			return DamageResponse.TookDamage;
		}
		else
		{
			return DamageResponse.WasShielded;
		}
	}

	void RPC_SyncShot(Message message, MyceliumPlayer sender)
	{
		if(!gameObject.activeSelf)
			return;

		int damage = message.ReadShort();

		var player = MyceliumPrefabManager.GetPlayerIdentity(sender);

		foreach(var activator in activators)
		{
			activator.Activate(player?.gameObject, -damage, sender);
		}
	}

	public void TakeKnockbackDamage(DamagePacket damagePacket, Vector3 knockback)
	{
		return;
	}
#endif
}
