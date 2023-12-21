using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class Collaterals
    {
        public Collateral[] collaterals;
    }

    [CreateAssetMenu(fileName = "CollateralDB", menuName = "AavegotchiKit/DB/CollateralDB")]
    public class CollateralDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        public Collateral[] collaterals;

        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            Debug.Log("Importing collaterals...");
            var tmp = JsonUtility.FromJson<Collaterals>(source.text);
            collaterals = tmp.collaterals;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

    }
}
