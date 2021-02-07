using System;
using CoreProject.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreProject.Resource;

namespace CoreProject.Pool
{
    public class PoolerManager : SingletonComponent<PoolerManager>
    {
        private Dictionary<string, IPool> _poolDictionary;
        private PoolData _poolData;
        private bool _initialized = false;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            _poolDictionary = new Dictionary<string, IPool>();
            try
            {
                _poolData = ResourceManager.Instance.GetResource<PoolData>("PoolData");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
                if (_poolData == null)
                {
                    Debug.LogError("PoolData does not found in Resources. Please create and design.");
                    _poolData = ScriptableObject.CreateInstance<PoolData>();
                    _poolData.Pools = new List<PoolModel>();
                }
            }
            foreach (var poolModel in _poolData.Pools)
            {
                Pool pool = new Pool(poolModel, transform);
                _poolDictionary.Add(poolModel.Name, pool);
            }
        }

        public PoolObject GetPoolObject(string poolName, Transform parent = null)
        {
            if (!_poolDictionary.ContainsKey(poolName))
            {
                throw new ArgumentNullException($"{poolName} does not initialized.");
            }

            return _poolDictionary[poolName].GetPoolObject(poolName, parent);
        }

        public void SetPoolObjectToPool(PoolObject poolObject)
        {
            if (poolObject == null)
            {
                Debug.LogError("Pool object is null");
                return;
            }

            if (poolObject.Pool == null)
            {
                Debug.LogError("Pool is null");
                return;
            }

            if (!_poolDictionary.ContainsKey(poolObject.Pool.Name))
            {
                throw new ArgumentNullException($"{poolObject.Pool.Name} does not initialized.");
            }

            _poolDictionary[poolObject.Pool.Name].SetPoolObject(poolObject);
        }

        public PoolObject GetPoolObjectForSeconds(string poolTag, float time, Transform parent = null)
        {
            PoolObject poolObject = GetPoolObject(poolTag, parent);
            StartCoroutine(GiveBackToPoolAfterSeconds(poolObject, time));
            return poolObject;
        }

        private IEnumerator GiveBackToPoolAfterSeconds(PoolObject go, float time)
        {
            yield return new WaitForSeconds(time);
            SetPoolObjectToPool(go);
        }
    }
}
