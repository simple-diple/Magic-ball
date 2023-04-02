using Data;
using UnityEngine;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float offset;

        private Transform _target;
        private LevelModel _levelModel;
        
        public void Connect(Transform target, LevelModel levelModel)
        {
            _target = target;
            _levelModel = levelModel;
            Focus(_target);
            
            _levelModel.OnLevelStateChange -= OnLevelStateChange;
            _levelModel.OnLevelStateChange += OnLevelStateChange;
        }

        private void OnLevelStateChange(LevelState state)
        {
            if (_target && state == LevelState.Paused)
            {
                Focus(_target);
            }
        }

        private void LateUpdate()
        {
            if (_target == false)
            {
                return;
            }
            
            if (_levelModel.State != LevelState.Playing)
            {
                return;
            }

            Focus(_target);
        }
        
        private void Focus(Transform target)
        {
            Transform cameraTransform = cam.transform;
            Vector3 position = cameraTransform.position;
            position =
                new Vector3(position.x, position.y, target.position.z + offset);
            cameraTransform.position = position;
        }
    }
}