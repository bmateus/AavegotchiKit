using UnityEngine;
using UnityEditor;

namespace PortalDefender.AavegotchiKit
{
    public class EditorCreateGotchiWithMeshAppearance : MonoBehaviour
    {
        [MenuItem("GameObject/AavegotchiKit/Create Mesh Gotchi", false, 10)]
        static void Create(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Gotchi - New");
            go.AddComponent<Gotchi>();

            string[] guids = AssetDatabase.FindAssets("GotchiBaseMesh t:prefab", new[] { "Packages" });
            if (guids.Length == 0)
            {
                Debug.LogError("Prefab 'GotchiBaseMesh' not found in packages");
                //look in the project
                guids = AssetDatabase.FindAssets("GotchiBaseMesh t:prefab");
                if (guids.Length == 0)
                {
                    Debug.LogError("Prefab 'GotchiBaseMesh' not found in project");
                    return;
                }
            }
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject gotchiBaseMesh = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            gotchiBaseMesh.transform.SetParent(go.transform);

            //Add an AppearanceFetcher component
            var appearanceFetcher = go.AddComponent<GotchiMeshAppearanceFetcher>();
            var so = new SerializedObject(appearanceFetcher);
            so.FindProperty("removeBG_").boolValue = true;
            so.FindProperty("removeShadow_").boolValue = true;
            so.FindProperty("appearanceMesh_").objectReferenceValue = gotchiBaseMesh.GetComponent<GotchiMeshAppearance>();
            so.FindProperty("initOnStart_").boolValue = true;
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Debug.Log("Created new gotchi: Next you can set the Id and Fetch its appearance.");

            Selection.activeObject = go;
        }
    }
}

