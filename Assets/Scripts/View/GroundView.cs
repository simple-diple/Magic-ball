using System.Collections;
using Data;
using UnityEngine;

namespace View
{
    public class GroundView : MonoBehaviour
    {
        [SerializeField] private Renderer render;
        public Vector2 point;

        private const float _FALL_DOWN_POSITION = 60;
        private const float _FALL_DOWN_SPEED = 2;
        private Color _defaultColor;
        private float? _defaultPositionZ;
        private Coroutine _fallingCoroutine;

        private void Awake()
        {
            _defaultColor = render.material.color;
        }

        private void SetFloor(bool hasFloor)
        {
            render.enabled = hasFloor;
            //render.material.color = hasFloor ? defaultColor : Color.white;
        }

        public void Set(Ground ground, bool stopFalling)
        {
            _defaultPositionZ ??= transform.position.z;
            if (stopFalling)
            {
                StopFalling();
            }
            SetFloor(ground.floor);
            SetTest(ground.test);
        }

        private void SetTest(bool groundTest)
        {
            if (groundTest == false)
            {
                return;
            }
            
            render.enabled = true;
            render.material.color = Color.yellow;
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