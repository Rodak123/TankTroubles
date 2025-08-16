using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField, Range(0.01f, 10f)] private float cellSize = 1f;
    [SerializeField, Range(0.01f, 1f)] private float wallSize = 0.2f;
    [SerializeField] private float stepDelay = 0.005f;
    [SerializeField] private WallType[] wallTypes;

    [SerializeField] private string TankBlockLayer = "TankBlock";
    [SerializeField] private string BulletBlockLayer = "BulletBlock";

    private IMazeGenerator mazeGenerator;

    private MapGenerationAnimator animator;

    private Bounds mapBounds;
    private Vector3 offset;

    private readonly List<GameObject> innerWalls = new();

    private WallType RandomWallType => wallTypes[Random.Range(0, wallTypes.Length)];

    public Vector2Int MapSize { get; private set; }
    public event EventHandler OnMapGenerated;

    private void Awake()
    {
        if (wallTypes.Length == 0)
            throw new ArgumentException($"{nameof(wallTypes)} can't be empty");

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

    private WallType GetWallType(Maze maze, int x, int y, Maze.Side side)
    {
        return maze.IsEdge(x, y, side) ? wallTypes[0] : RandomWallType;
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
                    WallType wallType = GetWallType(maze, x, y, Maze.Side.Top);
                    GameObject wall = GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Top), wallType);
                    if (y != 0) innerWalls.Add(wall);
                }

                if (maze.IsWallPresent(x, y, Maze.Side.Left))
                {
                    WallType wallType = GetWallType(maze, x, y, Maze.Side.Left);
                    GameObject wall = GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Left), wallType);
                    if (x != 0) innerWalls.Add(wall);
                }

                if (x == maze.Width - 1 && maze.IsWallPresent(x, y, Maze.Side.Right))
                {
                    WallType wallType = GetWallType(maze, x, y, Maze.Side.Right);
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Right), wallType);
                }

                if (y == maze.Height - 1 && maze.IsWallPresent(x, y, Maze.Side.Bottom))
                {
                    WallType wallType = GetWallType(maze, x, y, Maze.Side.Bottom);
                    GenerateWall(mazeWalls.transform, position, wallSize, cellSize, Maze.GetSideDirection(Maze.Side.Bottom), wallType);
                }
            }
        }

        return mazeWalls;
    }

    private GameObject GenerateWall(Transform parent, Vector3 centerPosition, float size, float cellSize, Vector2 direction, WallType wallType)
    {
        bool isVertical = direction.x != 0;

        Vector3 position = centerPosition + (Vector3)direction * cellSize / 2;

        GameObject wallGameObject = Instantiate(wallType.Prefab, position, Quaternion.identity, parent);
        wallGameObject.name = $"Wall[{direction.x}, {direction.y}]";

        Vector3 wallScale = wallGameObject.transform.localScale;
        wallGameObject.transform.localScale = new Vector3(cellSize + size, cellSize + size, wallScale.z);

        if (isVertical)
            wallGameObject.transform.rotation = Quaternion.Euler(0, 0, 90);

        if (wallType.Block != WallType.BlockType.All)
        {
            Collider2D[] colliders = wallGameObject.GetComponentsInChildren<Collider2D>();
            int layer = 0;
            switch (wallType.Block)
            {
                case WallType.BlockType.Bullets:
                    layer = LayerMask.NameToLayer(BulletBlockLayer);
                    break;
                case WallType.BlockType.Tanks:
                    layer = LayerMask.NameToLayer(TankBlockLayer);
                    break;
            }
            foreach (Collider2D collider in colliders)
                collider.gameObject.layer = layer;
        }

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
