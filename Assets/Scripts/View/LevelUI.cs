using System;
using Data;
using TMPro;
using UnityEngine;

namespace View
{
    public class LevelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text score;
        [SerializeField] private TMP_Text message;

        private LevelModel _levelModel;
        public void Connect(LevelModel levelModel)
        {
            _levelModel = levelModel;
            _levelModel.OnScoreChange -= OnScoreChange;
            _levelModel.OnScoreChange += OnScoreChange;
            _levelModel.OnLevelStateChange -= OnLevelStateChange;
            _levelModel.OnLevelStateChange += OnLevelStateChange;
            
            OnScoreChange(_levelModel.Score);
            OnLevelStateChange(_levelModel.State);
        }

        private void OnLevelStateChange(LevelState state)
        {
            switch (state)
            {
                case LevelState.Paused:
                    message.text = "Tap to start";
                    break;
                case LevelState.Playing:
                    message.text = "";
                    break;
                case LevelState.Finish:
                    message.text = "Game over";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnScoreChange(int value)
        {
            score.text = "Score: " + value;
        }
    }
}