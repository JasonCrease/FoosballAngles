using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace FoosballAngles
{
    class Engine2
    {
        public System.Drawing.Bitmap Bitmap { get; set; }

        internal void Go()
        {
            Bitmap = new System.Drawing.Bitmap(750, 350);
            Graphics g = Graphics.FromImage(Bitmap);
            Brush brush = new SolidBrush(Color.BlueViolet);
            g.DrawLine(new Pen(brush), 100, 200, 300, 400);
        }
    }
}
