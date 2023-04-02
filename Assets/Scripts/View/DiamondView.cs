using Data;
using UnityEngine;

namespace View
{
    public class DiamondView : MonoBehaviour
    {
        private LevelModel _levelModel;
        private Diamond _diamond;
        public void Connect(Ground diamond, LevelModel levelModel)
        {
            _diamond = diamond.diamond;
            _levelModel = levelModel;
            gameObject.SetActive(_diamond is { isTaken: false });
        }

        public void Take()
        {
            gameObject.SetActive(false);
            if (_diamond != null)
            {
                _levelModel.TakeDiamond();
            }
        }
    }
}