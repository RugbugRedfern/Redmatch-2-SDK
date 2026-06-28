using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthSyncer), true), CanEditMultipleObjects]
public class HealthSyncerEditor : SyncedBehaviourEditor
{
	public override void RenderGUI()
	{
		base.RenderGUI();

		HealthSyncer script = (HealthSyncer)target;

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.maxHealth)), new GUIContent("Max Health"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.health)), new GUIContent("Health"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.healthDisplays)), new GUIContent("Health Displays"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.OnDamaged)), new GUIContent("On Damaged"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.OnHealed)), new GUIContent("On Healed"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.OnDied)), new GUIContent("On Died"));
	}
}