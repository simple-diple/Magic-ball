using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Data
{
    public class LevelModel
    {
        public int Width => _grounds.GetLength(0);
        public int Height => _grounds.GetLength(1);
        public Vector2 SpawnPoint => _spawnPoint;
        public Vector2 PlayerPoint => _playerPoint;
        public LevelState State => _levelState;
        
        private const int _PLATFORM_SIZE = 3;
        private const int _PLATFORM_HEIGHT = 3;
        private const int _GENERATE_LEVEL_TRIGGER_PLAYER_HEIGHT = 40;
        public const int MOVE_LEVEL_DOWN_HEIGHT = -10;

        private Ground[,] _grounds;
        private readonly Settings _settings;
        private int _thickness;
        private Vector2 _spawnPoint;
        private Vector2 _playerPoint;
        private int _currentX;
        private int _currentY;
        private LineDirection _currentLineDirection;
        private LevelState _levelState = LevelState.Paused;

        public event Action OnGroundChanged;
        public event Action<Vector2> OnNewGroundsGenerated;
        public event Action<LevelState> OnLevelStateChange;
        
        public Ground GetGround(int x, int j)
        {
            return _grounds[x, j];
        }

        public LevelModel(Settings settings)
        {
            _settings = settings;
        }
        
        public void GenerateLevel()
        {
            _grounds = new Ground[(int)_settings.levelSize.x, (int)_settings.levelSize.y];
            _thickness = GetThicknessByDifficulty(_settings.difficulty);
            int xStart = Width / 2 - _PLATFORM_SIZE / 2 + 1;
            DrawLine(xStart, _PLATFORM_HEIGHT, _PLATFORM_SIZE, _PLATFORM_SIZE, LineDirection.Right);
            _spawnPoint = new Vector2(xStart, _PLATFORM_HEIGHT + 2);
            (_currentX, _currentY, _currentLineDirection) = GenerateLines(xStart, _PLATFORM_HEIGHT + 2, _thickness, LineDirection.Right);
            SetState(LevelState.Paused);
            _playerPoint = SpawnPoint;
        }

        public void StartLevel()
        {
            SetState(LevelState.Playing);
        }

        private void SetState(LevelState levelState)
        {
            _levelState = levelState;
            OnLevelStateChange?.Invoke(_levelState);
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

        public static LineDirection GetOtherDirection(LineDirection direction)
        {
            return  direction == LineDirection.Left ? 
                LineDirection.Right : 
                LineDirection.Left;
        }

        private (int, int, LineDirection) GenerateLines(int x, int y, int thickness, LineDirection startDirection)
        {
            int currentX = x;
            int currentY = y;

            int minWidth = 5;
            int maxWidth = Width;
            int currentWidth = 0;
            
            LineDirection currentDirection = startDirection;

            while (currentY + currentWidth + thickness + 5< Height)
            {
                currentWidth = Random.Range(minWidth, maxWidth);
                (currentX, currentY) = DrawLine(currentX, currentY, currentWidth, thickness, currentDirection);
                currentDirection = GetOtherDirection(currentDirection);
            }

            OnGroundChanged?.Invoke();
            return (currentX, currentY, currentDirection);

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

            return (currentX, currentY);
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

        public void SetPlayerGround(Vector2 groundViewPoint)
        {
            if (_levelState != LevelState.Playing)
            {
                return;
            }
            
            Ground ground = _grounds[(int)groundViewPoint.x, (int)groundViewPoint.y];
            _playerPoint = groundViewPoint;

            if (ground.floor == false)
            {
                _playerPoint = _spawnPoint;
                SetState(LevelState.Finish);
            }
            
            if (groundViewPoint.y < _GENERATE_LEVEL_TRIGGER_PLAYER_HEIGHT)
            {
                return;
            }

            MoveLevelDown(MOVE_LEVEL_DOWN_HEIGHT);
        }

        public void MoveLevelDown(int height)
        {
            _grounds = RotateArray(_grounds, height);
            _currentY += height;
            
            for (int yg = _currentY; yg < Height; yg++)
            {
                for (int xg = 0; xg < Width; xg++)
                {
                    _grounds[xg, yg] = default;
                }
            }
            
            _currentLineDirection = GetOtherDirection(_currentLineDirection);
            
            (_currentX, _currentY, _currentLineDirection) = 
                GenerateLines(_currentX, _currentY, _thickness, LineDirection.Right);
            
            _playerPoint = new Vector2(_playerPoint.x, _playerPoint.y + height);
            OnNewGroundsGenerated?.Invoke(_playerPoint);
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
            {
                result[x, (y + shift) % height] = array[x, y];
            }

            return result;
        }
    }
}