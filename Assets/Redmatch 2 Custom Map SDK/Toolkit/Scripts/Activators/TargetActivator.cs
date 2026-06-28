
using System.Collections.Generic;
using UnityEngine;

public class TargetActivator : Activator
{
	public enum TargetActivatorTargetMode { Dynamic, Preset };
	public TargetActivatorTargetMode targetMode = TargetActivatorTargetMode.Dynamic;
	public GameObject presetTarget;

	// Player
	public bool affectPlayers;
	public short playerDamage;

	// Physics Objects
	public bool applyForce;
	public Vector3 force;
	public enum RelativeForceMode { World, Self, Target, Orbital };
	public RelativeForceMode relativeForceMode = RelativeForceMode.Self;
	public enum TargetActivatorForceMode { Force, Acceleration, Impulse, VelocityChange, VelocityOverwrite };
	public TargetActivatorForceMode forceMode = TargetActivatorForceMode.VelocityOverwrite;
	public bool teleportToOriginalTarget;
	public bool teleportToPlayerCamera;
	public Transform teleport;
	public bool useTeleportLocationRotation;
	public bool keepOffsetWhenTeleporting;

	public override void RunErrorChecks(ref List<string> errors)
	{
		if(targetMode == TargetActivatorTargetMode.Preset)
		{
			if(presetTarget)
			{
				if(!presetTarget.TryGetComponent(out RigidbodySyncer rb))
				{
					errors.Add("Your preset target needs a RigidbodySyncer component.");
				}
			}
			else
			{
				errors.Add("When using the Preset target mode, you need to specify a target.");
			}

			if(affectPlayers)
			{
				errors.Add("You can't have Affect Players enabled with Preset target mode, since you can't specify a player ahead of time. If you want to affect players you need to use the Dynamic target mode.");
			}
		}

		BaseErrorChecks(ref errors);
	}

#if REDMATCH
	DamageData[] damageData = new DamageData[] { DamageData.explosive };

	protected override void InternalActivate(ActivatePayload payload)
	{
		GameObject target;

		if(targetMode == TargetActivatorTargetMode.Preset)
		{
			target = presetTarget;
		}
		else if(payload.target == null)
		{
			Debug.LogError("Cannot activate TargetActivator without a target.");
			return;
		}
		else
		{
			target = payload.target;
		}

		bool isPlayerMine = target.TryGetComponent<PlayerController>(out var player) && player.identity.IsMine;
		bool isRigidbodyMine = target.TryGetComponent<RigidbodySyncer>(out var rigidbodySyncer) && (!rigidbodySyncer.sync || (rigidbodySyncer.sync && MyceliumNetwork.IsHost));
		bool isRigidbodyTeleportable = rigidbodySyncer && (!rigidbodySyncer.sync || (rigidbodySyncer.sync && !rigidbodySyncer.serverOnly) || (rigidbodySyncer.sync && rigidbodySyncer.serverOnly && MyceliumNetwork.IsHost));

		if(affectPlayers && isPlayerMine)
		{
			if(playerDamage > 0)
			{
				PlayerController.LocalInstance.TakeDamage(new DamagePacket(playerDamage, damageData));
			}
		}

		// get the rigidbody
		Rigidbody rb = isPlayerMine ? player.Rigidbody : rigidbodySyncer.Rigidbody;

		if(applyForce && (isPlayerMine || isRigidbodyMine))
		{
			switch(relativeForceMode)
			{
				case RelativeForceMode.World:
					if(forceMode == TargetActivatorForceMode.VelocityOverwrite)
					{
						rb.velocity = force;
					}
					else
					{
						rb.AddForce(force, (ForceMode)forceMode);
					}
					break;
				case RelativeForceMode.Self:
					Vector3 selfRelativeForce = transform.TransformVector(force);

					if(forceMode == TargetActivatorForceMode.VelocityOverwrite)
					{
						rb.velocity = selfRelativeForce;
					}
					else
					{
						rb.AddForce(selfRelativeForce, (ForceMode)forceMode);
					}
					break;
				case RelativeForceMode.Target:

					Transform targetPivot = isPlayerMine ? player.GetCamera().transform : rb.transform;

					Vector3 targetRelativeForce = targetPivot.TransformVector(force);

					if(forceMode == TargetActivatorForceMode.VelocityOverwrite)
					{
						rb.velocity = targetRelativeForce;
					}
					else
					{
						rb.AddForce(targetRelativeForce, (ForceMode)forceMode);
					}
					break;
				case RelativeForceMode.Orbital:
					Vector3 direction = (rb.transform.position - transform.position).normalized;

					Vector3 orgitalRelativeForce = direction * force.z;

					if(forceMode == TargetActivatorForceMode.VelocityOverwrite)
					{
						rb.velocity = orgitalRelativeForce;
					}
					else
					{
						rb.AddForce(orgitalRelativeForce, (ForceMode)forceMode);
					}
					break;
			}
		}

		// If we want teleporting to occur, and this is a valid teleport target, run teleport logic
		if((teleport || (teleportToOriginalTarget && payload.target != null)) && (isPlayerMine || isRigidbodyTeleportable))
		{
			Vector3 offset = Vector3.zero;

			Transform teleTarget;
			if(teleportToOriginalTarget)
			{
				payload.target.TryGetComponent<PlayerController>(out var origPlayer);
				if(teleportToPlayerCamera && origPlayer != null)
				{
					teleTarget = origPlayer.GetCamera().transform;
				}
				else
				{
					teleTarget = payload.target.transform;
				}
			}
			else
			{
				teleTarget = teleport;
			}

			if(keepOffsetWhenTeleporting)
			{
				offset = rb.position - transform.position;
			}

			// Player teleport logic
			if(isPlayerMine)
			{
				player.ForceStopGrappling();

				if(useTeleportLocationRotation)
				{
					rb.rotation = Quaternion.Euler(Vector3.up * teleTarget.eulerAngles.y);
				}

				rb.position = teleTarget.position + offset;
			}
			// Object teleport logic
			else if(isRigidbodyTeleportable)
			{
				rigidbodySyncer.Teleport(teleTarget.position + offset, useTeleportLocationRotation ? teleTarget.rotation : rb.rotation);
			}
		}
	}
#endif
}
