using UnityEngine;

namespace GJGDemo.Extensions
{
    public static class ExtensionMethods
    {
        public static Vector3 GetWorlPositionForCell(this Transform transform)
        {
            Vector3 pos = transform.position;
            pos.y = 0f;
            pos.z += 0.5f;
            pos.x += 0.5f;
            return pos;
        }
    }
}
