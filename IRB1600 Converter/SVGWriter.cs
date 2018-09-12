using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IRB1600_Converter
{
    public class SVGWriter
    {
        private string filename;
        private XmlDocument xml;

        public SVGWriter(string filename, string width, string height, double viewBoxWidth, double viewBoxHeight, bool border = true)
        {
            this.filename = filename;
            xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", "no"));
            xml.AppendChild(xml.CreateDocumentType("svg", "-//W3C//DTD SVG 20010904//EN", "http://www.w3.org/TR/2001/REC-SVG-20010904/DTD/svg10.dtd", null));
            XmlElement svg = xml.CreateElement("svg", "http://www.w3.org/2000/svg");
            svg.SetAttribute("width", width);
            svg.SetAttribute("height", height);
            svg.SetAttribute("viewBox", $"0 0 {viewBoxWidth} {viewBoxHeight}");
            if (border)
            {
                XmlElement rect = xml.CreateElement("rect", svg.NamespaceURI);
                rect.SetAttribute("fill", "none");
                rect.SetAttribute("stroke", "black");
                rect.SetAttribute("stroke-width", "1");
                rect.SetAttribute("width", viewBoxWidth.ToString());
                rect.SetAttribute("height", viewBoxHeight.ToString());
                svg.AppendChild(rect);
            }
            XmlElement path = xml.CreateElement("path", svg.NamespaceURI);
            path.SetAttribute("fill", "none");
            path.SetAttribute("stroke", "black");
            path.SetAttribute("stroke-width", "1");
            svg.AppendChild(path);
            xml.AppendChild(svg);
        }

        public void AddCommands(List<SVGCommand> commands)
        {
            XmlElement path = (XmlElement)xml.DocumentElement.SelectSingleNode("//*[local-name()='path']");
            string pathValue = "";
            foreach (SVGCommand command in commands)
            {
                if (pathValue != "")
                {
                    pathValue += " ";
                }
                switch (command.CommandType)
                {
                    case SVGCommand.CommandTypes.MoveToAbsolute:
                        pathValue += $"M {command.Points[0].Y.ToString(CultureInfo.InvariantCulture)},{command.Points[0].X.ToString(CultureInfo.InvariantCulture)}";
                        break;
                    case SVGCommand.CommandTypes.LineToAbsolute:
                        pathValue += $"L {command.Points[0].Y.ToString(CultureInfo.InvariantCulture)},{command.Points[0].X.ToString(CultureInfo.InvariantCulture)}";
                        break;
                }
            }
            path.SetAttribute("d", pathValue);
        }

        public void Save()
        {
            xml.Save(filename);
        }
    }
}
