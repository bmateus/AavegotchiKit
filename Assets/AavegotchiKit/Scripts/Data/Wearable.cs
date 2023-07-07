using System.Collections.Generic;
using System;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class Wearable
    {
        public int id;
        public string name;
        public string description;
        public string author;
        public byte category;
        public int[] traitModifiers;
        public bool[] slotPositions;
        public string[] allowedCollaterals;
        public GotchiWearableRarity rarity;
        public List<WearablePose> poses;
        public Vector2[] offsets;
        public int[] dimensions;
        public int minLevel;

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

}
