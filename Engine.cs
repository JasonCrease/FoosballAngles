using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;

namespace FoosballAngles
{
    class Engine
    {
        // Measurements are from top left of table, from perspective of shooting defence

        // Position of players. Speficially, their centres
        private int pNum;
        private double[] pXs;
        private double[] pYs;

        // All numbers in real measured mm
        private const double TableLength = 700;
        private const double TableWidth = 300;
        private const double GoalWidth = 200;
        private const double PlayerWidth = 20;
        private const double PlayerDepth = 5;
        private const double BallWidth = 33f;

        public Engine()
        {
            pXs = new double[] { 100, 140, 250, 460, 530 };
            pYs = new double[] { 220, 20, 240, 240, 150 };
            if (pXs.Length != pYs.Length) throw new ApplicationException();
            pNum = pXs.Length;
        }

        public void Go()
        {
            double startX = 10f;
            double startY = 10f;
            double endX = TableLength;
            double endY = TableWidth / 2;

            IsPathClear(startX, startY, endX, endY);
        }

        private bool IsPathClear(double startX, double startY, double endX, double endY)
        {
            // Fire three rays (1) Left side of ball (2) Middle of ball (3) Right side of ball
            bool b1 = DoesRayHitAnyPlayers(startX, startY - (BallWidth / 2), endX, endY - (BallWidth / 2));
            bool b2 = DoesRayHitAnyPlayers(startX, startY, endX, endY);
            bool b3 = DoesRayHitAnyPlayers(startX, startY + (BallWidth / 2), endX, endY + (BallWidth / 2));

            if (!b1 && !b2 && !b3)
                return true;
            else return false;
        }

        private bool DoesRayHitAnyPlayers(double startX, double startY, double endX, double endY)
        {
            double xStepSize = 0.5;
            double gradient = (endY - endX) / (startY - startX);
            double y = startY;

            for (double x = startX; x < endX; x += xStepSize)
            {
                y += gradient * xStepSize;

                for (int p = 0; p < pNum; p++)
                {
                    if (IsInPlayerRectangle(p, x, y))
                        return true;
                }
            }

            return false;
        }

        private bool IsInPlayerRectangle(int p, double x, double y)
        {
            if (x < pXs[p] - (PlayerWidth / 2)) return false;
            if (x > pXs[p] + (PlayerWidth / 2)) return false;
            if (y < pYs[p] - (PlayerDepth / 2)) return false;
            if (y > pYs[p] + (PlayerDepth / 2)) return false;

            return true;
        }
    }
}
