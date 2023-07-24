using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
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

            //Unity's SVG Library can't handle this: need to explicitly offset it
            //string data = $"<svg x=\"{offset.x}\" y=\"{offset.y}\">" + svgs[(int)facing] + "</svg>";

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

        [Button]
        public async UniTask RefreshData()
        {
            Debug.Log("RefreshData");

            if (id > 0)
            {
                WearableFetcher fetcher = new WearableFetcher();
                var w = await fetcher.GetWearable(id);
                if (w != null)
                {
                    name = w.name;
                    description = w.description;
                    author = w.author;
                    category = w.category;
                    traitModifiers = w.traitModifiers;
                    slotPositions = w.slotPositions;
                    allowedCollaterals = w.allowedCollaterals;
                    rarity = w.rarity;
                    minLevel = w.minLevel;
                    svgs = w.svgs;
                    sleeves = w.sleeves;
                    dimensions = w.dimensions;
                }
            }
        }


    }

}
