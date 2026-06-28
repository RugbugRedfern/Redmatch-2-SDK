using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HostActivator)), CanEditMultipleObjects]
public class HostActivatorEditor : ErrorCollectionBehaviourEditor
{
	HostActivator activator;

	void OnEnable()
	{
		activator = (HostActivator)target;
	}

	public override void OnInspectorGUI()
	{
		if(target == null)
			return;

		EditorGUI.indentLevel = 0;
		EditorGUILayout.HelpBox("All logic in this activator is only able to run on the host. It is recommended to have all logic related to ending the game (including related activators and triggers) be handled by the host.", MessageType.Info);
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.endMatch)), new GUIContent("End Match", "Whether or not to end the current match when triggered."), true);
		EditorGUI.BeginDisabledGroup(!activator.endMatch);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.enableCustomWinLogic)), new GUIContent("Enable Custom Win Logic", "Whether or not to run custom win logic."), true);
		EditorGUI.EndDisabledGroup();
		EditorGUI.indentLevel++;
		EditorGUI.BeginDisabledGroup(!activator.endMatch || !activator.enableCustomWinLogic);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.winLogicType)), new GUIContent("Win Logic Type", "Which stat should be used to determine the winner, if using dynamic win logic."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.doDynamicTeamWinLogic)), new GUIContent("Use Dynamic Team Win Logic", "Whether to use dynamic team win logic, or to use the fixed Custom Team Win / Draw setting."), true);
		EditorGUI.EndDisabledGroup();
		EditorGUI.BeginDisabledGroup(!activator.endMatch || !activator.enableCustomWinLogic || activator.doDynamicTeamWinLogic);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.customWinTeam)), new GUIContent("Custom Win Team", "Which team wins when running custom team win logic."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.draw)), new GUIContent("Draw", "Whether or not to call a draw when running custom team win logic."), true);
		EditorGUI.EndDisabledGroup();

		EditorGUILayout.EndVertical();

		ActivatorEditorCommon.DrawCommon(serializedObject, activator);

		RunErrorChecks();

		serializedObject.ApplyModifiedProperties();
	}
}
