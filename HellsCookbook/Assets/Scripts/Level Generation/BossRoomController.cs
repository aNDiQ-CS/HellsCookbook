using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomController : RoomController
{
    [SerializeField] private GameObject portal;

    private void Start()
    {
        FindEnemiesInRoom();

        foreach (Enemy enemy in enemies)
        {
            Debug.Log("Я нашёл " + enemy.name);
        }

        CreateBarrier();
        barrierPrefab.SetActive(false);
        Debug.Log(enemies.Count);
        portal.SetActive(false);
    }

    override protected void DeactivateBarrier()
    {
        Debug.Log("Победили босса!");
        isActive = false;
        portal.SetActive(true);
    }
}
