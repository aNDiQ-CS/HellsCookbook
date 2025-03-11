using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class LevelGenerator : MonoBehaviour
{
    [Header("�������")]
    public GameObject[] roomPrefabs;
    public GameObject[] corridorPrefabs;
    public GameObject startRoomPrefab;
    public GameObject endRoomPrefab;

    [Header("���������")]
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
        // ��������� �������
        lastRoom = Instantiate(startRoomPrefab, Vector3.zero, Quaternion.identity);
        roomsGenerated = 1;

        // �������� �������
        int totalRooms = Random.Range(minRooms, maxRooms + 1);
        while (roomsGenerated < totalRooms)
        {
            GenerateNextRoom();
        }

        // ��������� �������
        GenerateEndRoom();
    }

    void GenerateNextRoom()
    {
        // ����� ���������� �������
        GameObject roomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];

        // ����������������
        Vector3 newPos = GetNextRoomPosition();
        Quaternion rotation = GetRandomRotation();

        // �������� �������
        GameObject newRoom = Instantiate(roomPrefab, newPos, rotation);

        // ���������� � ���������� ��������
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
        // ����� ���������� ��������
        GameObject corridorPrefab = corridorPrefabs[Random.Range(0, corridorPrefabs.Length)];

        // ������� ����� ���������
        Vector3 corridorPos = (roomA.transform.position + roomB.transform.position) / 2;

        // ����������
        Vector3 dir = (roomB.transform.position - roomA.transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(dir);

        // �������� ��������
        Instantiate(corridorPrefab, corridorPos, rotation);
    }

    void GenerateEndRoom()
    {
        Vector3 endPos = GetNextRoomPosition();
        Instantiate(endRoomPrefab, endPos, GetRandomRotation());
    }
}