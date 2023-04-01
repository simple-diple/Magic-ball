using System;
using Data;
using UnityEngine;
using View;

namespace Controller
{
    public class LevelController : MonoBehaviour
    {
        [SerializeField] private LevelView levelView;
        [SerializeField] private  Settings settings;
        private LevelModel _levelModel;

        private void Awake()
        {
            CreateLevel();
            levelView.Connect(_levelModel);
        }

        private void CreateLevel()
        {
            _levelModel = new LevelModel(settings);
        }
    }
}