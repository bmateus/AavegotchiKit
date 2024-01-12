using UnityEngine;
using UnityEditor;

namespace PortalDefender.AavegotchiKit
{
    public class EditorCreateGotchiWithChainAppearance : MonoBehaviour
    {
        [MenuItem("GameObject/AavegotchiKit/Create Chain Gotchi", false, 10)]
        static void Create(MenuCommand menuCommand)
        {
            GameObject go = new GameObject("Gotchi - New");
            go.AddComponent<Gotchi>();

            string[] guids = AssetDatabase.FindAssets("GotchiBaseChain t:prefab", new[] { "Packages" });
            if (guids.Length == 0)
            {
                Debug.LogError("Prefab 'GotchiBaseChain' not found in packages");
                //look in the project
                guids = AssetDatabase.FindAssets("GotchiBaseChain t:prefab");
                if (guids.Length == 0)
                {
                    Debug.LogError("Prefab 'GotchiBaseChain' not found in project");
                    return;
                }
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            GameObject gotchiBaseChain = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            gotchiBaseChain.transform.SetParent(go.transform);

            //Add an AppearanceFetcher component
            var appearanceFetcher = go.AddComponent<GotchiChainAppearanceGraphFetcher>();
            
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Debug.Log("Created new gotchi: Next you can set the Id.");

            Selection.activeObject = go;

        }
    }
}

