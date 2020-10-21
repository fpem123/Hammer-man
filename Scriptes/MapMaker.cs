using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapMaker : MonoBehaviour
{
    public int BoardRows, BoardColums;       // 맵의 크기
    public int minRoomSize, maxRoomSize;    // 맵에 생성된 방의 최소 크기 및 최대 크기
    public GameObject floorTile;
    public GameObject corridorTile;
    private GameObject[,] boardPositionsFloor;
    public static List<SubDungeon> leafRooms = new List<SubDungeon>();

    public static bool bossRoom = true;
    public static bool shopRoom = true;
    public static bool secretRoom = true;
    public static bool startingRoom = true;
    public static bool secretBossRoom = true;

    public static int probStR = 50;
    public static int probBR = 50;
    public static int probSpR = 60;
    public static int probScR = 95;
    
    public class SubDungeon
    {
        public SubDungeon left, right;
        public Rect rect;   // Rect -> 사각형모양저장 변수
        public Rect room = new Rect(-1, -1, 0, 0);
        public int debugId;
        public string special;
        private string nothingSpecial = "normalRoom";
        public List<Rect> corridors = new List<Rect>();
        private static int debugCounter = 0;

        public SubDungeon(Rect mrect)
        {
            rect = mrect;
            special = nothingSpecial;
            debugId = debugCounter;
            debugCounter++;
        }

        public void PickARoom()
        {
            if (special == nothingSpecial)
            {
                if (startingRoom)
                    if (Random.Range(0, 100) >= probStR)
                    {
                        special = "startRoom";
                        startingRoom = false;
                        Debug.Log("으하하하하 나는 " + special + "이다!!!!");
                        return;
                    }
                    else probStR -= 10;


                if (bossRoom)
                    if (Random.Range(0, 100) >= probBR)
                    {
                        special = "bossRoom";
                        bossRoom = false;
                        Debug.Log("으하하하하 나는 " + special + "이다!!!!");
                        return;
                    }
                    else probBR -= 10;

                if (shopRoom && Random.Range(0, 100) >= probSpR)
                {
                    special = "shopRoom";
                    shopRoom = false;
                    Debug.Log("으하하하하 나는 " + special + "이다!!!!");
                    return;
                }

                if (secretRoom && Random.Range(0, 100) >= probScR)
                {
                    special = "secretRoom";
                    secretRoom = false;
                    Debug.Log("으하하하하 나는 " + special + "이다!!!!");
                    return;
                }

                if (secretBossRoom && Random.Range(0, 100) == 1)
                {
                    special = "SecretBossRoom";
                    secretBossRoom = false;
                    Debug.Log("으하하하하 나는 " + special + "이다!!!!");
                    return;
                }
            }
        }

        // 방을 만드는 매서드
        public void CreateRoom()
        {
            if (left != null) left.CreateRoom();
            if (right != null) right.CreateRoom();

            PickARoom();

            if (IamLeaf())
            {
                int roomWidth = (int)Random.Range(rect.width / 2, rect.width - 2);
                int roomHeight = (int)Random.Range(rect.height / 2, rect.height - 2);
                int roomX = (int)Random.Range(1, rect.width - roomWidth - 1);
                int roomY = (int)Random.Range(1, rect.height - roomHeight - 1);

                room = new Rect(rect.x + roomX, rect.y + roomY, roomWidth, roomHeight);
                Debug.Log("Create room " + room + " in sub-dungeon " + special + debugId + " " + rect);
            }

            if (right != null && left != null)
                CreateCorridorBetween(left, right);
        }

        // 리프 노드인지 검사
        public bool IamLeaf() { return left == null && right == null; }

        // 방 나누기
        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if (!IamLeaf())
                return false;

            bool splitH;
            if (rect.width / rect.height >= 1.25) splitH = false;
            else if (rect.height / rect.width >= 1.25) splitH = true;
            else splitH = Random.Range(0.0f, 1.0f) > 0.5;

            if (Mathf.Min(rect.height, rect.width) / 2 < minRoomSize)
            {
                Debug.Log("sub-dungen " + debugId + "will be a leaf");
                return false;
            }

            if (splitH)
            {
                int split = Random.Range(minRoomSize, (int)(rect.width - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new SubDungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            } else
            {
                int split = Random.Range(minRoomSize, (int)(rect.height - minRoomSize));

                left = new SubDungeon(new Rect(rect.x, rect.y, split, rect.height));
                right = new SubDungeon(new Rect(rect.x + split, rect.y, rect.width - split, rect.height));
            }

            return true;
        }

        // 룸의 사각형 정보를 가져옴
        public Rect GetRoom()
        {
            if (IamLeaf())
                return room;

            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x != -1)
                    return lroom;
            }
            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x != -1)
                    return rroom;
            }

            return new Rect(-1, -1, 0, 0);
        }

        public void CreateCorridorBetween(SubDungeon left, SubDungeon right)
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();

            Debug.Log("Creating corridor(s) between " + left.debugId + "(" + lroom + ") and "
                + right.debugId + " (" + rroom + ")");

            Vector2 lpoint = new Vector2((int)Random.Range(lroom.x + 1, lroom.xMax - 1),
                (int)Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2((int)Random.Range(rroom.x + 1, rroom.xMax - 1),
                (int)Random.Range(rroom.y + 1, rroom.yMax - 1));

            if (lpoint.x > rpoint.x)
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }

            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);

            Debug.Log("lpoint: " + lpoint + ", rpoint: " + rpoint + ", w: " + w + ", h: " + h);

            if (w != 0)
            {
                if (Random.Range(0, 1) > 2)
                {
                    corridors.Add(new Rect(lpoint.x, lpoint.y, Mathf.Abs(w) + 1, 1));

                    if (h < 0)
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                    else
                        corridors.Add(new Rect(rpoint.x, lpoint.y, 1, -Mathf.Abs(h)));
                }
                else
                {
                    if (h < 0)
                        corridors.Add(new Rect(lpoint.x, lpoint.y, 1, Mathf.Abs(h)));
                    else
                        corridors.Add(new Rect(lpoint.x, rpoint.y, 1, Mathf.Abs(h)));

                    corridors.Add(new Rect(lpoint.x, rpoint.y, Mathf.Abs(w) + 1, 1));

                }
            }
            else
            {
                if (h < 0)
                    corridors.Add(new Rect((int)lpoint.x, (int)lpoint.y, 1, Mathf.Abs(h)));
                else
                    corridors.Add(new Rect((int)rpoint.x, (int)rpoint.y, 1, Mathf.Abs(h)));
            }

            Debug.Log("Corridor: ");
            foreach (Rect corridor in corridors)
                Debug.Log("corridor: " + corridor);
        }
    }

    // 만들어진 방을 시각화 한다
    public void DrawRooms(SubDungeon subDungeon)
    {
        if (subDungeon == null)
            return;

        if (subDungeon.IamLeaf())
            for (int i = (int)subDungeon.room.x; i < subDungeon.room.xMax; i++)
                for (int j = (int)subDungeon.room.y; j < subDungeon.room.yMax; j++)
                {
                    GameObject instance
                        = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                    instance.transform.SetParent(transform);
                    boardPositionsFloor[i, j] = instance;
                }
        else
        {
            DrawRooms(subDungeon.left);
            DrawRooms(subDungeon.right);
        }
    }

    
    // 만들어진 복도를 시각화 한다
    void DrawCorridors(SubDungeon subDungeon)
    {
        if (subDungeon == null)
            return;

        DrawCorridors(subDungeon.left);
        DrawCorridors(subDungeon.right);

        foreach (Rect corridor in subDungeon.corridors)
            for (int i = (int)corridor.x; i < corridor.xMax; i++)
                for (int j = (int)corridor.y; j < corridor.yMax; j++)
                    if (boardPositionsFloor[i,j] == null)
                    {
                        GameObject instance = Instantiate(corridorTile,
                            new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                        instance.transform.SetParent(transform);
                        boardPositionsFloor[i, j] = instance;
                    }
    }

    // 재귀호출로 BSP 트리를 형성한다.
    public void CreateBSP(SubDungeon subDungeon)
    {
        Debug.Log("Splitting sub-dungeon " + subDungeon.debugId + ": " + subDungeon.rect);

        if (subDungeon.IamLeaf())
        {
            if(subDungeon.rect.width > maxRoomSize
                || subDungeon.rect.height > maxRoomSize
                || Random.Range(0.0f, 1.0f) > 0.25)
            {
                if (subDungeon.Split (minRoomSize, maxRoomSize))
                {
                    Debug.Log("Splitted sub-dungeon " + subDungeon.debugId + "in"
                        + subDungeon.left.debugId +": " + subDungeon.left.rect + "' "
                        + subDungeon.right.debugId + ": " + subDungeon.right.rect);

                    CreateBSP(subDungeon.left);
                    CreateBSP(subDungeon.right);
                }
            }
        }
    }

    private void Start()
    {
        SubDungeon rootsubDungeon = new SubDungeon(new Rect(0, 0, BoardRows, BoardColums));
        CreateBSP(rootsubDungeon);
        rootsubDungeon.CreateRoom();

        boardPositionsFloor = new GameObject[BoardRows, BoardColums];
        DrawCorridors(rootsubDungeon);
        DrawRooms(rootsubDungeon);
    }
}
