using System;
using System.Collections.Generic;
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
        private const int _PLATFORM_HEIGHT = 10;
        private const int _GENERATE_LEVEL_TRIGGER_PLAYER_HEIGHT = 40;
        public const int MOVE_LEVEL_DOWN_HEIGHT = -20;
        private const int _DIAMONDS_GROUND_CANDIDATES = 5;

        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                OnScoreChange?.Invoke(_score);
            }
        }

        private Ground[,] _grounds;
        private int _score;
        private readonly Settings _settings;
        private int _thickness;
        private Vector2 _spawnPoint;
        private Vector2 _playerPoint;
        private int _currentX;
        private int _currentY;
        private LineDirection _currentLineDirection;
        private LevelState _levelState = LevelState.Paused;
        private readonly List<Vector2> _groundsDiamondCandidates = new List<Vector2>(_DIAMONDS_GROUND_CANDIDATES);

        public event Action OnGroundChanged;
        public event Action<Vector2> OnNewGroundsGenerated;
        public event Action<LevelState> OnLevelStateChange;

        public event Action<int> OnScoreChange;

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

            for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var ground = _grounds[x, y];
                ground.point = new Vector2(x, y);
                _grounds[x, y] = ground;
            }

            _thickness = GetThicknessByDifficulty(_settings.difficulty);
            int xStart = Width / 2 - _PLATFORM_SIZE / 2 + 1;
            DrawLine(xStart, _PLATFORM_HEIGHT, _PLATFORM_SIZE, _PLATFORM_SIZE, LineDirection.Right);
            _spawnPoint = new Vector2(xStart, _PLATFORM_HEIGHT + 2);
            (_currentX, _currentY, _currentLineDirection) =
                GenerateLines(xStart, _PLATFORM_HEIGHT + 2, _thickness, LineDirection.Right);
            SetState(LevelState.Paused);
            _playerPoint = SpawnPoint;
            _groundsDiamondCandidates.Clear();
            Score = 0;
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
        
        private void AddDiamondCandidate(Vector2 point)
        {
            _groundsDiamondCandidates.Add(point);
            
            if (_groundsDiamondCandidates.Count != _DIAMONDS_GROUND_CANDIDATES)
            {
                return;
            }
            
            switch (_settings.diamondsOrder)
            {
                case DiamondsOrder.Random:
                    var random = _groundsDiamondCandidates[Random.Range(0, _DIAMONDS_GROUND_CANDIDATES)];
                    Ground candidate = _grounds[(int)random.x, (int)random.y];
                    candidate.diamond = new Diamond(1);
                    _grounds[(int)point.x, (int)point.y] = candidate;
                    break;
                case DiamondsOrder.InOrder:
                    var first = _groundsDiamondCandidates[0];
                    Ground firstGround = _grounds[(int)first.x, (int)first.y];
                    firstGround.diamond = new Diamond(1);
                    _grounds[(int)point.x, (int)point.y] = firstGround;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _groundsDiamondCandidates.Clear();
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

                Ground ground = _grounds[currentX, currentY];
                ground.floor = true;
                _grounds[currentX, currentY] = ground;
                
                AddDiamondCandidate(new Vector2(currentX, currentY));

                if (stepX % 2 == 0)
                {
                    deltaX++;
                }

                stepX++;
            }

            return (currentX, currentY);

        }

        public void SetPlayerGround(Ground ground)
        {
            if (_levelState != LevelState.Playing)
            {
                return;
            }

            _playerPoint = ground.point;

            if (ground.floor == false)
            {
                _playerPoint = _spawnPoint;
                SetState(LevelState.Finish);
            }
            
            if (ground.point.y < _GENERATE_LEVEL_TRIGGER_PLAYER_HEIGHT)
            {
                return;
            }

            MoveLevelDown(MOVE_LEVEL_DOWN_HEIGHT);
        }

        private void MoveLevelDown(int height)
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
                var newY = (y + shift) % height;
                result[x, newY] = array[x, y];
                result[x, newY].point = new Vector2(x, newY);
            }

            return result;
        }

        public void TakeDiamond()
        {
            Ground ground = _grounds[(int)_playerPoint.x, (int)_playerPoint.y];
            Diamond diamond = ground.diamond;
            if (diamond != null)
            {
                diamond.Take();
                Score += diamond.score;
            }

            ground.diamond = diamond;
            _grounds[(int)_playerPoint.x, (int)_playerPoint.y] = ground;

        }
        
       
    }
}