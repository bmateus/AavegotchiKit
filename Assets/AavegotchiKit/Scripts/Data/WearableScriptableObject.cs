using Cysharp.Threading.Tasks;
using System;

//using Unity.VectorGraphics.Editor;
using UnityEditor;
using UnityEngine;

namespace PortalDefender.AavegotchiKit
{
    [CreateAssetMenu(fileName = "Wearable", menuName = "AavegotchiKit/DB/Wearable")]
    public class WearableScriptableObject : ScriptableObject
    {
        public Wearable data = new Wearable();

        public Sprite[] svgs;

        public Sprite[] sleeves;

#if UNITY_EDITOR

        [ContextMenu("Refresh Data")]
        public async UniTask Refresh()
        {
            Debug.Log($"Refreshing Data for wearable id: {data.id}");
            WearableFetcher fetcher = new WearableFetcher();
            var w = await fetcher.GetWearable(data.id);
            if (w != null)
            {
                data = w;

                //Want to create a sprite here and add it as a subasset
                //But Unity doesn't allow it (only works at runtime)
                //Sprite s = w.GetSprite(GotchiHandPose.DOWN_OPEN, GotchiFacing.FRONT);


                //instead, need to export the SVG as a file (i.e. ExportSvgs)
                //and then
                //import it as a sprite in a separate pass (i.e. ProcessSprites)
                ExportSvgs();
                ProcessSprites();

                EditorUtility.SetDirty(this);
            }
        }

        
        /// <summary>
        /// Exports SVGs for each wearable item.
        /// </summary>
        [ContextMenu("Export SVGs")]
        void ExportSvgs()
        {
            for (int i = 0; i < data.svgs.Length; i++)
            {
                var dimensions = data.GetDimensions(GotchiFacing.FRONT);

                var options = new SvgLoader.Options()
                {
                    id = data.id,
                    hideSleeves = false,
                    handPose = GotchiHandPose.DOWN_OPEN,
                    offset = new Vector2(dimensions.X, dimensions.Y),
                    size = new Vector2(dimensions.Width, dimensions.Height)
                };

                var style = SvgLoader.CreateStyle(options);

                //wrap the layer data
                var wrappedSvg = $"<svg xmlns=\"http://www.w3.org/2000/svg\" shape-rendering=\"crispEdges\" width=\"64\" height=\"64\" >"
                    + $"<svg x=\"{dimensions.X}\" y=\"{dimensions.Y}\">"
                    + style
                    + data.svgs[i]
                    + "</svg></svg>";

                //get path to this asset
                string path = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
                string filename = $"wearable-{data.id}-{i}.svg";
                path = System.IO.Path.Combine(path, filename);
                var existing = AssetDatabase.LoadMainAssetAtPath(path);
                if (existing != null)
                {
                    AssetDatabase.DeleteAsset(path);
                }

                Debug.Log($"Writing SVG to {path}");

                System.IO.File.WriteAllText(path, wrappedSvg);

                //var importer = (SVGImporter)AssetImporter.GetAtPath(path);
                //importer.SvgPixelsPerUnit = 64;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                //AssetDatabase.Refresh();
                //AssetDatabase.SaveAssets();
            }
        }


        [ContextMenu("Process Sprites")]
        void ProcessSprites()
        {
            ClearSprites();

            svgs = new Sprite[data.svgs.Length];
            
            //find the sprite asset and add them as subassets
            var path = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            for (int i = 0; i < data.svgs.Length; i++)
            {
                string filename = $"wearable-{data.id}-{i}.svg";
                string spritePath = System.IO.Path.Combine(path, filename);
                var existing = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (existing != null)
                {
                    AssetDatabase.RemoveObjectFromAsset(existing);
                    AssetDatabase.AddObjectToAsset(existing, this);
                    existing.name = $"wearable-{data.id}-{i}";
                    svgs[i] = existing;
                }
                
                AssetDatabase.DeleteAsset(spritePath);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [ContextMenu("Clear Sprites")]
        void ClearSprites()
        {
            svgs = null;
            //remove all sprite subassets from this asset
            var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (var subAsset in subAssets)
            {
                if (subAsset is Sprite)
                {
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
                    DestroyImmediate(subAsset, true);
                }
            }
            EditorUtility.SetDirty(this);
        }
#endif

    }
}