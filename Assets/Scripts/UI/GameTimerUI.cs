using System;
using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    private GameTimer gameTimer;
    private GameStateManager gameStateManager;

    private void Awake()
    {
        gameStateManager = GameManager.Instance.GetComponent<GameStateManager>() ?? throw new ArgumentException($"No {typeof(GameStateManager)} instance was found");
        gameTimer = GameManager.Instance.GetComponent<GameTimer>() ?? throw new ArgumentException($"No {typeof(GameTimer)} instance was found");
    }

    private void Update()
    {
        if (gameStateManager.GetGameState() == GameStateManager.GameState.Ended)
        {
            timerText.text = "Game Ended";
        }
        else if (gameTimer.IsOvertime())
        {
            timerText.text = "Overtime";
        }
        else
        {
            float time = gameTimer.GetTime();
            timerText.text = time == -1 ? "N/A" : Mathf.Round(time).ToString();
        }
    }
}
