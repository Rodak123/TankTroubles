
using UnityEngine;

public interface IBulletCollisionResolver
{
    public void ResolveCollision(TankBullet bullet, Collision2D collision, out bool destroyBullet);
}