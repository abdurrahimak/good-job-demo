using System;
using UnityEngine;

namespace CoreProject.Pool
{
    [Serializable]
    public class PoolModel
    {
        [SerializeField] private string _name;
        [Tooltip("Prefab must have PoolObject Component.")]
        [SerializeField] private GameObject _prefab;
        [Tooltip("Pool size.")]
        [SerializeField] private int _size;
        [Tooltip("If pool does not have the object when requested, the size at which the pool should recreate the object")]
        [SerializeField] private int _runtimeCreationSize = 2;

        private PoolObject _poolObject = null;
        public string Name => _name;
        public int Size => _size;
        public int RuntimeCreationSize => _runtimeCreationSize;
        public PoolObject PoolObject
        {
            get
            {
                if (_poolObject == null)
                {
                    _poolObject = _prefab.GetComponent<PoolObject>();
                    if (_poolObject == null)
                    {
                        _poolObject = _prefab.AddComponent<PoolObject>();
                    }
                    _poolObject.InitializePoolObject(this);
                }
                return _poolObject;
            }
        }
    }
}
