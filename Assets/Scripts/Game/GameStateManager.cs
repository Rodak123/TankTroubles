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

    [SerializeField] private float beforeWinDuration = 1f;
    [SerializeField, ReadOnly] private float beforeWinTimer = 0;

    [SerializeField] private KeyCode newGameKeyCode = KeyCode.Space;

    public event EventHandler<GameState> OnGameStateChanged;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

        gameManager.OnNewGameStarted += OnNewGameStarted;
    }

    private void OnNewGameStarted(object sender, EventArgs args)
    {
        ChangeGameState(GameState.Starting);
    }

    private void OnGameFullyStarted(object sender, EventArgs args)
    {
        ChangeGameState(GameState.Running);
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
            case GameState.Ending:
                EndingUpdate();
                break;
            case GameState.Ended:
                EndedUpdate();
                break;
        }
    }


    private void EndingUpdate()
    {
        beforeWinTimer += Time.deltaTime;
        if (beforeWinTimer >= beforeWinDuration)
            ChangeGameState(GameState.Ended);
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
                gameManager.GetTankManager().OnTankStateCreated += OnTankStateChanged;
                gameManager.GetTankManager().OnTankStateUpdated += OnTankStateChanged;
                gameManager.GetMapGenerator().OnMapGenerated += OnGameFullyStarted;
                break;
            case GameState.Ending:
                beforeWinTimer = 0;
                break;
        }
        OnGameStateChanged?.Invoke(this, gameState);
    }

    public GameState GetGameState() => gameState;
}
