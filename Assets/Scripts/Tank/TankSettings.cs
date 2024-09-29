using UnityEngine;

[CreateAssetMenu(fileName = "TankSettings", menuName = "ScriptableObjects/CreateTankSettings", order = 1)]
public class TankSettings : ScriptableObject
{
    public string Name;

    [Header("Tank")]
    public Sprite BodySprite;
    public Sprite RightTrackSprite;
    public Sprite LeftTrackSprite;

    [Header("Barrel")]
    public TankBarrelSettings TankBarrelSettings;
    public Sprite BarrelSprite;
    public Sprite BulletSprite;
}
