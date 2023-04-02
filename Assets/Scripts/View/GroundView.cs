using System.Collections;
using Data;
using UnityEngine;

namespace View
{
    public class GroundView : MonoBehaviour
    {
        [SerializeField] private Renderer render;
        [SerializeField] private DiamondView diamondView;

        public Ground Ground => _ground;

        private const float _FALL_DOWN_POSITION = 60;
        private const float _FALL_DOWN_SPEED = 4;
        private float? _defaultPositionZ;
        private Coroutine _fallingCoroutine;
        private Ground _ground;
        

        private void SetFloor(bool hasFloor)
        {
            render.enabled = hasFloor;
        }

        public void Set(Ground ground, LevelModel levelModel)
        {
            _ground = ground;
            _defaultPositionZ ??= transform.position.z;
            StopFalling();
            SetFloor(ground.floor);
            diamondView.Connect(ground, levelModel);
        }

        private void StopFalling()
        {
            if (_fallingCoroutine != null)
            {
                StopCoroutine(_fallingCoroutine);
                _fallingCoroutine = null;
                var groundTransform = transform;
                var position = groundTransform.position;
                position = new Vector3(position.x, position.y, _defaultPositionZ.Value);
                groundTransform.position = position;
            }
        }

        public void StartFalling()
        {
            if (_fallingCoroutine != null)
            {
                return;
            }
            
            _fallingCoroutine = StartCoroutine(FallDown());
        }

        private IEnumerator FallDown()
        {
            float deltaZ = transform.position.z;
            while (transform.position.z < _FALL_DOWN_POSITION)
            {
                deltaZ += _FALL_DOWN_SPEED * Time.deltaTime;
                var groundTransform = transform;
                var position = groundTransform.position;
                position = new Vector3(position.x, position.y, deltaZ);
                groundTransform.position = position;
                yield return null;
            }
        }
    }
}