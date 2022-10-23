
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class WearableSet
    {
        public string name;
        public string[] allowedCollaterals;
        public int[] wearableIds;
        public int[] traitsBonuses;
    }

    [Serializable]
    public class WearableSets
    {
        public WearableSet[] wearableSets;
    }

    [CreateAssetMenu(fileName = "WearableSetDB", menuName = "Aavegotchi/WearableSetDB")]
    public class WearableSetDB : ScriptableObject
    {

        [SerializeField]
        TextAsset source;

        public WearableSet[] wearableSets;

        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            var tmp = JsonUtility.FromJson<WearableSets>(source.text);
            Debug.Log("Importing wearable sets...");
            wearableSets = tmp.wearableSets;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

    }
}
