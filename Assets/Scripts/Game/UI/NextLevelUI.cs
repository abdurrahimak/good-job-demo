using UnityEngine;
using UnityEngine.UI;

namespace GJGDemo.Game.UI
{
    public class NextLevelUI : MonoBehaviour
    {
        [SerializeField] private Button _buttonNextLevel;

        private void Start()
        {
            _buttonNextLevel.onClick.AddListener(NextLevel_OnClick);
        }

        private void NextLevel_OnClick()
        {
            LevelManager.Instance.OpenNextLevel();
            Destroy(gameObject);
        }
    }
}
