using System;
using System.Linq;
using UnityEditor;
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
