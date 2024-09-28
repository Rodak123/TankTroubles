using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private KeyCode resetKeyCode = KeyCode.R;

    [Space(10)]
    [SerializeField] private TankManager tankManager;
    [SerializeField] private MapGenerator mapGenerator;

    public event EventHandler OnNewGameStarted;

    private void Awake()
    {
        if (tankManager == null)
            throw new ArgumentException($"{nameof(tankManager)} can't be null");

        if (mapGenerator == null)
            throw new ArgumentException($"{nameof(mapGenerator)} can't be null");
    }

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        mapGenerator.GenerateMap();
        tankManager.SpawnTanks();

        OnNewGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (Input.GetKeyDown(resetKeyCode))
            StartNewGame();
    }

    public TankManager GetTankManager() => tankManager;
    public MapGenerator GetMapGenerator() => mapGenerator;
}
