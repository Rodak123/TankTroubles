using System;
using UnityEngine;

public class Tank : MonoBehaviour
{
    [SerializeField, ReadOnly] private bool isActive;

    [Space(10)]
    [SerializeField] private TankSettings settings;
    [SerializeField] private TankTeam team;
    public IInput Input;


    private void Awake()
    {
        Input = GetComponent<IInput>();
    }

    private void Start()
    {
        gameObject.name = ToString();
    }

    public void SetSettings(TankSettings settings)
    {
        if (this.settings != null)
            return;
        this.settings = settings;
    }

    public TankSettings GetSettings()
    {
        if (settings == null)
            throw new ArgumentException($"{typeof(TankSettings)} where not set");
        return settings;
    }

    public void SetTeam(TankTeam team)
    {
        this.team = team;
    }

    public bool IsActive() => isActive;
    public bool SetIsActive(bool value) => isActive = value;

    public TankTeam GetTeam()
    {
        return team;
    }

    public override string ToString()
    {
        return $"{typeof(Tank)}[{settings.name ?? "None"}, team: {team.Name ?? "None"}]";
    }
}
