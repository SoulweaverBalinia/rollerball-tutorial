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

    private MazeAlg mazeAlgorithm;
    public Transform brick;
    private TileType[,] Maze;

    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> tileStack = new Stack<Vector2>();

    public override void Awake()
    {
        base.Awake();
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        Maze = new TileType[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = TileType.wall;
            }
        }

        mazeAlgorithm = new RandomizedPrims(width, height, ref tileStack);
        mazeAlgorithm.MakeMaze(ref Maze);

        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(1); j++)
            {
                switch (Maze[i, j])
                {
                    case TileType.wall:
                        Instantiate(brick, new Vector3(i * brick.localScale.x, 0, j * brick.localScale.z), Quaternion.identity);
                        break;
                    case TileType.path:
                        pathMazes.Add(new Vector3(i, 0, j));
                        break;
                    default:
                        throw new ArgumentException("Invalid tile type.");
                }
            }
        }
    }

}
