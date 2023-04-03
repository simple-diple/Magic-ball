using System.Collections;
using Data;
using UnityEngine;

namespace View
{
    public class GroundView : MonoBehaviour
    {
        [SerializeField] private Renderer render;
        [SerializeField] private DiamondView diamondView;

        public Ground Ground => _levelModel.GetGround(_point);

        private const float _FALL_DOWN_POSITION = 60;
        private const float _FALL_DOWN_SPEED = 4;
        private float? _defaultPositionZ;
        private Coroutine _fallingCoroutine;
        private Vector2 _point;
        private LevelModel _levelModel;

        private void SetFloor(bool hasFloor)
        {
            render.enabled = hasFloor;
        }

        public void Connect(Vector2 point, LevelModel levelModel)
        {
            _point = point;
            _levelModel = levelModel;
            _defaultPositionZ ??= transform.position.z;
            _levelModel.OnGroundChanged -= OnGroundChanged;
            _levelModel.OnGroundChanged += OnGroundChanged;
            UpdateData();
        }

        private void OnGroundChanged()
        {
            UpdateData();
        }

        private void UpdateData()
        {
            StopFalling();
            SetFloor(Ground.floor);
            diamondView.Connect(this, _levelModel);
        }

        private void StopFalling()
        {
            if (_fallingCoroutine != null)
            {
                StopCoroutine(_fallingCoroutine);
                _fallingCoroutine = null;
            }
            var groundTransform = transform;
            var position = groundTransform.position;
            position = new Vector3(position.x, position.y, _defaultPositionZ.Value);
            groundTransform.position = position;

        }

        public void StartFalling()
        {
            if (_fallingCoroutine != null)
            {
                return;
            }

            if (Ground.point.y > _levelModel.PlayerPoint.y)
            {
                return;
            }
            
            _fallingCoroutine = StartCoroutine(FallDown());
        }

        private IEnumerator FallDown()
        {
            float deltaZ = transform.position.z;
            while (transform.position.z < _FALL_DOWN_POSITION && Ground.point.y < _levelModel.PlayerPoint.y)
            {
                deltaZ += _FALL_DOWN_SPEED * Time.deltaTime;
                var groundTransform = transform;
                var position = groundTransform.position;
                position = new Vector3(position.x, position.y, deltaZ);
                groundTransform.position = position;
                yield return null;
            }

            if (Ground.point.y >= _levelModel.PlayerPoint.y)
            {
                StopFalling();
            }
        }

        public void Debug()
        {
            Debug(Ground.floor ? Color.yellow : Color.white);
        }

        public void Debug(Color color)
        {
            render.enabled = true;
            render.material.color = color;
        }
    }
}