
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class HealthSyncer : SyncedBehaviour
{
	public int maxHealth = 100;
	public int health = 100;
	public ValueDisplay[] healthDisplays = new ValueDisplay[0];
	public Activator[] OnDamaged = new Activator[0];
	public Activator[] OnHealed = new Activator[0];
	public Activator[] OnDied = new Activator[0];

	public override IEnumerable<Activator> GetReferences()
	{
		foreach(var x in OnDamaged)
			yield return x;
		foreach(var x in OnHealed)
			yield return x;
		foreach(var x in OnDied)
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

		foreach(var display in healthDisplays)
		{
			if(!display)
			{
				errors.Add("A specified object is null.");
			}
		}
	}

#if REDMATCH
	MyceliumIdentity identity;

	public void Initialize()
	{
		var securityLevel = serverOnly ? RPCSecurityLevel.Host : RPCSecurityLevel.Anyone;

		identity = GetComponent<MyceliumIdentity>();
		identity.RegisterRPC(192, RPC_RequestHealth, RPCSecurityLevel.Anyone);
		identity.RegisterRPC(193, RPC_SetHealth, RPCSecurityLevel.Anyone);
		identity.RegisterRPC(194, RPC_ChangeHealth, securityLevel);

		health = Mathf.Clamp(health, 0, maxHealth);

		if(sync && !MyceliumNetwork.IsHost)
		{
			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_RequestHealth), MyceliumNetwork.LobbyHost, ReliableType.Reliable);
		}
	}

	// client -> server
	void RPC_RequestHealth(Message message, MyceliumPlayer sender)
	{
		MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetHealth).WriteInt(health), sender, ReliableType.Reliable);
	}

	// server -> client
	void RPC_SetHealth(Message message, MyceliumPlayer player)
	{
		InternalSetHealth(message.ReadInt());
	}

	void InternalSetHealth(int health)
	{
		this.health = health;

		health = Mathf.Clamp(health, 0, maxHealth);

		UpdateHealthDisplays();
	}

	void RPC_ChangeHealth(Message message, MyceliumPlayer player)
	{
		if(!gameObject.activeSelf)
			return;

		int change = message.ReadInt();

		InternalChangeHealth(change);
	}

	void InternalChangeHealth(int change)
	{
		if(change == 0)
			return;

		health += change;

		health = Mathf.Clamp(health, 0, maxHealth);

		if(health <= 0)
		{
			foreach(var activator in OnDied)
			{
				activator.Activate(gameObject);
			}
		}
		else
		{
			if(change < 0)
			{
				foreach(var activator in OnDamaged)
				{
					activator.Activate(gameObject);
				}
			}
			else
			{
				foreach(var activator in OnHealed)
				{
					activator.Activate(gameObject);
				}
			}
		}

		UpdateHealthDisplays();
	}

	public void SetHealth(int amount)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_SetHealth).WriteInt(amount), ReliableType.Reliable);
		}
		else
		{
			InternalSetHealth(amount);
		}
	}

	public void ChangeHealth(int amount)
	{
		if(sync)
		{
			if(serverOnly)
			{
				if(!MyceliumNetwork.IsHost)
					return;
			}

			MyceliumNetwork.RPC(identity.GetRPCMessage(RPC_ChangeHealth).WriteInt(amount), ReliableType.Reliable);
		}
		else
		{
			InternalChangeHealth(amount);
		}
	}

	void UpdateHealthDisplays()
	{
		foreach(var healthDisplay in healthDisplays)
		{
			healthDisplay.DisplayValue(health, maxHealth);
		}
	}

	public int GetHealth()
	{
		return health;
	}
#endif
}