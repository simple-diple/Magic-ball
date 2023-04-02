using System;
using Data;
using UnityEngine;

namespace View
{
    public class GroundView : MonoBehaviour
    {
        [SerializeField] private Renderer render;
        public Vector2 point;

        private Color defaultColor;

        private void Awake()
        {
            defaultColor = render.material.color;
        }

        private void SetFloor(bool hasFloor)
        {
            render.enabled = hasFloor;
            render.material.color = defaultColor;
        }

        public void Set(Ground ground)
        {
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
    }
}