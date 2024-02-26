using System;
using System.Collections.Generic;
using UnityEngine;

namespace GotchiSDK
{
    [CreateAssetMenu(fileName = "AavegotchiWearableDataBase", menuName = "GotchiSDK/Aavegotchi Wearable Database")]
    [Serializable]
    public class AavegotchiWearable_Database : ScriptableObject
    {
        [Serializable]
        public struct AavegotchiWearable_DatabaseEntry
        {
            public int WearableID;
            public string WearableName;
            public Sprite WearableIcon;
        }

        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> HeadWearables = new List<AavegotchiWearable_DatabaseEntry>();
        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> BodyWearables = new List<AavegotchiWearable_DatabaseEntry>();
        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> FaceWearables = new List<AavegotchiWearable_DatabaseEntry>();
        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> EyeWearables = new List<AavegotchiWearable_DatabaseEntry>();
        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> HandWearables = new List<AavegotchiWearable_DatabaseEntry>();
        [SerializeField] public List<AavegotchiWearable_DatabaseEntry> PetWearables = new List<AavegotchiWearable_DatabaseEntry>();
    }
}