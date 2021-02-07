using System.Collections.Generic;
using CoreProject.Pool;
using GJGDemo.Data;
using GJGDemo.Factory;
using GJGDemo.Game.Managers;
using UnityEngine;

namespace GJGDemo.Game
{
    public class Level : MonoBehaviour, ISectionStateController
    {
        public int CountBetweenSections;

        private PlayerCube _playerCube;
        private LevelData _levelData;
        private Grid _grid;
        private List<Section> _sections;
        private ISectionState _currentSectionState;
        private List<PoolObject> _poolObjects;

        public void Init(LevelData levelData)
        {
            _poolObjects = new List<PoolObject>();
            _sections = new List<Section>();
            _grid = GridManager.Instance.Grid;
            _levelData = levelData;
            _playerCube = CubeFactory.Instance.CreatePlayerCube(transform);
            _playerCube.transform.position = Vector3.zero;
            Vector3Int sectionOffset = Vector3Int.zero;
            foreach (var sectionData in levelData.Sections)
            {
                Vector2Int sectionSize = sectionData.GetSize();
                sectionOffset.x = -1 * (sectionSize.x / 2);
                Section section = CreateSection(sectionOffset, sectionData);
                sectionOffset += new Vector3Int(0, 0, CountBetweenSections + sectionSize.y);
                _sections.Add(section);
            }
            InitWalls();
            SwitchSectionState(_sections[0]);
        }

        private void InitWalls()
        {
            int minimumOffsetX = 0;
            int maximumOffsetX = 0;
            int maximumY = 0;
            foreach (var section in _sections)
            {
                if (section.SectionOffset.z + section.Size.y > maximumY)
                {
                    maximumY = section.SectionOffset.z + section.Size.y;
                }

                if (minimumOffsetX > section.SectionOffset.x)
                {
                    minimumOffsetX = section.SectionOffset.x;
                }

                if (maximumOffsetX < section.SectionOffset.x + section.Size.x)
                {
                    maximumOffsetX = section.SectionOffset.x + section.Size.x;
                }
            }
            for (int i = minimumOffsetX + -5; i < maximumOffsetX + 5; i++)
            {
                for (int j = -5; j < maximumY + 5; j++)
                {
                    Vector3Int cellPos = new Vector3Int(i, 0, j);
                    if (_sections.Find(s => s.CubesByCellPos.ContainsKey(cellPos)) == null)
                    {
                        PoolObject poolObject = PoolerManager.Instance.GetPoolObject("Cube", transform);
                        Cube cube = poolObject.GetComponent<Cube>();
                        cube.CellPosition = cellPos;
                        cube.ChangeCubeType(CubeType.Wall);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var poolObject in gameObject.GetComponentsInChildren<PoolObject>())
            {
                PoolerManager.Instance.SetPoolObjectToPool(poolObject);
            }
        }

        private Section CreateSection(Vector3Int sectionOffset, SectionData sectionData)
        {
            return LevelFactory.Instance.CreateSection(sectionOffset, sectionData, _playerCube, transform);
        }

        public void SwitchSectionState(ISectionState sectionState)
        {
            if (_currentSectionState != null)
            {
                _currentSectionState.SectionCompleted -= SectionState_SectionCompleted;
                _currentSectionState.RestartLevel -= SectionState_RestartLevel;
                _currentSectionState?.End();
            }
            _currentSectionState = sectionState;
            if (_currentSectionState != null)
            {
                _currentSectionState.SectionCompleted += SectionState_SectionCompleted;
                _currentSectionState.RestartLevel += SectionState_RestartLevel;
                _currentSectionState?.Begin();
            }
        }

        private void SectionState_RestartLevel()
        {
            _playerCube.Stop();
            SwitchSectionState(null);
            UIManager.Instance.ShowRestart();
        }

        private void SectionState_SectionCompleted()
        {
            int index = _sections.FindIndex(x => (x as ISectionState) == _currentSectionState);
            index++;
            if (index < _sections.Count)
            {
                SwitchSectionState(_sections[index]);
            }
            else if (index == _sections.Count)
            {
                UIManager.Instance.ShowNextLevel();
            }
        }
    }
}
