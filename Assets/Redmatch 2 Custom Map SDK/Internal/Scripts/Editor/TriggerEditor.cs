using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Trigger), true), CanEditMultipleObjects]
public class TriggerEditor : Editor
{
	private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

	public override void OnInspectorGUI()
	{
		Trigger script = (Trigger)target;

		EditorGUILayout.LabelField("Target for TargetActivators: " + script.GetTarget(), EditorStyles.wordWrappedMiniLabel);

		DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

		serializedObject.ApplyModifiedProperties();
	}
}