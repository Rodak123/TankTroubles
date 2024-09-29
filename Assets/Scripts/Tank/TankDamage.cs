using System;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class TankDamage : MonoBehaviour
{
    public class TankDamageState
    {
        public readonly float BodyDamage;
        public readonly float LeftTrackDamage;
        public readonly float RightTrackDamage;

        public TankDamageState(float bodyDamage, float leftTrackDamage, float rightTrackDamage)
        {
            BodyDamage = bodyDamage;
            LeftTrackDamage = leftTrackDamage;
            RightTrackDamage = rightTrackDamage;
        }
    }

    [SerializeField] private float leftTrackMaxHealth = 2;
    [SerializeField] private float rightTrackMaxHealth = 2;
    [SerializeField] private float bodyMaxHealth = 1;

    private float leftTrackHealth;
    private float rightTrackHealth;
    private float bodyHealth;

    private bool tankIsDestroyed = false;

    public event EventHandler<TankDamageState> OnTankDestroyed;

    public event EventHandler<TankDamageState> OnRightTrackDestroyed;
    public event EventHandler<TankDamageState> OnLeftTrackDestroyed;


    public event EventHandler<TankDamageState> OnBodyDamaged;
    public event EventHandler<TankDamageState> OnRightTrackDamaged;
    public event EventHandler<TankDamageState> OnLeftTrackDamaged;

    private void Awake()
    {
        leftTrackHealth = leftTrackMaxHealth;
        rightTrackHealth = rightTrackMaxHealth;
        bodyHealth = bodyMaxHealth;
    }

    public float GetBodyDamage() => IsTankDestroyed() ? 1 : ResolveDamage(bodyHealth, bodyMaxHealth);

    public float GetRightTrackDamage() => IsTankDestroyed() ? 1 : ResolveDamage(rightTrackHealth, rightTrackMaxHealth);
    public float GetLeftTrackDamage() => IsTankDestroyed() ? 1 : ResolveDamage(leftTrackHealth, leftTrackMaxHealth);

    private float ResolveDamage(float health, float maxHealth)
    {
        if (maxHealth == 0) return 1f;
        return 1f - Mathf.Clamp01(health / maxHealth);
    }

    public bool IsTankDestroyed() => tankIsDestroyed;
    public bool AreTracksDestroyed() => IsLeftTrackDestroyed() && IsRightTrackDestroyed();

    public bool IsLeftTrackDestroyed() => GetLeftTrackDamage() >= 1;
    public bool IsRightTrackDestroyed() => GetRightTrackDamage() >= 1;


    public void DealBodyDamage(float damage)
    {
        if (IsTankDestroyed()) return;
        bodyHealth -= damage;

        OnBodyDamaged?.Invoke(this, GetTankDamageState());

        tankIsDestroyed = bodyHealth <= 0;

        if (IsTankDestroyed()) OnTankDestroyed?.Invoke(this, GetTankDamageState());
    }

    public void DealLeftTrackDamage(float damage)
    {
        if (IsLeftTrackDestroyed()) return;
        leftTrackHealth -= damage;

        OnLeftTrackDamaged?.Invoke(this, GetTankDamageState());
        if (IsLeftTrackDestroyed()) OnLeftTrackDestroyed?.Invoke(this, GetTankDamageState());
    }

    public void DealRightTrackDamage(float damage)
    {
        if (IsRightTrackDestroyed()) return;
        rightTrackHealth -= damage;

        OnRightTrackDamaged?.Invoke(this, GetTankDamageState());
        if (IsRightTrackDestroyed()) OnRightTrackDestroyed?.Invoke(this, GetTankDamageState());
    }

    public TankDamageState GetTankDamageState() => new(
            GetBodyDamage(),
            GetLeftTrackDamage(),
            GetRightTrackDamage()
        );

}
