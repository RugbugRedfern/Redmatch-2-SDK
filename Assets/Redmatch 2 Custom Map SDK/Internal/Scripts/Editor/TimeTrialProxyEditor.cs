using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeTrialProxy))]
public class TimeTrialProxyEditor : ErrorCollectionBehaviourEditor
{
	TimeTrialProxy proxy;

	void OnEnable()
	{
		proxy = (TimeTrialProxy)target;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.HelpBox("You can only have one time trial per map. The time trial leaderboard will be permanently tied to your map once uploaded to the workshop. If you remove this time trial and create another, the leaderboard will carry over. If you need to reset the leaderboard, you can use the /resetworkshoplb [workshop map id] in chat to reset it.", MessageType.Info);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(proxy.nodes)), new GUIContent("Nodes", "The nodes in this time trial. The order is the order the player has to complete them in."), true);
		if(GUILayout.Button("Populate Nodes from Children"))
		{
			proxy.nodes = proxy.GetComponentsInChildren<TimeTrialNodeProxy>();
			EditorUtility.SetDirty(proxy);
		}
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(proxy.platinumTime)), new GUIContent("Platinum Time", "The shortest possible time to complete this time trial. Gives the player a platinum medal."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(proxy.goldTime)), new GUIContent("Gold Time", "Gives the player a gold medal."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(proxy.silverTime)), new GUIContent("Silver Time", "Gives the player a silver medal."), true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(proxy.bronzeTime)), new GUIContent("Bronze Time", "The longest possible time to complete this time trial. Gives the player a bronze medal."), true);

		RunErrorChecks();

		serializedObject.ApplyModifiedProperties();
	}
}
