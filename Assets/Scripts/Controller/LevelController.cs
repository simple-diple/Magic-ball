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
        [SerializeField] private LevelUI levelUI;
        
        private LevelModel _levelModel;
        private PlayerView _playerView;

        private void Awake()
        {
            _levelModel = new LevelModel(settings);
            _levelModel.GenerateLevel();
            _playerView = Instantiate(playerPrefab);
            levelView.Connect(_levelModel);
            _playerView.Connect(_levelModel, levelView, settings.playerSpeed);
            cameraController.Connect(_playerView.transform, _levelModel);
            levelUI.Connect(_levelModel);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseClick();
            }
        }

        private void OnMouseClick()
        {
            if (_levelModel.State == LevelState.Paused)
            {
                _levelModel.StartLevel();
            }

            if (_levelModel.State == LevelState.Finish)
            {
                _levelModel.GenerateLevel();
            }
        }
        
    }
}