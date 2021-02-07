using System;
using CoreProject.Pool;
using GJGDemo.Data;
using GJGDemo.Extensions;
using UnityEngine;

namespace GJGDemo.Game
{
    [ExecuteInEditMode]
    public class BarrierCube : MonoBehaviour
    {
        public float MoveSpeed = 1.5f;
        public Vector3Int StartCellPosition;
        public Vector3Int EndCellPosition;

        private Grid Grid;
        private float _speed;
        private Vector3 _movementDir;
        private Rigidbody _rb;
        private Vector3 _startWorldPosition, _endWorldPosition, _targetWorldPosition;
        private bool _forward;

        public Vector3Int GetCurrentCellPos => Grid.WorldToCell(transform.GetWorlPositionForCell());

        public event Action TriggeredFillingCube;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            Grid = GridManager.Instance.Grid;
            _rb = GetComponent<Rigidbody>();
            _startWorldPosition = Grid.CellToWorld(StartCellPosition);
            _endWorldPosition = Grid.CellToWorld(EndCellPosition);
            _targetWorldPosition = _endWorldPosition;
            transform.position = _startWorldPosition;
            _forward = true;
            if (StartCellPosition != EndCellPosition)
            {
                _movementDir = (_targetWorldPosition - transform.position).normalized;
            }
            else
            {
                _movementDir = Vector3.zero;
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                if (Grid != null)
                    transform.position = Grid.CellToWorld(Grid.WorldToCell(transform.GetWorlPositionForCell()));
            }
            else
            {
                float distance = Vector3.Distance(transform.position, _targetWorldPosition);
                if (distance < _speed)
                {
                    transform.position = _targetWorldPosition;
                    _forward = !_forward;
                    _targetWorldPosition = _forward ? _startWorldPosition : _endWorldPosition;
                    _movementDir = (_targetWorldPosition - transform.position).normalized;
                }
            }
        }

        private void FixedUpdate()
        {
            Vector3 pos = transform.position;
            pos.y = 0f;
            _speed = Time.deltaTime * MoveSpeed;
            _rb.MovePosition(pos + (_movementDir * _speed));
        }

        private void OnTriggerEnter(Collider other)
        {
            Cube cube = other.gameObject.GetComponent<Cube>();
            if (cube)
            {
                if (cube.CubeType == CubeType.Filled)
                {
                    GameObject.Destroy(gameObject);
                }
                else if (cube.CubeType == CubeType.Filling)
                {
                    TriggeredFillingCube?.Invoke();
                }
            }
        }
    }
}
