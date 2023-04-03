using Data;
using UnityEngine;

namespace View
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private GroundView prefab;

        public float GroundHalfDiagonal => _halfDiagonal;

        private GroundView[,] _groundViews;
        private LevelModel _levelModel;
        private float _halfDiagonal;

        private const float _FALLING_OFFSET = 5f;

        public void Connect(LevelModel model)
        {
            _levelModel = model;

            Clear();
            CreateView(_levelModel);
        }
        
        private void FixedUpdate()
        {
            if (_levelModel.State != LevelState.Playing)
            {
                return;
            }
            
            for (int y = (int)_levelModel.PlayerPoint.y; y >= 0; y--)
            {
                if (NeedFalling(y) == false)
                {
                    continue;
                }
                
                for (int x = 0; x < _levelModel.Width; x++)
                {
                    _groundViews[x, y].StartFalling();
                }
            }
        }

        private void CreateView(LevelModel model)
        {
            _groundViews = new GroundView[model.Width, model.Height];
            
            Vector3 center = Vector3.zero;
            Vector3 firstGround = new Vector3(center.x + (float)model.Width / 2, center.y,
                center.y + (float)model.Height / 2);

            float diagonal = Mathf.Sqrt(2) * prefab.transform.localScale.x;
            _halfDiagonal = diagonal / 2;
            float currentDeltaX = 0;

            for (int j = 0; j < _levelModel.Height; j++)
            {
                for (int i = 0; i < _levelModel.Width; i++)
                {
                    var newGround = Instantiate(prefab);
                    newGround.transform.position = new Vector3(
                        x: firstGround.x - i * diagonal - currentDeltaX,
                        y: firstGround.y,
                        z: firstGround.z - j * _halfDiagonal);
                    
                    _groundViews[i, j] = newGround;
                    _groundViews[i, j].Connect(new Vector2(i, j), _levelModel);
                }
                currentDeltaX = j % 2 == 0 ? _halfDiagonal : 0;
            }
        }

        public GroundView GetGroundView(Vector2 point)
        {
            return _groundViews[(int)point.x, (int)point.y];
        }

        private void Clear()
        {
            if (_groundViews == null)
            {
                return;
            }
            
            foreach (var view in _groundViews)
            {
                Destroy(view.gameObject);
            }
        }

        private bool NeedFalling(float y)
        {
            return y + _FALLING_OFFSET < _levelModel.PlayerPoint.y;
        }
        
    }
}