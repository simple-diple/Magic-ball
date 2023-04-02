using System;
using System.Collections;
using Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public class LevelModel
    {
        public int Width => _grounds.GetLength(0);
        public int Height => _grounds.GetLength(1);
        public Vector2 SpawnPoint => _spawnPoint;
        
        private const int _PLATFORM_SIZE = 3;
        private const int _PLATFORM_HEIGHT = 3;

        private Ground[,] _grounds;
        private readonly Settings _settings;
        private readonly int _thickness = 1;
        private readonly Vector2 _spawnPoint;
        private int _currentX;
        private int _currentY;

        public event Action OnGroundChanged;
        public event Action<Vector2> OnNewGroundsGenerated;
        
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
            DrawLine(xStart, _PLATFORM_HEIGHT, _PLATFORM_SIZE, _PLATFORM_SIZE, LineDirection.Right);
            _spawnPoint = new Vector2(xStart, _PLATFORM_HEIGHT + 2);
            (_currentX, _currentY) = GenerateLines(xStart, _PLATFORM_HEIGHT, _thickness, LineDirection.Right);
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

        private (int, int) GenerateLines(int x, int y, int thickness, LineDirection startDirection)
        {
            int currentX = x;
            int currentY = y;

            int minWidth = 5;
            int maxWidth = Width;
            int currentWidth = 0;
            
            LineDirection currentDirection = startDirection;

            for (int yg = currentY + thickness; yg < Height; yg++)
            {
                for (int xg = 0; xg < Width; xg++)
                {
                    _grounds[xg, yg].floor = false;
                }
            }

            while (currentY + currentWidth + 1 < Height)
            {
                currentWidth = Random.Range(minWidth, maxWidth);
                
                if (currentX + thickness + currentWidth > Width * 2)
                {
                    currentDirection = LineDirection.Left;
                    currentWidth = minWidth;
                }
                
                else if (currentX * 2 - thickness - currentWidth < 0)
                {
                    currentDirection = LineDirection.Right;
                    if (_thickness == 3)
                    {
                        currentY--;
                    }
                    currentWidth = minWidth;
                }

                else
                {
                    currentDirection = 
                        currentDirection == LineDirection.Left ? 
                            LineDirection.Right : 
                            LineDirection.Left;
                }
                
                (currentX, currentY) = DrawLine(currentX, currentY, currentWidth, thickness, currentDirection);
            }

            OnGroundChanged?.Invoke();
            return (currentX, currentY);

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

        private Vector2 _playerPoint;
        public void SetPlayerGround(Vector2 groundViewPoint)
        {
            Ground ground = _grounds[(int)groundViewPoint.x, (int)groundViewPoint.y];
            _playerPoint = groundViewPoint;
            Debug.Log(_playerPoint);

            if (ground.floor == false)
            {
                // Debug.LogError("Loose!");
            }
            
            if (groundViewPoint.y < 32)
            {
                return;
            }

            GenerateNext();
        }

        public void MoveLevelDown(int height)
        {
            _grounds = RotateArray(_grounds, height);
            _currentY += height;
            (_currentX, _currentY) = GenerateLines(_currentX, _currentY, _thickness, LineDirection.Right);
            OnGroundChanged?.Invoke();
        }

        private static Ground[,] RotateArray(Ground[,] array, int shift)
        {
            int height = array.GetLength(1);
            int width = array.GetLength(0);
            
            shift %= height;
            shift = shift < 0 ? height - Math.Abs(shift) : shift;

            Ground[,] result = new Ground[width, height];

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, (y + shift) % height] = array[x, y];

            return result;
        }

        private void GenerateNext()
        {
            int backwardHeight = 10;
            int deadLine = (int)_playerPoint.y - backwardHeight;
            
            
            int deltaY = 0;

            for (int y = deadLine; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    _grounds[x, deltaY] = _grounds[x, y];
                }

                deltaY++;
            }

            _currentY -= deltaY;
            _playerPoint = new Vector2(_playerPoint.x, 0);
            //OnGroundChanged?.Invoke();
            //GenerateLines(_currentX, _currentY, _thickness, LineDirection.Right);
            OnNewGroundsGenerated?.Invoke(_playerPoint);
        }
    }
}