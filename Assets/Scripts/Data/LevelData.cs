using System;
using System.Collections.Generic;

namespace GJGDemo.Data
{
    [Serializable]
    public class LevelData
    {
        public List<SectionData> Sections;

        public LevelData()
        {
            Sections = new List<SectionData>();
        }
    }
}
