using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager), typeof(GameStateManager))]
public class GameScoreManager : MonoBehaviour
{
    public readonly Dictionary<TankSettings, int> WinCount = new();
    public readonly Dictionary<TankSettings, int> DeathCount = new();

    private TankManager tankManager;

    public event EventHandler OnScoreEvaluated;

    private void Awake()
    {
        GameStateManager stateManager = GameManager.Instance.GetComponent<GameStateManager>();
        stateManager.OnGameStateChanged += OnGameStateChanged;

        tankManager = GameManager.Instance.GetTankManager();
    }

    private void OnGameStateChanged(object sender, GameStateManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
                OnGameStarting();
                break;
            case GameStateManager.GameState.Ended:
                OnGameEnded();
                break;
        }
    }

    private void OnGameStarting()
    {
        Dictionary<TankTeam, List<Tank>> teamTanks = tankManager.GetTanksState().TeamTanks;
        List<Tank> allTanks = new();
        foreach (List<Tank> tanks in teamTanks.Values)
            allTanks.AddRange(tanks);

        if (WinCount.Count == allTanks.Count)
            return;

        WinCount.Clear();
        DeathCount.Clear();

        foreach (Tank tank in allTanks)
        {
            WinCount.Add(tank.GetSettings(), 0);
            DeathCount.Add(tank.GetSettings(), 0);
        }
    }

    private void OnGameEnded()
    {
        Dictionary<TankTeam, List<Tank>> teamTanks = tankManager.GetTanksState().TeamTanks;
        List<Tank> aliveTanks = tankManager.GetTanksState().AliveTanks;
        List<Tank> deadTanks = new();
        foreach (List<Tank> tanks in teamTanks.Values)
            foreach (Tank tank in tanks)
                if (!aliveTanks.Contains(tank))
                    deadTanks.Add(tank);

        aliveTanks.ForEach(tank => WinCount[tank.GetSettings()]++);
        deadTanks.ForEach(tank => DeathCount[tank.GetSettings()]++);

        OnScoreEvaluated?.Invoke(this, EventArgs.Empty);
    }
}
