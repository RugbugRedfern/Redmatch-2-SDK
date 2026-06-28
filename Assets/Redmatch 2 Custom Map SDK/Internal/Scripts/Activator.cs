
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using static ValueSource;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class Activator : ActivatorReferencer
{
	public enum ConditionalLogicType { NoRequirement, LessThan, LessThanOrEqualTo, EqualTo, MoreThanOrEqualTo, MoreThan, NotEqualTo };
	[Tooltip("If a requirement is set, this Activator will only activate if the conditional logic evaluates to true.")]
	public ConditionalLogicType conditionalLogicType;
	public ValueSource firstValue;
	public ValueSource secondValue;
	public enum PlayerFilterMode { NoFilter, NonPlayerOnly, PlayerOnly, LocalPlayerOnly, HostPlayerOnly }
	[Tooltip("If set, this activator can only activate when the triggering player (or object) matches the filter.")]
	public PlayerFilterMode playerFilter;
	public enum PlayerTeamRequirementType { NoRequirement, SpecificTeam, AnyTeamExcept };
	public PlayerTeamRequirementType playerTeamRequirementType;
	public Team requiredTeam;

	public enum PlayerStatRequirementType { NoRequirement, MoreThan, EqualTo, LessThan };
	public PlayerStatRequirementType playerStatRequirementType;
	public enum PlayerStat { Kills = 0, Deaths = 1, Points = 18 }
	public PlayerStat playerStat;
	public int requiredStat;

	public float actionDelay = 0f;
	public float cooldown = 0f;

	protected void BaseErrorChecks(ref List<string> errors)
	{
		if(conditionalLogicType != ConditionalLogicType.NoRequirement && !firstValue.Valid())
		{
			errors.Add("First value source is not valid.");
		}
		if(conditionalLogicType != ConditionalLogicType.NoRequirement && !secondValue.Valid())
		{
			errors.Add("Second value source is not valid.");
		}
	}

#if REDMATCH
	List<DelayedActivatorAction> delayedActions = new List<DelayedActivatorAction>();

	float nextTimeAllowedActivate;

	public void Activate(GameObject target = null, object data = null, MyceliumPlayer source = null)
	{
		if(!CheckCooldown())
			return;

		// Player filter logic
		switch(playerFilter)
		{
			case PlayerFilterMode.NonPlayerOnly:
				if(source != null)
					return;
				break;
			case PlayerFilterMode.PlayerOnly:
				if(source == null)
					return;
				break;
			case PlayerFilterMode.LocalPlayerOnly:
				if(source == null || !source.IsLocal)
					return;
				break;
			case PlayerFilterMode.HostPlayerOnly:
				if(source == null || !source.IsHost)
					return;
				break;

			default:
				break;
		}

		if(source != null)
		{
			// Check team requirement
			switch(playerTeamRequirementType)
			{
				case PlayerTeamRequirementType.SpecificTeam:
					{
						if(source.TryGetData(PlayerData.Team, out int teamInt))
						{
							Team team = (Team)teamInt;

							if(team != requiredTeam)
								return;
						}
						else // if they don't have a team for some reason, then they aren't this specific team!
						{
							return;
						}
						break;
					}
				case PlayerTeamRequirementType.AnyTeamExcept:
					{
						if(source.TryGetData(PlayerData.Team, out int teamInt))
						{
							Team team = (Team)teamInt;

							if(team == requiredTeam)
								return;
						}
						break;
					}
				default:
					break;
			}

			if(playerStatRequirementType != PlayerStatRequirementType.NoRequirement)
			{
				// Check stat requirement
				if(Enum.IsDefined(typeof(PlayerStat), (int)playerStat))
				{
					PlayerData playerData = (PlayerData)playerStat;

					int realValue = source.GetData<int>(playerData);

					switch(playerStatRequirementType)
					{
						case PlayerStatRequirementType.MoreThan:
							if(realValue <= requiredStat)
							{
								return;
							}
							break;
						case PlayerStatRequirementType.EqualTo:
							if(realValue != requiredStat)
							{
								return;
							}
							break;
						case PlayerStatRequirementType.LessThan:
							if(realValue >= requiredStat)
							{
								return;
							}
							break;
					}
				}
				else
				{
					Debug.LogError($"{requiredStat} is not a valid PlayerStat.");
				}
			}
		}

		var payload = new ActivatePayload() { target = target, data = data, source = source };

		switch(conditionalLogicType)
		{
			case ConditionalLogicType.LessThan:
				if(!(GetValue(firstValue, payload) < GetValue(secondValue, payload)))
					return;
				break;
			case ConditionalLogicType.LessThanOrEqualTo:
				if(!(GetValue(firstValue, payload) <= GetValue(secondValue, payload)))
					return;
				break;
			case ConditionalLogicType.EqualTo:
				if(!(GetValue(firstValue, payload) == GetValue(secondValue, payload)))
					return;
				break;
			case ConditionalLogicType.MoreThanOrEqualTo:
				if(!(GetValue(firstValue, payload) >= GetValue(secondValue, payload)))
					return;
				break;
			case ConditionalLogicType.MoreThan:
				if(!(GetValue(firstValue, payload) > GetValue(secondValue, payload)))
					return;
				break;
			case ConditionalLogicType.NotEqualTo:
				if(!(GetValue(firstValue, payload) != GetValue(secondValue, payload)))
					return;
				break;

			default:
				break;
		}

		if(actionDelay == 0)
		{
			InternalActivate(payload);
		}
		else
		{
			delayedActions.Add(new DelayedActivatorAction(payload, Time.time + actionDelay));
		}
	}

	protected abstract void InternalActivate(ActivatePayload payload);

	void Update()
	{
		for(int i = delayedActions.Count - 1; i >= 0; i--)
		{
			var action = delayedActions[i];

			if(Time.time > action.time)
			{
				InternalActivate(action.payload);

				delayedActions.RemoveAt(i);
			}
		}
	}

	bool CheckCooldown()
	{
		if(Time.time < nextTimeAllowedActivate)
		{
			Debug.Log("Ignored due to cooldown. Time left: " + (nextTimeAllowedActivate - Time.time));
			return false;
		}

		nextTimeAllowedActivate = Time.time + cooldown;
		return true;
	}

	/// <summary>
	/// This function is dumb and, if the source type is an IntSyncer or HealthSyncer, doesn't check if they are null or not (as that should have prevented map compilation)
	/// </summary>
	protected int GetValue(ValueSource source, ActivatePayload payload)
	{
		if(source.sourceType == SourceType.Constant)
			return source.constantInt;
		if(source.sourceType == SourceType.IntSyncer)
			return source.intSyncer.GetInt();
		if(source.sourceType == SourceType.HealthSyncer)
			return source.healthSyncer.GetHealth();
		if(source.sourceType == SourceType.RoomInfo)
		{
			switch(source.roomInfoType)
			{
				case RoomInfoType.CurrentPlayerCount:
					return MyceliumNetwork.PlayerCount;
				case RoomInfoType.MaxPlayerCount:
					return MyceliumNetwork.MaxPlayers;
				case RoomInfoType.CurrentTime:
					return Mathf.CeilToInt((float)GameMode_Base.GetRemainingTime());
				case RoomInfoType.TotalTime:
					return (int)(MyceliumNetwork.GetLobbyData<long>(LobbyData.RS_Time) * 60L);
				case RoomInfoType.TeamCount:
					return MyceliumNetwork.GetLobbyData<int>(LobbyData.RS_TeamCount);
				case RoomInfoType.IsTeamMode:
					return GamemodeManager.IsCurrentlyTeamMode() ? 1 : 0;
				case RoomInfoType.IsDeathmatch:
					return GamemodeManager.IsCurrentlyMode(GameMode.Deathmatch) ? 1 : 0;
				case RoomInfoType.IsKOTH:
					return GamemodeManager.IsCurrentlyMode(GameMode.KingOfTheHill) ? 1 : 0;
				case RoomInfoType.IsTDM:
					return GamemodeManager.IsCurrentlyMode(GameMode.TeamDeathmatch) ? 1 : 0;
				case RoomInfoType.IsOITC:
					return GamemodeManager.IsCurrentlyMode(GameMode.OneInTheChamber) ? 1 : 0;
				case RoomInfoType.IsInfection:
					return GamemodeManager.IsCurrentlyMode(GameMode.Infection) ? 1 : 0;
			}
		}
		if(source.sourceType == SourceType.FromTriggerSource)
		{
			if(payload.source == null)
				return 0;

			switch(source.triggerSourceType)
			{
				case (TriggerSourceType.Kills):
					return payload.source.GetData<int>(PlayerData.Kills);
				case (TriggerSourceType.Deaths):
					return payload.source.GetData<int>(PlayerData.Deaths);
				case (TriggerSourceType.Points):
					return payload.source.GetData<int>(PlayerData.Points);
				case (TriggerSourceType.TruncatedSteamID):
					return (int)(payload.source.SteamID.m_SteamID);
			}
		}

		return 0;
	}

	protected bool GetBool(ValueSource source, ActivatePayload payload)
	{
		return GetValue(source, payload) >= 1;
	}

	[System.Serializable] // for debugging
	protected struct DelayedActivatorAction
	{
		public ActivatePayload payload;
		public float time;

		public DelayedActivatorAction(ActivatePayload payload, float time)
		{
			this.payload = payload;
			this.time = time;
		}
	}

	[System.Serializable] // for debugging
	protected struct ActivatePayload
	{
		public GameObject target;
		public MyceliumPlayer source;
		public object data;
	}
#endif

#if UNITY_EDITOR
	List<ActivatorReferencer> cachedReferencers;

	public void RecacheReferences()
	{
		cachedReferencers = null;
	}

	void OnDrawGizmosSelected()
	{
		if(cachedReferencers == null)
		{
			cachedReferencers = new List<ActivatorReferencer>();

			foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				foreach(var reference in root.GetComponentsInChildren<ActivatorReferencer>())
				{
					if(reference.GetReferences().Contains(this))
					{
						cachedReferencers.Add(reference);
					}
				}
			}
		}

		Gizmos.color = Color.white;
		foreach(var reference in cachedReferencers)
		{
			if(reference)
			{
				Gizmos.DrawSphere(reference.transform.position, 0.3f);
				Gizmos.DrawLine(reference.transform.position, transform.position);
				Handles.Label(reference.transform.position + Vector3.up * 0.7f, $"Triggered by\n{reference}");
			}
		}
	}
#endif
}

[System.Serializable]
public class ValueSource
{
	public enum SourceType { Constant, IntSyncer, HealthSyncer, RoomInfo, FromTriggerSource }
	// Team count index is 32, in case extra RoomInfo types ever need to be inserted later on
	public enum RoomInfoType { CurrentPlayerCount, MaxPlayerCount, CurrentTime, TotalTime, TeamCount = 32, IsTeamMode, IsDeathmatch, IsKOTH, IsTDM, IsOITC, IsInfection }
	public enum TriggerSourceType { Kills, Deaths, Points, TruncatedSteamID }
	public SourceType sourceType;
	[ConditionalHide(nameof(sourceType), 0, true, false)]
	public int constantInt;
	[ConditionalHide(nameof(sourceType), 1, true, false)]
	public IntSyncer intSyncer;
	[ConditionalHide(nameof(sourceType), 2, true, false)]
	public HealthSyncer healthSyncer;
	[ConditionalHide(nameof(sourceType), 3, true, false)]
	public RoomInfoType roomInfoType;
	[ConditionalHide(nameof(sourceType), 4, true, false)]
	public TriggerSourceType triggerSourceType;

	public bool Valid()
	{
		if(sourceType == SourceType.IntSyncer && intSyncer == null)
			return false;

		if(sourceType == SourceType.HealthSyncer && healthSyncer == null)
			return false;

		return true;
	}
}