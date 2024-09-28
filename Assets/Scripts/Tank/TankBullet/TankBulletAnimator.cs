using UnityEngine;

[RequireComponent(typeof(TankBullet), typeof(SpriteRenderer))]
public class TankBulletAnimator : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.black;
    private Color baseColor;

    [SerializeField] private AnimationCurve flashIntervalCurve = AnimationCurve.Linear(0, 1, 1, 0.1f);
    [SerializeField, Range(0, 1)] private float flashStartLifespan = 0.66f;

    [SerializeField] private float flashDuration = 0.05f;

    private bool flashing = false;
    private float flashTimer = 0;
    private float flashIntervalTimer = 0;

    private TankBullet bullet;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        bullet = GetComponent<TankBullet>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        baseColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (!ShouldFlash()) return;

        if (flashing)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer >= flashDuration)
                EndFlash();
        }
        else
        {
            flashIntervalTimer += Time.deltaTime;
            if (flashIntervalTimer >= GetFlashInterval())
                StartFlash();
        }
    }

    private void StartFlash()
    {
        flashing = true;
        spriteRenderer.color = flashColor;

        flashTimer = 0;
    }

    private void EndFlash()
    {
        flashing = false;
        spriteRenderer.color = baseColor;

        flashIntervalTimer = 0;
    }

    private bool ShouldFlash() => bullet.GetLeftLifespanT() >= flashStartLifespan;

    private float GetFlashInterval()
    {
        if (!ShouldFlash()) return 0;
        float invervalT = (bullet.GetLeftLifespanT() - flashStartLifespan) / (1 - flashStartLifespan);
        return flashIntervalCurve.Evaluate(invervalT);
    }

}
