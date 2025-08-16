using System;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TankTracks : MonoBehaviour
{
    [Serializable]
    public enum Side
    {
        Right,
        Left
    }

    [SerializeField] private TankDamage tankDamage;
    [SerializeField] private Side side;

    private TrailRenderer trailRenderer;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        tankDamage.OnTankDestroyed += (_, _) => DisableTrail();
        if (side == Side.Left) tankDamage.OnLeftTrackDestroyed += (_, _) => DisableTrail();
        if (side == Side.Right) tankDamage.OnRightTrackDestroyed += (_, _) => DisableTrail();
    }

    private void DisableTrail()
    {
        trailRenderer.emitting = false;
    }
}
