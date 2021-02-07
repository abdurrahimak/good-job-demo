using System;
using UnityEngine;
using UnityEngine.UI;

namespace GJGDemo.Game.UI
{
    public class RestartUI : MonoBehaviour
    {
        [SerializeField] private Button _buttonRestart;

        private void Start()
        {
            _buttonRestart.onClick.AddListener(Restart_OnClick);
        }

        private void Restart_OnClick()
        {
            LevelManager.Instance.OpenLevel();
            Destroy(gameObject);
        }
    }
}
