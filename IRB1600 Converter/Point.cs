using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRB1600_Converter
{
    public struct Point
    {
        public double X { get; set; }

        public double Y { get; set; }

        public string XFormatted
        {
            get
            {
                return X.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public string YFormatted
        {
            get
            {
                return Y.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p, double d)
        {
            return new Point(p.X * d, p.Y * d);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            Point delta = p1 - p2;
            return Math.Abs(delta.X) <= 0.001 && Math.Abs(delta.Y) <= 0.001;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
