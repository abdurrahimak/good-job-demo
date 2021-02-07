using System;
using UnityEngine;

namespace CoreProject.Data
{
    public class PlayerPrefsStrategy : IDataStoreStrategy
    {
        public bool Has(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public object Read(string key)
        {
            return PlayerPrefs.GetString(key, String.Empty);
        }

        public void Write(string key, object value)
        {
            PlayerPrefs.SetString(key, (string)value);
            PlayerPrefs.Save();
        }
    }
}
