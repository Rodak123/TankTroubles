using System.Collections.Generic;
using UnityEngine;

public class TankDisplaysManager : MonoBehaviour
{
    [SerializeField] private TankDisplayUI[] tankDisplays;

    private void Awake()
    {
        TankManager tankManager = GameManager.Instance.GetTankManager();

        tankManager.OnTankStateCreated += OnTankStateCreated;
        tankManager.OnTankStateUpdated += OnTankStateUpdated;
    }

    private void OnTankStateCreated(object sender, TankManager.TanksState tanksState)
    {
        int tankDisplayIndex = 0;
        foreach (List<Tank> tanks in tanksState.TeamTanks.Values)
            foreach (Tank tank in tanks)
                tankDisplays[tankDisplayIndex++].SetTank(tank);

        for (; tankDisplayIndex < tankDisplays.Length; tankDisplayIndex++)
            tankDisplays[tankDisplayIndex].SetTank(null);
    }

    private void OnTankStateUpdated(object sender, TankManager.TanksState tanksState)
    {
        foreach (TankDisplayUI tankDisplay in tankDisplays)
            tankDisplay.UpdateUI();
    }
}
