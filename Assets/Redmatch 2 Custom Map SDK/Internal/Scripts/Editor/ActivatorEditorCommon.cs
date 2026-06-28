using UnityEditor;
using UnityEngine;

public static class ActivatorEditorCommon
{
	public static void DrawCommon(SerializedObject serializedObject, Activator activator)
	{
		// Logic filters
		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Conditional Logic", EditorStyles.boldLabel);

		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.conditionalLogicType)), new GUIContent("Conditional Logic Type", ""), true);

		if(activator.conditionalLogicType != Activator.ConditionalLogicType.NoRequirement)
		{
			EditorGUI.indentLevel--;
			EditorGUILayout.HelpBox("This Activator will only activate if the following conditional logic evaluates to true.", MessageType.Info);
			EditorGUI.indentLevel++;
		}

		if(activator.conditionalLogicType != Activator.ConditionalLogicType.NoRequirement)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.firstValue)), new GUIContent("First Value", ""), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.secondValue)), new GUIContent("Second Value", ""), true);
		}

		EditorGUILayout.EndVertical();

		// Player filters
		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Triggering Player Filter", EditorStyles.boldLabel);

		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.playerFilter)), new GUIContent("Player Filter", ""), true);

		if(activator.playerTeamRequirementType != Activator.PlayerTeamRequirementType.NoRequirement || activator.playerStatRequirementType != Activator.PlayerStatRequirementType.NoRequirement)
		{
			EditorGUI.indentLevel--;
			EditorGUILayout.HelpBox("When triggered by a player, this activator will only activate if that player passes the following criteria.", MessageType.Info);
			EditorGUI.indentLevel++;
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.playerTeamRequirementType)), new GUIContent("Team Requirement Type", ""), true);

		EditorGUI.indentLevel++;
		if(activator.playerTeamRequirementType != Activator.PlayerTeamRequirementType.NoRequirement)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.requiredTeam)), new GUIContent("Team", ""), true);
		}

		EditorGUI.indentLevel--;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.playerStatRequirementType)), new GUIContent("Stat Requirement Type", ""), true);

		EditorGUI.indentLevel++;
		if(activator.playerStatRequirementType != Activator.PlayerStatRequirementType.NoRequirement)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.playerStat)), new GUIContent("Stat", ""), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.requiredStat)), new GUIContent("Value", ""), true);
		}

		EditorGUILayout.EndVertical();
	}
}
