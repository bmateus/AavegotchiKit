using System.Xml;

namespace PortalDefender.AavegotchiKit
{
    // This is how the SVG's are styled in the minigame template
    // See: https://github.com/aavegotchi/aavegotchi-minigame-template/blob/main/app/src/helpers/aavegotchi/index.ts
    // Unfortunately, Unity doesn't like it when a style is used before it's defined
    // This workaround fixes the issue by moving the style to the top of the SVG

    [System.Serializable]
    public class GotchiSvgStyling
    {
        public bool removeBackground = true;
        
        public bool removeShadow = true;
               
        public bool removeHandsUp = false;

        public bool removeHandsDownClosed = false;

        public bool removeHandsDownOpen = false;

        public bool removeWearablePet = false;

        public bool removeWearableLHand = false;

        public bool removeWearableRHand = false;

        public string CustomizeSVG(string svg)
        {
            //move style to top (to work around Unity issue)
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(svg);
            //find the style node
            XmlNodeList nodes = doc.GetElementsByTagName("style");
            if (nodes.Count > 0)
            {
                //move it to the top
                XmlNode style = nodes[0];
                style.ParentNode.RemoveChild(style);

                //find the first node named "svg"
                var svgNodes = doc.GetElementsByTagName("svg");
                XmlNode svgNode = svgNodes[0];
                svgNode.PrependChild(style);

                //now modify the style by changing the body text of the style node
                if (removeBackground)
                {
                    style.InnerText = ".gotchi-bg,.wearable-bg{display:none;}" + style.InnerText;
                }

                if (removeShadow)
                {
                    style.InnerText = ".gotchi-shadow{display:none;}" + style.InnerText;
                }

                if (removeHandsUp)
                {
                    if (style.InnerText.Contains(".gotchi-handsUp"))
                    {
                        style.InnerText = style.InnerText.Replace(".gotchi-handsUp{display:block;}", ".gotchi-handsUp{display:none;}");
                    }
                    else
                    {
                        style.InnerText = ".gotchi-handsUp{display:none;}" + style.InnerText;
                    }

                    //also remove sleeves? hmm style doesn't seem to apply to sub svg's (bug)
                    if (style.InnerText.Contains(".gotchi-sleeves-up"))
                    {
                        style.InnerText = style.InnerText.Replace(".gotchi-sleeves-up{display:block;}", ".gotchi-sleeves-up{display:none;}");
                    }
                    else
                    {
                        style.InnerText = "gotchi-sleeves-up{display:none;}" + style.InnerText;
                    }
                }

                if (removeHandsDownClosed)
                {
                    if (style.InnerText.Contains(".gotchi-handsDownClosed"))
                    {
                        style.InnerText = style.InnerText.Replace(".gotchi-handsDownClosed{display:block;}", ".gotchi-handsDownClosed{display:none;}");
                    }
                    else
                    {
                        style.InnerText = ".gotchi-handsDownClosed{display:none;}" + style.InnerText;
                    }
                }

                if (removeHandsDownOpen)
                { 
                    if (style.InnerText.Contains(".gotchi-handsDownOpen"))
                    {
                        style.InnerText = style.InnerText.Replace(".gotchi-handsDownOpen{display:block;}", ".gotchi-handsDownOpen{display:none;}");
                    }
                    else
                    {
                        style.InnerText = ".gotchi-handsDownOpen{display:none;}" + style.InnerText;
                    }

                    //also remove sleeves? hmm style doesn't seem to apply to sub svg's (bug)
                    if (style.InnerText.Contains(".gotchi-sleeves-down"))
                    {
                        style.InnerText = style.InnerText.Replace(".gotchi-sleeves-down{display:block;}", ".gotchi-sleeves-down{display:none;}");
                    }
                    else
                    {
                        style.InnerText = "gotchi-sleeves-down{display:none;}" + style.InnerText;
                    }
                }   

                if (removeWearableLHand)
                {
                    style.InnerText = ".wearable-hand-left{display:none;}" + style.InnerText;
                }

                if (removeWearableRHand)
                {
                    style.InnerText = ".wearable-hand-right{display:none;}" + style.InnerText;
                }

                if (removeWearablePet)
                {
                    style.InnerText = ".wearable-pet{display:none;}" + style.InnerText;
                }

                svg = doc.OuterXml;
            }

            return svg;
        }
    }
}