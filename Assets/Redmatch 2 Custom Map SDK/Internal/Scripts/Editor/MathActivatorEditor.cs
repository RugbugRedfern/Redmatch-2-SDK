
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MathActivator)), CanEditMultipleObjects]
public class MathActivatorEditor : ErrorCollectionBehaviourEditor
{
	MathActivator activator;

	void OnEnable()
	{
		activator = (MathActivator)target;
	}

	public override void OnInspectorGUI()
	{
		if(target == null)
			return;

		EditorGUI.indentLevel = 0;
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField("Calculations", EditorStyles.boldLabel);
		EditorGUI.indentLevel = 1;
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(activator.calculations)), new GUIContent("Calculations", "Each calculation is performed in order, from top to bottom (first to last)."), true);
		EditorGUI.indentLevel--;
		EditorGUILayout.EndVertical();

		ActivatorEditorCommon.DrawCommon(serializedObject, activator);

		RunErrorChecks();

		serializedObject.ApplyModifiedProperties();
	}
}
