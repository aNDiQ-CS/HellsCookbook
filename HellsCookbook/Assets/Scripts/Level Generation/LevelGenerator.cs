using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class LevelGenerator : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject[] roomPrefabs;
    public GameObject[] corridorPrefabs;
    public GameObject startRoomPrefab;
    public GameObject endRoomPrefab;

    [Header("Настройки")]
    public int minRooms = 5;
    public int maxRooms = 10;
    public Vector2 roomSpacing = new Vector2(15, 25);

    private GameObject lastRoom;
    private int roomsGenerated;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Стартовая комната
        lastRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity);
        roomsGenerated = 1;

        // Основные комнаты
        int totalRooms = Random.Range(minRooms, maxRooms + 1);
        while (roomsGenerated < totalRooms)
        {
            GenerateNextRoom();
        }

        // Финальная комната
        GenerateEndRoom();
    }

    void GenerateNextRoom()
    {
        // Выбор случайного префаба
        GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        // Позиционирование
        Vector3 newPos = GetNextRoomPosition();
        Quaternion rotation = GetRandomRotation();

        // Создание комнаты
        GameObject newRoom = Instantiate(roomPrefab, newPos, rotation);

        // Соединение с предыдущей комнатой
        ConnectRooms(lastRoom, newRoom);

        lastRoom = newRoom;
        roomsGenerated++;
    }

    Vector3 GetNextRoomPosition()
    {
        Vector3 direction = Random.insideUnitCircle.normalized;
        return lastRoom.transform.position +
               new Vector3(direction.x, 0, direction.y) *
               Random.Range(roomSpacing.x, roomSpacing.y);
    }

    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
    }

    void ConnectRooms(GameObject roomA, GameObject roomB)
    {
        // Выбор случайного коридора
        GameObject corridorPrefab = corridorPrefabs[Random.Range(0, corridorPrefabs.Length)];

        // Позиция между комнатами
        Vector3 corridorPos = (roomA.transform.position + roomB.transform.position) / 2;

        // Ориентация
        Vector3 dir = (roomB.transform.position - roomA.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        // Создание коридора
        Instantiate(corridorPrefab, corridorPos, rotation);
    }

    void GenerateEndRoom()
    {
        Vector3 endPos = GetNextRoomPosition();
        Instantiate(endRoomPrefab, endPos, GetRandomRotation());
    }
}