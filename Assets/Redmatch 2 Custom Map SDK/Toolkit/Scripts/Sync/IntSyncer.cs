
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class IntSyncer : SyncedBehaviour
{
	[Tooltip("Min -2147483648, Max +2147483648")]
	public int startingValue;
	public ValueDisplay[] valueDisplays = new ValueDisplay[0];
	[Tooltip("OnSet activators are triggered when the value is modified in any way (whether by a 'Set' or 'Change' operation).")]
	public Activator[] OnSet = new Activator[0];
	public Activator[] OnChanged = new Activator[0];

	public override IEnumerable<Activator> GetReferences()
	{
		foreach(var x in OnSet)
			yield return x;
		foreach(var x in OnChanged)
			yield return x;
	}


	public override void RunErrorChecks(ref List<string> errors)
	{
		base.RunErrorChecks(ref errors);

		foreach(var reference in GetReferences())
		{
			if(!reference)
			{
				errors.Add("A specified object is null.");
			}
		}

		foreach(var display in valueDisplays)
		{
			if(!display)
			{
				errors.Add("A specified object is null.");
			}
		}
	}

#if REDMATCH
	int value;
	MyceliumIdentity identity;

	public void Initialize()
	{
		var securityLevel = serverOnly ? RPCSecurityLevel.Host : RPCSecurityLevel.Anyone;

		identity = GetComponent<MyceliumIdentity>();
		identity.RegisterRPC(219, RPC_RequestInt, RPCSecurityLevel.Anyone);
		identity.RegisterRPC(220, RPC_SetInt, RPCSecurityLevel.Anyone);
		identity.RegisterRPC(221, RPC_ChangeInt, RPCSecurityLevel.Anyone);

		value = startingValue;

		if(sync && !MyceliumNetwork.IsHost)
		{
			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_RequestInt), MyceliumNetwork.LobbyHost, ReliableType.Reliable);
		}
	}

	// client -> server
	void RPC_RequestInt(Message message, MyceliumPlayer sender)
	{
		MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetInt).WriteInt(value), sender, ReliableType.Reliable);
	}

	// server -> client
	void RPC_SetInt(Message message, MyceliumPlayer player)
	{
		InternalSetInt(message.ReadInt());
	}

	void RPC_ChangeInt(Message message, MyceliumPlayer player)
	{
		if(!gameObject.activeSelf)
			return;

		int change = message.ReadInt();

		InternalSetInt(value + change);

		foreach(var activator in OnChanged)
		{
			activator.Activate(gameObject);
		}
	}

	void InternalSetInt(int value)
	{
		this.value = value;

		foreach(var activator in OnSet)
		{
			activator.Activate(gameObject);
		}

		UpdateValueDisplays();
	}

	public void SetInt(int value)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetInt).WriteInt(value), ReliableType.Reliable);
		}
		else
		{
			InternalSetInt(value);
		}
	}

	public void ChangeInt(int change)
	{
		if(change == 0)
			return;

		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_ChangeInt).WriteInt(change), ReliableType.Reliable);
		}
		else
		{
			InternalSetInt(value + change);

			foreach(var activator in OnChanged)
			{
				activator.Activate(gameObject);
			}
		}
	}

	void UpdateValueDisplays()
	{
		foreach(var valueDisplay in valueDisplays)
		{
			valueDisplay.DisplayValue(value);
		}
	}

	public int GetInt()
	{
		return value;
	}
#endif
}
