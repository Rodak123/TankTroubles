using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TankBarrelMuzzleFlash : MonoBehaviour
{
    [SerializeField] private TankBarrelController tankBarrelController;
    [SerializeField, Range(0.001f, 10)] private float flashDuration = 0.1f;
    [SerializeField] private Sprite[] flashSprites;
    private SpriteRenderer spriteRenderer;

    private Sprite RandomFlashSprite => flashSprites[Random.Range(0, flashSprites.Length)];

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        tankBarrelController.OnBulletFired += (_, _) =>
        {
            StartCoroutine(ShowMuzzle());
        };
    }

    private IEnumerator ShowMuzzle()
    {
        spriteRenderer.sprite = RandomFlashSprite;
        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.enabled = false;
    }
}