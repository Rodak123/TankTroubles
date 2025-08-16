using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WallType", menuName = "WallType", order = 0)]
public class WallType : ScriptableObject
{
    [Serializable]
    public enum BlockType
    {
        All,
        Tanks,
        Bullets,
    }

    [SerializeField] public BlockType Block = BlockType.All;
    [SerializeField] public GameObject Prefab;
}