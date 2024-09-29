using System;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        Starting,
        Running,
        Ending,
        Ended
    }

    private GameManager gameManager;

    [SerializeField, ReadOnly] private GameState gameState;

    [SerializeField] private KeyCode newGameKeyCode = KeyCode.Space;

    public event EventHandler<GameState> OnGameStateChanged;

    public int DontStartGameVotes = 0;
    public int DontEndGameVotes = 0;

    private bool shouldStart;
    private bool shouldEnd;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

        gameManager.OnNewGameStarted += OnNewGameStarted;

        gameManager.GetTankManager().OnTankStateCreated += OnTankStateChanged;
        gameManager.GetTankManager().OnTankStateUpdated += OnTankStateChanged;
        gameManager.GetMapGenerator().OnMapGenerated += (object sender, EventArgs args) => DontStartGameVotes--;
    }

    private void OnNewGameStarted(object sender, EventArgs args)
    {
        ChangeGameState(GameState.Starting);
    }

    private void OnTankStateChanged(object sender, TankManager.TanksState tanksState)
    {
        if (gameState == GameState.Ended) return;

        if (tanksState.AliveTanks.Count == 1)
            ChangeGameState(GameState.Ending);
        else if (tanksState.AliveTanks.Count == 0)
            ChangeGameState(GameState.Ended);
    }

    private void Update()
    {
        switch (gameState)
        {
            case GameState.Starting:
                if (shouldStart && DontStartGameVotes <= 0)
                    ChangeGameState(GameState.Running);
                break;
            case GameState.Ending:
                if (shouldEnd && DontEndGameVotes <= 0)
                    ChangeGameState(GameState.Ended);
                break;
            case GameState.Ended:
                EndedUpdate();
                break;
        }
    }

    private void EndedUpdate()
    {
        if (Input.GetKeyDown(newGameKeyCode))
            gameManager.StartNewGame();
    }

    private void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
        switch (gameState)
        {
            case GameState.Starting:
                DontStartGameVotes = 0;
                shouldStart = true;

                DontStartGameVotes++;
                break;
            case GameState.Ending:
                DontEndGameVotes = 0;
                shouldEnd = true;
                break;
        }
        OnGameStateChanged?.Invoke(this, gameState);
    }

    public GameState GetGameState() => gameState;
}
