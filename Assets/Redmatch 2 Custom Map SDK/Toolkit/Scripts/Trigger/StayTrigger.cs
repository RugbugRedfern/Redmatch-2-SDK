
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public class StayTrigger : TriggerColliderTrigger
{
	public override string GetTarget()
	{
		return "The Player/ObjectSyncer that stays in this trigger.";
	}

	public float initialDelay = 0f;
	[Tooltip("This has a minimum value of 0.01.")]
	public float repeatDelay = 1f;

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
	float GetClampedRepeatDelay()
	{
		return Mathf.Max(repeatDelay, 0.01f);
	}

	Dictionary<GameObject, BodyData> bodies = new Dictionary<GameObject, BodyData>();

	// if the host changes, then people in the trigger
	// will stop being affected.

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

		if(!bodies.ContainsKey(other.gameObject))
		{
			bodies[other.gameObject] = new BodyData() { timer = -initialDelay + GetClampedRepeatDelay(), source = isPlayer ? player.identity.Owner : null };
		}
	}

	void OnTriggerExit(Collider other)
	{
		// no checks here because its always just chill to remove it ;)

		bool isPlayer = other.TryGetComponent<PlayerController>(out var player);
		bool isRigidbodySyncer = other.TryGetComponent<RigidbodySyncer>(out var rigidbodySyncer);

		if(!isPlayer && !isRigidbodySyncer)
		{
			return;
		}

		if(bodies.ContainsKey(other.gameObject))
		{
			bodies.Remove(other.gameObject);
		}
	}

	void FixedUpdate()
	{
		if(bodies.Count > 0)
		{ 
			RemoveEntriesWithNullKeys();
		}

		foreach(var kvp in bodies)
		{
			kvp.Value.timer += Time.deltaTime;

			// I don't think it'll ever get to 5 loops in FixedUpdate but just in case :)
			int maxLoops = 5;

			while(kvp.Value.timer >= GetClampedRepeatDelay())
			{
				Activate(kvp.Key, kvp.Value.source);

				kvp.Value.timer -= GetClampedRepeatDelay();

				maxLoops--;

				if(maxLoops <= 0) break;
			}
		}
	}

	void RemoveEntriesWithNullKeys()
	{
		// Create a list of keys with null values
		List<GameObject> keysToRemove = new List<GameObject>();
		foreach(var kvp in bodies)
		{
			if(kvp.Key == null)
			{
				keysToRemove.Add(kvp.Key);
			}
		}

		// Remove entries with null keys
		foreach(var key in keysToRemove)
		{
			bodies.Remove(key);
		}
	}

	class BodyData
	{
		public float timer;
		public MyceliumPlayer source;
	}
#endif
}
