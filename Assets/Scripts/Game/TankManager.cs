using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public class TankStateArgs : EventArgs
    {
        public readonly Tank[] Tanks;
        public readonly Tank[] AliveTanks;
        public readonly TankTeam[] TankTeams;

        public TankStateArgs(Tank[] tanks, Tank[] aliveTanks, TankTeam[] tankTeams)
        {
            Tanks = tanks;
            AliveTanks = aliveTanks;
            TankTeams = tankTeams;
        }
    }

    [SerializeField] private TankSettings[] tankSettings;
    [SerializeField] private ControlSettings[] controls;
    [SerializeField] private TankTeam[] teams;

    [SerializeField] private SpawnPositionsGenerator spawnPositionsGenerator;

    [SerializeField] private GameObject tankPrefab;

    public event EventHandler<TankStateArgs> OnTankStateUpdated;

    private readonly List<Tank> tanks = new();
    private readonly List<Tank> aliveTanks = new();

    private void Awake()
    {
        if (tankSettings.Length != controls.Length)
            throw new ArgumentException($"{nameof(controls)} must have the same number of items as {nameof(tankSettings)}");

        if (tankSettings.Length != teams.Length)
            throw new ArgumentException($"{nameof(teams)} must have the same number of items as {nameof(tankSettings)}");
    }

    public void SpawnTanks()
    {
        foreach (Tank tank in tanks)
            Destroy(tank.gameObject);
        tanks.Clear();
        aliveTanks.Clear();

        Vector3[] spawnPositions = spawnPositionsGenerator.RandomGenerateFor(tankSettings.Length).ToList().Shuffle().ToArray();

        for (int i = 0; i < tankSettings.Length; i++)
        {
            GameObject tankObject = Instantiate(tankPrefab, spawnPositions[i], Quaternion.identity, transform);

            Tank tank = tankObject.GetComponent<Tank>();
            tank.SetSettings(tankSettings[i]);
            tank.SetTeam(teams[i]);

            tankObject.GetComponent<ControlSettingsInput>().SetControls(controls[i]);

            if (tankObject.TryGetComponent(out TankDamage damage))
                damage.OnTankDestroyed += OnTankDestroyed;

            tanks.Add(tank);
        }
        aliveTanks.AddRange(tanks);
    }

    private void OnTankDestroyed(object sender, Tank tank)
    {
        aliveTanks.Remove(tank);
        OnTankStateUpdated?.Invoke(this, new TankStateArgs(
            tanks.ToArray(),
            aliveTanks.ToArray(),
            teams
        ));
    }
}
