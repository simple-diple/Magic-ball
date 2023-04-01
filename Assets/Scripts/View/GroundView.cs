using Data;
using UnityEngine;

namespace View
{
    public class GroundView : MonoBehaviour
    {
        [SerializeField] private Renderer renderer;

        //ToDo remove me!
        public Vector2 point;
        private void SetFloor(bool hasFloor)
        {
            renderer.material.color = hasFloor ? Color.magenta : Color.white;
        }

        public void Set(Ground ground)
        {
            SetFloor(ground.floor);

            if (ground.floor)
            {
                renderer.material.color = ground.first ? Color.red : Color.magenta;
                
                if (ground.temp)
                    renderer.material.color = Color.yellow;
            }
        }
    }
}