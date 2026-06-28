
using UnityEngine;

[RequireComponent(typeof(MyceliumIdentity))]
public class InputTrigger : CooldownTrigger
{
	public enum InputTriggerType { Pressed, Held, Released }
	public enum InputTriggerKey { forward, backward, left, right, jump, interact, pushToTalk, upgradeUp, upgradeDown, buyUpgrade, shoot, aim, melee, reload, flashlight, sprint, scoreboard, nextWeapon, previousWeapon, item1, item2, item3, item4, item5, hideUI, grappleLeft, grappleRight, restartTimeTrial, altEscapeMenu }
	public enum InputTriggerAxis { MouseX, MouseY, MouseScrollWheel }
	[Tooltip("Whether or not to send the inputs to other players.")]
	public bool sync = true;
	[Tooltip("If true (and synced is true), inputs will only be sent to the host, and not executed locally.")]
	public bool hostOnly;
	public bool useAxisInput;
	public InputTriggerKey inputKey;
	public InputTriggerAxis inputAxis;
	public InputTriggerType inputType;
	[Tooltip("Repeat delay for axis / held input types.")]
	public float repeatDelay = 1f;

	public override string GetTarget()
	{
		return "The Player that pressed / held the given input.";
	}

#if REDMATCH
	static readonly string[] triggerAxisConversionTable = { "Mouse X", "Mouse Y", "Mouse ScrollWheel" };
	MyceliumIdentity identity;

	public void Initialize()
	{
		identity = GetComponent<MyceliumIdentity>();

		identity.RegisterRPC(218, RPC_SyncInput, RPCSecurityLevel.Anyone);
	}

	float timer;
	bool prevAxisState;
	bool queuedAxisImpulse; // If using an axis input + pressed / released input type, this value is used to queue a press / release for the next Update call.

	float GetClampedRepeatDelay()
	{
		return Mathf.Max(repeatDelay, 0.01f);
	}

	private void Update()
	{
		// Axis handling mode
		if(useAxisInput)
		{
			bool curAxis = Input.GetAxisRaw(triggerAxisConversionTable[(int)inputAxis]) != 0;

			switch(inputType)
			{
				case InputTriggerType.Pressed:
					if(curAxis && !prevAxisState)
					{
						SendInputRPC();
					}

					prevAxisState = curAxis;
					return;

				case InputTriggerType.Held:
					int maxLoops = 5;
					timer += Time.deltaTime;

					while(timer >= GetClampedRepeatDelay())
					{
						if(Input.GetAxisRaw(triggerAxisConversionTable[(int)inputAxis]) != 0)
						{
							SendInputRPC();
						}

						timer -= GetClampedRepeatDelay();

						maxLoops--;

						if(maxLoops <= 0) break;
					}
					return;

				case InputTriggerType.Released:
					if(!curAxis && prevAxisState)
					{
						SendInputRPC();
					}

					prevAxisState = curAxis;
					return;
			}

			return;
		}

		// Key handling mode
		switch(inputType)
		{
			case InputTriggerType.Pressed:
				if(InputManager.Instance.GetButtonDown(inputKey.ToString()))
				{
					SendInputRPC();
				}
				return;

			case InputTriggerType.Held:
				int maxLoops = 5;
				timer += Time.deltaTime;

				while(timer >= GetClampedRepeatDelay())
				{
					if(InputManager.Instance.GetButton(inputKey.ToString()))
					{
						SendInputRPC();
					}

					timer -= GetClampedRepeatDelay();

					maxLoops--;

					if(maxLoops <= 0) break;
				}
				return;

			case InputTriggerType.Released:
				if(InputManager.Instance.GetButtonUp(inputKey.ToString()))
				{
					SendInputRPC();
				}
				return;
		}
	}

	// axisSign to be utilised fully later
	void SendInputRPC(bool axisSign = false)
	{
		if(!sync)
		{
			Activate(PlayerController.LocalInstance?.gameObject, MyceliumNetwork.LocalPlayer);
			return;
		}

		Message rpcMessage = identity.GetRPCMessage(RPC_SyncInput);
		rpcMessage.WriteBool(axisSign);

		if(hostOnly)
		{
			MyceliumNetwork.RPC(rpcMessage, MyceliumNetwork.LobbyHost, ReliableType.Reliable);
			return;
		}

		MyceliumNetwork.RPC(rpcMessage, ReliableType.Reliable);
	}

	void RPC_SyncInput(Message message, MyceliumPlayer sender)
	{
		if(!gameObject.activeSelf)
			return;

		var player = MyceliumPrefabManager.GetPlayerIdentity(sender);

		Activate(player?.gameObject, sender);
	}
#endif
}
