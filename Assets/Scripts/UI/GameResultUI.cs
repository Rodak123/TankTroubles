using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GameResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    private GameObject uiGameObject;

    private GameScoreManager gameScoreManager;

    private void Awake()
    {
        uiGameObject = transform.GetChild(0).gameObject;

        GameStateManager stateManager = GameManager.Instance.GetComponent<GameStateManager>() ?? throw new ArgumentException($"No {typeof(GameStateManager)} instance was found");
        stateManager.OnGameStateChanged += OnGameStateChanged;

        gameScoreManager = GameManager.Instance.GetComponent<GameScoreManager>() ?? throw new ArgumentException($"No {typeof(GameScoreManager)} instance was found");
        gameScoreManager.OnScoreEvaluated += OnScoreEvaluated;

        uiGameObject.SetActive(false);
    }

    private void OnGameStateChanged(object sender, GameStateManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
                uiGameObject.SetActive(false);
                break;
        }
    }

    private void OnScoreEvaluated(object sender, EventArgs args)
    {
        uiGameObject.SetActive(true);
        resultText.text = GetResultText();
    }

    private string GetResultText()
    {
        string space = "  ";
        StringBuilder resultText = new();
        Dictionary<TankTeam, List<Tank>> teamTanks = GameManager.Instance.GetTankManager().GetTanksState().TeamTanks;
        foreach (KeyValuePair<TankTeam, List<Tank>> tankTeam in teamTanks)
        {
            resultText.AppendLine($"{tankTeam.Key}: ");
            foreach (Tank tank in tankTeam.Value)
            {
                int winCount = gameScoreManager.WinCount[tank.GetSettings()];
                int deathCount = gameScoreManager.DeathCount[tank.GetSettings()];
                resultText.Append(space).AppendLine($"{tank} w: {winCount}, d: {deathCount}");
            }
        }
        return resultText.ToString();
    }

}
