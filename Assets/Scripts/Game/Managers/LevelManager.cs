using System;
using CoreProject.Data;
using CoreProject.Pool;
using CoreProject.Resource;
using CoreProject.Singleton;
using GJGDemo.Data;
using GJGDemo.Factory;
using UnityEngine;

namespace GJGDemo.Game
{
    public class LevelManager : SingletonComponent<LevelManager>
    {
        private Levels _levels;
        private IDataStoreStrategy _datastoreStrategy;
        private int _currentLevel;
        private int _maxLevel;
        private Level _currentLevelObject;

        private void Start()
        {
            PoolerManager.Instance.Initialize();
            _datastoreStrategy = new PlayerPrefsStrategy();
            _levels = ResourceManager.Instance.GetResource<Levels>("Levels");
            _maxLevel = _levels.LevelDatas.Count - 1;
            ReadLevel();
            OpenLevel();
        }

        public void OpenLevel()
        {
            Time.timeScale = 1f;
            if (_currentLevelObject != null)
            {
                GameObject.Destroy(_currentLevelObject.gameObject);
            }
            LevelData levelData = _levels.LevelDatas[_currentLevel];
            _currentLevelObject = LevelFactory.Instance.CreateLevel(levelData);
        }

        public void OpenNextLevel()
        {
            _currentLevel++;
            _currentLevel %= _maxLevel;
            SaveLevel(_currentLevel);
            OpenLevel();
        }

        private void ReadLevel()
        {
            if (_datastoreStrategy.Has("Level"))
            {
                _currentLevel = Int32.Parse(_datastoreStrategy.Read("Level").ToString());
            }
            else
            {
                _currentLevel = 0;
                _datastoreStrategy.Write("Level", _currentLevel.ToString());
            }
        }

        private void SaveLevel(int level)
        {
            _datastoreStrategy.Write("Level", level.ToString());
        }
    }
}
