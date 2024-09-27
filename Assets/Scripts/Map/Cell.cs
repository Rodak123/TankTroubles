using System;
using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private GameObject[] walls = new GameObject[4];

    [Space(10)]
    public bool LeftWall = true;
    public bool RightWall = true;
    public bool UpperWall = true;
    public bool LowerWall = true;

    private void Awake()
    {
        if (walls.Length != 4)
            throw new ArgumentException($"{nameof(walls)} must have exactly 4 items");

        if (walls.Contains(null))
            throw new ArgumentException($"{nameof(walls)} must not have null values");
    }

    private void Start()
    {
        bool[] wallsEnabled = new bool[]{
            LeftWall,
            RightWall,
            UpperWall,
            LowerWall
        };

        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].SetActive(wallsEnabled[i]);
        }
    }
}
