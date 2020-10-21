using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager_dig : MonoBehaviour
{
    public int BoardRows, BoardColums;          // 맵의 크기
    public int minSize, maxSize;                // 방의 최소크기 및 최대 크기
    public List<Coord> bigRoom;
    [Range(1, 10)]
    public int smooth;
    [Range(0, 100)]
    public int FillPercent;
    public string seed;
    public bool useRandomSeed;

    public GameObject outerTile;
    public GameObject floorTile;
    public GameObject wallTile;
    
    List<Coord> floorList = new List<Coord>();

    int[,] map;

    void GenerateMap()
    {
        floorList.Clear();

        map = new int[BoardRows, BoardColums];

        FillMap();

        for (int i = 0; i < smooth;i++)
            SmoothMap();

        ProcessMap();
    }

    // 맵에 랜덤하게 벽을 생성한다.
    void FillMap()
    {
        if (useRandomSeed)
            seed = Time.time.ToString();

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < BoardRows; x++)
            for (int y = 0; y < BoardColums; y++)
            {
                if (x == 0 || x == BoardRows - 1 || y == 0 || y == BoardColums - 1)
                    map[x, y] = 1;
                else
                    map[x, y] = (pseudoRandom.Next(0, 100) < FillPercent) ? 1 : 0;
            }
    }

    // 맵을 다듬는다
    void SmoothMap()
    {
       for(int x = 0; x < BoardRows; x++)
            for (int y = 0; y < BoardColums; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
    }

    // 맵 경계 여부 확인
    bool IsInMapRange(int x, int y)
    {
        return x >= 0 && x < BoardRows && y >= 0 && y < BoardColums;
    }


    // 벽 주변 상태 확인
    // 주변 타일 (3 x 3) 상태를 확인하여 가중치를 결정
    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;

        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            for(int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (IsInMapRange(neighbourX, neighbourY))
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                        wallCount += map[neighbourX, neighbourY];
                }
                else
                    wallCount++;
            }

        return wallCount;
    }

    // 큰 방을 제외하고 자잘한 맵들을 제거
    void ProcessMap()
    {
        List<List<Coord>> wallRegions = GetRegions(0);
        List<List<Coord>> processOneRoom = new List<List<Coord>>();
        bigRoom = new List<Coord>();

        foreach (List<Coord> wallRegion in wallRegions)
        {
            if (wallRegion.Count < bigRoom.Count)
                processOneRoom.Add(wallRegion);
            else
            {
                processOneRoom.Add(bigRoom);
                bigRoom = wallRegion;
            }
        }

        foreach(List<Coord> room in processOneRoom)
        {
            foreach (Coord tile in room)
                map[tile.tileX, tile.tileY] = 1;
        }

    }

    //지도 작성 함수
    List<List<Coord>> GetRegions(int tileType)
    {
        List<List<Coord>> regions = new List<List<Coord>>();
        int[,] mapFlags = new int[BoardRows, BoardColums];

        for (int x = 0; x < BoardRows; x++)
            for (int y = 0; y < BoardColums; y++)
            {
                if (mapFlags[x, y] == 0 && map[x,y] == tileType)
                {
                    List<Coord> newRegion = GetRegionTiles(x, y);
                    regions.Add(newRegion);

                    foreach (Coord tile in newRegion)
                        mapFlags[tile.tileX, tile.tileY] = 1;
                }
            }

        return regions;
    }

    // startX, startY 좌표를 시작으로 방의 모든 장소를 방문
    List<Coord> GetRegionTiles(int startX, int startY)
    {
        List<Coord> tiles = new List<Coord>();
        int[,] mapFlags = new int[BoardRows, BoardColums];
        int tileType = map[startX, startY];
        Queue<Coord> queue = new Queue<Coord>();

        queue.Enqueue(new Coord(startX, startY));
        mapFlags[startX, startY] = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            tiles.Add(tile);

            for(int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
                for(int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if(IsInMapRange(x, y) && (y == tile.tileY || x == tile.tileX))
                    {
                        if (mapFlags[x, y] == 0 && map[x, y] == tileType)
                        {
                            mapFlags[x, y] = 1;
                            queue.Enqueue(new Coord(x, y));
                        }
                    }
                }
        }

        return tiles;
    }

    public struct Coord
    {
        public int tileX;
        public int tileY;
        public Coord(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
    }

    private void BoardLoader()
    {
        for (int x = -1; x < BoardRows + 1; x++)
            for (int y = -1; y < BoardColums + 1; y++)
            {
                if (x == -1 || x == BoardRows || y == -1 || y == BoardColums)
                {
                    GameObject instance = Instantiate(outerTile,
                            new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(transform);
                }
                else
                {
                    if (map[x, y] == 1)
                    {
                        GameObject instance = Instantiate(wallTile,
                              new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                        instance.transform.SetParent(transform);
                    }
                    else
                    {
                        floorList.Add(new Coord(x, y));

                        GameObject instance = Instantiate(floorTile,
                            new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                        instance.transform.SetParent(transform);
                    }
                }
            }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            GenerateMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        BoardLoader();
    }
}
