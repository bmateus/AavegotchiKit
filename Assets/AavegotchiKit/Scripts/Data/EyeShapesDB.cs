using System;
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

            var sprite = SvgLoader.GetSvgLayerSprite($"eyes-{shapeIndex}-{facing}",
                eyeShape.svgs[(int)facing],
                new SvgLoader.Options
                {
                    size = new Vector2(64, 64),
                    primary = collateral.PrimaryColor,
                    secondary = collateral.SecondaryColor,
                    cheeks = collateral.CheekColor,
                    eyes = eyeColor
                }
               );

            if (sprite == null)
                Debug.LogError($"Failed to load eye shape sprite for eyes-{shapeIndex}-{facing}!");

            return sprite;
        }

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
