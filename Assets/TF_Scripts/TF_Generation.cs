using UnityEngine;
using System.Collections.Generic;

public class TF_Generation : MonoBehaviour
{

    /*
    OBS: Esse codigo foi ALTAMENTE INSPIRADO no codigo de binding of isaac (so pra desencargo de consciencia)
    */

    public int setorSize = 10;
    private int[,] floorplan = new int[5, 5]; // 0 = vazio, 1 = sala
    private List<Vector2Int> queue = new List<Vector2Int>();
    private List<Vector2Int> deadEnds = new List<Vector2Int>();
    public int seed = 12037960;
    public List<GameObject> salaPrefabs;
    public GameObject playerRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject corredorPrefab;
    private Vector2Int[,] roomSizes = new Vector2Int[5, 5]; 
    private HashSet<(Vector2Int, Vector2Int)> corridorConnections = new HashSet<(Vector2Int, Vector2Int)>();

    void Start()
    {
        int level = 1;
        GenerateFloorplan(level);
        DebugFloorplan();
        var (roomA, roomB) = GetMostDistantRooms();
        RegisterCorridorConnections(roomA, roomB);
        InstantiateFloorplanTiles(roomA, roomB);
        InstantiateCorridors();
    }

    void GenerateFloorplan(int level)
    {
        int roomsNeeded = Random.Range(0, 2) + 10 + Mathf.RoundToInt(level * 3.5f);
        floorplan = new int[5, 5];
        roomSizes = new Vector2Int[5, 5];
        queue.Clear();
        deadEnds.Clear();
        Vector2Int startCell = new Vector2Int(2, 2);
        floorplan[startCell.x, startCell.y] = 1;
        queue.Add(startCell);
        int roomsPlaced = 1;
        System.Random rand = new System.Random(seed);

        while (queue.Count > 0 && roomsPlaced < roomsNeeded)
        {
            Vector2Int cell = queue[0];
            queue.RemoveAt(0);
            int added = 0;
            // embaralha as direções
            List<Vector2Int> dirs = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
            for (int i = 0; i < dirs.Count; i++)
            {
                int swapIdx = rand.Next(i, dirs.Count);
                Vector2Int temp = dirs[i];
                dirs[i] = dirs[swapIdx];
                dirs[swapIdx] = temp;
            }
            foreach (Vector2Int dir in dirs)
            {
                Vector2Int neighbor = cell + dir;
                if (neighbor.x < 0 || neighbor.x > 4 || neighbor.y < 0 || neighbor.y > 4) continue;
                if (floorplan[neighbor.x, neighbor.y] == 1) continue;
                int filledNeighbors = 0;
                foreach (Vector2Int d in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int n = neighbor + d;
                    if (n.x < 0 || n.x > 4 || n.y < 0 || n.y > 4) continue;
                    if (floorplan[n.x, n.y] == 1) filledNeighbors++;
                }
                if (filledNeighbors > 2) continue;
                if (roomsPlaced >= roomsNeeded) continue;
                if (rand.NextDouble() > 0.5) continue;
                floorplan[neighbor.x, neighbor.y] = 1;
                queue.Add(neighbor);
                roomsPlaced++;
                added++;
            }
            if (added == 0) deadEnds.Add(cell);
        }
    }

    Vector3 GetRoomCenter(int x, int y)
    {
        Vector2Int size = roomSizes[x, y];
        return new Vector3(x * setorSize + size.x / 2f, (-y) * setorSize - size.y / 2f, 0);
    }

    void InstantiateFloorplanTiles(Vector2Int roomA, Vector2Int roomB)
    {
        System.Random rand = new System.Random(seed);
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (floorplan[x, y] == 1)
                {
                    Vector3 pos = new Vector3(x * setorSize + 5, (-y) * setorSize + 5, 0);
                    GameObject prefab;
                    if (new Vector2Int(x, y) == roomA && playerRoomPrefab != null)
                        prefab = playerRoomPrefab;
                    else if (new Vector2Int(x, y) == roomB && bossRoomPrefab != null)
                        prefab = bossRoomPrefab;
                    else if (salaPrefabs != null && salaPrefabs.Count > 0)
                        prefab = salaPrefabs[rand.Next(salaPrefabs.Count)]; 
                    else
                        continue;
                    GameObject sala = Instantiate(prefab, pos, Quaternion.identity);
                    Vector2Int size = GetPrefabSize(sala);
                    roomSizes[x, y] = size;
                }
            }
        }
    }

    Vector2Int GetPrefabSize(GameObject sala)
    {
        var rend = sala.GetComponent<Renderer>();
        if (rend != null)
        {
            Vector3 s = rend.bounds.size;
            return new Vector2Int(Mathf.RoundToInt(s.x), Mathf.RoundToInt(s.y));
        }
        var col = sala.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Vector2 s = col.size;
            return new Vector2Int(Mathf.RoundToInt(s.x), Mathf.RoundToInt(s.y));
        }
        return new Vector2Int(setorSize, setorSize);
    }

    void DebugFloorplan()
    {
        string debug = "";
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                debug += floorplan[x, y] == 1 ? "[X]" : "[ ]";
            }
            debug += "\n";
        }
        Debug.Log(debug);
    }

    (Vector2Int, Vector2Int) GetMostDistantRooms()
    {
        
        List<Vector2Int> rooms = new List<Vector2Int>();
        for (int y = 0; y < 5; y++)
            for (int x = 0; x < 5; x++)
                if (floorplan[x, y] == 1)
                    rooms.Add(new Vector2Int(x, y));

        // BFS para cada sala, encontra o mais distante
        int maxDist = -1;
        Vector2Int roomA = Vector2Int.zero, roomB = Vector2Int.zero;
        foreach (var start in rooms)
        {
            Dictionary<Vector2Int, int> dist = new Dictionary<Vector2Int, int>();
            Queue<Vector2Int> q = new Queue<Vector2Int>();
            dist[start] = 0;
            q.Enqueue(start);
            while (q.Count > 0)
            {
                var curr = q.Dequeue();
                foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    var next = curr + dir;
                    if (next.x < 0 || next.x > 4 || next.y < 0 || next.y > 4) continue;
                    if (floorplan[next.x, next.y] != 1) continue;
                    if (dist.ContainsKey(next)) continue;
                    dist[next] = dist[curr] + 1;
                    q.Enqueue(next);
                    if (dist[next] > maxDist)
                    {
                        maxDist = dist[next];
                        roomA = start;
                        roomB = next;
                    }
                }
            }
        }
        Debug.Log($"Salas mais distantes: {roomA} <-> {roomB} (distância: {maxDist})");
        return (roomA, roomB);
    }

    void RegisterCorridorConnections(Vector2Int start, Vector2Int end)
    {
        corridorConnections.Clear();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        q.Enqueue(start);
        visited.Add(start);
        while (q.Count > 0)
        {
            var curr = q.Dequeue();
            foreach (var dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                var next = curr + dir;
                if (next.x < 0 || next.x > 4 || next.y < 0 || next.y > 4) continue;
                if (floorplan[next.x, next.y] != 1) continue;
                if (!visited.Contains(next))
                {
                    parent[next] = curr;
                    q.Enqueue(next);
                    visited.Add(next);
                }
                // registra todas as conexoes (bifurcacoes)
                if (floorplan[next.x, next.y] == 1)
                {
                    var tuple = curr.x < next.x || curr.y < next.y ? (curr, next) : (next, curr);
                    corridorConnections.Add(tuple);
                }
            }
        }
        // caminho principal de end ate start
        Vector2Int node = end;
        while (parent.ContainsKey(node))
        {
            var prev = parent[node];
            var tuple = node.x < prev.x || node.y < prev.y ? (node, prev) : (prev, node);
            corridorConnections.Add(tuple);
            node = prev;
        }
    }

    void InstantiateCorridors()
    {
        foreach (var conn in corridorConnections)
        {
            Vector3 from = GetRoomCenter(conn.Item1.x, conn.Item1.y);
            Vector3 to = GetRoomCenter(conn.Item2.x, conn.Item2.y);
            InstantiateCorridor(from, to);
        }
    }

    void InstantiateCorridor(Vector3 from, Vector3 to)
    {
        if (corredorPrefab == null) return;
        Vector3 mid = (from + to) / 2f;
        Vector3 dir = to - from;
        float length = dir.magnitude;
        Quaternion rot = Quaternion.identity;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            rot = Quaternion.Euler(0, 0, 0); 
        else
            rot = Quaternion.Euler(0, 0, 90); 
        GameObject corredor = Instantiate(corredorPrefab, mid, rot);
        corredor.transform.localScale = new Vector3(length, 1, 1); // ajusta o tamanho do corredor
    }
}