using UnityEngine;

public record Maze
{
    public enum Side
    {
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    public static Vector2Int GetSideDirection(Side side)
    {
        return side switch
        {
            Side.Top => new(0, -1),
            Side.Bottom => new(0, 1),
            Side.Left => new(-1, 0),
            Side.Right => new(1, 0),
            _ => throw new System.ArgumentException("This side is not supported"),
        };
    }

    public static Side GetOppositeSide(Side side)
    {
        return side switch
        {
            Side.Top => Side.Bottom,
            Side.Bottom => Side.Top,
            Side.Left => Side.Right,
            Side.Right => Side.Left,
            _ => throw new System.ArgumentException("This side is not supported"),
        };
    }

    private readonly int[,] grid;

    public int Width => grid.GetLength(0);
    public int Height => grid.GetLength(1);

    public Maze(int[,] grid)
    {
        this.grid = grid;
    }

    private bool IsEdge(int x, int y, Side side) =>
           (x == 0 && side == Side.Left)
        || (x == grid.GetLength(0) - 1 && side == Side.Right)
        || (y == 0 && side == Side.Top)
        || (y == grid.GetLength(1) - 1 && side == Side.Bottom);

    public bool IsWallPresent(int x, int y, Side side) => IsEdge(x, y, side) || (grid[x, y] & (int)side) != 0;
}
