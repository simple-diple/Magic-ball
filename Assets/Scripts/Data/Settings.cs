using UnityEngine;

namespace Data
{
    [CreateAssetMenu(menuName = "Create Settings", fileName = "Settings", order = 0)]
    public class Settings : ScriptableObject
    {
        public Difficulty difficulty;
        public DiamondsOrder diamondsOrder;
        public Vector2 levelSize;
        public float playerSpeed = 1;
    }
}