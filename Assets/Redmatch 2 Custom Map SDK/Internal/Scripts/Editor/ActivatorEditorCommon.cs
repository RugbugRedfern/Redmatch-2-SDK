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
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.compoundCheckEvaluation)), new GUIContent("Compound Check Evaluation", ""), true);

		if(activator.conditionalLogicChecks.Length > 0)
		{
			EditorGUI.indentLevel--;
			if(activator.compoundCheckEvaluation == Activator.ConditionalLogicCheckEvaluation.PassIfAnyAreTrue)
			{
				EditorGUILayout.HelpBox("This Activator will only activate if any of the following conditional logic evaluates to true.", MessageType.Info);
			}
			else if(activator.compoundCheckEvaluation == Activator.ConditionalLogicCheckEvaluation.PassIfAllAreTrue)
			{
				EditorGUILayout.HelpBox("This Activator will only activate if all of the following conditional logic evaluates to true.", MessageType.Info);
			}
			EditorGUI.indentLevel++;
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.conditionalLogicChecks)), new GUIContent("Conditional Logic Checks", ""), true);

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
