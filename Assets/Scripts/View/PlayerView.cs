using Data;
using UnityEngine;

namespace View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float offsetY = 0.65f;
        [SerializeField] private Rigidbody body;

        private LevelModel _levelModel;
        private LevelView _levelView;
        private float _speed;
        private const float _ANGLE_FORWARD = 135;
        private LineDirection _currentDirection = LineDirection.Right;
       
        

        private void Wrap(GroundView groundView)
        {
            body.isKinematic = true;
            transform.position =
                groundView.transform.position + Vector3.up * offsetY;
            RotatePlayer(LineDirection.Right);
        }

        private void Update()
        {
            if (_levelModel.State != LevelState.Playing)
            {
                return;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                LineDirection direction = LevelModel.GetOtherDirection(_currentDirection);
                RotatePlayer(direction);
            }

            Transform playerTransform = transform;
            playerTransform.position += playerTransform.forward * (_speed * Time.deltaTime);
        }

        private void RotatePlayer(LineDirection direction)
        {
            _currentDirection = direction;
            var angle = _currentDirection == LineDirection.Left ? 90 : 0;
            var playerTransform = transform;
            var rotation = playerTransform.rotation;
            rotation.eulerAngles = new Vector3(0, _ANGLE_FORWARD + angle, 0);
            playerTransform.rotation = rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            GroundView groundView = other.gameObject.GetComponent<GroundView>();
            if (groundView)
            {
                _levelModel.SetPlayerGround(groundView.Ground);
            }
            
            DiamondView diamondView = other.gameObject.GetComponent<DiamondView>();
            if (diamondView)
            {
                diamondView.Take();
            }
            
        }

        public void Connect(LevelModel levelModel, LevelView levelView, float speed)
        {
            _levelModel = levelModel;
            _levelView = levelView;
            _levelModel.OnNewGroundsGenerated -= OnNewGroundsGenerated;
            _levelModel.OnNewGroundsGenerated += OnNewGroundsGenerated;
            _levelModel.OnLevelStateChange -= OnLevelStateChange;
            _levelModel.OnLevelStateChange += OnLevelStateChange;

            OnLevelStateChange(_levelModel.State);
            _speed = speed;
        }

        private void OnLevelStateChange(LevelState state)
        {
            if (state == LevelState.Paused)
            {
                GroundView groundView = _levelView.GetGroundView(_levelModel.SpawnPoint);
                Wrap(groundView);
            }

            if (state == LevelState.Finish)
            {
                PlayDieEffect();
            }
        }

        private void PlayDieEffect()
        {
            body.isKinematic = false;
            body.AddForce(transform.forward * _speed, ForceMode.Impulse);
        }

        private void OnNewGroundsGenerated(Vector2 playerPoint)
        {
            var playerTransform = transform;
            var position = playerTransform.position;
            float z = position.z + LevelModel.MOVE_LEVEL_DOWN_HEIGHT * _levelView.GroundHalfDiagonal;
            position = new Vector3(position.x, position.y, z);
            playerTransform.position = position;
        }
        
        
    }
}