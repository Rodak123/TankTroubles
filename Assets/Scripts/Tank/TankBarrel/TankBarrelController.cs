using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Tank), typeof(TankDamage))]
public class TankBarrelController : MonoBehaviour
{
    public class TankMagazineState
    {
        public readonly int MaxAmmo;
        public readonly int UsedAmmo;

        public TankMagazineState(int maxAmmo, int usedAmmo)
        {
            MaxAmmo = maxAmmo;
            UsedAmmo = usedAmmo;
        }
    }

    private Tank tank;
    private TankDamage damage;

    [SerializeField] private Transform bulletSpawn;

    [SerializeField] private GameObject bulletPrefab;

    private readonly List<TankBullet> bullets = new();

    public event EventHandler<TankMagazineState> OnBulletFired;
    public event EventHandler<TankMagazineState> OnBulletReloaded;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        damage = GetComponent<TankDamage>();
    }

    private void Update()
    {
        if (!tank.IsActive())
            return;

        if (tank.Input.GetPrimaryAction())
            FireBullet();
    }

    public int GetMaxAmmo()
    {
        return tank.GetSettings().TankBarrelSettings.MaxAmmo;
    }

    public int GetShotAmmo()
    {
        return bullets.Count;
    }

    private bool CantFireBullet()
    {
        return GetShotAmmo() >= GetMaxAmmo();
    }

    private void FireBullet()
    {
        if (damage.IsTankDestroyed() || CantFireBullet())
            return;

        GameObject bulletObject = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bulletObject.name = $"{tank.GetSettings().Name} bullet";

        TankBullet bullet = bulletObject.GetComponent<TankBullet>();

        bullet.shooter = tank;
        bullets.Add(bullet);
        bullet.OnFinished += (object sender, EventArgs args) =>
        {
            bullets.Remove(bullet);
            OnBulletReloaded?.Invoke(this, GetTankMagazineState());
        };

        OnBulletFired?.Invoke(this, GetTankMagazineState());
    }

    public TankMagazineState GetTankMagazineState() => new(GetMaxAmmo(), GetShotAmmo());
}
