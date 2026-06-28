using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TargetActivator)), CanEditMultipleObjects]
public class TargetActivatorEditor : ErrorCollectionBehaviourEditor
{
	TargetActivator activator;

	void OnEnable()
	{
		activator = (TargetActivator)target;
	}

	public override void OnInspectorGUI()
	{
		if(target == null)
			return;


		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.targetMode)), new GUIContent("Target Mode", "Dynamic: The target is whatever triggered the Trigger. Some triggers don't have targets, like the Round Started trigger. Preset: Specify a preset target"), true);
		EditorGUI.indentLevel++;
		EditorGUI.BeginDisabledGroup(activator.targetMode != TargetActivator.TargetActivatorTargetMode.Preset);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.presetTarget)), new GUIContent("Preset Target", "The preset target to execute all actions on"), true);
		EditorGUI.EndDisabledGroup();
		EditorGUI.indentLevel--;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.actionDelay)), new GUIContent("Delay", "The delay time in seconds between an activator being triggered and its actions executing."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.cooldown)), new GUIContent("Cooldown", "The cooldown in seconds before an action can be triggered again. This value is not synced, and is calculated locally for each player."), true);
		EditorGUILayout.EndVertical();

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Target Actions", EditorStyles.boldLabel);
		EditorGUILayout.HelpBox("\"Target Actions\" apply to a specific Target that is triggering this Activator. For example, a Player that enters an EnterTrigger which triggers this Activator will be the Target. You can use these to apply actions specifically to the Target, like teleporting them or dealing damage. Unlike Generic Actions, Target Actions do not have an authority requirement. For players, they will be executed on that player's client (locally). For Physics Objects, they will be executed based on the RigidbodySyncer's sync settings.", MessageType.Info);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.affectPlayers)), new GUIContent("Affect Players", ""), true);
		EditorGUI.indentLevel = 2;
		EditorGUI.BeginDisabledGroup(!activator.affectPlayers);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.playerDamage)), new GUIContent("Damage", "Damage to deal to any target player"), true);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.Space();

		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.applyForce)), new GUIContent("Apply Force", ""), true);
		EditorGUI.indentLevel = 2;
		EditorGUI.BeginDisabledGroup(!activator.applyForce);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.force)), new GUIContent("Force", "The amount of force to add to the Rigidbody."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.forceMode)), new GUIContent("Force Mode", "Force: Add a continuous force to the rigidbody, using its mass.\nAcceleration: Add a continuous acceleration to the rigidbody, ignoring its mass.\nImpulse: Add an instant force impulse to the rigidbody, using its mass.\nVelocity Change: Add an instant velocity change to the rigidbody, ignoring its mass.\nVelocity Overwrite: Directly replace the velocity of the rigidbody."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.relativeForceMode)), new GUIContent("Relative Force Mode", "World: Applies force relative to the world coordinate system. Self: Applies force relative to this Activator's local coordinate system. Target: Applies force relative to the target's coordinate system. Orbital: Applies force aligning to the direction between the target and this Activator. Only the Z parameter is used for this force."), true);
		EditorGUI.EndDisabledGroup();
		EditorGUI.indentLevel = 1;

		EditorGUILayout.Space();

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.teleportToOriginalTarget)), new GUIContent("Teleport to Original Target", "Use this activator's original target as the teleport location (e.g. Player / object triggering an Enter/Stay/Exit trigger). Ensure \"Target Mode\" is set to \"Preset\", otherwise this will do nothing."), true);
		if(activator.teleportToOriginalTarget == false)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.teleport)), new GUIContent("Teleport Location", "The Transform which the Rigidbody will be teleported to"), true);
		}
		else
		{
			EditorGUI.indentLevel = 2;
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.teleportToPlayerCamera)), new GUIContent("Teleport to Player Camera", "If this Activator is triggered by a player, use their camera transform as the teleport location instead of the player transform."), true);
		}
		EditorGUI.indentLevel = 2;
		EditorGUI.BeginDisabledGroup(activator.teleport == null && activator.teleportToOriginalTarget == false);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.useTeleportLocationRotation)), new GUIContent("Use Teleport Location Rotation", "Set the rotation of the target to the rotation of the teleport target when teleporting."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.keepOffsetWhenTeleporting)), new GUIContent("Keep Offset When Teleporting", "Preserve the offset that the target had when entering the teleport and use it as an offset for the teleport destination. This can be used for seamless teleporting."), true);
		EditorGUI.EndDisabledGroup();
		EditorGUI.indentLevel = 1;
		EditorGUILayout.EndVertical();

		ActivatorEditorCommon.DrawCommon(serializedObject, activator);

		RunErrorChecks();

		serializedObject.ApplyModifiedProperties();
	}
}
