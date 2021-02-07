
using UnityEngine;
using System;

namespace CoreProject.Serilization
{
    public class UnityJsonSerializer<T> : ISerilizationStrategy<T>
    {
        public T Deserialize(object data)
        {
            return JsonUtility.FromJson<T>((string)data);
        }

        public object Serialize(T obj)
        {
            return JsonUtility.ToJson(obj);;
        }
    }
}