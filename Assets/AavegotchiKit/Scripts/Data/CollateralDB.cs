using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace com.mycompany
{
    [Serializable]
    public class Collateral
    {
        public string collateralType;
        public int[] modifiers;
        public string primaryColor;
        public string secondaryColor;
        public string cheekColor;
        public int haunt;

        public int svgId;
        public int eyeShapeSvgId;

        public Sprite[] sprites;
        public Sprite[] eyeSprites;

        public Sprite GetCollateralSprite(AavegotchiData.Facing facing)
        {
            if (facing == AavegotchiData.Facing.BACK)
                return null;

            return sprites[(int)facing];
        }

        public Sprite GetCollateralEyeSprite(AavegotchiData.Facing facing)
        {
            if (facing == AavegotchiData.Facing.BACK)
                return null;

            return eyeSprites[(int)facing];
        }

        static Color parseColor(string color)
        {            
            ColorUtility.TryParseHtmlString(color.Replace("0x","#"), out var c);
            return c;
        }

        public Color PrimaryColor => parseColor(primaryColor);
        public Color SecondaryColor => parseColor(secondaryColor);

        public Color CheekColor => parseColor(cheekColor);

    }

    [Serializable]
    public class Collaterals
    {
        public Collateral[] collaterals;
    }


    [CreateAssetMenu(fileName = "CollateralDB", menuName = "Aavegotchi/CollateralDB")]
    public class CollateralDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        public Collateral[] collaterals;

#if UNITY_EDITOR
        Sprite GetSpriteAsset(string spritename)
        {
            try
            {
                var guid = AssetDatabase.FindAssets(spritename).First();
                return AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guid));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return null;
            }
        }
#endif


        [ContextMenu("Import Data")]
        void Import()
        {
#if UNITY_EDITOR
            var tmp = JsonUtility.FromJson<Collaterals>(source.text);
            Debug.Log("Importing collaterals...");
            for (int i = 0; i < tmp.collaterals.Length; ++i)
            {
                var collateral = tmp.collaterals[i];
                //assign sprite
                var sprites = new Sprite[3];
                sprites[0] = GetSpriteAsset($"collateral_{collateral.svgId}");
                sprites[1] = GetSpriteAsset($"collateral_{collateral.svgId}_left");
                sprites[2] = GetSpriteAsset($"collateral_{collateral.svgId}_right");
                collateral.sprites = sprites;

                var eyeSprites = new Sprite[3];
                eyeSprites[0] = GetSpriteAsset($"eyeShapes-{collateral.eyeShapeSvgId}");
                eyeSprites[1] = GetSpriteAsset($"eyeShapes-{collateral.eyeShapeSvgId}_left");
                eyeSprites[2] = GetSpriteAsset($"eyeShapes-{collateral.eyeShapeSvgId}_right");
                collateral.eyeSprites = eyeSprites;
            }
            collaterals = tmp.collaterals;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

    }
}
