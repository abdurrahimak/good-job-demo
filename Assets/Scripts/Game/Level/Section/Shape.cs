using System.Collections.Generic;
using GJGDemo.Game;

public class Shape
{
    public List<Cube> Cubes;
    public bool HasWallNeighbor;
    public Shape()
    {
        HasWallNeighbor = false;
        Cubes = new List<Cube>();
    }
}
