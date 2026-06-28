
using System.Collections.Generic;

public class HostActivator : Activator
{
	public enum WinLogicType { MostKills, MostPoints, MostDeaths, TriggerSource }
	public bool endMatch;
	public bool enableCustomWinLogic;
	public WinLogicType winLogicType;
	public bool doDynamicTeamWinLogic;
	public Team customWinTeam;
	public bool draw;

#if REDMATCH
	protected override void InternalActivate(ActivatePayload payload)
	{
		if(!MyceliumNetwork.IsHost)
			return;

		if(endMatch)
		{
			EndGame(payload);
		}
	}

	void EndGame(ActivatePayload payload)
	{
		if(!MyceliumNetwork.IsHost)
			return;
		if((LobbyState)MyceliumNetwork.GetLobbyData<int>(LobbyData.Status) == LobbyState.Ended)
			return;

		if(enableCustomWinLogic)
		{
			CustomWinLogic(payload);
		}

		MyceliumNetwork.SetLobbyData(LobbyData.EndTime, 0f);
		MyceliumNetwork.SetLobbyData(LobbyData.Status, (int)LobbyState.Ended);
	}

	void CustomWinLogic(ActivatePayload payload)
	{
		if(GamemodeManager.IsCurrentlyTeamMode())
		{
			int winningTeamIndex = (int)customWinTeam;
			if(draw)
			{
				winningTeamIndex = -1;
			}
			if(doDynamicTeamWinLogic)
			{
				winningTeamIndex = GetDynamicWinningTeam(payload);
			}
			MyceliumNetwork.SetLobbyData(LobbyData.CustomWinnerTeamIndex, winningTeamIndex);
		}
		else
		{
			MyceliumNetwork.SetLobbyData(LobbyData.CustomWinner, draw ? 0 : GetDynamicWinningPlayer(payload));
		}
	}

	int GetPlayerContribution(MyceliumPlayer player, WinLogicType winLogicType)
	{
		switch(winLogicType)
		{
			case WinLogicType.MostKills:
				return player.GetData<int>(PlayerData.Kills);
			case WinLogicType.MostPoints:
				return player.GetData<int>(PlayerData.Points);
			case WinLogicType.MostDeaths:
				return player.GetData<int>(PlayerData.Deaths);
		}

		return 0;
	}

	int GetDynamicWinningTeam(ActivatePayload payload)
	{
		if(winLogicType == WinLogicType.TriggerSource)
		{
			if(payload.source == null)
			{
				return -1;
			}

			return payload.source.GetData<int>(PlayerData.Team);
		}
		Dictionary<Team, int> teamPoints = new Dictionary<Team, int>();
		foreach(MyceliumPlayer player in MyceliumNetwork.Players)
		{
			Team playerTeam = (Team)player.GetData<int>(PlayerData.Team);
			int playerContribution = GetPlayerContribution(player, winLogicType);
			if(teamPoints.ContainsKey(playerTeam))
			{
				teamPoints[playerTeam] += playerContribution;
			}
			else
			{
				teamPoints.Add(playerTeam, playerContribution);
			}
		}

		Team first = Team.Red;
		Team second = Team.Blue;

		int i = 0;
		foreach(Team team in teamPoints.Keys)
		{
			if(i == 0)
			{
				first = team;

				if(teamPoints.Keys.Count == 1)
				{
					second = team;
				}
			}
			if(i == 1)
			{
				second = team;
			}
			if(i >= 2)
				break;

			i++;
		}

		foreach(Team team in teamPoints.Keys)
		{
			if(teamPoints[team] > teamPoints[first])
			{
				second = first;
				first = team;
			}
		}

		if(teamPoints[first] == teamPoints[second])
		{
			return -1;
		}
		else
		{
			return (int)first;
		}
	}

	ulong GetDynamicWinningPlayer(ActivatePayload payload)
	{
		if(winLogicType == WinLogicType.TriggerSource)
		{
			if(payload.source == null)
			{
				return 0;
			}

			return payload.source.SteamID.m_SteamID;
		}

		MyceliumPlayer first = MyceliumNetwork.LocalPlayer;
		MyceliumPlayer second = null;

		foreach(var player in MyceliumNetwork.Players)
		{
			if(!VisiblePlayerManager.IsPlayerVisible(player))
				continue;

			if(GetPlayerContribution(player, winLogicType) > GetPlayerContribution(first, winLogicType))
			{
				second = first;
				first = player;
			}
		}

		if(second != null && GetPlayerContribution(first, winLogicType) == GetPlayerContribution(second, winLogicType))
		{
			return 0;
		}
		else
		{
			return first.SteamID.m_SteamID;
		}
	}
#endif
}
