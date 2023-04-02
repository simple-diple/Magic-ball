using UnityEngine;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float offset;

        private Transform _target;
        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target == false)
            {
                return;
            }
            
            Transform cameraTransform = cam.transform;
            Vector3 position = cameraTransform.position;
            position =
                new Vector3(position.x, position.y, _target.position.z + offset);
            cameraTransform.position = position;
        }
    }
}