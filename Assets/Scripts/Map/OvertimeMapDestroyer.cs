using System;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
public class OvertimeMapDestroyer : MonoBehaviour
{
    private void Awake()
    {
        MapGenerator mapGenerator = GetComponent<MapGenerator>();
        GameManager.Instance.GetComponent<GameTimer>().OnOvertimeStarted += (object sender, EventArgs args) =>
        {
            foreach (GameObject wall in mapGenerator.GetInnerWalls())
                Destroy(wall);
        };
    }

}
