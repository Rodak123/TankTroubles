using System;
using UnityEngine;

[RequireComponent(typeof(TankManager))]
public class TankManagerSetter : MonoBehaviour
{
    [SerializeField] private TankSettings[] tankSettings;
    [SerializeField] private ControlSettings[] controls;
    [SerializeField] private TankTeam[] teams;

    [SerializeField] private int playerCount = 2;

    private TankManager tankManager;

    private void Awake()
    {
        tankManager = GetComponent<TankManager>();

        GameManager.Instance.OnBeforeNewGameStarted += (object sender, EventArgs args) =>
        {
            SetupTankManager();
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            playerCount = 1;
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            playerCount = 2;
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            playerCount = 3;
        }
    }

    private void SetupTankManager()
    {
        playerCount = Mathf.Clamp(playerCount, 1, tankSettings.Length);

        TankSettings[] newTankSettings = new TankSettings[playerCount];
        ControlSettings[] newControls = new ControlSettings[playerCount];
        TankTeam[] newTankTeams = new TankTeam[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            newTankSettings[i] = tankSettings[i];
            newControls[i] = controls[i];
            newTankTeams[i] = teams[i];
        }

        tankManager.Setup(newTankSettings, newControls, newTankTeams);
    }

}
