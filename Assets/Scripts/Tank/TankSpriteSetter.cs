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

        tankBodySpriteRenderer.sprite = settings.BodySprite;
        tankRightTrackSpriteRenderer.sprite = settings.RightTrackSprite;
        tankLeftTrackSpriteRenderer.sprite = settings.LeftTrackSprite;

        barrelSpriteRenderer.sprite = settings.BarrelSprite;

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
