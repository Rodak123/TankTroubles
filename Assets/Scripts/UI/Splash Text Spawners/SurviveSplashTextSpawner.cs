using UnityEngine;

public class SurviveSplashTextSpawner : MonoBehaviour
{
    [SerializeField] private SplashTextSettings surviveSettings;

    private void Awake()
    {
        GameManager.Instance.GetComponent<GameStateManager>().OnGameStateChanged += (object sender, GameStateManager.GameState gameState) =>
        {
            if (gameState != GameStateManager.GameState.Ending) return;
            SplashTextManager.Instance.ShowSplashText(surviveSettings);
        };
    }
}
