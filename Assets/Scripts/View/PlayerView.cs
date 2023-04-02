using Data;
using UnityEngine;

namespace View
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private float offsetY = 0.65f;

        private LevelModel _levelModel;
        private LevelView _levelView;
        private float _speed;

        public void SetSpeed(float value)
        {
            _speed = value;
        }

        public void Wrap(GroundView groundView)
        {
            transform.position =
                groundView.transform.position + Vector3.up * offsetY;
        }

        private void Update()
        {
            //Transform playerTransform = transform;
            //playerTransform.position += playerTransform.forward * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            GroundView groundView = other.gameObject.GetComponent<GroundView>();
            _levelModel.SetPlayerGround(groundView.point);
        }

        public void Connect(LevelModel levelModel, LevelView levelView)
        {
            _levelModel = levelModel;
            _levelView = levelView;
            _levelModel.OnNewGroundsGenerated -= OnNewGroundsGenerated;
            _levelModel.OnNewGroundsGenerated += OnNewGroundsGenerated;
        }

        private void OnNewGroundsGenerated(Vector2 playerPoint)
        {
            var point = _levelView.GetGroundView(playerPoint);
            Wrap(point);
        }
    }
}