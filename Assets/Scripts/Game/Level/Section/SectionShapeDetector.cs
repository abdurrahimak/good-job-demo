using System.Collections.Generic;
using System.Linq;
using GJGDemo.Data;
using GJGDemo.Game;
using UnityEngine;

public class SectionShapeDetector
{

    private Section _section;
    private Grid _grid;
    private Dictionary<Vector3Int, Cube> _cubesByCellPos;

    public SectionShapeDetector(Grid grid, Section section, Dictionary<Vector3Int, Cube> cubesByCellPos)
    {
        _cubesByCellPos = cubesByCellPos;
        _section = section;
        _grid = grid;
    }

    public List<Shape> FindShapes()
    {
        List<Shape> shapes = new List<Shape>();
        foreach (var cubesByCellPosKvp in _cubesByCellPos)
        {
            if (shapes.Find(s => s.Cubes.Contains(cubesByCellPosKvp.Value)) != null)
            {
                continue;
            }

            if (cubesByCellPosKvp.Value.CubeType == CubeType.Empty)
            {
                Shape shape = new Shape();
                FindShape(shape, cubesByCellPosKvp.Value);
                shapes.Add(shape);
            }
        }
        return shapes;
    }

    Vector3Int[] moveableNeighbors = new Vector3Int[]{
        Vector3Int.left,
        Vector3Int.right,
        new Vector3Int(0, 0, 1), // forward
        new Vector3Int(0, 0, -1) // backward
    };

    private void FindShape(Shape shape, Cube cube)
    {
        if (cube.CubeType == CubeType.Empty && !shape.Cubes.Contains(cube))
        {
            shape.Cubes.Add(cube);
            foreach (var neighbor in moveableNeighbors)
            {
                Vector3Int neighborPos = cube.CellPosition + neighbor;
                if (IsAvaliable(neighborPos))
                {
                    FindShape(shape, _cubesByCellPos[neighborPos]);
                }
                else
                {
                    shape.HasWallNeighbor = true;
                }
            }
        }
        else
        {
            return;
        }
    }

    private bool IsAvaliable(Vector3Int cellPos) => _cubesByCellPos.ContainsKey(cellPos);
}
