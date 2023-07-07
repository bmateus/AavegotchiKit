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
        //public Sprite[] sprites;
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
            Debug.Log("Importing eye shapes...");
            var tmp = JsonUtility.FromJson<EyeShapes>(source.text);
            eyeShapes = tmp.eyeShapes;

            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }


    }
}
