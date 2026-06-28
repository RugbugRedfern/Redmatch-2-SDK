using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RigidbodySyncer), true), CanEditMultipleObjects]
public class RigidbodySyncerEditor : SyncedBehaviourEditor
{
	public override void RenderGUI()
	{
		base.RenderGUI();

		RigidbodySyncer script = (RigidbodySyncer)target;

		if(script.serverOnly == false)
		{
			EditorGUILayout.HelpBox("If Host Only is disabled, any player will be able to teleport this RigidbodySyncer. Only do this if you know what you are doing.", MessageType.Info);
		}

		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.syncPosition)), new GUIContent("Sync Position", "Sync the position of this GameObject"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.syncRotation)), new GUIContent("Sync Rotation", "Sync the rotation of this GameObject"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.syncRate)), new GUIContent("Sync Rate", "How many times the object should sync per second. You can lower this if you have a lot of objects to save performance."));
	}
}