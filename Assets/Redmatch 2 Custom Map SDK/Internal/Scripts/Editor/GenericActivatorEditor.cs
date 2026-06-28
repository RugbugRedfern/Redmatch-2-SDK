using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(GenericActivator)), CanEditMultipleObjects]
public class GenericActivatorEditor : ErrorCollectionBehaviourEditor
{
	GenericActivator activator;

	void OnEnable()
	{
		activator = (GenericActivator)target;
	}

	public override void OnInspectorGUI()
	{
		if(target == null)
			return;

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.authorityRequirement)), new GUIContent("Execute On", "Choose on what clients this activator gets executed."), true);
		EditorGUILayout.EndVertical();

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.actionDelay)), new GUIContent("Delay", "The delay time in seconds between an activator being triggered and its actions executing."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.cooldown)), new GUIContent("Cooldown", "The cooldown in seconds before an action can be triggered again. This value is not synced, and is calculated locally for each player."), true);
		EditorGUILayout.EndVertical();

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Generic Actions", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.objectsToEnable)), new GUIContent("Objects to Enable", "Enables all objects in list"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.objectsToDisable)), new GUIContent("Objects to Disable", "Disables all objects in list"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.randomObjectToEnable)), new GUIContent("Random Object to Enable", "Randomly enables object in list"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.randomObjectToDisable)), new GUIContent("Random Object to Disable", "Randomly disables object in list"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.randomActivatorToTrigger)), new GUIContent("Random Activator to Trigger", "Randomly trigger Activator in list"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.animatorActions)), new GUIContent("Animator Actions", ""), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.activatorActions)), new GUIContent("Activator Actions", "Executes all actions on the specified Activators"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.healthSyncerActions)), new GUIContent("Health Syncer Actions", "Executes all actions on the specified Health Syncers"), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.intSyncerActions)), new GUIContent("Int Syncer Actions", "Executes all actions on the specified Int Syncers"), true);
		if(activator.animatorActions.Length > 0)
		{
			if(GUILayout.Button("Scan for Animation Errors"))
			{
				ScanForAnimationErrors();
			}
		}

		EditorGUILayout.EndVertical();

		ActivatorEditorCommon.DrawCommon(serializedObject, activator);

		RunErrorChecks();

		serializedObject.ApplyModifiedProperties();
	}

	void ScanForAnimationErrors()
	{
		bool hadError = false;

		foreach(var animatorAction in activator.animatorActions)
		{
			if(animatorAction.animatorSyncer)
			{
				var animator = (AnimatorController)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(animatorAction.animatorSyncer.GetComponent<Animator>().runtimeAnimatorController), typeof(AnimatorController));

				bool hasParameter = false;

				for(int i = 0; i < animator.parameters.Length; i++)
				{
					if(animator.parameters[i].name == animatorAction.parameter)
					{
						hasParameter = true;
						break;
					}
				}

				if(!hasParameter)
				{
					Debug.LogError($"You have an animator action that applies to the parameter \"{animatorAction.parameter}\", but that parameter does not exist on the specified Animator.");
					hadError = true;
				}
			}
			else
			{
				Debug.LogError($"You have an animator action that does not specify an animator.");
				hadError = true;
			}
		}

		if(!hadError)
		{
			Debug.Log("No errors found.");
		}
	}
}
