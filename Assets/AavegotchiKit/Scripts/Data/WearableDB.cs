using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        public Vector2[] offsets;
        public int[] dimensions;

        public string[] svgs;
        public string[] sleeves;

        public string note; // any special notes about this wearable

        public bool HasSleeves => sleeves.Length > 0;

        public Vector2 GetOffset(GotchiFacing facing)
        {
            return offsets[(int)facing];
        }

        public Sprite GetSprite(GotchiHandPose handPose, GotchiFacing facing)
        {
            Vector2 offset = offsets[(int)facing];
            //Vector2 size;

            //if (facing == GotchiFacing.FRONT || facing == GotchiFacing.BACK)
            //{
            //    size = new Vector2(dimensions[0], dimensions[1]); //i think these are wrong
            //}
            //else
            //{
            //    size = new Vector2(dimensions[2], dimensions[3]);
            //}

            //Unity's SVG Library can't handle this: need to explicitly offset it
            //string data = $"<svg x=\"{offset.x}\" y=\"{offset.y}\">" + svgs[(int)facing] + "</svg>";
            
            string data = svgs[(int)facing];

            return SvgLoader.GetSvgLayerSprite($"wearable-{id}-{facing}-{handPose}",
                data,
                new SvgLoader.Options()
                {
                    hideSleeves = true,
                    hideSleevesUp = true,//handPose != GotchiHandPose.UP,
                    hideSleevesDown = true,//handPose != GotchiHandPose.DOWN_OPEN,
                    size = offset
                });
        }

        public Sprite GetSleeveSprite(GotchiHandPose handPose, GotchiFacing facing)
        {
            if (!HasSleeves)
                return null;

            Vector2 size;

            if (facing == GotchiFacing.FRONT || facing == GotchiFacing.BACK)
            {
                size = new Vector2(dimensions[0], dimensions[1]);
            }
            else
            {
                size = new Vector2(dimensions[2], dimensions[3]);
            }

            return SvgLoader.GetSvgLayerSprite($"wearable-{id}-sleeves-{facing}-{handPose}",
                HasSleeves ? sleeves[(int)facing] : svgs[(int)facing],
                new SvgLoader.Options()
                {
                    hideSleeves = false,
                    hideSleevesUp = handPose != GotchiHandPose.UP,
                    hideSleevesDown = handPose != GotchiHandPose.DOWN_OPEN,
                    size = size
                });
        }

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
            catch (Exception /*ex*/)
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
