#if UNITY_EDITOR
using CoreProject.Pool;
using UnityEditor;
using UnityEngine;

namespace CoreProject.Editor.Pool
{
    public class PoolEditor : UnityEditor.Editor
    {
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            PoolData poolData = Resources.Load<PoolData>("PoolData");
            if(poolData == null)
            {
                poolData = ScriptableObject.CreateInstance<PoolData>();
                poolData.Pools = new System.Collections.Generic.List<PoolModel>(); 
                AssetDatabase.CreateAsset(poolData, "Assets/Resources/PoolData.asset");
            }
        }
    }
}
#endif
