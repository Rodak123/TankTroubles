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

    private void Awake()
    {
        if (wallPrefab == null)
            throw new ArgumentException($"{nameof(wallPrefab)} can't be null");

        mazeGenerator = GetComponent<IMazeGenerator>();
    }

    public void GenerateMap()
    {
        Bounds bounds = Camera.main.OrthographicBounds();
        Vector2 cameraSize = bounds.max - bounds.min;

        MapSize = Vector2Int.FloorToInt(
            cameraSize / cellSize
        );

        IEnumerator<Maze> mazeEnumerator = mazeGenerator.EnumerateGenerateMaze(MapSize.x, MapSize.y);

        if (animator != null)
            Destroy(animator.gameObject);

        GameObject lastGeneratedMaze = new("Maze", new Type[]{
            typeof(MapGenerationAnimator)
        });
        animator = lastGeneratedMaze.GetComponent<MapGenerationAnimator>();
        animator.Setup(cellSize, MapSize, stepDelay, wallSize, wallPrefab);
        animator.AnimateMazeGeneration(mazeEnumerator);

    }

    public Vector3 GetCellPosition(Vector2Int cell) => animator?.GetCellPosition(cell) ?? throw new NullReferenceException($"No maze is generated");
    public MapGenerationAnimator GetMapGenerationAnimator() => animator;
}
