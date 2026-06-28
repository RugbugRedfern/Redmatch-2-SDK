using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Spawnpoint : MonoBehaviour
{
	public enum SpawnpointType { AnyMode, OnlyFreeForAll, OnlyTeams };
	public enum SpawnpointTeam { Red = 0, Blue = 1, Yellow = 2, Green = 3, Infected = 5 };

	[SerializeField] SpawnpointType spawnpointType;
	[ConditionalHide(nameof(spawnpointType), 1, false, true)]
	[SerializeField] SpawnpointTeam team;


// Some code here has been excluded from the SDK.


	#region Editor
#if UNITY_EDITOR
	static Mesh previewMesh
	{
		get
		{
			if(_playerMesh == null)
			{
				_playerMesh = Resources.Load<GameObject>("SpawnpointPreview").GetComponent<MeshFilter>().sharedMesh;
			}

			return _playerMesh;
		}
	}
	static Mesh _playerMesh;

	void OnDrawGizmos()
	{
		SetGizmoColor();

		Gizmos.DrawMesh(previewMesh, 0, transform.position, transform.rotation, Vector3.one * 0.8f);

		if(spawnpointType == SpawnpointType.AnyMode)
		{
			Gizmos.color = Color.cyan;
		}
		else
		{
			Gizmos.color = Color.white;
		}
		Gizmos.DrawLine(transform.position + Vector3.up * 0.4f + transform.forward * 0.4f, transform.position + Vector3.up * 0.4f + transform.forward * 1.5f);

		if(Camera.current != null && Vector3.Distance(transform.position, Camera.current.transform.position) < 10f)
		{
			GUIStyle handleStyle = new GUIStyle { fontSize = 24, alignment = TextAnchor.MiddleCenter };
			Handles.Label(transform.position + Vector3.up * 1.2f, spawnpointType.ToString(), handleStyle);
		}
	}

	void OnDrawGizmosSelected()
	{
		SetGizmoColor();

		Gizmos.DrawWireMesh(previewMesh, 0, transform.position, transform.rotation, Vector3.one * 0.8f);
	}

	void SetGizmoColor()
	{
		if(spawnpointType != SpawnpointType.OnlyFreeForAll)
		{
			switch(team)
			{
				case SpawnpointTeam.Red:
					Gizmos.color = Color.red;
					break;
				case SpawnpointTeam.Blue:
					Gizmos.color = Color.blue;
					break;
				case SpawnpointTeam.Yellow:
					Gizmos.color = Color.yellow;
					break;
				case SpawnpointTeam.Green:
					Gizmos.color = Color.green;
					break;
				case SpawnpointTeam.Infected:
					Gizmos.color = Color.black + Color.green * 0.5f;
					break;
			}
		}
		else
		{
			Gizmos.color = Color.white;
		}
	}
#endif
	#endregion
}