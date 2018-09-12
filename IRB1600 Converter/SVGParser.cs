using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace IRB1600_Converter
{
    public class SVGParser
    {
        private const string commandPattern = @"(?<command>[a-zA-Z])[ ]*(?<parameters>(?:-{0,1}[\d\.]+[ ,\r\n]*)*)";
        private const string pointsPattern = @"(?<x>-{0,1}[\d\.]+)[ ,](?<y>-{0,1}[\d\.]+)";

        public int ViewBoxWidth { get; private set; }
        public int ViewBoxHeight { get; private set; }

        public List<SVGCommand> Parse(string filename)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);
            string[] values = xmlDocument.DocumentElement.GetAttribute("viewBox").Split(' ');
            ViewBoxWidth = Convert.ToInt32(values[2]);
            ViewBoxHeight = Convert.ToInt32(values[3]);
            List<SVGCommand> result = new List<SVGCommand>();
            foreach (XmlNode elementNode in xmlDocument.DocumentElement.ChildNodes)
            {
                if (elementNode.Name == "g")
                {
                    foreach (XmlNode childNode in elementNode.ChildNodes)
                    {
                        result.AddRange(ParseNode(childNode));
                    }
                }
                else if (elementNode.Name == "path")
                {
                    result.AddRange(ParseNode(elementNode));
                }
            }
            return result;
        }

        private List<SVGCommand> ParseNode(XmlNode elementNode)
        {
            List<SVGCommand> result = new List<SVGCommand>();
            if (elementNode.Name == "path" && elementNode.Attributes.GetNamedItem("d") != null)
            {
                Point firstPoint = new Point(0, 0);
                bool firstPointFound = false;
                Point lastPoint = new Point(0, 0);
                Regex commandRegex = new Regex(commandPattern);
                MatchCollection commandMatches = commandRegex.Matches(elementNode.Attributes["d"].Value);
                foreach (Match commandMatch in commandMatches)
                {
                    SVGCommand command = new SVGCommand(commandMatch.Groups["command"].Value);
                    Regex pointsRegex = new Regex(pointsPattern);
                    MatchCollection pointsMatches = pointsRegex.Matches(commandMatch.Groups["parameters"].Value);
                    foreach (Match pointMatch in pointsMatches)
                    {
                        Point point = new Point(Convert.ToDouble(pointMatch.Groups["x"].Value, CultureInfo.InvariantCulture), Convert.ToDouble(pointMatch.Groups["y"].Value, CultureInfo.InvariantCulture));
                        command.Points.Add(point);
                        if (!firstPointFound)
                        {
                            firstPointFound = true;
                            firstPoint = point;
                            if (command.IsRelative)
                            {
                                firstPoint += lastPoint;
                            }
                        }
                    }
                    if (command.CommandType == SVGCommand.CommandTypes.ClosePath)
                    {
                        command.Points.Add(firstPoint);
                        firstPointFound = false;
                    }
                    lastPoint = command.Points.Last();
                    result.Add(command);
                }
            }
            return result;
        }
    }
}
