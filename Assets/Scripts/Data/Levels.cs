using System.Collections.Generic;
using UnityEngine;

namespace GJGDemo.Data
{
    [CreateAssetMenu(fileName = "Levels.asset", menuName = "gjgdemo / Create Levels Asset")]
    public class Levels : ScriptableObject
    {
        public List<LevelData> LevelDatas;
    }
}