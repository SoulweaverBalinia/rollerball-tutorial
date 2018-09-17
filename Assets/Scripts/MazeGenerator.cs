using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { path, wall };

public class MazeGenerator : GenericSingletonClass<MazeGenerator>
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    private enum Algorithm { Prims };

    [SerializeField]
    private Algorithm alg = Algorithm.Prims;

    public Transform brick;
    private List<Vector3> pathMazes = new List<Vector3>();

    public override void Awake()
    {
        base.Awake();
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        MazeAlg mazeAlgorithm;

        switch (alg)
        {
            case Algorithm.Prims:
                mazeAlgorithm = new RandomizedPrims(width, height);
                mazeAlgorithm.MakeMaze();
                break;
            default:
                mazeAlgorithm = new RandomizedPrims(width, height);
                mazeAlgorithm.MakeMaze();
                break;
        }

        for (int i = 0; i <= mazeAlgorithm.Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mazeAlgorithm.Maze.GetUpperBound(1); j++)
            {
                switch (mazeAlgorithm.Maze[i, j])
                {
                    case TileType.wall:
                        Instantiate(brick, new Vector3(i * brick.localScale.x, 0, j * brick.localScale.z), Quaternion.identity);
                        break;
                    case TileType.path:
                        pathMazes.Add(new Vector3(i, 0, j));
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
