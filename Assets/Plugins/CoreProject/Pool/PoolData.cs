using System.Collections.Generic;
using UnityEngine;

namespace CoreProject.Pool
{
    [CreateAssetMenu(fileName = "PoolData.asset", menuName = "CoreProject / Create Pool Data")]
    public class PoolData : ScriptableObject
    {
        [SerializeField] public List<PoolModel> Pools;
    }
}
