using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TankDamage))]
public class TankBulletCollisionResolver : MonoBehaviour, IBulletCollisionResolver
{
    private TankDamage damage;

    public event EventHandler OnTankExplosionStarts;
    public event EventHandler OnTankExplosionEnds;

    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Collider2D leftTrackCollider;
    [SerializeField] private Collider2D rightTrackCollider;

    private void Awake()
    {
        damage = GetComponent<TankDamage>();
    }

    private bool IsSameCollider(Collider2D a, Collider2D b)
    {
        return a == b;
    }

    private void DisableCollider(Collider2D collider)
    {
        collider.enabled = false;
        collider.gameObject.SetActive(false);
    }

    public void ResolveCollision(TankBullet bullet, Collision2D collision, out bool destroyBullet)
    {
        if (damage.IsTankDestroyed())
        {
            destroyBullet = false;
            return;
        }

        // Debug.Log($"{tank} got hit by {bullet} at {collision.collider.gameObject.name}");

        destroyBullet = true;

        ExplosionManager.Instance.CreateExplosionAt(collision.contacts[0].point);

        if (IsSameCollider(collision.collider, bodyCollider))
        {
            damage.DealBodyDamage(1);
        }
        else if (IsSameCollider(collision.collider, leftTrackCollider))
        {
            damage.DealLeftTrackDamage(1);
            if (damage.IsLeftTrackDestroyed())
                DisableCollider(leftTrackCollider);
        }
        else if (IsSameCollider(collision.collider, rightTrackCollider))
        {
            damage.DealRightTrackDamage(1);
            if (damage.IsRightTrackDestroyed())
                DisableCollider(rightTrackCollider);
        }

        if (damage.IsTankDestroyed())
            StartCoroutine(ExposionSequence());
    }

    private IEnumerator ExposionSequence()
    {
        DisableCollider(leftTrackCollider);
        DisableCollider(rightTrackCollider);

        OnTankExplosionStarts?.Invoke(this, EventArgs.Empty);

        float range = bodyCollider.bounds.extents.x + bodyCollider.bounds.extents.y * 1.1f;

        for (int i = 0; i < UnityEngine.Random.Range(2, 4); i++)
        {
            Vector2 position = (Vector2)transform.position + new Vector2(
                UnityEngine.Random.Range(-range, range),
                UnityEngine.Random.Range(-range, range)
            );
            ExplosionManager.Instance.CreateExplosionAt(position);
            yield return new WaitForSeconds(0.33f);
        }
        yield return new WaitForSeconds(0.05f);


        ExplosionManager.Instance.CreateExplosionAt(transform.position);

        OnTankExplosionEnds?.Invoke(this, EventArgs.Empty);
    }
}
