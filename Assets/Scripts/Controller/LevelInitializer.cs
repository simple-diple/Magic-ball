using Data;
using UnityEngine;
using View;

namespace Controller
{
    public class LevelInitializer : MonoBehaviour
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private PlayerView playerPrefab;
        [SerializeField] private Settings settings;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private LevelUI levelUI;
        [SerializeField] private InputController inputController;
        
        private LevelModel _levelModel;
        private PlayerView _playerView;

        private void Awake()
        {
            _levelModel = new LevelModel(settings, new LevelGenerator());
            _levelModel.GenerateLevel();
            _playerView = Instantiate(playerPrefab);
            levelView.Connect(_levelModel);
            _playerView.Connect(_levelModel, levelView, settings.playerSpeed);
            cameraController.Connect(_playerView.transform, _levelModel);
            levelUI.Connect(_levelModel);
            inputController.Connect(_playerView, _levelModel);
        }
    }
}