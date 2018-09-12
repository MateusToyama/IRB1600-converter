using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRB1600_Converter
{
    public class ProgramParser
    {
        private const string linePattern = @"MoveL Offs\(p0, (?<x>-{0,1}[\d.]+), (?<y>-{0,1}[\d.]+), (?<z>-{0,1}[\d.]+)\), v\d+, z\d+, tool0;";

        public List<SVGCommand> Parse(string filename)
        {
            List<SVGCommand> result = new List<SVGCommand>();
            TextReader textReader = new StreamReader(filename);
            string line;
            Regex regex = new Regex(linePattern);
            Point lastPoint = new Point();
            while ((line = textReader.ReadLine()) != null)
            {
                Match match = regex.Match(line);
                if (match.Success)
                {
                    Point point = new Point(Convert.ToDouble(match.Groups["x"].Value, CultureInfo.InvariantCulture), Convert.ToDouble(match.Groups["y"].Value, CultureInfo.InvariantCulture));
                    if (point != lastPoint)
                    {
                        SVGCommand svgCommand = new SVGCommand(Convert.ToDouble(match.Groups["z"].Value) > 0 ? "M" : "L");
                        svgCommand.Points.Add(point);
                        result.Add(svgCommand);
                    }
                }
            }
            textReader.Close();
            return result;
        }
    }
}
