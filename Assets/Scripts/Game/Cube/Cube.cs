using GJGDemo.Data;
using UnityEngine;
using DG.Tweening;

namespace GJGDemo.Game
{
    [ExecuteInEditMode]
    public class Cube : MonoBehaviour
    {
        public Vector3Int CellPosition;
        public CubeType CubeType;
        public BoxCollider BoxCollider;

        public Material _emptyMaterial;
        public Material _fillingMaterial;
        public Material _filledMaterial;
        public Material _wallMaterial;

        private Vector3Int _tempCellPos;
        private MeshRenderer _meshRenderer;
        private Grid _grid;

        private void Awake()
        {
            _grid = GridManager.Instance.Grid;
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        void Start()
        {
            OnCubeTypeChanged(CubeType);
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                ChangeCubeType(CubeType);
            }
        }

        private void OnCubeTypeChanged(CubeType cubeType)
        {
            _tempCellPos = CellPosition;
            if (cubeType == CubeType.Empty)
            {
                _tempCellPos.y = -2;
                UpdatePosition(_tempCellPos);
                _meshRenderer.material = _emptyMaterial;
                BoxCollider.isTrigger = true;
            }
            else if (cubeType == CubeType.Filling)
            {
                _tempCellPos.y = -1;
                UpdatePositionWithAnimation(_tempCellPos);
                _meshRenderer.material = _fillingMaterial;
                BoxCollider.isTrigger = true;
            }
            else if (cubeType == CubeType.Filled)
            {
                _tempCellPos.y = 0;
                UpdatePositionWithAnimation(_tempCellPos);
                _meshRenderer.material = _filledMaterial;
                BoxCollider.isTrigger = true;
            }
            else if (cubeType == CubeType.Wall)
            {
                _tempCellPos.y = 0;
                UpdatePosition(_tempCellPos);
                _meshRenderer.material = _wallMaterial;
                BoxCollider.isTrigger = false;
            }
        }

        private void UpdatePosition(Vector3Int cellPosition)
        {
            if (_grid)
            {
                transform.position = _grid.CellToWorld(cellPosition);
            }
        }

        private void UpdatePositionWithAnimation(Vector3Int cellPosition)
        {
            if (_grid)
            {
                transform.DOMove(_grid.CellToWorld(cellPosition), 0.5f);
            }
        }

        public void ChangeCubeType(CubeType cubeType)
        {
            CubeType = cubeType;
            OnCubeTypeChanged(cubeType);
        }
    }
}
