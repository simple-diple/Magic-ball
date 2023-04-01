using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public class LevelModel
    {
        public int Width => _grounds.GetLength(0);
        public int Height => _grounds.GetLength(1);
        
        private const int _PLATFORM_SIZE = 3;

        private readonly Ground[,] _grounds;
        private readonly Settings _settings;
        private readonly int _thickness = 1;
        
        public Ground GetGround(int x, int j)
        {
            return _grounds[x, j];
        }

        public LevelModel(Settings settings)
        {
            _settings = settings;
            _grounds = new Ground[(int)_settings.levelSize.x, (int)_settings.levelSize.y];
            _thickness = GetThicknessByDifficulty(settings.difficulty);
            int xStart = Width / 2 - _PLATFORM_SIZE / 2 + 1;
            DrawLine(xStart, 0, _PLATFORM_SIZE, _PLATFORM_SIZE, LineDirection.Right);
            GenerateLines(xStart, 0, _thickness, LineDirection.Right);

            if (false)
            {
                int testThickness = 3;
                var(x, y) = DrawLine(xStart, 0, 6, testThickness, LineDirection.Right);
                (x, y) = DrawLine(x, y, 6, testThickness, LineDirection.Left);
                (x, y) = DrawLine(x, y, 6, testThickness, LineDirection.Right);
                (x, y) = DrawLine(x, y, 6, testThickness, LineDirection.Left);
                (x, y) = DrawLine(x, y, 6, testThickness, LineDirection.Right);
            }
           
        }

        private int GetThicknessByDifficulty(Difficulty settingsDifficulty)
        {
            switch (settingsDifficulty)
            {
                case Difficulty.Easy:
                    return 3;
                case Difficulty.Medium:
                    return 2;
                case Difficulty.Hard:
                    return 1;
            }

            return 1;
        }

        private void GenerateLines(int x, int y, int thickness, LineDirection startDirection)
        {
            int currentX = x;
            int currentY = y;

            int minWidth = thickness * thickness + 1;
            int maxWidth = Width * 2;

            int currentWidth = Random.Range(minWidth, maxWidth);
            LineDirection currentDirection = startDirection;

            while (currentY + currentWidth + 1 < Height)
            {
                _grounds[currentX, currentY].first = true; 
                (currentX, currentY) = DrawLine(currentX, currentY, currentWidth, thickness, currentDirection);
                
                currentDirection = 
                    currentDirection == LineDirection.Left ? 
                        LineDirection.Right : 
                        LineDirection.Left;

                currentWidth = Random.Range(minWidth, maxWidth);
            }
            
            
        }

        private (int, int) DrawLine(int x, int y, int length, int thickness, LineDirection lineDirection)
        {
            int deltaX = 0;
            int stepX = y % 2;
            
            int currentX = x;
            int currentY = y;

            int realThickness = lineDirection == LineDirection.Right ? thickness : length;
            int realLenght = lineDirection == LineDirection.Right ? length : thickness;
            
            for (int i = y; i < realThickness + y; i++)
            {
                int cellX = x + deltaX;
                (currentX, currentY) = DrawLine(cellX, i, realLenght);
                if (stepX % 2 == 0)
                {
                    deltaX--;
                }

                stepX++;
            }

            int adjustmentY = thickness == 1 ? 0 : thickness;
            
            return (currentX, currentY - adjustmentY);
        }

        private (int, int)  DrawLine(int x, int y, int length)
        {
            int deltaX = 0;
            int stepX = y % 2 == 0 ? 1 : 0;

            int currentX = x;
            int currentY = y;

            for (int i = y; i < length + y; i++)
            {
                currentX = x + deltaX;
                currentY = i;

                currentX = Mathf.Clamp(currentX, 0, Width - 1);
                currentY = Mathf.Clamp(currentY, 0, Height - 1);

                _grounds[currentX, currentY].floor = true;

                if (stepX % 2 == 0)
                {
                    deltaX++;
                }

                stepX++;
            }

            return (currentX, currentY);

        }

        enum LineDirection
        {
            Left,
            Right
        }
    }
}