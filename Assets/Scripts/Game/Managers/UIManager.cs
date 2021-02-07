using System.Collections;
using System.Collections.Generic;
using CoreProject.Singleton;
using GJGDemo.Factory;
using UnityEngine;

namespace GJGDemo.Game.Managers
{
    public class UIManager : SingletonComponent<UIManager>
    {
        public void ShowRestart()
        {
            Time.timeScale = 0f;
            UIFactory.Instance.CreateRestartUI(transform);
        }

        public void ShowNextLevel()
        {
            Time.timeScale = 0f;
            UIFactory.Instance.CreateNextLevelUI(transform);
        }
    }
}
