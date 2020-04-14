using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Juke.UI.Wpf
{
    public class SineDataGenerator
    {
        public int ScaleX { get; set; }
        public int ScaleY { get; set; }

        public SineDataGenerator()
        {
            ScaleX = 10;
            ScaleY = 10;
        }

        public Point[] Generate(double min, double max, double unit)
        {

            var points = new List<Point>();
            for (double i = min; i <= max; i += unit)
            {
                points.Add(new Point(ScaleX * i, ScaleY * Math.Sin(i) + 100));
            }

            return points.ToArray();
        }
    }
}
