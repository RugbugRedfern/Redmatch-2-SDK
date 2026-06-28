using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Spawnpoint)), CanEditMultipleObjects]
public class SpawnpointEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if(GUILayout.Button("Ground"))
		{
			foreach(var target in targets)
			{
				Spawnpoint script = (Spawnpoint)target;

				if(Physics.Raycast(script.transform.position, Vector3.down, out RaycastHit hit, 50f))
				{
					script.transform.position = hit.point + Vector3.up * 0.8f;
				}

				EditorUtility.SetDirty(target);
			}
		}
	}
}
