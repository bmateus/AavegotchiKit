using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [System.Serializable]
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
        public int minLevel;

        public string[] svgs;
        public string[] sleeves;

        public GotchiWearableDimensions[] dimensions;

        public string note; // any special notes about this wearable

        public bool HasSleeves => sleeves.Length > 0;

        public GotchiWearableDimensions GetDimensions(GotchiFacing facing)
        {
            return dimensions[(int)facing];
        }

        public Sprite GetSprite(GotchiHandPose handPose, GotchiFacing facing)
        {
            var dimensions = GetDimensions(facing);

            string data = svgs[(int)facing];

            return SvgLoader.GetSvgLayerSprite($"wearable-{id}-{facing}-{handPose}",
                data,
                new SvgLoader.Options()
                {
                    id = id,
                    hideSleeves = false,
                    handPose = handPose,
                    offset = new Vector2(dimensions.X, dimensions.Y),
                    size = new Vector2(dimensions.Width, dimensions.Height)
                });
        }

        public Sprite GetSleeveSprite(GotchiHandPose handPose, GotchiFacing facing)
        {
            if (!HasSleeves)
                return null;

            var dimensions = GetDimensions(facing);

            return SvgLoader.GetSvgLayerSprite($"wearable-{id}-sleeves-{facing}-{handPose}",
                HasSleeves ? sleeves[(int)facing] : svgs[(int)facing],
                new SvgLoader.Options()
                {
                    id = id,
                    hideSleeves = false,
                    handPose = handPose,
                    offset = new Vector2(dimensions.X, dimensions.Y),
                    size = new Vector2(dimensions.Width, dimensions.Height)
                });
        }

    }

}
