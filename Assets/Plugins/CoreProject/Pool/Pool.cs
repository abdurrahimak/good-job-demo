using System.Collections.Generic;
using UnityEngine;

namespace CoreProject.Pool
{
    public class Pool : IPool
    {
        private Queue<PoolObject> _poolObjectQueue;
        private Transform _poolParentTransform;
        private PoolModel _poolModel;

        public Pool(PoolModel poolModel, Transform poolParentTransform)
        {
            _poolModel = poolModel;
            _poolObjectQueue = new Queue<PoolObject>();
            CreatePoolParent(poolParentTransform);
            InitializePool();
        }

        private void InitializePool()
        {
            CreateObjectToPool(_poolModel, _poolModel.Size);
        }

        private void CreatePoolParent(Transform poolParentTransform)
        {
            GameObject go = new GameObject(_poolModel.Name + "_Parent");
            go.transform.SetParent(poolParentTransform);
            go.SetActive(false);
            _poolParentTransform = go.transform;
        }

        public PoolObject GetPoolObject(string poolName, Transform parent = null)
        {
            if (_poolObjectQueue == null || _poolObjectQueue.Count == 0)
            {
                CreateObjectToPool(_poolModel, _poolModel.RuntimeCreationSize);
            }

            PoolObject poolObject = _poolObjectQueue.Dequeue();
            poolObject.transform.SetParent(parent);
            poolObject.FromPool();
            return poolObject;
        }

        public void SetPoolObject(PoolObject poolObject)
        {
            if(poolObject.Pool == _poolModel)
            {
                poolObject.transform.SetParent(_poolParentTransform);
                _poolObjectQueue.Enqueue(poolObject);
                poolObject.ToPool();
            }
        }

        private void CreateObjectToPool(PoolModel pool)
        {
            PoolObject poolObject = pool.PoolObject;
            GameObject go = GameObject.Instantiate(poolObject.gameObject, _poolParentTransform);
            go.GetComponent<PoolObject>().InitializePoolObject(_poolModel);
            _poolObjectQueue.Enqueue(go.GetComponent<PoolObject>());
        }

        private void CreateObjectToPool(PoolModel pool, int size)
        {
            Debug.Log($"Creating {size} item into {pool.Name} pool");
            for (int i = 0; i < size; i++)
            {
                CreateObjectToPool(pool);
            }
        }
    }
}
