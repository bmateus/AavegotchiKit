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
    }
}
