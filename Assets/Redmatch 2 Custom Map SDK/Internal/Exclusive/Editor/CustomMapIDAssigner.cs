using System.Security.Principal;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

#if !REDMATCH
public class CustomMapIDAssigner : MonoBehaviour
{
	public static void AssignIDs(Scene scene)
	{
		int startingID = 20000;

		int id = startingID;

		foreach(var root in scene.GetRootGameObjects())
		{
			foreach(MyceliumIdentity identity in root.GetComponentsInChildren(typeof(MyceliumIdentity), true))
			{
				identity.NetID = id++;
				EditorUtility.SetDirty(identity);
				PrefabUtility.RecordPrefabInstancePropertyModifications(identity);
			}

			foreach(UpgradeCabinetProxy cabinet in root.GetComponentsInChildren(typeof(UpgradeCabinetProxy), true))
			{
				cabinet.id = id++;
				EditorUtility.SetDirty(cabinet);
				PrefabUtility.RecordPrefabInstancePropertyModifications(cabinet);
			}
		}

		Debug.Log($"Assigned {(id - startingID)} ids");

		EditorSceneManager.SaveScene(scene, scene.path);
	}
}
#endif