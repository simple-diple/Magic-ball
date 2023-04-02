using System;
using Data;
using UnityEngine;
using View;

namespace Controller
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private PlayerView playerPrefab;
        [SerializeField] private Settings settings;
        [SerializeField] private CameraController cameraController;
        
        private LevelModel _levelModel;
        private PlayerView _playerView;

        private void Awake()
        {
            CreateLevel();
            _playerView = Instantiate(playerPrefab);
            levelView.Connect(_levelModel);
            GroundView groundView = levelView.GetGroundView(_levelModel.SpawnPoint);
            _playerView.Connect(_levelModel, levelView);
            _playerView.Wrap(groundView);
            _playerView.SetSpeed(settings.playerSpeed);
            cameraController.SetTarget(_playerView.transform);
        }

        private void CreateLevel()
        {
            _levelModel = new LevelModel(settings);
        }
    }
}