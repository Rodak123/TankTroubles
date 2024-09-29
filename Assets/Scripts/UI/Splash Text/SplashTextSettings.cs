using UnityEngine;

[CreateAssetMenu(fileName = "SplashText", menuName = "ScriptableObjects/CreateSplashText", order = 1)]
public class SplashTextSettings : ScriptableObject
{
    [Header("Settings")]
    public string Text = "Splash Text";
    public float FontSize = 24;
    public float Duration = 1.5f;

    [Space(10)]
    public Gradient ColorGradient;
    public AnimationCurve FontSizeCurve = AnimationCurve.Constant(0, 1, 1);
}
