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
        public Color primary;
        public Color secondary;
        public Color cheeks;
        public Color eyes;

        public bool hideMouth;
        public bool hideSleeves;
        public bool hideSleevesUp;
        public bool hideSleevesDown;
        public bool hideHandsUp;
        public bool hideHandsDownOpen;
        public bool hideHandsDownClosed;
        public bool hideShadow;

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
        var style = "<style>";
        style += $".gotchi-primary{{fill:#{ColorUtility.ToHtmlStringRGB(options.primary)};}}";
        style += $".gotchi-secondary{{fill:#{ColorUtility.ToHtmlStringRGB(options.secondary)};}}";
        style += $".gotchi-cheek{{fill:#{ColorUtility.ToHtmlStringRGB(options.cheeks)};}}";
        style += $".gotchi-eyeColor{{fill:#{ColorUtility.ToHtmlStringRGB(options.eyes)};}}";

        if (options.hideMouth)
        {
            style += $".gotchi-primary-mouth{{display:none;}}";
        }

        if (options.hideSleeves)
        {
            style += $".gotchi-sleeves{{display:none;}}";
        }
        else
        {
            style += $".gotchi-sleeves{{display:block;}}";
        }
        
        if (options.hideSleevesUp)
        {
            style += $".gotchi-sleeves-up{{display:none;}}";
        }
        else
        {
            style += $".gotchi-sleeves-up{{display:block;}}";
        }

        if (options.hideSleevesDown)
        {
            style += $".gotchi-sleeves-down{{display:none;}}";
        }
        else
        {
            style += $".gotchi-sleeves-down{{display:block;}}";
        }

        if (options.hideHandsUp)
        {
            style += $".gotchi-handsUp{{display:none;}}";
        }

        if (options.hideHandsDownOpen)
        {
            style += $".gotchi-handsDownOpen{{display:none;}}";
        }

        if (options.hideHandsDownClosed)
        {
            style += $".gotchi-handsDownClosed{{display:none;}}";
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
        var cachedName = $"{name}-{options.GetHashCode()}";

        if (_cached.TryGetValue(cachedName, out var sprite))
        {
            return sprite;
        }

        var style = CreateStyle(options);

        //wrap the layer data
        var wrappedData = $"<svg xmlns=\"http://www.w3.org/2000/svg\" shape-rendering=\"crispEdges\" width=\"{64-options.size.x}\" height=\"{64-options.size.y}\" >"
            + style
            + layerData
            + "</svg>";

        return GetSvgSprite(cachedName, wrappedData, options.customPivot);
    }


    // Uses the Unity SVG Lib to import an SVG Sprite at runtime
    public static Sprite CreateSvgSprite(string data, Vector2 customPivot, bool preserveViewport = true)
    {
        if (string.IsNullOrEmpty(data))
            return null;

        // the sprites were exported from 64x64 => to 256x256

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
        

        float svgPixelsPerUnit = 75f; //100 
        //note: 75 = (300/4): 300 is the PPU set in the sprite import settings and they were exported from 64x64 to 256x256 a factor of 4
        VectorUtils.Alignment alignment = VectorUtils.Alignment.Center;

        if (customPivot != Vector2.zero)
            alignment = VectorUtils.Alignment.Custom;
        //Vector2 customPivot = Vector2.zero;

        ushort gradientResolution = 256;
        bool flipYAxis = true;

        //var sceneInfo = SVGParser.ImportSVG(new StringReader(data));
        var sceneInfo = SVGParser.ImportSVG(new StringReader(data), viewportOptions, 0, 1, 100, 100);
        //var sceneInfo = SVGParser.ImportSVG(new StringReader(data), viewportOptions, dpi, pixelsPerUnit, windowWidth, windowHeight);

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

        Sprite sprite = null;
        //if (preserveViewPort)
        {
            sprite = VectorUtils.BuildSprite(geoms, sceneInfo.SceneViewport, svgPixelsPerUnit, alignment, customPivot, gradientResolution, flipYAxis);
        }
        //else
        //{
        //    sprite = VectorUtils.BuildSprite(geoms, svgPixelsPerUnit, alignment, customPivot, gradientResolution, flipYAxis);
        //}

        return sprite;
    }
}
