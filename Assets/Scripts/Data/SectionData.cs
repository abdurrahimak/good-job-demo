using System;
using System.Collections.Generic;
using UnityEngine;

namespace GJGDemo.Data
{
    [Serializable]
    public class SectionData
    {
        public List<CubeData> CubeDatas;
        public List<BarrierData> BarrierDatas;

        public SectionData()
        {
            CubeDatas = new List<CubeData>();
            BarrierDatas = new List<BarrierData>();
        }

        public Vector2Int GetSize()
        {
            Vector2Int size = Vector2Int.zero;
            foreach (var cubeData in CubeDatas)
            {
                if (cubeData.CellPosition.x > size.x)
                {
                    size.x = cubeData.CellPosition.x;
                }

                if (cubeData.CellPosition.z > size.y)
                {
                    size.y = cubeData.CellPosition.z;
                }
            }
            size += Vector2Int.one;
            return size;
        }
    }
}
