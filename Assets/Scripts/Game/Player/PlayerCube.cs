using System;
using GJGDemo.Data;
using GJGDemo.Extensions;
using UnityEngine;

namespace GJGDemo.Game
{
    public class PlayerCube : MonoBehaviour
    {
        [SerializeField] public float MovementSpeed = 1f;
        [SerializeField] public Vector3 _movementDir;
        [SerializeField] private Grid _grid;

        private Rigidbody _rb;
        private BoxCollider _boxCollider;
        private Vector3 _startPos = Vector3.zero, _lastPos = Vector3.zero;
        private Vector3 _changeDir = Vector3.zero;
        bool _changeAxis = false, _canMove;
        private float maxMoveThreshold;

        public bool CanMove
        {
            get => _canMove;
            set
            {
                _canMove = value;
                if (!_canMove)
                {
                    Stop();
                }
            }
        }
        public BoxCollider BoxCollider => _boxCollider ?? GetComponent<BoxCollider>();
        public void Stop() => _movementDir = Vector3.zero;

        public event Action CollideBarrier;
        public event Action CollideTheWall;
        public event Action EnterFilledarea;
        public event Action EnterFillingArea;

        void Start()
        {
            _canMove = false;
            _rb = GetComponent<Rigidbody>();
            _boxCollider = GetComponent<BoxCollider>();
            _grid = GridManager.Instance.Grid;
        }

        void Update()
        {
            Vector3 pos = transform.position;
            pos.y = 0f;
            Vector3Int currentCellPos = _grid.WorldToCell(pos);
            if (Input.GetMouseButtonDown(0))
            {
                _startPos = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _lastPos = Input.mousePosition;
                Vector3 diff = _lastPos - _startPos;
                diff.z = diff.y;
                diff.y = 0f;
                diff.Normalize();
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
                {
                    diff.z = 0f;
                }
                else
                {
                    diff.x = 0f;
                }
                if (_movementDir.x > 0f || _movementDir.x < 0f)
                {
                    if (Mathf.Abs(diff.z) > 0f)
                    {
                        _changeAxis = true;
                    }
                }
                else if (_movementDir.z > 0f || _movementDir.z < 0f)
                {
                    if (Mathf.Abs(diff.x) > 0f)
                    {
                        _changeAxis = true;
                    }
                }
                else
                {
                    _movementDir = diff;
                }
                _changeDir = diff;
            }

            float distance = Vector3.Distance(pos, _grid.CellToWorld(currentCellPos));
            if (_changeAxis && distance <= maxMoveThreshold)
            {
                _changeAxis = false;
                transform.position = _grid.CellToWorld(currentCellPos);
                _movementDir = _changeDir;
            }
        }

        private void FixedUpdate()
        {
            if (!_canMove)
            {
                return;
            }
            Vector3 pos = transform.position;
            pos.y = 0f;
            maxMoveThreshold = Time.deltaTime * MovementSpeed;
            _rb.MovePosition(pos + (_movementDir * Time.deltaTime * MovementSpeed));
        }

        private void OnCollisionEnter(Collision other)
        {
            _movementDir = Vector3.zero;
            if (other.gameObject.GetComponent<Cube>())
            {
                Vector3Int currentCellPos = _grid.WorldToCell(transform.GetWorlPositionForCell());
                transform.position = _grid.CellToWorld(currentCellPos);
                CollideTheWall?.Invoke();
            }
            else if (other.gameObject.GetComponent<BarrierCube>())
            {
                CollideBarrier?.Invoke();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Cube cube = other.gameObject.GetComponent<Cube>();
            if (cube != null)
            {
                if (cube.CubeType == CubeType.Filled)
                {
                    EnterFilledarea?.Invoke();
                }
                if (cube.CubeType == CubeType.Filling)
                {
                    EnterFillingArea?.Invoke();
                }
            }
        }
    }
}
