using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using CoreProject.Singleton;
using GJGDemo.Data;
using GJGDemo.Game;
using GJGDemo.Factory;

namespace GJGDemo.Editor
{
    [CustomEditor(typeof(Grid))]
    public class GridEditor : UnityEditor.Editor
    {
        Grid _grid;
        Vector2Int _size;
        int _selectedLevel;
        int _selectedSection;
        Levels _levels;
        string[] _levelIndexes;
        string[] _sectionIndexes;
        public override VisualElement CreateInspectorGUI()
        {
            _grid = serializedObject.targetObject as Grid;
            LoadLevel();
            return base.CreateInspectorGUI();
        }

        private void LoadLevel()
        {
            _levels = Resources.Load<Levels>("Levels");
            if (_levels == null)
            {
                _levels = ScriptableObject.CreateInstance<Levels>();
                _levels.LevelDatas = new System.Collections.Generic.List<LevelData>();
                AssetDatabase.CreateAsset(_levels, "Assets/Resources/Levels.asset");
            }
            _sectionIndexes = new string[0];
            _levelIndexes = new string[0];
            CreateLevelIndexes(_levels);
            if (_levels.LevelDatas.Count > 0)
            {
                CreateSectionIndexes(_levels.LevelDatas[0]);
                _selectedSection = 0;
            }
        }

        private void CreateLevelIndexes(Levels levels)
        {
            _levelIndexes = new string[levels.LevelDatas.Count];
            for (int i = 0; i < levels.LevelDatas.Count; i++)
            {
                _levelIndexes[i] = $"{i}";
            }
            _selectedLevel = 0;
        }

        private void CreateSectionIndexes(LevelData levelData)
        {
            _sectionIndexes = new string[levelData.Sections.Count];
            for (int i = 0; i < levelData.Sections.Count; i++)
            {
                _sectionIndexes[i] = $"{i}";
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField("Grid Editor");
                _size = EditorGUILayout.Vector2IntField("Size", _size);
                if (GUILayout.Button("Create Barrier Cubes"))
                {
                    ClearBarrierCubes();
                    CreateBarrierCubes(_size);
                }
                if (GUILayout.Button("Create Empty Cubes"))
                {
                    ClearCubes();
                    CreateCubes(_size);
                }
                if (GUILayout.Button("Clear Cubes"))
                {
                    ClearCubes();
                }
                if (GUILayout.Button("Clear Barrier Cubes"))
                {
                    ClearBarrierCubes();
                }
                EditorGUILayout.Separator();
                if (GUILayout.Button("Add Level"))
                {
                    AddLevel();
                }
                if (_levels.LevelDatas.Count != 0)
                {
                    if (GUILayout.Button("Remove Selected Level"))
                    {
                        RemoveSelectedLevel();
                    }
                    if (GUILayout.Button("Add Section"))
                    {
                        AddSection();
                    }
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Level :");
                        _selectedLevel = EditorGUILayout.Popup(_selectedLevel, _levelIndexes);
                        CreateSectionIndexes(_levels.LevelDatas[_selectedLevel]);
                    }
                    EditorGUILayout.EndVertical();
                    if (_levels.LevelDatas[_selectedLevel].Sections.Count != 0)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            EditorGUILayout.LabelField("Section :");
                            _selectedSection = EditorGUILayout.Popup(_selectedSection, _sectionIndexes);
                        }
                        EditorGUILayout.EndVertical();
                        if (GUILayout.Button("Remove Selected Section"))
                        {
                            RemoveSelectedSection();
                        }
                        if (GUILayout.Button("Load Section"))
                        {
                            LoadSelectedSection();
                        }
                        if (GUILayout.Button("Save Section"))
                        {
                            SaveSelectedSection();
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void CreateBarrierCubes(Vector2Int size)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    BarrierCube barrierCube = CubeFactory.Instance.CreateBarrierCube(new Vector3Int(i, 0, j), new Vector3Int(i, 0, j), _grid.transform);
                }
            }
        }

        private void CreateCubes(Vector2Int size)
        {
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Cube cube = CubeFactory.Instance.CreateCube(new Vector3Int(i, 0, j), CubeType.Wall, _grid.transform);
                }
            }
        }

        private void ClearBarrierCubes()
        {
            foreach (var item in _grid.GetComponentsInChildren<BarrierCube>())
            {
                DestroyImmediate(item.gameObject);
            }
        }

        private void ClearCubes()
        {
            foreach (var item in _grid.GetComponentsInChildren<Cube>())
            {
                DestroyImmediate(item.gameObject);
            }
        }

        private void LoadSelectedSection()
        {
            ClearBarrierCubes();
            ClearCubes();
            SectionData selectedSectionData = _levels.LevelDatas[_selectedLevel].Sections[_selectedSection];
            foreach (var cubeData in selectedSectionData.CubeDatas)
            {
                Cube cube = CubeFactory.Instance.CreateCube(cubeData.CellPosition, cubeData.CubeType, _grid.transform);
            }
            foreach (var cubeData in selectedSectionData.BarrierDatas)
            {
                BarrierCube barrierCube = CubeFactory.Instance.CreateBarrierCube(cubeData.StartCellPosition, cubeData.EndCellPosition, _grid.transform);
            }
            Debug.Log($"Loaded {selectedSectionData.CubeDatas.Count} cubes and {selectedSectionData.BarrierDatas.Count} barrier cubes from Level {_selectedLevel}, Section {_selectedSection}");
        }

        private void SaveSelectedSection()
        {
            SectionData selectedSectionData = _levels.LevelDatas[_selectedLevel].Sections[_selectedSection];
            selectedSectionData.CubeDatas = new System.Collections.Generic.List<CubeData>();
            selectedSectionData.BarrierDatas = new System.Collections.Generic.List<BarrierData>();
            foreach (var cube in _grid.GetComponentsInChildren<Cube>())
            {
                CubeData cubeData = new CubeData()
                {
                    CellPosition = cube.CellPosition,
                    CubeType = cube.CubeType
                };
                selectedSectionData.CubeDatas.Add(cubeData);
            }

            foreach (var barrierCube in _grid.GetComponentsInChildren<BarrierCube>())
            {
                BarrierData barrierData = new BarrierData()
                {
                    StartCellPosition = barrierCube.StartCellPosition,
                    EndCellPosition = barrierCube.EndCellPosition
                };
                selectedSectionData.BarrierDatas.Add(barrierData);
            }
            SaveLevelsAsset(_levels);
            Debug.Log($"Saved {selectedSectionData.CubeDatas.Count} cubes, {selectedSectionData.BarrierDatas.Count} barriers to Level {_selectedLevel}, Section {_selectedSection}");
        }

        private void RemoveSelectedSection()
        {
            LevelData selectedLevelData = _levels.LevelDatas[_selectedLevel];
            selectedLevelData.Sections.RemoveAt(_selectedSection);
            CreateSectionIndexes(selectedLevelData);
            _selectedSection = 0;
            SaveLevelsAsset(_levels);
        }

        private void AddSection()
        {
            LevelData selectedLevelData = _levels.LevelDatas[_selectedLevel];
            selectedLevelData.Sections.Add(new SectionData());
            CreateSectionIndexes(selectedLevelData);
            _selectedSection = selectedLevelData.Sections.Count - 1;
            SaveLevelsAsset(_levels);
        }

        private void RemoveSelectedLevel()
        {
            _levels.LevelDatas.RemoveAt(_selectedLevel);
            CreateLevelIndexes(_levels);
            SaveLevelsAsset(_levels);
        }

        private void AddLevel()
        {
            LevelData levelData = new LevelData();
            _levels.LevelDatas.Add(levelData);
            CreateLevelIndexes(_levels);
            SaveLevelsAsset(_levels);
        }

        private void SaveLevelsAsset(Levels levels)
        {
            EditorUtility.SetDirty(levels);
            AssetDatabase.SaveAssets();
        }
    }
}