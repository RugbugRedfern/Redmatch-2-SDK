
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * The general architecture of this class is that it should facilitate
 * networked events, not actually deal with networking. That way
 * all state can be managed by other components and conflicts such as
 * enabling and disabling the same GameObject from multiple Activators
 * can be avoided.
 */

public class GenericActivator : Activator
{
	public enum AuthorityRequirement { OnlyHost, OnlyLocalPlayer, Everyone };
	public AuthorityRequirement authorityRequirement;
	public GameObjectSyncer[] objectsToEnable = new GameObjectSyncer[0];
	public GameObjectSyncer[] objectsToDisable = new GameObjectSyncer[0];
	public GameObjectSyncer[] randomObjectToEnable = new GameObjectSyncer[0];
	public GameObjectSyncer[] randomObjectToDisable = new GameObjectSyncer[0];
	public Activator[] randomActivatorToTrigger = new Activator[0];
	public AnimatorActionData[] animatorActions = new AnimatorActionData[0];
	public Activator[] activatorActions = new Activator[0];
	public HealthSyncerActionData[] healthSyncerActions = new HealthSyncerActionData[0];
	public IntSyncerActionData[] intSyncerActions = new IntSyncerActionData[0];

	public override IEnumerable<Activator> GetReferences()
	{
		foreach(var x in activatorActions) yield return x;
	}

	public override void RunErrorChecks(ref List<string> errors)
	{
		foreach(var obj in AllObjects())
		{
			if(obj == null)
			{
				errors.Add($"A specified object is null.");
			}
		}

		BaseErrorChecks(ref errors);
	}

	IEnumerable<object> AllObjects()
	{
		foreach(var x in objectsToEnable)
			yield return x;
		foreach(var x in objectsToDisable)
			yield return x;
		foreach(var x in randomObjectToEnable)
			yield return x;
		foreach(var x in randomObjectToDisable)
			yield return x;
		foreach(var x in randomActivatorToTrigger)
			yield return x;
		foreach(var x in animatorActions)
			yield return x.animatorSyncer;
		foreach(var x in activatorActions)
			yield return x;
		foreach(var x in healthSyncerActions)
			yield return x.healthSyncer;
	}


#if REDMATCH

	protected override void InternalActivate(ActivatePayload payload)
	{
		switch(authorityRequirement)
		{
			case AuthorityRequirement.OnlyHost:
				if(!MyceliumNetwork.IsHost)
					return;
				break;
			case AuthorityRequirement.OnlyLocalPlayer:
				if(payload.source != null)
				{
					if(!payload.source.IsLocal)
						return;
				}
				break;
			case AuthorityRequirement.Everyone:
				break;
		}

		EnableObjects();
		DisableObjects();
		EnableRandomObject();
		DisableRandomObject();

		// Trigger random activator
		if(randomActivatorToTrigger.Length > 0)
		{
			randomActivatorToTrigger[Random.Range(0, randomActivatorToTrigger.Length)].Activate(payload.target, payload.data);
		}

		foreach(var action in animatorActions)
		{
			ApplyAnimationAction(action);
		}

		foreach(var action in activatorActions)
		{
			action.Activate(payload.target, payload.data);
		}

		foreach(var action in healthSyncerActions)
		{
			ApplyHealthSyncerAction(payload, action);
		}

		foreach(var action in intSyncerActions)
		{
			ApplyIntSyncerAction(payload, action);
		}
	}

	void EnableObjects()
	{
		foreach(var go in objectsToEnable)
		{
			go.gameObject.SetActive(true);
		}
	}

	void DisableObjects()
	{
		foreach(var go in objectsToDisable)
		{
			go.gameObject.SetActive(false);
		}
	}

	void EnableRandomObject()
	{
		if(randomObjectToEnable.Length > 0)
		{
			randomObjectToEnable[Random.Range(0, randomObjectToEnable.Length)].gameObject.SetActive(true);
		}
	}

	void DisableRandomObject()
	{
		if(randomObjectToDisable.Length > 0)
		{
			randomObjectToDisable[Random.Range(0, randomObjectToDisable.Length)].gameObject.SetActive(false);
		}
	}

	void ApplyAnimationAction(AnimatorActionData data)
	{
		switch(data.actionType)
		{
			case AnimatorActionData.AnimatorActionType.SetFloat:
				data.animatorSyncer.SetFloat(data.parameter, data.floatValue);
				break;
			case AnimatorActionData.AnimatorActionType.SetRandomFloatFrom0To1:
				data.animatorSyncer.SetRandomFloatFrom0To1(data.parameter);
				break;
			case AnimatorActionData.AnimatorActionType.SetInteger:
				data.animatorSyncer.SetInteger(data.parameter, data.intValue);
				break;
			case AnimatorActionData.AnimatorActionType.SetBool:
				data.animatorSyncer.SetBool(data.parameter, data.boolValue);
				break;
			case AnimatorActionData.AnimatorActionType.SetTrigger:
				data.animatorSyncer.SetTrigger(data.parameter);
				break;
			case AnimatorActionData.AnimatorActionType.ResetTrigger:
				data.animatorSyncer.ResetTrigger(data.parameter);
				break;
		}
	}

	void ApplyHealthSyncerAction(ActivatePayload payload, HealthSyncerActionData data)
	{
		switch(data.actionType)
		{
			case HealthSyncerActionData.HealthSyncerActionType.ChangeValue:
				data.healthSyncer.ChangeHealth(data.amount);
				break;
			case HealthSyncerActionData.HealthSyncerActionType.SetValue:
				data.healthSyncer.SetHealth(data.amount);
				break;
			case HealthSyncerActionData.HealthSyncerActionType.DynamicFromDamageableTrigger:

				if(!(payload.data is int))
				{
					Debug.LogError($"Activator attempted to execute a DynamicFromDamageableTrigger but the payload data is not an int! (payload.data: {payload.data}, gameObject.name: {gameObject.name})");
					break;
				}

				data.healthSyncer.ChangeHealth((int)payload.data);
				break;
		}
	}

	void ApplyIntSyncerAction(ActivatePayload payload, IntSyncerActionData data)
	{
		switch(data.actionType)
		{
			case HealthSyncerActionData.HealthSyncerActionType.ChangeValue:
				data.intSyncer.ChangeInt(data.amount);
				break;
			case HealthSyncerActionData.HealthSyncerActionType.SetValue:
				data.intSyncer.SetInt(data.amount);
				break;
			case HealthSyncerActionData.HealthSyncerActionType.DynamicFromDamageableTrigger:

				if(!(payload.data is int))
				{
					Debug.LogError($"Activator attempted to execute a DynamicFromDamageableTrigger but the payload data is not an int! (payload.data: {payload.data}, gameObject.name: {gameObject.name})");
					break;
				}

				data.intSyncer.ChangeInt((int)payload.data);
				break;
		}
	}
#endif
}

[System.Serializable]
public class AnimatorActionData
{
	public enum AnimatorActionType { SetFloat, SetRandomFloatFrom0To1, SetInteger, SetBool, SetTrigger, ResetTrigger };
	public AnimatorSyncer animatorSyncer;
	public AnimatorActionType actionType;
	public string parameter;
	[ConditionalHide(nameof(actionType), 0, true, false)]
	public float floatValue;
	[ConditionalHide(nameof(actionType), 2, true, false)]
	public int intValue;
	[ConditionalHide(nameof(actionType), 3, true, false)]
	public bool boolValue;
}

[System.Serializable]
public class HealthSyncerActionData
{
	public enum HealthSyncerActionType { ChangeValue, SetValue, DynamicFromDamageableTrigger };

	public HealthSyncer healthSyncer;
	public HealthSyncerActionType actionType;
	[ConditionalHide(nameof(actionType), 2, true, true)]
	public int amount;
}

[System.Serializable]
public class IntSyncerActionData
{
	public IntSyncer intSyncer;
	public HealthSyncerActionData.HealthSyncerActionType actionType;
	[ConditionalHide(nameof(actionType), 2, true, true)]
	public int amount;
}