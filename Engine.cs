using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace FoosballAngles
{
    class Engine
    {
        // Measurements are from top left of table, from perspective of shooting 2-bar

        // Position of players. Speficially, their top-lefts
        private int pNum;
        private float[] pXs;
        private float[] pYs;

        // All numbers in real measured mm
        private const float TableLength = 750;
        private const float TableWidth = 375;
        private const float GoalWidth = 100;
        private const float PlayerWidth = 23;
        private const float PlayerDepth = 8;
        private const float BallWidth = 16f;
        private int m_Score = 0;

        public int Score
        {
            get { return m_Score; }
        }
        Brush greenBrush = new SolidBrush(Color.DarkGreen);
        Brush yellowBrush = new SolidBrush(Color.Yellow);
        Brush BisqueBrush = new SolidBrush(Color.Red);
        Brush goalBrush = new SolidBrush(Color.BlueViolet);

        public Bitmap Bmp { get; private set; }

        public Engine()
        {
            pXs = new float[] { 200, 200, 200,  // attack
                350, 350, 350, 350, 350,        // midfield
                500, 500,                       // defence
                650                             //keeper
            };

            pYs = new float[] { 100, 200, 300,  // attack
                100, 150, 200, 250, 300,        // midfield
                100, 300,                       // defence
                200                             //keeper
            };

            if (pXs.Length != pYs.Length) throw new ApplicationException();
            pNum = pXs.Length;
        }

        public void ScorePath()
        {
            float startX = 50f;
            float endX = TableLength;

            Bmp = new Bitmap((int)TableLength, (int)TableWidth);
            m_Graphics = Graphics.FromImage(Bmp);

            RenderPitch();

            for (float startY = 50; startY < TableWidth - 50; startY += 1f)
            {
                for (float endY = (TableWidth - GoalWidth) / 2f; endY < (TableWidth + GoalWidth) / 2f; endY += 1f)
                {
                    bool pathClear = IsPathClear(startX, startY, endX, endY);
                    if (pathClear)
                    {
                        m_Graphics.DrawLine(new Pen(yellowBrush), startX, startY, endX - 8, endY);
                        m_Score++;
                    }
                }
            }

            RenderPlayers();
        }

        Graphics m_Graphics;

        private void RenderPitch()
        {
            m_Graphics.FillRectangle(greenBrush, 0, 0, (int)TableLength, (int)TableWidth);
            m_Graphics.FillRectangle(goalBrush, TableLength - 8, (int)(TableWidth - GoalWidth) / 2, TableLength, (int)GoalWidth);
        }


        private void RenderPlayers()
        {
            for (int i = 0; i < pNum; i++)
                m_Graphics.FillRectangle(BisqueBrush, (int) pXs[i], (int) pYs[i], (int) PlayerDepth, (int) PlayerWidth);
        }


        private bool IsPathClear(float startX, float startY, float endX, float endY)
        {
            // Fire three rays (1) Left side of ball (2) Middle of ball (3) Right side of ball
            bool b1 = DoesRayHitAnyPlayers(startX, startY - (BallWidth / 2), endX, endY - (BallWidth / 2));
            if (b1) return false;
            bool b2 = DoesRayHitAnyPlayers(startX, startY, endX, endY);
            if (b2) return false;
            bool b3 = DoesRayHitAnyPlayers(startX, startY + (BallWidth / 2), endX, endY + (BallWidth / 2));
            if (b3) return false;

            return true;
        }

        private bool DoesRayHitAnyPlayers(float startX, float startY, float endX, float endY)
        {
            const float xStepSize = 0.5f;
            float gradient = (endY - startY) / (endX - startX);
            float y = startY;

            for (float x = startX; x < endX; x += xStepSize)
            {
                y += gradient * xStepSize;

                //m_Graphics.FillRectangle(yellowBrush, x, y, 1, 1);
                for (int p = 0; p < pNum; p++)
                {
                    if (IsInPlayerRectangle(p, x, y))
                        return true;
                }
            }

            return false;
        }

        private bool IsInPlayerRectangle(int p, float x, float y)
        {
            if (x < pXs[p]) return false;
            if (y < pYs[p]) return false;

            if (x > pXs[p] + PlayerDepth) return false;
            if (y > pYs[p] + PlayerWidth) return false;

            return true;
        }
    }
}
