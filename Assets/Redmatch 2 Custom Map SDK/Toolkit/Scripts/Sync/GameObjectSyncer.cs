
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class GameObjectSyncer : SyncedBehaviour
{
#if REDMATCH
	MyceliumIdentity identity;

	bool eatNext = true;

	public void Initialize()
	{
		if(!sync)
			return;

		// eatNext has to be true by default because OnEnable is called before
		// Initialize. So if we are currently disabled, we don't want to eat the
		// next OnEnable call.
		if(!gameObject.activeSelf)
		{
			eatNext = false;
		}

		identity = GetComponent<MyceliumIdentity>();

		var securityLevel = serverOnly ? RPCSecurityLevel.Host : RPCSecurityLevel.Anyone;

		identity.RegisterRPC(216, RPC_RequestActiveState, RPCSecurityLevel.Anyone);
		identity.RegisterRPC(217, RPC_SendActiveState, securityLevel);

		if(!MyceliumNetwork.IsHost)
		{
			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_RequestActiveState), MyceliumNetwork.LobbyHost, ReliableType.Reliable);
		}
	}

	void OnEnable()
	{
		if(eatNext)
		{
			eatNext = false;
			return;
		}

		if(!sync)
			return;

		if(serverOnly)
		{
			if(!MyceliumNetwork.IsHost)
				return;
		}

		SendActiveState(true);
	}

	void OnDisable()
	{
		if(eatNext)
		{
			eatNext = false;
			return;
		}

		if(!sync)
			return;

		if(serverOnly)
		{
			if(!MyceliumNetwork.IsHost)
				return;
		}

		SendActiveState(false);
	}

	void SendActiveState(bool active)
	{
		MyceliumNetwork.RPCOthers(identity.GetRPCMessage(RPC_SendActiveState).WriteBool(active), ReliableType.Reliable);
	}

	void RPC_RequestActiveState(Message message, MyceliumPlayer sender)
	{
		MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SendActiveState).WriteBool(gameObject.activeSelf), sender, ReliableType.Reliable);
	}

	void RPC_SendActiveState(Message message, MyceliumPlayer sender)
	{
		bool active = message.ReadBool();
		eatNext = true;
		gameObject.SetActive(active);
	}
#endif
}
