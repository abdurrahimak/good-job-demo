using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using CoreProject.Pool;
using GJGDemo.Data;
using GJGDemo.Game;
using GJGDemo.Factory;
using GJGDemo.Extensions;

public class Section : MonoBehaviour, ISectionState
{
    private Vector3Int _sectionOffset;
    private Grid _grid;
    private SectionData _sectionData;
    private PlayerCube _playerCube;
    private Dictionary<Vector3Int, Cube> _cubesByCellPos;
    private Vector2Int _size;
    private Vector3Int _minBounds = Vector3Int.zero, _maxBounds = Vector3Int.zero;
    private bool _isActive = false;
    private List<Cube> _fillingCubes = new List<Cube>();
    private List<BarrierCube> _barrierCubes = new List<BarrierCube>();
    private SectionShapeDetector _shapeDetector;

    public event Action SectionCompleted;
    public event Action RestartLevel;
    public Vector2Int Size => _size;
    public Vector3Int SectionOffset => _sectionOffset;
    public Dictionary<Vector3Int, Cube> CubesByCellPos => _cubesByCellPos;

    public void Init(Vector3Int sectionOffset, SectionData sectionData, PlayerCube playerCube)
    {
        _grid = GridManager.Instance.Grid;
        _sectionOffset = sectionOffset;
        _sectionData = sectionData;
        _playerCube = playerCube;
        _cubesByCellPos = new Dictionary<Vector3Int, Cube>();
        CreateCubes();
        _shapeDetector = new SectionShapeDetector(_grid, this, _cubesByCellPos);
    }

    private void CreateCubes()
    {
        _size = Vector2Int.zero;
        foreach (var cubeData in _sectionData.CubeDatas)
        {
            PoolObject poolObject = PoolerManager.Instance.GetPoolObject("Cube", transform);
            Cube cube = poolObject.GetComponent<Cube>();
            cube.CellPosition = cubeData.CellPosition + _sectionOffset;
            cube.ChangeCubeType(cubeData.CubeType);
            _cubesByCellPos.Add(cubeData.CellPosition + _sectionOffset, cube);

            if (cubeData.CellPosition.x > _size.x)
            {
                _size.x = cubeData.CellPosition.x;
            }

            if (cubeData.CellPosition.z > _size.y)
            {
                _size.y = cubeData.CellPosition.z;
            }
        }

        foreach (var barrierData in _sectionData.BarrierDatas)
        {
            PoolObject poolObject = PoolerManager.Instance.GetPoolObject("BarrierCube", transform);
            BarrierCube barrierCube = poolObject.GetComponent<BarrierCube>();
            barrierCube.StartCellPosition = barrierData.StartCellPosition + _sectionOffset;
            barrierCube.EndCellPosition = barrierData.EndCellPosition + _sectionOffset;
            barrierCube.Init();
            barrierCube.TriggeredFillingCube += BarrierCube_TriggerFillingCube;
            _barrierCubes.Add(barrierCube);
        }

        _size += Vector2Int.one;
    }

    void Update()
    {
        if (!_playerCube.CanMove)
        {
            return;
        }
        Vector3Int currentCellPos = _grid.WorldToCell(_playerCube.transform.GetWorlPositionForCell());

        if (_cubesByCellPos.TryGetValue(currentCellPos, out Cube cube))
        {
            if (cube.CubeType == CubeType.Empty)
            {
                cube.ChangeCubeType(CubeType.Filling);
                _fillingCubes.Add(cube);
            }
        }
    }
    bool started;
    public void Begin()
    {
        PrepareCamera();
        PreparePlayerPosition();
    }

    public void End()
    {
        UnregisterPlayerEvents();
    }

    private void RegisterPlayerEvents()
    {
        _playerCube.CollideTheWall += PlayerCube_CollideTheWall;
        _playerCube.EnterFilledarea += PlayerCube_EnterFilledArea;
        _playerCube.EnterFillingArea += PlayerCube_EnterFillingArea;
        _playerCube.CollideBarrier += PlayerCube_CollideBarrier;
    }

    private void UnregisterPlayerEvents()
    {
        foreach (var barrierCube in _barrierCubes)
        {
            barrierCube.TriggeredFillingCube -= BarrierCube_TriggerFillingCube;
        }
        _playerCube.CollideTheWall -= PlayerCube_CollideTheWall;
        _playerCube.EnterFilledarea -= PlayerCube_EnterFilledArea;
        _playerCube.EnterFillingArea -= PlayerCube_EnterFillingArea;
        _playerCube.CollideBarrier -= PlayerCube_CollideBarrier;
    }

    private void PreparePlayerPosition()
    {
        Sequence sequence = DOTween.Sequence();
        Vector3 targetPos = _grid.CellToWorld(new Vector3Int((_size.x / 2) + _sectionOffset.x, 0, _sectionOffset.z));
        _playerCube.CanMove = false;
        sequence.Append(_playerCube.transform.DOMoveX(targetPos.x, 0.5f));
        sequence.Append(_playerCube.transform.DOMoveZ(targetPos.z, 0.5f));
        sequence.OnComplete(() =>
        {
            _playerCube.CanMove = true;
            RegisterPlayerEvents();
        }).Play();
    }

    private void PrepareCamera()
    {
        Vector3 offset = new Vector3(0, 2 * _size.x, -3f);
        Vector3 cameraLookPosition = _grid.CellToWorld(new Vector3Int(_sectionOffset.x + _size.x / 2, 0, _sectionOffset.z + _size.y / 2));
        Camera.main.transform.DOMove(cameraLookPosition + offset, 1f).OnComplete(() =>
        {
            Camera.main.transform.LookAt(cameraLookPosition, Vector3.up);
        }).Play();
    }

    private void BarrierCube_TriggerFillingCube()
    {
        SendRestartLevelEvent();
    }

    private void PlayerCube_CollideBarrier()
    {
        SendRestartLevelEvent();
    }

    private void PlayerCube_EnterFillingArea()
    {
        if (_fillingCubes.Count > 0)
        {
            SendRestartLevelEvent();
        }
    }

    private void SendRestartLevelEvent()
    {
        RestartLevel?.Invoke();
    }

    private void PlayerCube_CollideTheWall()
    {
        FillShape();
    }

    private void PlayerCube_EnterFilledArea()
    {
        FillShape();
    }

    private void FillShape()
    {
        if (FillTheFillingCubes())
        {
            List<Shape> shapes = _shapeDetector.FindShapes();
            if (shapes.Count > 1)
            {
                do
                {
                    Shape shape = shapes.Find(s => !s.HasWallNeighbor);
                    if (shape == null)
                    {
                        List<Shape> sortedShapes = shapes.OrderBy(s => s.Cubes.Count).ToList();
                        shape = sortedShapes[0];
                    }
                    FillCubes(shape);
                    shapes.Remove(shape);
                } while (shapes.Count > 1 || (shapes.Count == 1 && shapes[0].Cubes.Count < 10));
            }
            else if (shapes.Count > 0)
            {
                if (shapes[0].Cubes.Count < 15 || !shapes[0].HasWallNeighbor)
                {
                    FillCubes(shapes[0]);
                }
            }

            CheckSectionCompleted();
        }
    }

    private void CheckSectionCompleted()
    {
        foreach (var cubeByCellPosKvp in _cubesByCellPos)
        {
            if (cubeByCellPosKvp.Value.CubeType == CubeType.Empty)
            {
                return;
            }
        }
        _playerCube.Stop();
        SectionCompleted?.Invoke();
    }

    private void FillCubes(Shape shape)
    {
        foreach (var cube in shape.Cubes)
        {
            if (cube.CubeType != CubeType.Wall)
            {
                cube.ChangeCubeType(CubeType.Filled);
            }
        }
    }

    private bool FillTheFillingCubes()
    {
        if (_fillingCubes.Count > 0)
        {
            foreach (var cube in _fillingCubes)
            {
                cube.ChangeCubeType(CubeType.Filled);
            }
            _fillingCubes.Clear();
            return true;
        }
        return false;
    }
}
