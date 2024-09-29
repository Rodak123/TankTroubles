using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class TankBullet : MonoBehaviour
{
    private static readonly List<TankBullet> AllTankBullets = new();

    public static void DestroyAllBullets()
    {
        foreach (TankBullet bullet in AllTankBullets)
            bullet.DestroySelf();
    }

    public Tank shooter;

    private Rigidbody2D rb2D;

    [SerializeField] private float speed = 5;
    [SerializeField] private float lifespan = 15;
    [SerializeField] private float spawnProtectionDuration = 0.05f;
    private bool spawnProtection = true;
    private float lifespanLeft;

    public event EventHandler OnFinished;

    private void Awake()
    {
        AllTankBullets.Add(this);
    }

    private void Start()
    {
        if (shooter == null) throw new ArgumentException($"{nameof(shooter)} can't be null");

        GetComponent<SpriteRenderer>().sprite = shooter.GetSettings().BulletSprite;
        rb2D = GetComponent<Rigidbody2D>();

        lifespanLeft = lifespan;

        rb2D.velocity = transform.up * speed;
    }

    private void Update()
    {
        lifespanLeft -= Time.deltaTime;
        if (lifespanLeft <= 0)
            DestroySelf();

        if (spawnProtection && (lifespan - lifespanLeft) > spawnProtectionDuration)
            spawnProtection = false;
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.forward, rb2D.velocity);
        rb2D.velocity = rb2D.velocity.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Tank tank = collision.gameObject.GetComponentInParent<Tank>();

        if (!tank)
        {
            spawnProtection = false;
            return;
        }
        else if (tank.GetTeam() != null && tank.GetTeam().IsSameAs(shooter.GetTeam()) && spawnProtection)
            return;

        IBulletCollisionResolver resolver = collision.gameObject.GetComponentInParent<IBulletCollisionResolver>();
        if (resolver == null)
            return;

        resolver.ResolveCollision(this, collision, out bool destroyBullet);

        if (destroyBullet)
            DestroySelf();
    }

    public override string ToString()
    {
        return $"{typeof(TankBullet)}[shooter: {shooter}]";
    }

    public float GetLeftLifespanT() => 1f - lifespanLeft / lifespan;

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        AllTankBullets.Remove(this);
        OnFinished?.Invoke(this, EventArgs.Empty);
    }
}
