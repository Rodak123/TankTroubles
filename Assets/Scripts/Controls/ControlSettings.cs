using UnityEngine;

[CreateAssetMenu(fileName = "ControlSettings", menuName = "ScriptableObjects/CreateControlSettings", order = 1)]
public class ControlSettings : ScriptableObject
{
    [Header("Axis Controls")]
    public KeyCode Up = KeyCode.W;
    public KeyCode Down = KeyCode.S;
    public KeyCode Left = KeyCode.A;
    public KeyCode Right = KeyCode.D;

    [Header("Action Controls")]
    public KeyCode PrimaryAction = KeyCode.X;
    public KeyCode SecondaryAction = KeyCode.C;
}
