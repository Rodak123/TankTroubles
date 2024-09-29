using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField, Range(0.01f, 10f)] private float cellSize = 1f;
    [SerializeField, Range(0.01f, 1f)] private float wallSize = 0.2f;
    [SerializeField] private float stepDelay = 0.005f;
    [SerializeField] private GameObject wallPrefab;

    private IMazeGenerator mazeGenerator;

    public Vector2Int MapSize { get; private set; }

    private MapGenerationAnimator animator;
    public event EventHandler OnMapGenerated;

    private Bounds mapBounds;
    private Vector3 offset;

    private readonly List<GameObject> innerWalls = new();

    private void Awake()
    {
        if (wallPrefab == null)
            throw new ArgumentException($"{nameof(wallPrefab)} can't be null");

        mazeGenerator = GetComponent<IMazeGenerator>();
    }

    public void SetMapBounds(Bounds bounds)
    {
        mapBounds = bounds;

        Vector2 gameAreaSize = bounds.size;

        MapSize = Vector2Int.FloorToInt(
            gameAreaSize / cellSize
        );
        offset = transform.position - (Vector3)((Vector2)MapSize * cellSize / 2) + Vector3.one * cellSize / 2;
    }

    public void GenerateMap()
    {
        MapSize = new(
            Mathf.Max(2, MapSize.x),
            Mathf.Max(2, MapSize.y)
        );

        IEnumerator<Maze> mazeEnumerator = mazeGenerator.EnumerateGenerateMaze(MapSize.x, MapSize.y);

        animator?.DestroySelf();

        GameObject lastGeneratedMaze = new("Maze", new Type[]{
            typeof(MapGenerationAnimator)
        });
        animator = lastGeneratedMaze.GetComponent<MapGenerationAnimator>();
        animator.AnimateMazeGeneration(mazeEnumerator, stepDelay, GenererateMazeWalls, () =>
        {
            OnMapGenerated.Invoke(this, EventArgs.Empty);
        });
        lastGeneratedMaze.transform.parent = transform;
    }

    private GameObject GenererateMazeWalls(Maze maze)
    {
        innerWalls.Clear();
        GameObject mazeWalls = new("Maze Walls");

        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                Vector3 position = GetCellPosition(new(x, y));

                if (maze.IsWallPresent(x, y, Maze.Side.Top))
                {
                    GameObject wall = GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Top));
                    if (y != 0) innerWalls.Add(wall);
                }

                if (maze.IsWallPresent(x, y, Maze.Side.Left))
                {
                    GameObject wall = GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Left));
                    if (x != 0) innerWalls.Add(wall);
                }

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
        wallGameObject.transform.localScale = new Vector3(cellSize + size, size, wallScale.z);

        if (isVertical)
            wallGameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
        return wallGameObject;
    }

    public Vector3 GetCellPosition(Vector2Int cell)
    {
        Vector3 position = mapBounds.center + offset + (Vector3)((Vector2)cell * cellSize);
        position.z = 0;
        return position;
    }

    public MapGenerationAnimator GetMapGenerationAnimator() => animator;
    public List<GameObject> GetInnerWalls() => innerWalls;
}
