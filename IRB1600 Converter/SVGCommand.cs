using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRB1600_Converter
{
    public class SVGCommand
    {
        public enum CommandTypes
        {
            Unknown, MoveToAbsolute, MoveToRelative, BezierAbsolute, BezierRelative, LineToAbsolute, LineToRelative, ClosePath
        }

        public CommandTypes CommandType { get; set; }

        public bool IsRelative
        {
            get
            {
                switch (CommandType)
                {
                    case CommandTypes.BezierRelative:
                    case CommandTypes.LineToRelative:
                    case CommandTypes.MoveToRelative:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public List<Point> Points { get; }

        public SVGCommand(string command)
        {
            switch (command)
            {
                case "M":
                    CommandType = CommandTypes.MoveToAbsolute;
                    break;
                case "m":
                    CommandType = CommandTypes.MoveToRelative;
                    break;
                case "C":
                    CommandType = CommandTypes.BezierAbsolute;
                    break;
                case "c":
                    CommandType = CommandTypes.BezierRelative;
                    break;
                case "z":
                case "Z":
                    CommandType = CommandTypes.ClosePath;
                    break;
                case "L":
                    CommandType = CommandTypes.LineToAbsolute;
                    break;
                case "l":
                    CommandType = CommandTypes.LineToRelative;
                    break;
                default:
                    CommandType = CommandTypes.Unknown;
                    break;
            }
            Points = new List<Point>();
        }

        public override string ToString()
        {
            return CommandType.ToString();
        }
    }
}
