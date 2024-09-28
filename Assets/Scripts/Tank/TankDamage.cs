using System;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class TankDamage : MonoBehaviour
{
    [SerializeField] private float leftTrackMaxHealth = 2;
    [SerializeField] private float rightTrackMaxHealth = 2;
    [SerializeField] private float bodyMaxHealth = 1;

    private float leftTrackHealth;
    private float rightTrackHealth;
    private float bodyHealth;

    private Tank tank;

    public event EventHandler<Tank> OnTankDestroyed;

    public event EventHandler<Tank> OnRightTrackDestroyed;
    public event EventHandler<Tank> OnLeftTrackDestroyed;

    private void Awake()
    {
        tank = GetComponent<Tank>();

        leftTrackHealth = leftTrackMaxHealth;
        rightTrackHealth = rightTrackMaxHealth;
        bodyHealth = bodyMaxHealth;
    }

    public float GetRightTrackDamage() => ResolveDamage(rightTrackHealth, rightTrackMaxHealth);

    public float GetLeftTrackDamage() => ResolveDamage(leftTrackHealth, leftTrackMaxHealth);

    public float GetBodyDamage() => ResolveDamage(bodyHealth, bodyMaxHealth);

    private float ResolveDamage(float health, float maxHealth)
    {
        if (maxHealth == 0) return 1f;
        return 1f - Mathf.Clamp01(health / maxHealth);
    }

    public bool IsTankDestroyed() => GetBodyDamage() >= 1;
    public bool AreTracksDestroyed() => IsLeftTrackDestroyed() && IsRightTrackDestroyed();

    public bool IsLeftTrackDestroyed() => GetLeftTrackDamage() >= 1;
    public bool IsRightTrackDestroyed() => GetRightTrackDamage() >= 1;


    public void DealBodyDamage(float damage)
    {
        if (IsTankDestroyed()) return;
        bodyHealth -= damage;

        if (IsTankDestroyed()) OnTankDestroyed?.Invoke(this, tank);
    }

    public void DealLeftTrackDamage(float damage)
    {
        if (IsLeftTrackDestroyed()) return;
        leftTrackHealth -= damage;

        if (IsLeftTrackDestroyed()) OnLeftTrackDestroyed?.Invoke(this, tank);
    }

    public void DealRightTrackDamage(float damage)
    {
        if (IsRightTrackDestroyed()) return;
        rightTrackHealth -= damage;

        if (IsRightTrackDestroyed()) OnRightTrackDestroyed?.Invoke(this, tank);
    }

}
