using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedPrims : MazeAlg {
    // based on Prim's Algorithm
    // does not use weights to inform generation
    // instead randomly chooses neighbors

    private System.Random rnd = new System.Random();
    private readonly int width;
    private readonly int height;

    private Stack<Vector2> tileStack;

    private List<Vector2> offsets = new List<Vector2>
        {
            new Vector2(0, 1),
            new Vector2(0, -1),
            new Vector2(1, 0),
            new Vector2(-1, 0)
        };

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

    public RandomizedPrims(int width, int height, ref Stack<Vector2> tileStack) : base()
    {
        this.width = width;
        this.height = height;
        this.tileStack = tileStack;
    }

    public override void MakeMaze(ref TileType[,] Maze)
    {
        base.MakeMaze(ref Maze);
        CurrentTile = Vector2.one;
        tileStack.Push(CurrentTile);

        List<Vector2> neighbors;

        while (tileStack.Count > 0)
        {
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = TileType.path;

            neighbors = GetValidNeighbors(ref Maze, CurrentTile);

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
    private List<Vector2> GetValidNeighbors(ref TileType[,] Maze, Vector2 centerTile)
    {
        List<Vector2> validNeighbors = new List<Vector2>();

        foreach (Vector2 offset in offsets)
        {
            Vector2 toCheck = centerTile + offset;

            if (toCheck.x % 2 == 1 || toCheck.y % 2 == 1)
            {
                if (Maze[(int)toCheck.x, (int)toCheck.y] == TileType.wall && HasThreeWallsIntact(ref Maze, toCheck))
                {
                    validNeighbors.Add(toCheck);
                }
            }
        }

        return validNeighbors;
    }

    private bool HasThreeWallsIntact(ref TileType[,] Maze, Vector2 Vector2ToCheck)
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
