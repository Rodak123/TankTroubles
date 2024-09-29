using UnityEngine;

public class SplashTextManager : MonoBehaviour
{
    public static SplashTextManager Instance { get; private set; }

    [SerializeField] private GameObject splashTextPrefab;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"{typeof(SplashTextManager)} instance exists already, aborting", this);
            return;
        }
        Instance = this;
    }

    public void ShowSplashText(SplashTextSettings settings)
    {
        GameObject splashText = Instantiate(splashTextPrefab, transform);
        splashText.GetComponent<SplashText>().Setup(settings);
    }
}
