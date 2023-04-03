using System;
using UnityEngine;

namespace Data
{
    public class LevelModel
    {
        public int Width => _grounds.GetLength(0);
        public int Height => _grounds.GetLength(1);
        public Vector2 SpawnPoint => _spawnPoint;
        public Vector2 PlayerPoint => _playerPoint;
        public LevelState State => _levelState;
        public event Action OnGroundChanged;
        public event Action<int> OnNewGroundsGenerated;
        public event Action<LevelState> OnLevelStateChange;
        public event Action<int> OnScoreChange;
        
        private const int _GENERATE_LEVEL_TRIGGER_PLAYER_HEIGHT = 40;
        private const byte _DIAMOND_SCORE = 1;

        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                OnScoreChange?.Invoke(_score);
            }
        }

        private readonly ILevelGenerator _levelGenerator;
        private readonly Settings _settings;
        
        private Ground[,] _grounds;
        private int _score;
        private int _thickness;
        private Vector2 _spawnPoint;
        private Vector2 _playerPoint;
        private int _currentX;
        private int _currentY;
        private LineDirection _currentLineDirection;
        private LevelState _levelState = LevelState.Paused;

        public LevelModel(Settings settings, ILevelGenerator levelGenerator)
        {
            _settings = settings;
            _levelGenerator = levelGenerator;
        }
        
        public Ground GetGround(Vector2 point)
        {
            return _grounds[(int)point.x, (int)point.y];
        }

        public void GenerateLevel()
        {
            _grounds = new Ground[(int)_settings.levelSize.x, (int)_settings.levelSize.y];
            _thickness = GetThicknessByDifficulty(_settings.difficulty);
            _grounds =
                _levelGenerator.GenerateGround(
                    width: (int)_settings.levelSize.x, 
                    height: (int)_settings.levelSize.y, 
                    _thickness, 
                    _settings.diamondsOrder);
            
            _spawnPoint = _levelGenerator.GetSpawnPoint();
           
            SetState(LevelState.Paused);
            _playerPoint = SpawnPoint;
            Score = 0;
            OnGroundChanged?.Invoke();
        }

        public void StartLevel()
        {
            SetState(LevelState.Playing);
        }

        public static LineDirection GetOtherDirection(LineDirection direction)
        {
            return  direction == LineDirection.Left ? 
                LineDirection.Right : 
                LineDirection.Left;
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

            _grounds = _levelGenerator.MoveLevelDown();
            OnGroundChanged?.Invoke();
            OnNewGroundsGenerated?.Invoke(_levelGenerator.MoveDownHeight);
        }

        public bool TryTakeDiamond(Vector2 groundPoint)
        {
            if (_grounds[(int)groundPoint.x, (int)groundPoint.y].diamond == false)
            {
                return false;
            }
            
            _grounds[(int)groundPoint.x, (int)groundPoint.y].diamond = false;
            Score += _DIAMOND_SCORE;
            return true;
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

        
    }
}