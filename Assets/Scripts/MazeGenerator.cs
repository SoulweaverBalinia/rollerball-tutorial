using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private int width;

    [SerializeField]
    private int height;

    private enum TileType { path, wall };
    public Transform brick;
    private TileType[,] Maze;


    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> tileStack = new Stack<Vector2>();

    private System.Random rnd = new System.Random();

    private Vector2 currentTile;
    public Vector2 CurrentTile
    {
        get { return currentTile; }

        private set
        {
            if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
            {
                throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
            }
            else if (value.x % 2 == 1 || value.y % 2 == 1)
            {
                currentTile = value;
            }
            else
            {
                throw new ArgumentException("The current square must not be both on an even X-axis and an even Y-axis, to ensure we can get walls around all tunnels");
            }
        }
    }

    private static MazeGenerator instance;
    public static MazeGenerator Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
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

        CurrentTile = Vector2.one;
        tileStack.Push(CurrentTile);
        RandomMazeGenerator();

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
                //if (Maze[i, j] == TileType.wall)
                //{
                //    Instantiate(brick, new Vector3(i * brick.localScale.x, 0, j * brick.localScale.z), Quaternion.identity);
                //}
                //else if (Maze[i, j] == TileType.path)
                //{
                //    pathMazes.Add(new Vector3(i, 0, j));
                //}
            }
        }
    }

    // based on Prim's Algorithm
    // does not use weights to inform generation
    // instead randomly chooses neighbors
    private void RandomMazeGenerator()
    {
        List<Vector2> neighbors;
        List<Vector2> offsets = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0)
        };
        while (tileStack.Count > 0)
        {
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = TileType.path;

            neighbors = GetValidNeighbors(CurrentTile, offsets);

            if (neighbors.Count > 0)
            {
                tileStack.Push(CurrentTile);
                CurrentTile = neighbors[rnd.Next(neighbors.Count)];
            }
            else
            {
                CurrentTile = tileStack.Pop();
            }
        }
    }

    /// <summary>
    /// Get all the prospective neighboring tiles
    /// </summary>
    /// <param name="centerTile">The tile to test</param>
    /// <returns>Any and all valid neighbors</returns>
    private List<Vector2> GetValidNeighbors(Vector2 centerTile, List<Vector2> offsets)
    {
        List<Vector2> validNeighbors = new List<Vector2>();

        foreach (Vector2 offset in offsets)
        {
            Vector2 toCheck = centerTile + offset;

            if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
            {
                if (Maze[(int)toCheck.x, (int)toCheck.y] == TileType.wall && HasThreeWallsIntact(toCheck, offsets))
                {
                    validNeighbors.Add(toCheck);
                }
            }
        }

        return validNeighbors;
    }

    private bool HasThreeWallsIntact(Vector2 Vector2ToCheck, List<Vector2> offsets)
    {
        int intactWallCounter = 0;

        foreach (Vector2 offset in offsets)
        {
            Vector2 neighborToCheck = Vector2ToCheck + offset;

            if (IsInsideMaze(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == TileType.wall)
            {
                intactWallCounter++;
            }
        }

        return intactWallCounter == 3;
    }

    private bool IsInsideMaze(Vector2 p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }
}
