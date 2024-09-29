using System;
using System.Collections;
using UnityEngine;

public class CountdownSplashTextSpawner : MonoBehaviour
{
    [SerializeField] private SplashTextSettings countdownSettings;
    [SerializeField] private SplashTextSettings startSettings;

    private GameTimer timer;
    private GameStateManager stateManager;

    private void Awake()
    {
        timer = GameManager.Instance.GetComponent<GameTimer>() ?? throw new ArgumentException($"No {typeof(GameTimer)} instance was found");
        stateManager = GameManager.Instance.GetComponent<GameStateManager>() ?? throw new ArgumentException($"No {typeof(GameStateManager)} instance was found");

        stateManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(object sender, GameStateManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameStateManager.GameState.Starting:
                StartCoroutine(CountdownTimer());
                break;
            case GameStateManager.GameState.Running:
                SplashTextManager.Instance.ShowSplashText(startSettings);
                break;
        }
    }

    private IEnumerator CountdownTimer()
    {
        int time;

        do
        {
            time = Mathf.CeilToInt(timer.GetTime());
            if (time == 0) break;
            countdownSettings.Text = time.ToString();
            SplashTextManager.Instance.ShowSplashText(countdownSettings);
            yield return new WaitForSeconds(1);
        }
        while (stateManager.GetGameState() == GameStateManager.GameState.Starting);
    }
}

