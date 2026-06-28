using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class ExitTrigger : TriggerColliderTrigger
{
	public override string GetTarget()
	{
		return "The Player/ObjectSyncer that exits this trigger.";
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
	Dictionary<GameObject, MyceliumPlayer> bodies = new Dictionary<GameObject, MyceliumPlayer>();

	void OnTriggerEnter(Collider other)
	{
		bool isPlayer = other.TryGetComponent<PlayerController>(out var player);
		bool isRigidbodySyncer = other.TryGetComponent<RigidbodySyncer>(out var rigidbodySyncer);

		if(!isPlayer && !isRigidbodySyncer)
		{
			return;
		}

		if(!bodies.ContainsKey(other.gameObject))
		{
			bodies[other.gameObject] = isPlayer ? player.identity.Owner : null;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(bodies.ContainsKey(other.gameObject))
		{
			bodies.Remove(other.gameObject);
		}

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

	void FixedUpdate()
	{
		if(bodies.Count > 0)
		{
			List<GameObject> keysToRemove = new List<GameObject>();

			foreach(var kvp in bodies)
			{
				if(kvp.Key == null)
				{
					if(CheckCooldown())
					{
						Activate(null, kvp.Value);
					}

					keysToRemove.Add(kvp.Key);
				}
			}

			// Remove entries with null keys
			foreach(var key in keysToRemove)
			{
				bodies.Remove(key);
			}
		}
	}
#endif
}
