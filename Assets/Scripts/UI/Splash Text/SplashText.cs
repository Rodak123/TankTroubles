using TMPro;
using UnityEngine;

public class SplashText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    SplashTextSettings settings;
    private float timer;

    public void Setup(SplashTextSettings settings)
    {
        this.settings = settings;

        gameObject.name = $"{gameObject.name}[{settings.Text}]";

        text.text = settings.Text;
        text.fontSize = settings.FontSizeCurve.Evaluate(0);
        text.color = settings.ColorGradient.Evaluate(0);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > settings.Duration)
        {
            text.fontSize = settings.FontSizeCurve.Evaluate(1);
            text.color = settings.ColorGradient.Evaluate(1);
            Destroy(gameObject);
            return;
        }

        float t = Mathf.Clamp01(timer / settings.Duration);
        text.fontSize = Mathf.Clamp01(settings.FontSizeCurve.Evaluate(t)) * settings.FontSize;
        text.color = settings.ColorGradient.Evaluate(t);
    }

}
