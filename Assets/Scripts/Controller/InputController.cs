using System;
using Data;
using UnityEngine;

namespace View
{
    public class InputController : MonoBehaviour
    {
        private PlayerView _playerView;
        private LevelModel _levelModel;
        private float _pause = 0;
        
        private const float _PAUSE_TIME = 2f;

        public void Connect(PlayerView playerView, LevelModel levelModel)
        {
            _playerView = playerView;
            _levelModel = levelModel;
            
            _levelModel.OnLevelStateChange += OnLevelStateChange;
        }
        
        private void OnLevelStateChange(LevelState state)
        {
            if (state == LevelState.Finish)
            {
                _pause = _PAUSE_TIME;
            }
        }

        private void Update()
        {
            if (_pause > 0)
            {
                _pause -= Time.deltaTime;
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                OnTap();
            }
        }

        private void OnTap()
        {
            switch (_levelModel.State)
            {
                case LevelState.Paused:
                    _levelModel.StartLevel();
                    break;
                case LevelState.Playing:
                    _playerView.TogglePlayerRotation();
                    break;
                case LevelState.Finish:
                    _levelModel.GenerateLevel();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}