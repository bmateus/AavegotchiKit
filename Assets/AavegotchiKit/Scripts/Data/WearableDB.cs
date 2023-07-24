using Cysharp.Threading.Tasks;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class Wearables
    {
        public Wearable[] wearables;
    }

    [CreateAssetMenu(fileName ="WearableDB", menuName = "Aavegotchi/WearableDB")]
    public class WearableDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        public Wearable[] wearables;


        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            Debug.Log("Importing wearables...");
            var tmp = JsonUtility.FromJson<Wearables>(source.text);
            wearables = tmp.wearables;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        [ContextMenu("Refresh Wearables")]
        async UniTask RefreshAll()
        {
#if UNITY_EDITOR
            for(int i=0; i < wearables.Length; i++)
            {
                var wearable = wearables[i];
                EditorUtility.DisplayProgressBar("Refreshing Wearables", $"Refreshing {wearable.name}...", i / (float)wearables.Length);
                await wearable.RefreshData();
            }

            EditorUtility.ClearProgressBar();
#endif
        }

    }
}
