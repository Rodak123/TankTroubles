using System;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class TankSpriteSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer tankBodySpriteRenderer;
    [SerializeField] private SpriteRenderer tankRightTrackSpriteRenderer;
    [SerializeField] private SpriteRenderer tankLeftTrackSpriteRenderer;
    [SerializeField] private SpriteRenderer barrelSpriteRenderer;

    private void Start()
    {
        TankSettings settings = GetComponent<Tank>().GetSettings();

        tankBodySpriteRenderer.sprite = settings.bodySprite;
        tankRightTrackSpriteRenderer.sprite = settings.rightTrackSprite;
        tankLeftTrackSpriteRenderer.sprite = settings.leftTrackSprite;

        barrelSpriteRenderer.sprite = settings.barrelSprite;

        if (TryGetComponent(out TankBulletCollisionResolver collisionResolver))
        {
            collisionResolver.OnTankExplosionStarts += (object sender, EventArgs args) =>
            {
                tankBodySpriteRenderer.sortingOrder = -5;
                barrelSpriteRenderer.sortingOrder = -4;

            };
            collisionResolver.OnTankExplosionEnds += (object sender, EventArgs args) =>
            {
                barrelSpriteRenderer.enabled = false;
            };
        }
    }
}
