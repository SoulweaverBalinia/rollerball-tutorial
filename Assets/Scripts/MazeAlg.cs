public class MazeAlg
{
    public TileType[,] Maze;
    public int width;
    public int height;

    public MazeAlg(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public virtual void MakeMaze()
    {
        Maze = new TileType[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = TileType.wall;
            }
        }
    }
}
