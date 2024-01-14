using UnityEngine;
using UnityEditor;

namespace PortalDefender.AavegotchiKit
{
    public class EditorCreateHelpers
    {
        static void CreatePrefab(string prefabName)
        {
            string[] guids = AssetDatabase.FindAssets(prefabName + " t:prefab", new[] { "Packages" });
            if (guids.Length == 0)
            {
                Debug.LogError("Prefab '" + prefabName + "' not found in packages");
                //look in the project
                guids = AssetDatabase.FindAssets(prefabName + " t:prefab");
                if (guids.Length == 0)
                {
                    Debug.LogError("Prefab '" + prefabName + "' not found in project");
                    return;
                }
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Debug.Log("Created new '" + prefabName + "'");

            Selection.activeObject = go;

        }

        [MenuItem("GameObject/AavegotchiKit/Create Web3 Provider", false, 11)]
        static void CreateWeb3Provider(MenuCommand menuCommand)
        {
            CreatePrefab("Web3Provider");
        }

        [MenuItem("GameObject/AavegotchiKit/Create Graph Manager", false, 11)]
        static void CreateGraphProvider(MenuCommand menuCommand)
        {
            CreatePrefab("GraphManager");
        }
    }
}

