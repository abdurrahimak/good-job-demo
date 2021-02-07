using UnityEngine;
using CoreProject.Singleton;

namespace GJGDemo.Game
{
    public class GridManager : SingletonComponent<GridManager>
    {
        private Grid _grid;
        public Grid Grid
        {
            get
            {
                if (_grid == null)
                {
                    _grid = GetComponent<Grid>();
                    if (_grid == null)
                    {
                        _grid = gameObject.AddComponent<Grid>();
                        _grid.cellSize = new Vector3(1f, 0.5f, 1f);
                        _grid.cellGap = Vector3.zero;
                        _grid.cellLayout = GridLayout.CellLayout.Rectangle;
                        _grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
                        transform.position = Vector3.zero;
                    }
                }
                return _grid;
            }
        }

        private void Start()
        {
            foreach (var item in transform.GetComponentsInChildren<Cube>())
            {
                Destroy(item.gameObject);
            }
            foreach (var item in transform.GetComponentsInChildren<BarrierCube>())
            {
                Destroy(item.gameObject);
            }
        }
    }
}
