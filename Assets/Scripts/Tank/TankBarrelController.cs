using UnityEngine;

[RequireComponent(typeof(Tank), typeof(TankDamage))]
public class TankBarrelController : MonoBehaviour
{
    private Tank tank;
    private TankDamage damage;

    [SerializeField] private Transform barrel;
    [SerializeField] private Transform bulletSpawn;

    [SerializeField] private GameObject bulletPrefab;

    private void Awake()
    {
        tank = GetComponent<Tank>();
        damage = GetComponent<TankDamage>();
    }

    private void Update()
    {
        if (tank.Input.GetPrimaryAction())
            FireBullet();
    }

    private void FireBullet()
    {
        if (damage.IsTankDestroyed())
            return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.name = $"{tank.GetSettings().Name} bullet";

        bullet.GetComponent<TankBullet>().shooter = tank;
    }
}
