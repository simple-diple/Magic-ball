using Data;
using UnityEngine;

namespace View
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private GroundView prefab;

        private GroundView[,] _groundViews;
        
        public void Connect(LevelModel model)
        {
            Clear();
            _groundViews = new GroundView[model.Width, model.Height];
            Vector3 center = Vector3.zero;
            Vector3 firstGround = new Vector3(center.x + (float)model.Width / 2, center.y,
                center.y + (float)model.Height / 2);

            float diagonal = Mathf.Sqrt(2) * prefab.transform.localScale.x;
            float halfDiagonal = diagonal / 2;
            float currentDeltaX = 0;

            for (int j = 0; j < model.Height; j++)
            {
                for (int i = 0; i < model.Width; i++)
                {
                    var newGround = Instantiate(prefab);
                    newGround.transform.position = new Vector3(
                        x: firstGround.x - i * diagonal - currentDeltaX,
                        y: firstGround.y,
                        z: firstGround.z - j * halfDiagonal);
                    
                    newGround.point = new Vector2(i, j);
                    newGround.Set(model.GetGround(i, j));
                }
                currentDeltaX = j % 2 == 0 ? halfDiagonal : 0;
            }
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
    }
}