using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerationAnimator : MonoBehaviour
{
    private GameObject mazeWalls;

    private float cellSize;
    private Vector3 offset;
    private float stepDelay;
    private float wallSize;
    private GameObject wallPrefab;

    public event EventHandler OnMapGenerated;

    public void Setup(float cellSize, Vector2Int mapSize, float stepDelay, float wallSize, GameObject wallPrefab)
    {
        this.cellSize = cellSize;
        this.stepDelay = stepDelay;
        this.wallSize = wallSize;
        this.wallPrefab = wallPrefab;

        offset = transform.position - (Vector3)((Vector2)mapSize * cellSize / 2) + Vector3.one * cellSize / 2;
    }

    public void AnimateMazeGeneration(IEnumerator<Maze> mazeEnumerator)
    {
        StartCoroutine(AnimateMaze(mazeEnumerator));
    }

    private IEnumerator AnimateMaze(IEnumerator<Maze> mazeEnumerator)
    {
        while (mazeEnumerator.MoveNext())
        {
            if (mazeWalls != null) Destroy(mazeWalls);
            mazeWalls = GenererateMazeWalls(mazeEnumerator.Current);
            yield return new WaitForSeconds(stepDelay);
        }
        OnMapGenerated.Invoke(this, EventArgs.Empty);
    }

    private GameObject GenererateMazeWalls(Maze maze)
    {
        GameObject mazeWalls = new("Maze Walls");
        mazeWalls.transform.parent = transform;

        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                Vector3 position = GetCellPosition(new(x, y));

                if (maze.IsWallPresent(x, y, Maze.Side.Top))
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Top));

                if (maze.IsWallPresent(x, y, Maze.Side.Left))
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Left));

                if (x == maze.Width - 1 && maze.IsWallPresent(x, y, Maze.Side.Right))
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Right));

                if (y == maze.Height - 1 && maze.IsWallPresent(x, y, Maze.Side.Bottom))
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Bottom));
            }
        }

        return mazeWalls;
    }

    private GameObject GenerateWall(Transform parent, Vector3 centerPosition, float size, float cellSize, Vector2 direction)
    {
        bool isVertical = direction.x != 0;

        Vector3 position = centerPosition + (Vector3)direction * cellSize / 2;

        GameObject wallGameObject = Instantiate(wallPrefab, position, Quaternion.identity, parent);
        wallGameObject.name = $"Wall[{direction.x}, {direction.y}]";

        Vector3 wallScale = wallGameObject.transform.localScale;
        wallGameObject.transform.localScale = new Vector3(cellSize, size, wallScale.z);

        if (isVertical)
            wallGameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        return wallGameObject;
    }

    public Vector3 GetCellPosition(Vector2Int cell)
    {
        return offset + (Vector3)((Vector2)cell * cellSize);
    }
}
