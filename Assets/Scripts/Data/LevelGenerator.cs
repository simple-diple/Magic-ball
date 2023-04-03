using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public class LevelGenerator : ILevelGenerator
    {
        public int MoveDownHeight => _MOVE_LEVEL_DOWN_HEIGHT;

        private int Width => _grounds.GetLength(0);
        private int Height => _grounds.GetLength(1);
        
        private readonly HashSet<Vector2> _groundsDiamondCandidates = new(_DIAMONDS_GROUND_CANDIDATES);
       
        private Ground[,] _grounds;
        private int _thickness;
        private Vector2 _spawnPoint;
        private Vector2 _playerPoint;
        private int _currentX;
        private int _currentY;
        private LineDirection _currentLineDirection;
        private DiamondsOrder _diamondsOrder;
        
        private const int _PLATFORM_SIZE = 3;
        private const int _PLATFORM_HEIGHT = 10;
        private const int _MOVE_LEVEL_DOWN_HEIGHT = 30;
        private const int _DIAMONDS_GROUND_CANDIDATES = 5;
        private const int _GENERATE_SAVE_BORDER_R = -3;
        private const int _GENERATE_SAVE_BORDER_L = -1;
        
        public Ground[,] GenerateGround(int width, int height, int thickness, DiamondsOrder diamondsOrder)
        {
            _grounds = new Ground[width, height];
            _diamondsOrder = diamondsOrder;

            for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                _grounds[x, y].point = new Vector2(x, y);;
            }

            _thickness = thickness;
            int xStart = Width / 2 - _PLATFORM_SIZE / 2 + 1;
            DrawLine(xStart, _PLATFORM_HEIGHT, _PLATFORM_SIZE, _PLATFORM_SIZE, LineDirection.Right);
            (_currentX, _currentY, _currentLineDirection) =
                GenerateLines(xStart, _PLATFORM_HEIGHT + 2, _thickness, LineDirection.Right);

            return _grounds;

        }
        
        public (int, int) DrawLine(int x, int y, int length, int thickness, LineDirection lineDirection)
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

            switch (thickness)
            {
                case 2:
                    currentY -= 2;
                    break;
                case 3:
                    currentY -= 3;
                    break;
            }

            return (currentX, currentY);
        }

        public (int, int, LineDirection) GenerateLines(int x, int y, int thickness, LineDirection startDirection)
        {
            int currentX = x;
            int currentY = y;

            int minWidth = thickness + 1;
            int currentWidth = 0;
            
            LineDirection currentDirection = startDirection;

            while (currentY + currentWidth * 2 < Height)
            {
                var maxWidth = currentDirection == LineDirection.Right
                    ? Width * 2 - currentX * 2 + _GENERATE_SAVE_BORDER_R
                    : currentX * 2 + _GENERATE_SAVE_BORDER_L;
                maxWidth -= thickness / 2;
                currentWidth = Random.Range(minWidth, maxWidth);
                (currentX, currentY) = DrawLine(currentX, currentY, currentWidth, thickness, currentDirection);
                currentDirection = LevelModel.GetOtherDirection(currentDirection);
            }
            
            return (currentX, currentY, currentDirection);
        }

        public Vector2 GetSpawnPoint()
        {
            int xStart = Width / 2 - _PLATFORM_SIZE / 2 + 1;
            return new Vector2(xStart, _PLATFORM_HEIGHT + 2);
        }

        public Ground[,] MoveLevelDown()
        {
            MoveLevelDown(_MOVE_LEVEL_DOWN_HEIGHT);
            return _grounds;
        }

        private void MoveLevelDown(int height)
        {
            _grounds = MoveArray(_grounds, height);
            _currentY -= height;

            _currentLineDirection = LevelModel.GetOtherDirection(_currentLineDirection);
            
            (_currentX, _currentY, _currentLineDirection) = 
                GenerateLines(_currentX, _currentY, _thickness, LineDirection.Right);
            
            _playerPoint = new Vector2(_playerPoint.x, _playerPoint.y - height);
        }

        private static Ground[,] MoveArray(Ground[,] array, int shift)
        {
            int height = array.GetLength(1);
            int width = array.GetLength(0);

            Ground[,] result = new Ground[width, height];

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                var newY = y + shift;

                if (newY >= height)
                {
                    break;
                }
                
                result[x, y] = array[x, newY];
                result[x, y].point = new Vector2(x, y);
            }

            return result;
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
                AddDiamondCandidate(new Vector2(currentX, currentY));

                if (stepX % 2 == 0)
                {
                    deltaX++;
                }

                stepX++;
            }

            return (currentX, currentY);

        }
        
        private void AddDiamondCandidate(Vector2 point)
        {
            _groundsDiamondCandidates.Add(point);
            
            if (_groundsDiamondCandidates.Count != _DIAMONDS_GROUND_CANDIDATES)
            {
                return;
            }
            
            switch (_diamondsOrder)
            {
                case DiamondsOrder.Random:
                    var random = _groundsDiamondCandidates.ElementAt(Random.Range(0, _DIAMONDS_GROUND_CANDIDATES));
                    _grounds[(int)random.x, (int)random.y].diamond = true;
                    break;
                case DiamondsOrder.InOrder:
                    var first = _groundsDiamondCandidates.ElementAt(0);
                    _grounds[(int)first.x, (int)first.y].diamond = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _groundsDiamondCandidates.Clear();
        }
    }
}