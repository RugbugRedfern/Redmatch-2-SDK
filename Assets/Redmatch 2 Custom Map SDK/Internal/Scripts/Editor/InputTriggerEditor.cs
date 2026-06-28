using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InputTrigger), true), CanEditMultipleObjects]
public class InputTriggerEditor : TriggerEditor
{
	protected GUIStyle style;
	protected string hostString;
	protected string onString;
	protected string offString;

	private static readonly string[] _dontIncludeMe = new string[] { "m_Script", "sync", "hostOnly", "keyName", "inputAxis", "useAxisInput", "inputKey", "inputType", "repeatDelay" };

	void SetUpStyle()
	{
		style = EditorStyles.wordWrappedLabel;
		style.richText = true;

		hostString = "<color=#2fff2b><b>HOST ONLY</b></color> - ";
		onString = "<color=#2fff2b><b>ON</b></color> - ";
		offString = "<b>OFF</b> - ";
	}

	public virtual void RenderGUI()
	{
		InputTrigger script = (InputTrigger)target;

		EditorGUILayout.LabelField("Target for TargetActivators: " + script.GetTarget(), EditorStyles.wordWrappedMiniLabel);

		DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

		// Key Input readout
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField($"Input Behaviour", style);
		// Axis Input conditional
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.useAxisInput)), new GUIContent("Use Axis Input"));
		if(script.useAxisInput)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.inputAxis)), new GUIContent("Input Axis"));
		}
		else
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.inputKey)), new GUIContent("Input Key"));
		}
		// Input Type + Held Repeat Delay
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.inputType)), new GUIContent("Input Type"));
		EditorGUI.BeginDisabledGroup(script.inputType != InputTrigger.InputTriggerType.Held);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.repeatDelay)), new GUIContent("Held Repeat Delay"));
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndVertical();

		// Current sync behaviour readout
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.LabelField($"Sync Behaviour", style);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.sync)), new GUIContent("Sync"));
		if(script.sync)
		{
			if(script.hostOnly)
			{
				EditorGUILayout.LabelField($"{onString}Send the input to the host only (overriden by Host Only).", style);
			}
			else
			{
				EditorGUILayout.LabelField($"{onString}Send the input to other players, and trigger locally.", style);
			}
		}
		else
		{
			EditorGUILayout.LabelField($"{offString}Don't send the input to other players, only trigger locally.", style);
		}

		EditorGUI.BeginDisabledGroup(!script.sync);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(script.hostOnly)), new GUIContent("Host Only"));
		if(script.sync)
		{
			if(script.hostOnly)
			{
				EditorGUILayout.LabelField($"{onString}This will only trigger for the host player.", style);
			}
			else
			{
				EditorGUILayout.LabelField($"{offString}This will trigger for all players.", style);
			}
		}
		EditorGUILayout.EndVertical();
		EditorGUI.EndDisabledGroup();
	}

	public override void OnInspectorGUI()
	{
		SetUpStyle();
		RenderGUI();

		serializedObject.ApplyModifiedProperties();
	}
}
