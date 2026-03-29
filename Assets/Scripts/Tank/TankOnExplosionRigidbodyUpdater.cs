using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TankBulletCollisionResolver))]
public class TankOnExplosionRigidbodyUpdater : MonoBehaviour
{
    private void Awake()
    {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        TankBulletCollisionResolver collisionResolver = GetComponent<TankBulletCollisionResolver>();

        collisionResolver.OnTankExplosionStarts += (object sender, EventArgs args) =>
        {
            rb2d.linearDamping = 2;
        };

        collisionResolver.OnTankExplosionEnds += (object sender, EventArgs args) =>
        {
            rb2d.angularDamping = 10000f;
            rb2d.mass *= 0.5f;
        };
    }
}
