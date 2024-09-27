using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm
public class KruskalsMazeGenerator : MonoBehaviour, IMazeGenerator
{

    private record Edge
    {
        public readonly Vector2Int Cell;
        public readonly Maze.Side Side;

        public Edge(Vector2Int cell, Maze.Side side)
        {
            Cell = cell;
            Side = side;
        }
    }

    private class Tree
    {
        private Tree parent = null;

        private Tree Root()
        {
            if (parent == null)
                return this;

            parent = parent.Root();
            return parent;
        }

        public bool IsConnected(Tree other) => Root() == other.Root();

        public void Connect(Tree other)
        {
            other.Root().parent = Root();
        }

    }

    public Maze GenerateMaze(int width, int height)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<Maze> EnumerateGenerateMaze(int width, int height)
    {
        int[,] grid = new int[width, height];
        Tree[,] sets = new Tree[width, height];
        Stack<Edge> edges = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int cell = new(x, y);

                grid[x, y] = (int)Maze.Side.Top | (int)Maze.Side.Left | (int)Maze.Side.Bottom | (int)Maze.Side.Right;
                sets[x, y] = new Tree();
                if (y > 0)
                    edges.Push(new(cell, Maze.Side.Top));
                if (x > 0)
                    edges.Push(new(cell, Maze.Side.Left));
            }
        }

        edges = new Stack<Edge>(edges.Shuffle());

        yield return new(grid);

        while (edges.Count > 0)
        {
            Edge edge = edges.Pop();

            Vector2Int cell1 = edge.Cell;
            Vector2Int cell2 = edge.Cell + Maze.GetSideDirection(edge.Side);

            Tree set1 = sets[cell1.x, cell1.y];
            Tree set2 = sets[cell2.x, cell2.y];

            if (!set1.IsConnected(set2))
            {
                set1.Connect(set2);

                grid[cell1.x, cell1.y] -= (int)edge.Side;
                grid[cell2.x, cell2.y] -= (int)Maze.GetOppositeSide(edge.Side);
            }
            yield return new(grid);
        }
    }
}
