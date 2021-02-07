using UnityEngine;
using UnityEditor;
using GJGDemo.Game;

namespace GJGDemo.Editor
{
    [CustomEditor(typeof(BarrierCube))]
    [CanEditMultipleObjects]
    public class BarrierCubeEditor : UnityEditor.Editor
    {
        Object[] _barrierCubes;
        private void OnEnable()
        {
            _barrierCubes = serializedObject.targetObjects;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (GUILayout.Button("Set Start Cell Position"))
            {
                foreach (var barrierCube in _barrierCubes)
                {
                    (barrierCube as BarrierCube).StartCellPosition = (barrierCube as BarrierCube).GetCurrentCellPos;
                }
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Set End Cell Position"))
            {
                foreach (var barrierCube in _barrierCubes)
                {
                    (barrierCube as BarrierCube).EndCellPosition = (barrierCube as BarrierCube).GetCurrentCellPos;
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}