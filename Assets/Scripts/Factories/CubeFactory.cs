using CoreProject.Resource;
using CoreProject.Singleton;
using GJGDemo.Data;
using GJGDemo.Game;
using UnityEngine;

namespace GJGDemo.Factory
{
    public class CubeFactory : SingletonClass<CubeFactory>
    {
        public Cube CreateCube(Vector3Int cellPos, CubeType cubeType, Transform parent = null)
        {
            Cube cube = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("Cube"), parent).GetComponent<Cube>();
            cube.CellPosition = cellPos;
            cube.ChangeCubeType(cubeType);
            return cube;
        }

        public BarrierCube CreateBarrierCube(Vector3Int startCellPosition, Vector3Int endCellPosition, Transform parent = null)
        {
            BarrierCube barrierCube = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("BarrierCube"), parent).GetComponent<BarrierCube>();
            barrierCube.StartCellPosition = startCellPosition;
            barrierCube.EndCellPosition = endCellPosition;
            return barrierCube;
        }

        public PlayerCube CreatePlayerCube(Transform parent = null)
        {
            PlayerCube playerCube = GameObject.Instantiate(ResourceManager.Instance.GetResource<GameObject>("PlayerCube"), parent).GetComponent<PlayerCube>();
            return playerCube;
        }
    }
}