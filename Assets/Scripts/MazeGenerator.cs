using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{

    public int width, height;
    public Transform Brick;
    private int[,] Maze;
    private List<Vector3> pathMazes = new List<Vector3>();
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2> { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };
    private System.Random rnd = new System.Random();
    private int _width, _height;
    private Vector2 _currentTile;
    public Vector2 CurrentTile
    {
        get
        {
            return _currentTile;
        }

        private set
        {
            if (value.x < 1 || value.x >= this.width - 1 || value.y < 1 || value.y >= this.height - 1)
            {
                throw new ArgumentException("CurrentTile must be within the one tile border all around the maze");
            }
            if (value.x % 2 == 1 || value.y % 2 == 1)
            {
                _currentTile = value;
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
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        GenerateMaze();
    }

    void GenerateMaze()
    {
        Maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = 1;
            }
        }
        CurrentTile = Vector2.one;
        _tiletoTry.Push(CurrentTile);
        Maze = CreateMaze();
        //GameObject ptype = null;

        for (int i = 0; i <= Maze.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= Maze.GetUpperBound(0); j++)
            {
                if (Maze[i, j] == 1)
                {
                    //ptype = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //ptype.transform.position = new Vector3(i * ptype.transform.localScale.x, j * ptype.transform.localScale.y, 0);
                    //if (brick != null)
                    //{
                    //    ptype.renderer.material = brick;
                    //}
                    //ptype.transform.parent = transform;

                    Instantiate(Brick, new Vector3(i, 0, j), Quaternion.identity);
                }
                else if (Maze[i, j] == 0)
                {
                    pathMazes.Add(new Vector3(i, 0, j));
                }
            }
        }
    }

    public int[,] CreateMaze()
    {
        //local variable to store neighbors to current square
        List<Vector2> neighbors;
        //as long as there are sitll tiles to try
        while (_tiletoTry.Count > 0)
        {
            //excavate the square we are on
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;

            //get all valid neighbors for the new tile
            neighbors = GetValidNeighbors(CurrentTile);

            //if there are any interesting looking neighbors
            if (neighbors.Count > 0)
            {
                //remember this tile by pushing it onto the stack
                _tiletoTry.Push(CurrentTile);
                //move on to a random of the neighboring tiles
                CurrentTile = neighbors[rnd.Next(neighbors.Count)];
            }
            else
            {
                //if there were no neighbors to try, it's a dead end
                //toss the tile out
                //(and return to a previous tile on the list to check)
                CurrentTile = _tiletoTry.Pop();
            }
        }

        return Maze;
    }

    /// <summary>
    /// Get all the prospective neighboring tiles
    /// </summary>
    /// <param name="centerTile">The tile to test</param>
    /// <returns>Any and all valid neighbors</returns>
    private List<Vector2> GetValidNeighbors(Vector2 centerTile)
    {

        List<Vector2> validNeighbors = new List<Vector2>();

        foreach (var offset in offsets)
        {
            Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);

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

        foreach (var offset in offsets)
        {
            Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

            if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
            {
                intactWallCounter++;
            }
        }

        return intactWallCounter == 3;
    }

    private bool IsInside(Vector2 p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }
}
