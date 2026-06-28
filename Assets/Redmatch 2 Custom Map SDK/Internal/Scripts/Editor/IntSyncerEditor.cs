using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IntSyncer), true), CanEditMultipleObjects]
public class IntSyncerEditor : SyncedBehaviourEditor
{
	public override void RenderGUI()
	{
		base.RenderGUI();

		IntSyncer script = (IntSyncer)target;

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.startingValue)), new GUIContent("Starting Value"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.valueDisplays)), new GUIContent("Value Displays"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.OnSet)), new GUIContent("On Value Set"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.OnChanged)), new GUIContent("On Value Changed"));
	}
}
