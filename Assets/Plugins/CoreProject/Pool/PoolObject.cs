using UnityEngine;

namespace CoreProject.Pool
{
    public class PoolObject : MonoBehaviour
    {
        private PoolModel _pool;
        public PoolModel Pool => _pool;

        public void InitializePoolObject(PoolModel pool)
        {
            _pool = pool;
        }

        public virtual void FromPool() { }
        public virtual void ToPool() { }
    }
}
