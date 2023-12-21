
using UnityEngine;

#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEditor;
#endif

namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
    public class Wearables
    {
        public Wearable[] wearables;
    }

    [CreateAssetMenu(fileName ="WearableDB", menuName = "AavegotchiKit/DB/WearableDB")]
    public class WearableDB : ScriptableObject
    {
        public WearableScriptableObject[] wearables;

#if UNITY_EDITOR

        [ContextMenu("Refresh Existing Wearables")]
        public async UniTask RefreshAll()
        {
            for(int i=0; i < wearables.Length; i++)
            {
                var wearable = wearables[i];
                
                if (EditorUtility.DisplayCancelableProgressBar("Refreshing Wearables",
                    $"Refreshing {wearable.name}... {i} / {wearables.Length}",
                    i / (float)wearables.Length))
                {
                    break;
                }

                await wearable.Refresh();
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
        }

        [ContextMenu("Collect All Wearables")]
        public void CollectAllWearables()
        {
            //find all scriptable objects of type WearableScriptableObject
            wearables = AssetDatabase.FindAssets("t:WearableScriptableObject")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<WearableScriptableObject>(path))
                .ToArray();
            //save database
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        
        //TODO: still need a way to create new wearables when they are added to the blockchain


#endif




    }
}
