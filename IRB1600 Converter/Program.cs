using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace IRB1600_Converter
{
    class Program
    {
        static void GenerateProgram(string inputFile, string outputFile, double sheetWidth, double sheetHeight, double margin, bool autoMode, double scale, double offsetX, double offsetY)
        {
            SVGParser svgParser = new SVGParser();
            List<SVGCommand> commands = svgParser.Parse(inputFile);
            if (autoMode)
            {
                double scaleX = (sheetWidth - 2 * margin) / svgParser.ViewBoxWidth;
                double scaleY = (sheetHeight - 2 * margin) / svgParser.ViewBoxHeight;
                scale = Math.Min(scaleX, scaleY);
                offsetX = (sheetWidth - (svgParser.ViewBoxWidth * scale)) / 2;
                offsetY = (sheetHeight - (svgParser.ViewBoxHeight * scale)) / 2;
            }
            ProgramWriter programWriter = new ProgramWriter(outputFile, scale, offsetX, offsetY);
            programWriter.AddCommands(commands);
            programWriter.Save();
        }

        static void GenerateSVG(string inputFile, string outputFile, string sheetWidth, string sheetHeight, double viewboxWidth, double viewBoxHeight)
        {
            ProgramParser programParser = new ProgramParser();
            List<SVGCommand> commands = programParser.Parse(inputFile);
            SVGWriter svgWriter = new SVGWriter(outputFile, sheetWidth, sheetHeight, viewboxWidth, viewBoxHeight);
            svgWriter.AddCommands(commands);
            svgWriter.Save();
        }

        static void Main(string[] args)
        {
            string inputFile = null;
            string outputFile = null;
            bool autoMode = false;
            string scale = null;
            string offsetX = null;
            string offsetY = null;
            string sheetWidth = null;
            string sheetHeight = null;
            string margin = null;
            string viewBoxWidth = null;
            string viewBoxHeight = null;
            CommandSet commandSet = new CommandSet("program", Console.Out, Console.Error)
            {
                { "i|input=", "File used as input", a => inputFile = a },
                { "o|output=", "File used as output", a => outputFile = a },
                new Command("program", "Generate program from SVG")
                {
                    Options = new OptionSet()
                    {
                        { "w|width=", "Sheet width (mm)", a => sheetWidth = a },
                        { "h|height=", "Sheet height (mm)", a => sheetHeight = a },
                        { "m|margin=", "Margin (mm)", a => margin = a },
                        { "a|auto", "Auto set scale and offsets", a => autoMode = true },
                        { "s|scale=", "Scale", a => scale = a },
                        { "x|offsetx=", "Offset x", a => offsetX = a },
                        { "y|offsety=", "Offset y", a => offsetY = a }
                    },
                    Run = a =>
                    {
                        if (string.IsNullOrEmpty(inputFile))
                        {
                            Console.WriteLine("Error: input argument missing");
                        }
                        else if (!File.Exists(inputFile))
                        {
                            Console.WriteLine("Error: input file does not exist");
                        }
                        else if (string.IsNullOrEmpty(outputFile))
                        {
                            Console.WriteLine("Error: output argument missing");
                        }
                        else if (string.IsNullOrEmpty(sheetWidth))
                        {
                            Console.WriteLine("Error: width argument missing");
                        }
                        else if (string.IsNullOrEmpty(sheetHeight))
                        {
                            Console.WriteLine("Error: height argument missing");
                        }
                        else
                        {
                            double marginValue = string.IsNullOrEmpty(margin) ? 0 : Convert.ToDouble(margin);
                            double scaleValue = string.IsNullOrEmpty(scale) ? 1 : Convert.ToDouble(scale);
                            double offsetXValue = string.IsNullOrEmpty(offsetX) ? 0 : Convert.ToDouble(offsetX);
                            double offsetYValue = string.IsNullOrEmpty(offsetY) ? 0 : Convert.ToDouble(offsetY);
                            GenerateProgram(inputFile, outputFile, Convert.ToDouble(sheetWidth, CultureInfo.InvariantCulture), Convert.ToDouble(sheetHeight, CultureInfo.InvariantCulture), marginValue, autoMode, scaleValue, offsetXValue, offsetYValue);
                        }
                    }
                },
                new Command("svg", "Generate SVG from program")
                {
                    Options = new OptionSet()
                    {
                        { "w|width=", "SVG width parameter", a => sheetWidth = a },
                        { "h|height=", "SVG height parameter", a => sheetHeight = a },
                        { "x|viewboxwidth=", "SVG viewBox width", a => viewBoxWidth = a },
                        { "y|viewboxheight=", "SVG viewBox height", a => viewBoxHeight = a }
                    },
                    Run = a =>
                    {
                        if (string.IsNullOrEmpty(inputFile))
                        {
                            Console.WriteLine("Error: input argument missing");
                        }
                        else if (!File.Exists(inputFile))
                        {
                            Console.WriteLine("Error: input file does not exist");
                        }
                        else if (string.IsNullOrEmpty(outputFile))
                        {
                            Console.WriteLine("Error: output argument missing");
                        }
                        else if (string.IsNullOrEmpty(sheetWidth))
                        {
                            Console.WriteLine("Error: width argument missing");
                        }
                        else if (string.IsNullOrEmpty(sheetHeight))
                        {
                            Console.WriteLine("Error: height argument missing");
                        }
                        else if (string.IsNullOrEmpty(viewBoxWidth))
                        {
                            Console.WriteLine("Error: viewboxwidth argument missing");
                        }
                        else if (string.IsNullOrEmpty(viewBoxHeight))
                        {
                            Console.WriteLine("Error: viewboxheight argument missing");
                        }
                        else
                        {
                            GenerateSVG(inputFile, outputFile, sheetWidth, sheetHeight, Convert.ToDouble(viewBoxWidth, CultureInfo.InvariantCulture), Convert.ToDouble(viewBoxHeight, CultureInfo.InvariantCulture));
                        }
                    }
                }
            };
            try
            {
                commandSet.Run(args);
            }
            catch (OptionException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
