using Data;
using UnityEngine;

namespace View
{
    public class DiamondView : MonoBehaviour, ICollectable
    {
        private LevelModel _levelModel;
        private GroundView _groundView;
        
        public void Connect(GroundView ground, LevelModel levelModel)
        {
            _levelModel = levelModel;
            _groundView = ground;
            gameObject.SetActive(ground.Ground is { floor: true, diamond: true });
        }

        public void Collect()
        {
            if (_levelModel.TryTakeDiamond(_groundView.Ground.point))
            {
                gameObject.SetActive(false);
            }
            else
            {
                _groundView.Debug(Color.cyan);
            }
            
        }
    }
}