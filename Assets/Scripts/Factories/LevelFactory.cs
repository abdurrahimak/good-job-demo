using CoreProject.Resource;
using CoreProject.Singleton;
using GJGDemo.Data;
using GJGDemo.Game;
using UnityEngine;

namespace GJGDemo.Factory
{
    public class LevelFactory : SingletonClass<LevelFactory>
    {
        public Section CreateSection(Vector3Int sectionOffset, SectionData sectionData, PlayerCube playerCube, Transform parent = null)
        {
            Section section = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("Section"), parent).GetComponent<Section>();
            section.Init(sectionOffset, sectionData, playerCube);
            return section;
        }

        public Level CreateLevel(LevelData levelData, Transform parent = null)
        {
            Level level = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("Level"), parent).GetComponent<Level>();
            level.Init(levelData);
            return level;
        }
    }
}
