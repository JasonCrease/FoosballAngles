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
        public int imageWidth = 750;
        public int imageHeight = 350;
        public int goalWidth = 200;

        internal void Go()
        {
            Bitmap = new System.Drawing.Bitmap(imageWidth, imageHeight);
            Graphics g = Graphics.FromImage(Bitmap);
            Brush greenBrush = new SolidBrush(Color.DarkGreen);
            Brush yellowBrush = new SolidBrush(Color.DarkGreen);
            Brush goalBrush = new SolidBrush(Color.BlueViolet);
            g.FillRectangle(greenBrush, 0, 0, imageWidth, imageHeight);
            g.FillRectangle(goalBrush, 0, (imageHeight - goalWidth) /2, 8, goalWidth);


            //g.DrawLine(new Pen(brush), 100, 200, 300, 400);
        }
    }
}
