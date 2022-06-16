using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class WearablePose
    {
        public Sprite[] sprites;
        public Sprite[] sleeves;
    }

    [Serializable]
    public class Wearable
    {
        public int id;
        public string name;
        public string description;
        public int[] traitModifiers;
        public bool[] slotPositions;
        public string[] allowedCollaterals;
        public List<WearablePose> poses;
    }

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

#if UNITY_EDITOR
        Sprite GetSpriteAsset(string spritename)
        {
            try
            {
                var guid = AssetDatabase.FindAssets(spritename).First();
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                //matches might not be exact... check that it is
                if ( assetPath.EndsWith($"{spritename}.png") )
                    return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                return null;
            }
            catch (Exception ex)
            {
                //Debug.LogException(ex);
                return null;
            }
        }
#endif


        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            var tmp = JsonUtility.FromJson<Wearables>(source.text);
            Debug.Log("Importing wearables...");
            for(int i=0; i < tmp.wearables.Length; ++i)
            {
                var wearable = tmp.wearables[i];
                wearable.poses = new List<WearablePose>();

                var pose = new WearablePose();

                //assign sprite
                var sprites = new Sprite[4];
                sprites[0] = GetSpriteAsset($"wearable_{wearable.id}");
                sprites[1] = GetSpriteAsset($"wearable_{wearable.id}_left");
                sprites[2] = GetSpriteAsset($"wearable_{wearable.id}_right");
                sprites[3] = GetSpriteAsset($"wearable_{wearable.id}_back");
                pose.sprites = sprites;

                var sleeves = new Sprite[4];
                sleeves[0] = GetSpriteAsset($"sleeves_{wearable.id}");
                sleeves[1] = GetSpriteAsset($"sleeves_{wearable.id}_left");
                sleeves[2] = GetSpriteAsset($"sleeves_{wearable.id}_right");
                //sleeves[3] = null;
                pose.sleeves = sleeves;

                wearable.poses.Add(pose);

                if ( wearable.slotPositions[(int)GotchiEquipmentSlot.BODY])
                {
                    // add sleeves up pose
                    pose = new WearablePose();

                    //assign sprite
                    sprites = new Sprite[4];
                    sprites[0] = GetSpriteAsset($"wearable_{wearable.id}_sleeves_up");
                    sprites[1] = GetSpriteAsset($"wearable_{wearable.id}_sleeves_up_left");
                    sprites[2] = GetSpriteAsset($"wearable_{wearable.id}_sleeves_up_right");
                    sprites[3] = GetSpriteAsset($"wearable_{wearable.id}_sleeves_up_back");
                    pose.sprites = sprites;

                    sleeves = new Sprite[4];
                    sleeves[0] = GetSpriteAsset($"sleeves_{wearable.id}_sleeves_up");
                    sleeves[1] = GetSpriteAsset($"sleeves_{wearable.id}_sleeves_up_left");
                    sleeves[2] = GetSpriteAsset($"sleeves_{wearable.id}_sleeves_up_right");
                    //sleeves[3] = null;
                    pose.sleeves = sleeves;

                    wearable.poses.Add(pose);
                }
            }
            wearables = tmp.wearables;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }
}
