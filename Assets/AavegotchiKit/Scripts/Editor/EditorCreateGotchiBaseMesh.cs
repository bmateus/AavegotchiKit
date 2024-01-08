using UnityEngine;
using UnityEditor;

namespace PortalDefender.AavegotchiKit
{
    public class EditorCreateGotchiBaseMesh : MonoBehaviour
    {
        [MenuItem("GameObject/AavegotchiKit/Create Mesh Gotchi", false, 10)]
        static void Create(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Gotchi - New");

            string[] guids = AssetDatabase.FindAssets("GotchiBaseMesh t:prefab", new[] { "Packages" });
            if (guids.Length == 0)
            {
                Debug.LogError("Prefab 'GotchiBaseMesh' not found");
                return;
            }
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject gotchiBaseMesh = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            gotchiBaseMesh.transform.SetParent(go.transform);

            //Add a AppearanceFetcher component
            var appearanceFetcher = go.AddComponent<AppearanceFetcher>();
            var so = new SerializedObject(appearanceFetcher);
            so.FindProperty("removeBG_").boolValue = true;
            so.FindProperty("removeShadow_").boolValue = true;
            so.FindProperty("appearanceMesh_").objectReferenceValue = gotchiBaseMesh.GetComponent<GotchiAppearanceMesh>();
            so.ApplyModifiedProperties();

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Debug.Log("Created new gotchi: Next set the id and Fetch");

            Selection.activeObject = go;
        }
    }
}

