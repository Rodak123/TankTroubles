using UnityEngine;

[CreateAssetMenu(fileName = "TankTeam", menuName = "ScriptableObjects/CreateTankTeam", order = 1)]
public class TankTeam : ScriptableObject
{
    public string Name = "Team";

    public bool IsSameAs(TankTeam other)
    {
        return other != null && Name.Equals(other.Name);
    }

    public override string ToString()
    {
        return $"{Name}";
    }
}
