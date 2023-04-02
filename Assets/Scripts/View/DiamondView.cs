using Data;
using UnityEngine;

namespace View
{
    public class DiamondView : MonoBehaviour
    {
        private LevelModel _levelModel;
        public void Connect(bool diamond, LevelModel levelModel)
        {
            _levelModel = levelModel;
            gameObject.SetActive(diamond);
        }

        public void Take()
        {
            gameObject.SetActive(false);
            _levelModel.TakeDiamond();
        }
    }
}