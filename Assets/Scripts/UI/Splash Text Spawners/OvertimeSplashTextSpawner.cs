using System;
using UnityEngine;

public class OvertimeSplashTextSpawner : MonoBehaviour
{
    [SerializeField] private SplashTextSettings overtimeSettings;

    private void Awake()
    {
        GameManager.Instance.GetComponent<GameTimer>().OnOvertimeStarted += (object sender, EventArgs args) =>
        {
            SplashTextManager.Instance.ShowSplashText(overtimeSettings);
        };
    }
}
