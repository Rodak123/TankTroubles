using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField, Range(0.01f, 10f)] private float cellSize = 1f;
    [SerializeField, Range(0.01f, 1f)] private float wallSize = 0.2f;
    [SerializeField] private float stepDelay = 0.05f;

    [Space]
    [SerializeField] private GameObject wallPrefab;

    private IMazeGenerator mazeGenerator;

    private Vector3 offset;
    public Vector2Int MapSize { get; private set; }

    private GameObject mazeWalls;

    private void Awake()
    {
        if (wallPrefab == null)
            throw new ArgumentException($"{nameof(wallPrefab)} can't be null");

        mazeGenerator = GetComponent<IMazeGenerator>();

        GenerateMap();
    }

    public void GenerateMap()
    {
        Bounds bounds = Camera.main.OrthographicBounds();
        Vector2 cameraSize = bounds.max - bounds.min;

        MapSize = Vector2Int.FloorToInt(
            cameraSize / cellSize
        );
        offset = transform.position - (Vector3)((Vector2)MapSize * cellSize / 2) + Vector3.one * cellSize / 2;

        IEnumerator<Maze> mazeEnumerator = mazeGenerator.EnumerateGenerateMaze(MapSize.x, MapSize.y);

        IEnumerator AnimateMaze()
        {
            while (mazeEnumerator.MoveNext())
            {
                if (mazeWalls != null) Destroy(mazeWalls);
                mazeWalls = GenererateMazeWalls(mazeEnumerator.Current);
                yield return new WaitForSeconds(stepDelay);
            }
        }

        StartCoroutine(AnimateMaze());
    }

    public Vector3 GetCellPosition(Vector2Int cell)
    {
        return offset + (Vector3)((Vector2)cell * cellSize);
    }

    private GameObject GenererateMazeWalls(Maze maze)
    {
        GameObject gameObject = new("Maze Walls");

        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                Vector3 position = GetCellPosition(new(x, y));

                if (maze.IsWallPresent(x, y, Maze.Side.Top))
                    GenerateWall(gameObject.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Top));

                if (maze.IsWallPresent(x, y, Maze.Side.Left))
                    GenerateWall(gameObject.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Left));

                if (x == maze.Width - 1 && maze.IsWallPresent(x, y, Maze.Side.Right))
                    GenerateWall(gameObject.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Right));

                if (y == maze.Height - 1 && maze.IsWallPresent(x, y, Maze.Side.Bottom))
                    GenerateWall(gameObject.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Bottom));
            }
        }

        return gameObject;
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


}
