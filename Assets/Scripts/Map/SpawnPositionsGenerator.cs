using System;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class SpawnPositionsGenerator : MonoBehaviour
{

    private MapGenerator mapGenerator;

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    public Vector3[] RandomGenerateFor(int playerCount)
    {
        Vector3[][] spawnPositions = GenerateFor(playerCount);
        return spawnPositions[Mathf.FloorToInt(UnityEngine.Random.Range(0, spawnPositions.Length))];
    }

    public Vector3[][] GenerateFor(int playerCount)
    {
        int width = mapGenerator.MapSize.x;
        int height = mapGenerator.MapSize.y;
        int centerX = width / 2;
        int centerY = height / 2;
        return playerCount switch
        {
            2 => new Vector3[][] {
                    new Vector3[]{ mapGenerator.GetCellPosition(new(0, centerY)), mapGenerator.GetCellPosition(new(width - 1, centerY)) },
                    new Vector3[]{ mapGenerator.GetCellPosition(new(centerX, 0)), mapGenerator.GetCellPosition(new(centerX, height - 1)) }
                },
            3 => new Vector3[][] {
                new Vector3[]{ mapGenerator.GetCellPosition(new(0, centerY)), mapGenerator.GetCellPosition(new(width - 1, centerY)), mapGenerator.GetCellPosition(new(centerX, 0)) },
                new Vector3[]{ mapGenerator.GetCellPosition(new(0, centerY)), mapGenerator.GetCellPosition(new(width - 1, centerY)), mapGenerator.GetCellPosition(new(centerX, height - 1)) }
            },
            4 => new Vector3[][] {
                new Vector3[]{ mapGenerator.GetCellPosition(new(0, centerY)), mapGenerator.GetCellPosition(new(width - 1, centerY)), mapGenerator.GetCellPosition(new(centerX, 0)), mapGenerator.GetCellPosition(new(centerX, height - 1)) },
            },
            _ => throw new ArgumentException($"Player count {playerCount} not supported.")
        };
    }
}
