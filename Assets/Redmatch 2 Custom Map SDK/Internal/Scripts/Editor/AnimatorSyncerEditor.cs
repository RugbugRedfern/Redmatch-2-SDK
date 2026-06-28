using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimatorSyncer), true), CanEditMultipleObjects]
public class AnimatorSyncerEditor : SyncedBehaviourEditor
{
	public override void RenderGUI()
	{
		base.RenderGUI();

		AnimatorSyncer script = (AnimatorSyncer)target;

		if(advancedFoldout)
		{
			EditorGUI.BeginDisabledGroup(!script.sync);
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.desyncProtection)), new GUIContent("Desync Protection"));
			if(script.sync)
			{
				if(script.desyncProtection)
				{
					EditorGUILayout.LabelField($"{onString}The state will be sent from the Host to the clients at regular intervals to make sure they are up-to-date.", style);
				}
				else
				{
					EditorGUILayout.LabelField($"{offString}Clients may deviate from each other due to lag. It's recommended to have this ON for long animations or complex animators.", style);
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUI.EndDisabledGroup();
		}
	}
}