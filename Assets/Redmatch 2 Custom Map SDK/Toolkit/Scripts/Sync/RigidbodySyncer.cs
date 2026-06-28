
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
[RequireComponent(typeof(Rigidbody))]
public class RigidbodySyncer : SyncedBehaviour
{
	public bool syncPosition = true;
	public bool syncRotation = true;
	[Range(1f, 12f)]public int syncRate = 12;

#if REDMATCH
	public MyceliumIdentity Identity { get; private set; }
	[HideInInspector] public Rigidbody Rigidbody { get; private set; }

	float nextTimeToSync;

	Vector3 targetPosition;
	Quaternion targetRotation;

	float clampedSyncRate => Mathf.Clamp(syncRate, 1f, 12f);

	bool first = true;

	void Awake()
	{
		Rigidbody = GetComponent<Rigidbody>();

		if(!sync)
			return;

		Identity = GetComponent<MyceliumIdentity>();

		Identity.RegisterRPC(215, RPC_SendData, serverOnly ? RPCSecurityLevel.Host : RPCSecurityLevel.Anyone);
		Identity.RegisterRPC(222, RPC_Teleport, RPCSecurityLevel.Anyone);

		MyceliumNetwork.PlayerLeft += OnPlayerLeft;
	}

	void Start()
	{
		if(!sync)
			return;

		UpdateHost();
	}

	void OnDestroy()
	{
		if(!sync)
			return;

		MyceliumNetwork.PlayerLeft -= OnPlayerLeft;
	}

	void OnPlayerLeft(MyceliumPlayer player)
	{
		if(!sync)
			return;

		UpdateHost();
	}

	void UpdateHost()
	{
		if(!sync)
			return;

		Rigidbody.isKinematic = !MyceliumNetwork.IsHost;
	}

	void Update()
	{
		if(!sync)
			return;

		if(MyceliumNetwork.IsHost)
		{
			if(Time.realtimeSinceStartup > nextTimeToSync)
			{
				SendData();

				nextTimeToSync = Time.realtimeSinceStartup + (1f / clampedSyncRate);
			}
		}
		else
		{
			if(syncPosition)
			{
				transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * clampedSyncRate);
			}

			if(syncRotation)
			{
				transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, Quaternion.Angle(transform.localRotation, targetRotation) * Time.deltaTime * 60 / clampedSyncRate * 2);
			}
		}
	}

	void SendData()
	{
		if(!sync)
			return;

		Message message = Identity.GetRPCMessage(RPC_SendData);

		if(syncPosition)
		{
			message.WriteVector3(transform.position);
		}

		if(syncRotation)
		{
			message.WriteQuaternion(transform.rotation);
		}

		MyceliumNetwork.RPCOthers(message, ReliableType.Unreliable);
	}

	void RPC_SendData(Message message, MyceliumPlayer sender)
	{
		if(!sync)
			return;

		if(syncPosition)
		{
			targetPosition = message.ReadVector3();
		}

		if(syncRotation)
		{
			targetRotation = message.ReadQuaternion();
		}

		if(first || (sync && !serverOnly && MyceliumNetwork.IsHost))
		{
			if(syncPosition)
			{
				transform.position = targetPosition;
			}

			if(syncRotation)
			{
				transform.rotation = targetRotation;
			}

			first = false;
		}
	}

	void RPC_Teleport(Message message, MyceliumPlayer sender)
	{
		if(!sync || serverOnly)
			return;
		if(!MyceliumNetwork.IsHost)
			return;

		transform.position = message.ReadVector3();
		transform.rotation = message.ReadQuaternion();
	}


	public void Teleport(Vector3 pos, Quaternion rot)
	{
		if(!sync || MyceliumNetwork.IsHost)
		{
			transform.position = pos;
			transform.rotation = rot;
			return;
		}
		if(!sync && serverOnly && !MyceliumNetwork.IsHost)
			return;

		Message message = Identity.GetRPCMessage(RPC_Teleport);
		message.WriteVector3(pos);
		message.WriteQuaternion(rot);

		// if sync is on, and serverOnly is off, tell the host to teleport this rigidbody.
		if(Time.realtimeSinceStartup > nextTimeToSync)
		{
			MyceliumNetwork.RPC(message, MyceliumNetwork.LobbyHost, ReliableType.Reliable);

			nextTimeToSync = Time.realtimeSinceStartup + (1f / clampedSyncRate);
		}
	}
#endif
}
