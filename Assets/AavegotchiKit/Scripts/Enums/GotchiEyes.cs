using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    public static class GotchiEyes
    {
        public static Color MythicalLow = new Color32(0xFF, 0x00, 0xFF, 0xFF);
        public static Color RareLow = new Color32(0x00, 0x64, 0xFF, 0xFF);
        public static Color UncommonLow = new Color32(0x5D, 0x24, 0xBF, 0xFF);
        public static Color UncommonHigh = new Color32(0x36, 0x81, 0x8E, 0xFF);
        public static Color RareHigh = new Color32(0xEA, 0x8C, 0x27, 0xFF);
        public static Color MythicalHigh = new Color32(0x51, 0xFF, 0xA8, 0xFF);
        
        public static Color GetEyeColor(int eyeColorTrait, Collateral collateral)
        {
            if (eyeColorTrait < 2) return MythicalLow;
            if (eyeColorTrait < 10) return RareLow;
            if (eyeColorTrait < 25) return UncommonLow;
            if (eyeColorTrait < 75) return collateral.PrimaryColor;
            if (eyeColorTrait < 90) return UncommonHigh;
            if (eyeColorTrait < 98) return RareHigh;
            return MythicalHigh;
        }

        static int[] eyeShapeTraitTraitRange = { 0, 1, 2, 5, 7, 10, 15, 20, 25, 42, 58, 75, 80, 85, 90, 93, 95, 98 };

        public static int GetEyeShapeId(int eyeShapeTrait)
        {
            if (eyeShapeTrait < 0)
                return 0;

            for (int i = 0; i < eyeShapeTraitTraitRange.Length - 1; i++)
            {
                if (eyeShapeTrait >= eyeShapeTraitTraitRange[i]
                    && eyeShapeTrait < eyeShapeTraitTraitRange[i + 1])
                {
                    return i;
                }
            }
            return -1;
        }
    }
}