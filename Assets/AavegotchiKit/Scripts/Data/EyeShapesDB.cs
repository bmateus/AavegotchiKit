using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [Serializable]
    public class EyeShape
    {
        public int id;
        public int haunt;
        public int rangeMin;
        public int rangeMax;
        public Sprite[] sprites;
        public string[] svgs;
    }

    [Serializable]
    public class EyeShapes
    {
        public EyeShape[] eyeShapes;
    }

    [CreateAssetMenu(fileName = "EyeShapesDB", menuName = "Aavegotchi/EyeShapesDB")]
    public class EyeShapesDB : ScriptableObject
    {
        [SerializeField]
        TextAsset source;

        public EyeShape[] eyeShapes;

        public Sprite GetEyeShapeSprite(int shapeIndex, Color eyeColor, Collateral collateral, GotchiFacing facing)
        {
            var eyeShape = eyeShapes[shapeIndex];

#if USE_VECTOR_GFX
            return SvgLoader.GetSvgLayerSprite($"eyes-{shapeIndex}-{facing}",
                eyeShape.svgs[(int)facing],
                new SvgLoader.Options
                {
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    cheeks = collateral.CheekColor,
                    eyes = eyeColor
                }
               );
#else
            return eyeShape.sprites[(int)facing];
#endif
        }

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
            var tmp = JsonUtility.FromJson<EyeShapes>(source.text);
            Debug.Log("Importing eye shapes...");
            for (int i = 0; i < tmp.eyeShapes.Length; ++i)
            {
                var eyeShape = tmp.eyeShapes[i];
                //assign sprite
                var sprites = new Sprite[3];
                if (eyeShape.haunt == 1)
                {
                    sprites[0] = GetSpriteAsset($"eyeShapes-{eyeShape.id}");
                    sprites[1] = GetSpriteAsset($"eyeShapes-{eyeShape.id}_left");
                    sprites[2] = GetSpriteAsset($"eyeShapes-{eyeShape.id}_right");
                }
                else
                {
                    sprites[0] = GetSpriteAsset($"eyeShapesH{eyeShape.haunt}-{eyeShape.id}");
                    sprites[1] = GetSpriteAsset($"eyeShapesH{eyeShape.haunt}-{eyeShape.id}_left");
                    sprites[2] = GetSpriteAsset($"eyeShapesH{eyeShape.haunt}-{eyeShape.id}_right");
                }
                
                eyeShape.sprites = sprites;
            }
            eyeShapes = tmp.eyeShapes;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }


    }
}
