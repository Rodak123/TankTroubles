using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TankManager : MonoBehaviour
{
    public class TanksState : EventArgs
    {
        public readonly List<Tank> AliveTanks = new();
        public readonly Dictionary<TankTeam, List<Tank>> TeamTanks = new();

        public TanksState(Dictionary<TankTeam, List<Tank>> teamTanks, List<Tank> aliveTanks)
        {
            TeamTanks = teamTanks;
            AliveTanks = aliveTanks;
        }
    }

    [SerializeField] private TankSettings[] tankSettings;
    [SerializeField] private ControlSettings[] controls;
    [SerializeField] private TankTeam[] teams;

    [SerializeField] private SpawnPositionsGenerator spawnPositionsGenerator;

    [SerializeField] private GameObject tankPrefab;

    public event EventHandler<TanksState> OnTankStateCreated;
    public event EventHandler<TanksState> OnTankStateUpdated;

    private readonly List<Tank> aliveTanks = new();
    private readonly Dictionary<TankTeam, List<Tank>> teamTanks = new();

    private void Awake()
    {
        Setup(tankSettings, controls, teams);

        if (GameManager.Instance.gameObject.TryGetComponent(out GameStateManager stateManager))
            stateManager.OnGameStateChanged += OnGameStateChanged;
    }

    public void Setup(TankSettings[] tankSettings, ControlSettings[] controls, TankTeam[] teams)
    {
        if (tankSettings.Length != controls.Length)
            throw new ArgumentException($"{nameof(controls)} must have the same number of items as {nameof(tankSettings)}");

        if (tankSettings.Length != teams.Length)
            throw new ArgumentException($"{nameof(teams)} must have the same number of items as {nameof(tankSettings)}");

        this.tankSettings = tankSettings;
        this.controls = controls;
        this.teams = teams;
    }

    private void OnGameStateChanged(object sender, GameStateManager.GameState gameState)
    {
        void SetTanksActive(bool value)
        {
            foreach (List<Tank> tanks in teamTanks.Values)
                foreach (Tank tank in tanks)
                    tank.SetIsActive(value);
        }

        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
            case GameStateManager.GameState.Ended:
                TankBullet.DestroyAllBullets();
                SetTanksActive(false);
                break;
            case GameStateManager.GameState.Running:
                SetTanksActive(true);
                break;
        }
    }

    public void SpawnTanks()
    {
        foreach (List<Tank> tanks in teamTanks.Values)
            foreach (Tank tank in tanks)
                Destroy(tank.gameObject);
        aliveTanks.Clear();
        teamTanks.Clear();

        SpawnPositionsGenerator.SpawnPosition[] spawnPositions = spawnPositionsGenerator.RandomGenerateFor(tankSettings.Length).ToList().Shuffle().ToArray();

        for (int i = 0; i < tankSettings.Length; i++)
        {
            GameObject tankObject = Instantiate(tankPrefab, spawnPositions[i].Position, Quaternion.Euler(spawnPositions[i].Rotation.eulerAngles + new Vector3(0, 0, -90f)), transform);

            TankTeam team = teams[i];

            Tank tank = tankObject.GetComponent<Tank>();
            tank.SetSettings(tankSettings[i]);
            tank.SetTeam(team);

            tankObject.GetComponent<ControlSettingsInput>().SetControls(controls[i]);

            if (tankObject.TryGetComponent(out TankDamage damage))
                damage.OnTankDestroyed += OnTankDestroyed;

            aliveTanks.Add(tank);
            teamTanks.Add(team, new List<Tank>() { tank });
        }

        OnTankStateCreated?.Invoke(this, GetTanksState());
    }

    private void OnTankDestroyed(object sender, TankDamage.TankDamageState damageState)
    {
        Tank tank = ((TankDamage)sender).gameObject.GetComponent<Tank>();
        aliveTanks.Remove(tank);
        OnTankStateUpdated?.Invoke(this, GetTanksState());
    }

    public TanksState GetTanksState() => new(teamTanks, aliveTanks);
}
