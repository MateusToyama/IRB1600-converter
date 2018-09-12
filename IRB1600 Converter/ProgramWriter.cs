using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace IRB1600_Converter
{
    public class ProgramWriter
    {
        private double scale;
        private Point offset;
        private int bezierPoints;
        private int drawingSpeed;
        private int moveSpeed;

        private TextWriter textWriter;
        private Point lastPoint;
        private string lastLine;

        public ProgramWriter(string filename, double scale = 1, double offsetX = 0, double offsetY = 0, int bezierPoints = 10, int drawingSpeed = 10, int moveSpeed = 50)
        {
            this.scale = scale;
            offset = new Point(offsetX, offsetY);
            this.bezierPoints = bezierPoints;
            this.drawingSpeed = drawingSpeed;
            this.moveSpeed = moveSpeed;
            textWriter = new StreamWriter(filename);
            textWriter.WriteLine("MODULE MainModule");
            textWriter.WriteLine("    CONST robtarget p0:=[[600.00,-297.00,500.00],[3.48237E-08,-0.707107,-0.707107,1.17632E-08],[-1,0,0,0],[9E+09,9E+09,9E+09,9E+09,9E+09,9E+09]];");
            textWriter.WriteLine("    PROC main()");
            textWriter.WriteLine($"        MoveJ p0, v{drawingSpeed}, z10, tool0;");
            lastPoint = new Point(0, 0);
            lastLine = $"        MoveL Offs(p0, {lastPoint.YFormatted}, {lastPoint.XFormatted}, 20.00), v{drawingSpeed}, z10, tool0;";
            textWriter.WriteLine(lastLine);
        }

        private void WriteProgramLine(Point point, double z, int speed)
        {
            point *= scale;
            point += offset;
            string line = $"        MoveL Offs(p0, {point.YFormatted}, {point.XFormatted}, {z.ToString("0.00", CultureInfo.InvariantCulture)}), v{speed}, z10, tool0;";
            if (line != lastLine)
            {
                textWriter.WriteLine(line);
                lastLine = line;
            }
        }

        public void AddCommands(List<SVGCommand> commands)
        {
            foreach (SVGCommand command in commands)
            {
                switch (command.CommandType)
                {
                    case SVGCommand.CommandTypes.MoveToAbsolute:
                        WriteProgramLine(lastPoint, 20, drawingSpeed);
                        lastPoint = command.Points[0];
                        WriteProgramLine(lastPoint, 20, moveSpeed);
                        WriteProgramLine(lastPoint, 0, drawingSpeed);
                        break;
                    case SVGCommand.CommandTypes.MoveToRelative:
                        WriteProgramLine(lastPoint, 20, drawingSpeed);
                        lastPoint += command.Points[0];
                        WriteProgramLine(lastPoint, 20, moveSpeed);
                        WriteProgramLine(lastPoint, 0, drawingSpeed);
                        break;
                    case SVGCommand.CommandTypes.BezierAbsolute:
                        for (int i = 0; i < command.Points.Count; i += 3)
                        {
                            List<Point> linearizedBezier = LinearizeBezierCurve(lastPoint, command.Points[i], command.Points[i + 1], command.Points[i + 2]);
                            foreach (Point point in linearizedBezier)
                            {
                                WriteProgramLine(point, 0, drawingSpeed);
                            }
                            lastPoint = linearizedBezier.Last();
                        }
                        break;
                    case SVGCommand.CommandTypes.BezierRelative:
                        for (int i = 0; i < command.Points.Count; i += 3)
                        {
                            List<Point> linearizedBezier = LinearizeBezierCurve(lastPoint, lastPoint + command.Points[i], lastPoint + command.Points[i + 1], lastPoint + command.Points[i + 2]);
                            foreach (Point point in linearizedBezier)
                            {
                                WriteProgramLine(point, 0, drawingSpeed);
                            }
                            lastPoint = linearizedBezier.Last();
                        }
                        break;
                    case SVGCommand.CommandTypes.LineToAbsolute:
                        for (int i = 0; i < command.Points.Count; i++)
                        {
                            lastPoint = command.Points[i];
                            WriteProgramLine(lastPoint, 0, drawingSpeed);
                        }
                        break;
                    case SVGCommand.CommandTypes.LineToRelative:
                        for (int i = 0; i < command.Points.Count; i++)
                        {
                            lastPoint += command.Points[i];
                            WriteProgramLine(lastPoint, 0, drawingSpeed);
                        }
                        break;
                    case SVGCommand.CommandTypes.ClosePath:
                        lastPoint = command.Points[0];
                        WriteProgramLine(lastPoint, 0, drawingSpeed);
                        break;
                }
            }
        }

        public void Save()
        {
            WriteProgramLine(lastPoint, 20, drawingSpeed);
            textWriter.WriteLine($"        MoveL Offs(p0, 0.00, 0.00, 20.00), v{moveSpeed}, z10, tool0;");
            textWriter.WriteLine($"        MoveL Offs(p0, 0.00, 0.00, 0.00), v{drawingSpeed}, z10, tool0;");
            textWriter.WriteLine("    ENDPROC");
            textWriter.WriteLine("ENDMODULE");
            textWriter.Close();
        }

        private List<Point> LinearizeBezierCurve(Point pointStart, Point pointControl1, Point pointControl2, Point pointEnd)
        {
            List<Point> result = new List<Point>();
            double delta = (double)1 / bezierPoints;
            for (double t = delta; t <= 1; t += delta)
            {
                double x = Math.Pow(1 - t, 3) * pointStart.X + 3 * t * Math.Pow(1 - t, 2) * pointControl1.X + 3 * Math.Pow(t, 2) * (1 - t) * pointControl2.X + Math.Pow(t, 3) * pointEnd.X;
                double y = Math.Pow(1 - t, 3) * pointStart.Y + 3 * t * Math.Pow(1 - t, 2) * pointControl1.Y + 3 * Math.Pow(t, 2) * (1 - t) * pointControl2.Y + Math.Pow(t, 3) * pointEnd.Y;
                result.Add(new Point(x, y));
            }
            return result;
        }
    }
}
