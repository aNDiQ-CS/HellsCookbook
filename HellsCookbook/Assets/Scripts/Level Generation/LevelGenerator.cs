using UnityEngine;
using System.Collections.Generic;

public class AdvancedLevelGenerator : MonoBehaviour
{
    [System.Serializable]
    public class RoomSet
    {
        public GameObject[] roomPrefabs;
        public GameObject[] corridorPrefabs;
    }

    [Header("Настройки генерации")]
    public int gridSize = 50;
    public int maxRooms = 150;
    public float tileSize = 64f;
    public float corridorLength = 16f;

    [Header("Префабы")]
    public RoomSet roomSet;
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;

    private Dictionary<Vector2Int, GameObject> spawnedRooms = new Dictionary<Vector2Int, GameObject>();
    private Queue<Vector2Int> generationQueue = new Queue<Vector2Int>();
    private Vector2Int farthestRoom;

    void Start()
    {
        GenerateProceduralDungeon();
    }

    void GenerateProceduralDungeon()
    {
        InitializeStartingRoom();
        GenerateMainPath();
        FindFarthestRoom();
        PlaceBossRoom();
        ConnectAllRooms();
    }

    void InitializeStartingRoom()
    {
        Vector2Int startPos = new Vector2Int(gridSize / 2, gridSize / 2);
        GameObject startRoom = InstantiateRoom(startRoomPrefab, startPos);
        spawnedRooms.Add(startPos, startRoom);
        generationQueue.Enqueue(startPos);
    }

    void GenerateMainPath()
    {
        while (generationQueue.Count > 0 && spawnedRooms.Count < maxRooms)
        {
            Vector2Int currentPos = generationQueue.Dequeue();

            foreach (var direction in GetRandomDirections())
            {
                Vector2Int newPos = currentPos + direction;

                if (IsPositionValid(newPos))
                {
                    GameObject newRoom = CreateRandomRoom(newPos);
                    ConnectRooms(currentPos, newPos, direction);
                    generationQueue.Enqueue(newPos);
                }
            }
        }
    }

    GameObject CreateRandomRoom(Vector2Int gridPos)
    {
        GameObject prefab = roomSet.roomPrefabs[Random.Range(0, roomSet.roomPrefabs.Length)];
        GameObject room = InstantiateRoom(prefab, gridPos);
        spawnedRooms.Add(gridPos, room);
        return room;
    }

    void ConnectRooms(Vector2Int from, Vector2Int to, Vector2Int direction)
    {
        Vector3 corridorPosition = CalculateCorridorPosition(from, to);
        Quaternion rotation = GetCorridorRotation(direction);

        GameObject corridorPrefab = roomSet.corridorPrefabs[
            Random.Range(0, roomSet.corridorPrefabs.Length)];

        Instantiate(corridorPrefab, corridorPosition, rotation, transform);
    }

    Vector3 CalculateCorridorPosition(Vector2Int from, Vector2Int to)
    {
        Vector3 start = GridToWorld(from);
        Vector3 end = GridToWorld(to);
        return Vector3.Lerp(start, end, 0.5f);
    }

    Quaternion GetCorridorRotation(Vector2Int direction)
    {
        return direction.x != 0 ?
            Quaternion.Euler(0, 90, 0) :
            Quaternion.identity;
    }

    void FindFarthestRoom()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> checkQueue = new Queue<Vector2Int>();
        checkQueue.Enqueue(new Vector2Int(gridSize / 2, gridSize / 2));

        int maxDistance = 0;

        while (checkQueue.Count > 0)
        {
            Vector2Int current = checkQueue.Dequeue();
            visited.Add(current);

            foreach (var dir in GetFourDirections())
            {
                Vector2Int neighbor = current + dir;
                if (spawnedRooms.ContainsKey(neighbor) && !visited.Contains(neighbor))
                {
                    int distance = CalculateGridDistance(neighbor);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestRoom = neighbor;
                    }
                    checkQueue.Enqueue(neighbor);
                }
            }
        }
    }

    void PlaceBossRoom()
    {
        if (spawnedRooms.TryGetValue(farthestRoom, out GameObject oldRoom))
        {
            Destroy(oldRoom);
            spawnedRooms[farthestRoom] = InstantiateRoom(bossRoomPrefab, farthestRoom);
        }
    }

    void ConnectAllRooms()
    {
        foreach (var pos in spawnedRooms.Keys)
        {
            foreach (var dir in GetFourDirections())
            {
                Vector2Int neighbor = pos + dir;
                if (spawnedRooms.ContainsKey(neighbor) && !IsConnected(pos, neighbor))
                {
                    ConnectRooms(pos, neighbor, dir);
                }
            }
        }
    }

    bool IsConnected(Vector2Int a, Vector2Int b)
    {
        return Physics.CheckBox(
            CalculateCorridorPosition(a, b),
            new Vector3(corridorLength / 2, 1, corridorLength / 2)
        );
    }

    #region Helper Methods
    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(
            (gridPos.x - gridSize / 2) * tileSize,
            0,
            (gridPos.y - gridSize / 2) * tileSize
        );
    }

    GameObject InstantiateRoom(GameObject prefab, Vector2Int gridPos)
    {
        return Instantiate(prefab, GridToWorld(gridPos), Quaternion.identity, transform);
    }

    List<Vector2Int> GetRandomDirections()
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        // Fisher-Yates shuffle
        for (int i = directions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = directions[i];
            directions[i] = directions[j];
            directions[j] = temp;
        }

        return directions;
    }

    bool IsPositionValid(Vector2Int gridPos)
    {
        return !spawnedRooms.ContainsKey(gridPos) &&
               gridPos.x >= 0 && gridPos.x < gridSize &&
               gridPos.y >= 0 && gridPos.y < gridSize;
    }

    int CalculateGridDistance(Vector2Int pos)
    {
        Vector2Int center = new Vector2Int(gridSize / 2, gridSize / 2);
        return Mathf.Abs(pos.x - center.x) + Mathf.Abs(pos.y - center.y);
    }

    Vector2Int[] GetFourDirections()
    {
        return new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };
    }
    #endregion
}