using System;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class SpawnPositionsGenerator : MonoBehaviour
{
    public record SpawnPosition
    {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public SpawnPosition(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }

    private MapGenerator mapGenerator;

    private int width, height;
    private int centerX, centerY;

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    public SpawnPosition[] RandomGenerateFor(int playerCount)
    {
        SpawnPosition[][] spawnPositions = GenerateFor(playerCount);
        return spawnPositions[Mathf.FloorToInt(UnityEngine.Random.Range(0, spawnPositions.Length))];
    }

    public SpawnPosition[][] GenerateFor(int playerCount)
    {
        width = mapGenerator.MapSize.x;
        height = mapGenerator.MapSize.y;
        centerX = width / 2;
        centerY = height / 2;

        return playerCount switch
        {
            1 => new SpawnPosition[][] {
                    new SpawnPosition[]{
                            GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, centerY))),
                        },
                },
            2 => new SpawnPosition[][] {
                    new SpawnPosition[]{
                            GetSpawnPosition(mapGenerator.GetCellPosition(new(0, centerY))),
                            GetSpawnPosition(mapGenerator.GetCellPosition(new(width - 1, centerY)))
                        },
                    new SpawnPosition[]{
                            GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, 0))),
                            GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, height - 1)))
                        }
                },
            3 => new SpawnPosition[][] {
                new SpawnPosition[]{
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(0, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(width - 1, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, 0)))
                    },
                new SpawnPosition[]{
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(0, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(width - 1, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, height - 1)))
                    }
            },
            4 => new SpawnPosition[][] {
                new SpawnPosition[]{
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(0, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(width - 1, centerY))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, 0))),
                        GetSpawnPosition(mapGenerator.GetCellPosition(new(centerX, height - 1)))
                    },
            },
            _ => throw new ArgumentException($"Player count {playerCount} not supported.")
        };
    }

    private SpawnPosition GetSpawnPosition(Vector3 position)
    {
        Vector3 target = mapGenerator.GetCellPosition(new(centerX, centerY));

        Vector3 direction = target - position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        return new(position, Quaternion.Euler(0, 0, angle));
    }
}
