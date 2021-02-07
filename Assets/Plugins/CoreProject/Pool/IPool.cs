using UnityEngine;

namespace CoreProject.Pool
{
    public interface IPool
    {
        PoolObject GetPoolObject(string poolName, Transform parent = null);
        void SetPoolObject(PoolObject poolObject);
    }
}
