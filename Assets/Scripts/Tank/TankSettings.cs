using UnityEngine;

[CreateAssetMenu(fileName = "TankSettings", menuName = "ScriptableObjects/CreateTankSettings", order = 1)]
public class TankSettings : ScriptableObject
{
    public string Name;

    [Header("Tank")]
    public Sprite bodySprite;
    public Sprite rightTrackSprite;
    public Sprite leftTrackSprite;

    [Header("Barrel")]
    public Sprite barrelSprite;
    public Sprite bulletSprite;
}
