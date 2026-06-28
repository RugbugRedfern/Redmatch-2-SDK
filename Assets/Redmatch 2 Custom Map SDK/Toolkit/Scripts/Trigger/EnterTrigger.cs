using UnityEngine;

public class EnterTrigger : TriggerColliderTrigger
{
	public override string GetTarget()
	{
		return "The Player/ObjectSyncer that enters this trigger.";
	}

	void Reset()
	{
		if(gameObject.GetComponent<Rigidbody>() == null)
		{
			var rb = gameObject.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.useGravity = false;
			rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		}
	}

#if REDMATCH
	void OnTriggerEnter(Collider other)
	{
		if(!CheckCooldown())
			return;

		bool isPlayer = other.TryGetComponent<PlayerController>(out var player);
		bool isRigidbodySyncer = other.TryGetComponent<RigidbodySyncer>(out var rigidbodySyncer);

		if(!isPlayer && !isRigidbodySyncer)
		{
			return;
		}

		Activate(other.gameObject, isPlayer ? player.identity.Owner : null);
	}
#endif
}
