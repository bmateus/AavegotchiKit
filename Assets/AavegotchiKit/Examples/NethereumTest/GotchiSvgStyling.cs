using System.Xml;

namespace PortalDefender.AavegotchiKit.Examples
{
    // This is how the SVG's are styled in the minigame template
    // See: https://github.com/aavegotchi/aavegotchi-minigame-template/blob/main/app/src/helpers/aavegotchi/index.ts
    // Unfortunately, Unity doesn't like it when a style is used before it's defined
    // This workaround fixes the issue by moving the style to the top of the SVG

    public class GotchiSvgStyling
    {
        public bool RemoveBackground { get; set; }
        public bool RemoveShadow { get; set; }

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
                XmlNode svgNode = doc.GetElementsByTagName("svg")[0];
                svgNode.PrependChild(style);

                //now modify the style by changing the body text of the style node
                if (RemoveBackground)
                {
                    style.InnerText = ".gotchi-bg,.wearable-bg{display:none;}" + style.InnerText;
                }

                if (RemoveShadow)
                {
                    style.InnerText = ".gotchi-shadow{display:none;}" + style.InnerText;
                }

                svg = doc.OuterXml;
            }

            return svg;
        }
    }
}