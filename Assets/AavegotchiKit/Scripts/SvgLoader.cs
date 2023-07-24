using PortalDefender.AavegotchiKit;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VectorGraphics;
using UnityEngine;

public class SvgLoader
{
    static Dictionary<string, Sprite> _cached = new Dictionary<string, Sprite>();

    public struct Options
    {
        public int id;

        public Color primary;
        public Color secondary;
        public Color cheeks;
        public Color eyes;

        public bool hideMouth;
        public bool hideSleeves;
        public bool hideShadow;

        public GotchiHandPose handPose;

        public Vector2 offset;
        public Vector2 size;
        public Vector2 customPivot;

        public override int GetHashCode()
        {
            return (primary, secondary, cheeks, eyes).GetHashCode();

            //var hashCode = primary.GetHashCode();
            //hashCode = (hashCode * 397) ^ secondary.GetHashCode();
            //hashCode = (hashCode * 397) ^ cheeks.GetHashCode();
            //hashCode = (hashCode * 397) ^ eyes.GetHashCode();
            //return hashCode;
        }
    }

    public static Sprite GetSvgSprite(string name, string data, Vector2 customPivot)
    {
        if (_cached.TryGetValue(name, out var sprite))
        {
            //Debug.Log("Cache Hit!");
            return sprite;
        }

        sprite = CreateSvgSprite(data, customPivot);
        if (sprite != null)
        {
            sprite.name = name;
            _cached.Add(name, sprite);
        }
        return sprite;
    }

    static string CreateStyle(SvgLoader.Options options)
    {
        //note:
        // - in SvgFacet, a gotchi has closed hands if they dont have anything equipped in the
        //   body slot or in their hands. Otherwise they have open hands
        // - the tags "gotchi-sleeves-left" and "gotchi-sleeves-right" aren't used in code at all

        var style = "<style>";
        style += $".gotchi-primary{{fill:#{ColorUtility.ToHtmlStringRGB(options.primary)};}}";
        style += $".gotchi-secondary{{fill:#{ColorUtility.ToHtmlStringRGB(options.secondary)};}}";
        style += $".gotchi-cheek{{fill:#{ColorUtility.ToHtmlStringRGB(options.cheeks)};}}";
        style += $".gotchi-eyeColor{{fill:#{ColorUtility.ToHtmlStringRGB(options.eyes)};}}";

        if (options.hideMouth)
        {
            style += ".gotchi-primary-mouth{display:none;}";
        }

        if (options.hideSleeves || options.handPose == GotchiHandPose.DOWN_CLOSED)
        {
            style += ".gotchi-sleeves{display:none;}";
        }
        else
        {
            style += ".gotchi-sleeves{display:block;}";
        }
        
        switch (options.handPose)
        {
            case GotchiHandPose.DOWN_CLOSED:
                style += ".gotchi-handsDownOpen{display:none;}";
                style += ".gotchi-handsDownClosed{display:block;}";
                style += ".gotchi-handsUp{display:none;}";
                style += ".gotchi-sleeves-up{display:none;}";
                style += ".gotchi-sleeves-down{display:none;}";
                break;
            case GotchiHandPose.DOWN_OPEN:
                style += ".gotchi-handsDownOpen{display:block;}";
                style += ".gotchi-handsDownClosed{display:none;}";
                style += ".gotchi-handsUp{display:none;}";
                style += ".gotchi-sleeves-up{display:none;}";
                style += ".gotchi-sleeves-down{display:block;}";
                break;
            case GotchiHandPose.UP:
                style += ".gotchi-handsDownOpen{display:none;}";
                style += ".gotchi-handsDownClosed{display:none;}";
                style += ".gotchi-handsUp{display:block;}";
                if (!options.hideSleeves)
                    style += ".gotchi-sleeves-up{display:block;}";
                else
                    style += ".gotchi-sleeves-up{display:none;}";
                style += ".gotchi-sleeves-down{display:none;}";
                // some additional styling for hands up see:
                // https://github.com/aavegotchi/aavegotchi-minigame-template/blob/main/app/src/helpers/aavegotchi/index.ts
                style += ".wearable-hand {transform: translateY(var(--hand_translateY, -4px));}";
                // todo: special styling for wearable id 207, 217, 223
                break;
        }

        if (options.hideShadow)
        {
            style += $".gotchi-shadow{{display:none;}}";
        }

        style += "</style>";
        return style;
    }

    //The Unity VectorGraphics package has issues handling multiple layers in an SVG

    public static Sprite GetSvgLayerSprite(string name, string layerData, SvgLoader.Options options)
    {
        var cachedName = $"{name}-0x{options.GetHashCode():X}";

        if (_cached.TryGetValue(cachedName, out var sprite))
        {
            return sprite;
        }

        var style = CreateStyle(options);

        //wrap the layer data
        var wrappedData = $"<svg xmlns=\"http://www.w3.org/2000/svg\" shape-rendering=\"crispEdges\" width=\"64\" height=\"64\" >"
            + $"<svg x=\"{options.offset.x}\" y=\"{options.offset.y}\">"
            + style
            + layerData
            + "</svg></svg>";

        return GetSvgSprite(cachedName, wrappedData, options.customPivot);
    }


    // Uses the Unity SVG Lib to import an SVG Sprite at runtime
    public static Sprite CreateSvgSprite(string data, Vector2 customPivot, bool preserveViewport = true)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        try
        {
            ViewportOptions viewportOptions = ViewportOptions.PreserveViewport;
            if (!preserveViewport)
            {
                viewportOptions = ViewportOptions.DontPreserve;
            }

            //float dpi = 0f;
            //float pixelsPerUnit = 1.0f;
            //int windowWidth = 0;
            //int windowHeight = 0;

            /*
            float stepDistance = 100.0f;
            float samplingStepSize = 0.01f;
            float maxCoordDeviation = 0.5f; //0.01
            float maxTanAngleDeviation = 0.1f;
            */

            float stepDistance = 10.0f;
            float samplingStepSize = 100.0f;
            float maxCoordDeviation = float.MaxValue;
            float maxTanAngleDeviation = Mathf.PI * 0.5f;

            float svgPixelsPerUnit = 64f;

            VectorUtils.Alignment alignment = VectorUtils.Alignment.Center;

            if (customPivot != Vector2.zero)
                alignment = VectorUtils.Alignment.Custom;

            //Vector2 customPivot = Vector2.zero;

            ushort gradientResolution = 256;
            bool flipYAxis = true;

            var sceneInfo = SVGParser.ImportSVG(new StringReader(data), viewportOptions, 0, 1, 64, 64);            

            if (sceneInfo.Scene == null || sceneInfo.Scene.Root == null)
                throw new Exception("Wowzers!");


            var tessOptions = new VectorUtils.TessellationOptions()
            {
                StepDistance = stepDistance,
                SamplingStepSize = samplingStepSize,
                MaxCordDeviation = maxCoordDeviation,
                MaxTanAngleDeviation = maxTanAngleDeviation,
            };

            var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions, sceneInfo.NodeOpacity);

            if (geoms.Count == 0)
            {
                //Debug.Log("No Geoms?");
                //Unity doesn't like making sprites with no geometry
                return null;
            }

            Sprite sprite = null;
            //if (preserveViewPort)
            {
                sprite = VectorUtils.BuildSprite(geoms, sceneInfo.SceneViewport, svgPixelsPerUnit, 
                    alignment, customPivot, gradientResolution, flipYAxis);
            }
            //else
            //{
            //    sprite = VectorUtils.BuildSprite(geoms, svgPixelsPerUnit, alignment, customPivot, gradientResolution, flipYAxis);
            //}

            return sprite;

        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }
}
