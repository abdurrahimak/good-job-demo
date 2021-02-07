using CoreProject.Resource;
using CoreProject.Singleton;
using GJGDemo.Game.UI;
using UnityEngine;

namespace GJGDemo.Factory
{
    public class UIFactory : SingletonClass<UIFactory>
    {
        public RestartUI CreateRestartUI(Transform parent)
        {
            RestartUI restartUI = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("RestartUI"), parent).GetComponent<RestartUI>();
            return restartUI;
        }

        public NextLevelUI CreateNextLevelUI(Transform parent)
        {
            NextLevelUI nextLevelUI = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("NextLevelUI"), parent).GetComponent<NextLevelUI>();
            return nextLevelUI;
        }
    }
}
