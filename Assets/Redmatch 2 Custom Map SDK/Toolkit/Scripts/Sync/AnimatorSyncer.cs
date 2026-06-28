
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MyceliumIdentity))]
public class AnimatorSyncer : SyncedBehaviour
{
	public bool desyncProtection = true;

	void Reset()
	{
		GetComponent<Animator>().updateMode = AnimatorUpdateMode.AnimatePhysics;
	}

#if REDMATCH
	Animator animator;
	MyceliumIdentity identity;

	float nextTimeToSync;
	float syncDelay = 5f; // 5 seconds between sync, just to keep everything tip top in case there are desyncs

	public void Initialize()
	{
		animator = GetComponent<Animator>();

		if(!sync)
			return;

		identity = GetComponent<MyceliumIdentity>();

		var securityLevel = serverOnly ? RPCSecurityLevel.Host : RPCSecurityLevel.Anyone;
		identity.RegisterRPC(85, RPC_SetFloat, securityLevel);
		identity.RegisterRPC(86, RPC_SetInteger, securityLevel);
		identity.RegisterRPC(87, RPC_SetBool, securityLevel);
		identity.RegisterRPC(88, RPC_SetTrigger, securityLevel);
		identity.RegisterRPC(89, RPC_ResetTrigger, securityLevel);
		identity.RegisterRPC(90, RPC_SyncState, RPCSecurityLevel.Host);
		identity.RegisterRPC(91, RPC_RequestState, RPCSecurityLevel.Anyone);

		// get the current state, as up-to-date as possible, from the host.
		if(!MyceliumNetwork.IsHost)
		{
			MyceliumNetwork.RPC(identity.GetRPCMessage(91), MyceliumNetwork.LobbyHost, ReliableType.Reliable);
		}
	}

	void Update()
	{
		if(!sync)
			return;

		if(MyceliumNetwork.IsHost && desyncProtection)
		{
			if(Time.realtimeSinceStartup > nextTimeToSync)
			{
				MyceliumNetwork.RPCOthers(GetStateSyncMessage(), ReliableType.Unreliable);
				nextTimeToSync = Time.realtimeSinceStartup + syncDelay;
			}
		}
	}

	Message GetStateSyncMessage()
	{
		Message message = identity.GetRPCMessage(90);

		for(int i = 0; i < animator.layerCount; i++)
		{
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(i);
			message.WriteInt(info.shortNameHash);
			message.WriteFloat(info.normalizedTime);
		}

		return message;
	}

	void RPC_SyncState(Message message, MyceliumPlayer sender)
	{
		for(int i = 0; i < animator.layerCount; i++)
		{
			int stateNameHash = message.ReadInt();
			float normalizedTime = message.ReadFloat();

			animator.Play(stateNameHash, i, normalizedTime);
		}
	}

	void RPC_RequestState(Message message, MyceliumPlayer sender)
	{
		MyceliumNetwork.RPC(GetStateSyncMessage(), sender, ReliableType.Reliable);
	}

	#region Float
	public void SetFloat(string parameter, float value)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			SyncFloat(parameter, value);
		}
		else
		{
			animator.SetFloat(parameter, value);
		}
	}

	public void SetRandomFloatFrom0To1(string parameter)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			SyncFloat(parameter, Random.Range(0f, 1f));
		}
		else
		{
			animator.SetFloat(parameter, Random.Range(0f, 1f));
		}
	}

	void SyncFloat(string parameter, float value)
	{
		MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetFloat).WriteInt(Animator.StringToHash(parameter)).WriteFloat(value), ReliableType.Reliable);
	}

	void RPC_SetFloat(Message message, MyceliumPlayer player)
	{
		animator.SetFloat(message.ReadInt(), message.ReadFloat());
	}
	#endregion

	#region Int
	public void SetInteger(string parameter, int value)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetInteger).WriteInt(Animator.StringToHash(parameter)).WriteInt(value), ReliableType.Reliable);
		}
		else
		{
			animator.SetInteger(parameter, value);
		}
	}

	void RPC_SetInteger(Message message, MyceliumPlayer player)
	{
		animator.SetInteger(message.ReadInt(), message.ReadInt());
	}
	#endregion

	#region Bool
	public void SetBool(string parameter, bool value)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetBool).WriteInt(Animator.StringToHash(parameter)).WriteBool(value), ReliableType.Reliable);

		}
		else
		{
			animator.SetBool(parameter, value);
		}
	}

	void RPC_SetBool(Message message, MyceliumPlayer player)
	{
		animator.SetBool(message.ReadInt(), message.ReadBool());
	}
	#endregion

	#region SetTrigger
	public void SetTrigger(string parameter)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetTrigger).WriteInt(Animator.StringToHash(parameter)), ReliableType.Reliable);

		}
		else
		{
			animator.SetTrigger(parameter);
		}
	}

	void RPC_SetTrigger(Message message, MyceliumPlayer player)
	{
		animator.SetTrigger(message.ReadInt());
	}
	#endregion

	#region ResetTrigger
	public void ResetTrigger(string parameter)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_ResetTrigger).WriteInt(Animator.StringToHash(parameter)), ReliableType.Reliable);
		}
		else
		{
			animator.ResetTrigger(parameter);
		}
	}

	void RPC_ResetTrigger(Message message, MyceliumPlayer player)
	{
		animator.ResetTrigger(message.ReadInt());
	}
	#endregion
#endif
}
