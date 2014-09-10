using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace FoosballAngles
{
    class PolePosition
    {
        public float PosGoalie { get; set; }
        public float PosDefence { get; set; }
        public float PosMidfield { get; set; }
        public float PosAttack { get; set; }
        public int Score { get; set; }
    }

    class Engine
    {
        // Measurements are from top left of table, from perspective of shooting 2-bar

        // Position of players. Speficially, their top-lefts
        private int pNum;
        private float[] pXs;
        private float[] pYs;

        // All numbers in real measured mm
        private const float TableLength = 1200;
        private const float TableWidth = 700;
        private const float GoalWidth = 202;
        private const float PlayerWidth = 20;
        private const float PlayerDepth = 10;
        private const float BallWidth = 33f;
        private float BallGoInWidth = (BallWidth) / 2;
        private int m_Score = 0;

        Graphics m_Graphics;

        public int Score
        {
            get { return m_Score; }
        }
        Brush greenBrush = new SolidBrush(Color.DarkGreen);
        Brush yellowBrush = new SolidBrush(Color.Goldenrod);
        Brush BisqueBrush = new SolidBrush(Color.Tomato);
        Brush goalBrush = new SolidBrush(Color.Magenta);

        public Bitmap Bmp { get; private set; }

        public Engine()
        {
            ResetPlayers();
            if (pXs.Length != pYs.Length) throw new ApplicationException();
            pNum = pXs.Length;
        }

        private void ResetPlayers()
        {
            pXs = new float[] { 370, 370, 370,  // attack
                670, 670, 670, 670, 670,        // midfield
                970, 970,                       // defence
                1120                             //keeper
            };

            pYs = new float[] { 0, 207, 207 * 2, // attack
                0, 119, 119 *2, 119 *3, 119 *4, // midfield
                0, 239,                         // defence
                220                             //keeper
            };
        }

        public void FindOptimalPolePositions()
        {
            Bmp = new Bitmap((int)TableLength, (int)TableWidth);
            m_Graphics = Graphics.FromImage(Bmp);

            RenderPitch();

            List<PolePosition> polePositions = new List<PolePosition>();
            Random r = new Random();

            for (int i = 0; i < 100; i++)
            {
                PolePosition pp = new PolePosition();
                pp.PosAttack += (float)(r.NextDouble() * 210) + 36;
                pp.PosMidfield += (float)(r.NextDouble() * 120) + 36;
                pp.PosDefence += (float)(r.NextDouble() * 240) + 36;
                pp.PosGoalie += (float)(r.NextDouble() * 250);

                polePositions.Add(pp);
                pp.Score = ScorePath(pp, false);
            }

            PolePosition bestPos = polePositions.OrderBy(x => x.Score).First();
            m_Score = ScorePath(bestPos, true);

            RenderPlayers();
        }

        public int ScorePath(PolePosition pp, bool draw)
        {
            float startX = 220f;
            float endX = TableLength;
            float yStepSize = 2f;

            int score = 0;
            int[] shotScores = new int[400];

            if (draw)
                yStepSize = 1f;

            ResetPlayers();

            for (int i = 0; i < 3; i++)
                pYs[i] += pp.PosAttack;
            for (int i = 3; i < 8; i++)
                pYs[i] += pp.PosMidfield;
            for (int i = 8; i < 10; i++)
                pYs[i] += pp.PosDefence;
            pYs[10] += pp.PosGoalie;

            for (float startY = 30; startY < TableWidth / 2f; startY += yStepSize)
            {
                for (float endY = ((TableWidth - GoalWidth) + BallGoInWidth) / 2f; endY < ((TableWidth + GoalWidth) - BallGoInWidth) / 2f; endY += yStepSize)
                {
                    bool pathClear = IsPathClear(startX, startY, endX, endY);
                    if (pathClear)
                    {
                        if (draw)
                            m_Graphics.DrawLine(new Pen(yellowBrush), startX, startY, endX - 8, endY);
                        score++;
                        shotScores[(int)startY]++;
                    }
                }
            }

            //return shotScores.Max();
            return score;
        }

        private void RenderPitch()
        {
            m_Graphics.FillRectangle(greenBrush, 0, 0, (int)TableLength, (int)TableWidth);
            m_Graphics.FillRectangle(goalBrush, TableLength - 8, (int)(TableWidth - GoalWidth) / 2, TableLength, (int)GoalWidth);
        }


        private void RenderPlayers()
        {
            for (int i = 0; i < pNum; i++)
                m_Graphics.FillRectangle(BisqueBrush, (int)pXs[i], (int)pYs[i], (int)PlayerDepth, (int)PlayerWidth);
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
            const float xStepSize = 3f;
            float gradient = (endY - startY) / (endX - startX);
            float y = startY;

            for (float x = startX; x < endX; x += xStepSize)
            {
                y += gradient * xStepSize;

                //m_Graphics.FillRectangle(yellowBrush, x, y, 1, 1);
                for (int p = 3; p < pNum; p++)
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
