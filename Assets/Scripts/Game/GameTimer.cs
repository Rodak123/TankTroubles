using System;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class GameTimer : MonoBehaviour
{
    [SerializeField] private float countdownDuration = 3;
    [SerializeField] private float gameDuration = 60;
    [SerializeField] private float endingDuration = 5;

    private bool dontStartGame;
    private bool isOvertime;
    private bool dontEnd;

    [SerializeField, ReadOnly] private float timer = 0;

    private GameStateManager.GameState gameState;
    private GameStateManager gameStateManager;

    public event EventHandler OnOvertimeStarted;

    private void Awake()
    {
        gameStateManager = GameManager.Instance.GetComponent<GameStateManager>();

        gameStateManager.OnGameStateChanged += OnGameStateChanged;
    }

    public void OnGameStateChanged(object sender, GameStateManager.GameState gameState)
    {
        timer = 0;
        this.gameState = gameState;
        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
                dontStartGame = true;
                gameStateManager.DontStartGameVotes++;
                break;
            case GameStateManager.GameState.Ending:
                dontEnd = true;
                gameStateManager.DontEndGameVotes++;
                break;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
                if (dontStartGame && timer >= countdownDuration)
                {
                    dontStartGame = false;
                    gameStateManager.DontStartGameVotes--;
                }
                break;
            case GameStateManager.GameState.Running:
                if (!isOvertime && timer >= gameDuration)
                {
                    isOvertime = true;
                    timer = 0;
                    OnOvertimeStarted?.Invoke(this, EventArgs.Empty);
                }
                break;
            case GameStateManager.GameState.Ending:
                isOvertime = false;
                if (dontEnd && timer >= endingDuration)
                {
                    dontEnd = false;
                    gameStateManager.DontEndGameVotes--;
                }
                break;
        }
    }

    public bool IsOvertime() => isOvertime;

    public float GetTime()
    {
        return gameState switch
        {
            GameStateManager.GameState.Starting => countdownDuration - Mathf.Clamp(timer, 0, countdownDuration),
            GameStateManager.GameState.Running => isOvertime ? -2 : (gameDuration - Mathf.Clamp(timer, 0, gameDuration)),
            GameStateManager.GameState.Ending => endingDuration - Mathf.Clamp(timer, 0, endingDuration),
            _ => -1
        };
    }

}
