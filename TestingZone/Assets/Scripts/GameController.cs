using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject tilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        int seed = 0;
        Random.InitState(seed);
        InitMap(50);
    }

    private void InitMap(int size)
    {
        GameObject[,] tileSet = new GameObject[size, size];
        TileType[,] tileMap = new TileType[size, size];

        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Vector3 position = new Vector3(x, 0, y);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tileSet[x, y] = tile;
                tileMap[x, y] = TileType.Basic;
                if (Random.Range(0, 5) == 0) {
                    tileMap[x, y] = TileType.Controlled;
                }
            }
        }

        for (int i = 0; i < 20; i++) {
            ClusterMap(tileMap, TileType.Controlled);
        }

        ApplyTypeChanges(tileSet, tileMap);
    }

    private void ApplyTypeChanges(GameObject[,] tileSet, TileType[,] newTypes)
    {
        for (int x = 0; x < tileSet.GetLength(0); x++) {
            for (int y = 0; y < tileSet.GetLength(1); y++) {
                if (newTypes[x, y] == TileType.Controlled)
                    tileSet[x, y].GetComponent<Renderer>().material.color = Color.blue;
            }
        }
    }

    private void ClusterMap(TileType[,] map, TileType target)
    {
        const int LOW_BOUND = 2;
        const int HIGH_BOUND = 3;
        TileType[,] oldMap = (TileType[,]) map.Clone();
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                int numOfNeigbours = GetNeighbours(oldMap, x, y, target);
                if (numOfNeigbours < LOW_BOUND) {
                    map[x, y] = TileType.Basic;
                }
                else if (numOfNeigbours >= HIGH_BOUND) {
                    map[x, y] = TileType.Controlled;
                }
            }
        }
    }

    private int GetNeighbours(TileType[,] map, int x, int y, TileType target) {
        const int RADIUS = 1;
        int lowX = Mathf.Max(x - RADIUS, 0);
        int highX = Mathf.Min(x + RADIUS, map.GetLength(0) - 1);
        int lowY = Mathf.Max(y - RADIUS, 0);
        int highY = Mathf.Min(y + RADIUS, map.GetLength(1) - 1);
        int total = 0;
        for (int nx = lowX; nx <= highX; nx++) {
            for (int ny = lowY; ny <= highY; ny++) { 
                if (map[nx, ny] == target)
                    total++;
            }
        }
        total--; // Do not count yourself
        return total;
    }
}
