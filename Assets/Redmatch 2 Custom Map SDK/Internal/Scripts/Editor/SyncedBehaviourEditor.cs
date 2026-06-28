using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SyncedBehaviour), true), CanEditMultipleObjects]
public class SyncedBehaviourEditor : ErrorCollectionBehaviourEditor
{
	protected GUIStyle style;
	protected string onString;
	protected string offString;

	protected bool advancedFoldout = false;

	void SetUpStyle()
	{
		style = EditorStyles.wordWrappedLabel;
		style.richText = true;

		onString = "<color=#2fff2b><b>ON</b></color> - ";
		offString = "<b>OFF</b> - ";
	}

	public virtual void RenderGUI()
	{
		SyncedBehaviour script = (SyncedBehaviour)target;

		advancedFoldout = EditorGUILayout.Foldout(advancedFoldout, $"Advanced", true);
		if(advancedFoldout)
		{
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.sync)), new GUIContent("Sync"));
			if(script.sync)
			{
				EditorGUILayout.LabelField($"{onString}This will have its data synced across all clients.", style);
			}
			else
			{
				EditorGUILayout.LabelField($"{offString}This is not synced. All clients will have their own version, and will not be aware of the others.", style);
			}
			EditorGUILayout.EndVertical();

			EditorGUI.BeginDisabledGroup(!script.sync);
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.serverOnly)), new GUIContent("Host Only"));
			if(script.sync)
			{
				if(script.serverOnly)
				{
					EditorGUILayout.LabelField($"{onString}This will be handled by the Host.", style);
				}
				else
				{
					EditorGUILayout.LabelField($"{offString}This will be handled by any client that interacts with it.", style);
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUI.EndDisabledGroup();
		}
	}

	public override void OnInspectorGUI()
	{
		SetUpStyle();
		RenderGUI();
		serializedObject.ApplyModifiedProperties();
		RunErrorChecks();
	}
}