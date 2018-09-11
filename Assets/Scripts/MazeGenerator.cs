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

    public Transform brick;
    private int[,] Maze;

    private enum TileType { path, wall };
    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> tileStack = new Stack<Vector2>();

    private readonly List<Vector2> offsets = new List<Vector2>
    {
        new Vector2(0, 1),
        new Vector2(0, -1),
        new Vector2(1, 0),
        new Vector2(-1, 0)
    };

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
        Maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = (int)TileType.wall;
            }
        }

        CurrentTile = Vector2.one;
        tileStack.Push(CurrentTile);
        PrimsAlg();

        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(1); j++)
            {
                if (Maze[i, j] == (int)TileType.wall)
                {
                    Instantiate(brick, new Vector3(i * brick.localScale.x, 0, j * brick.localScale.z), Quaternion.identity);
                }
                else if (Maze[i, j] == (int)TileType.path)
                {
                    pathMazes.Add(new Vector3(i, 0, j));
                }
            }
        }
    }

    private void PrimsAlg()
    {
        List<Vector2> neighbors;
        while (tileStack.Count > 0)
        {
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = (int)TileType.path;

            neighbors = GetValidNeighbors(CurrentTile);

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
    private List<Vector2> GetValidNeighbors(Vector2 centerTile)
    {
        List<Vector2> validNeighbors = new List<Vector2>();

        foreach (Vector2 offset in offsets)
        {
            Vector2 toCheck = centerTile + offset;

            if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
            {
                if (Maze[(int)toCheck.x, (int)toCheck.y] == 1 && HasThreeWallsIntact(toCheck))
                {
                    validNeighbors.Add(toCheck);
                }
            }
        }

        return validNeighbors;
    }

    private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
    {
        int intactWallCounter = 0;

        foreach (Vector2 offset in offsets)
        {
            Vector2 neighborToCheck = Vector2ToCheck + offset;

            if (IsInsideMaze(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == (int)TileType.wall)
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
