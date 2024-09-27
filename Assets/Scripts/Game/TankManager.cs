using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    [SerializeField] private TankSettings[] tankSettings;
    [SerializeField] private ControlSettings[] controls;
    [SerializeField] private TankTeam[] teams;

    [SerializeField] private SpawnPositionsGenerator spawnPositionsGenerator;

    [SerializeField] private GameObject tankPrefab;

    private readonly List<Tank> tanks = new();

    private void Awake()
    {
        SpawnTanks();
    }

    private void SpawnTanks()
    {
        if (tankSettings.Length != controls.Length)
            throw new ArgumentException($"{nameof(controls)} must have the same number of items as {nameof(tankSettings)}");

        if (tankSettings.Length != teams.Length)
            throw new ArgumentException($"{nameof(teams)} must have the same number of items as {nameof(tankSettings)}");

        Vector3[] spawnPositions = spawnPositionsGenerator.RandomGenerateFor(tankSettings.Length).ToList().Shuffle().ToArray();

        for (int i = 0; i < tankSettings.Length; i++)
        {
            GameObject tankObject = Instantiate(tankPrefab, spawnPositions[i], Quaternion.identity, transform);

            Tank tank = tankObject.GetComponent<Tank>();
            tank.SetSettings(tankSettings[i]);
            tank.SetTeam(teams[i]);

            tankObject.GetComponent<ControlSettingsInput>().SetControls(controls[i]);

            tanks.Add(tank);
        }
    }
}
