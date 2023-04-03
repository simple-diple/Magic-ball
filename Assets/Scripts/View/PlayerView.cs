using Data;
using UnityEngine;

namespace View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float offsetY = 0.65f;
        [SerializeField] private Rigidbody body;
        [SerializeField] private bool debug;

        private LevelModel _levelModel;
        private LevelView _levelView;
        private float _speed;
        private const float _ANGLE_FORWARD = 135;
        private LineDirection _currentDirection = LineDirection.Right;

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

        private void Wrap(GroundView groundView)
        {
            body.isKinematic = true;
            transform.position =
                groundView.Position + Vector3.up * offsetY;
            RotatePlayer(LineDirection.Right);
        }

        private void Update()
        {
            if (_levelModel.State != LevelState.Playing)
            {
                return;
            }

            Transform playerTransform = transform;
            playerTransform.position += playerTransform.forward * (_speed * Time.deltaTime);
        }

        public void TogglePlayerRotation()
        {
            LineDirection direction = LevelModel.GetOtherDirection(_currentDirection);
            RotatePlayer(direction);
        }

        private void RotatePlayer(LineDirection direction)
        {
            _currentDirection = direction;
            var angle = _currentDirection == LineDirection.Right ? 90 : 0;
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
                if (debug)
                {
                    groundView.Debug();
                }
            }
            
            var collectable = other.gameObject.GetComponent<ICollectable>();
            collectable?.Collect();

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

        private void OnNewGroundsGenerated(int height)
        {
            var playerTransform = transform;
            var position = playerTransform.position;
            float z = position.z + height * _levelView.GroundHalfDiagonal;
            position = new Vector3(position.x, position.y, z);
            playerTransform.position = position;
        }
        
        
    }
}