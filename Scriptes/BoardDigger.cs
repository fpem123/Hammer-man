using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardDigger : MonoBehaviour
{
    public int BoardRows, BoardColums;
    public int RoomX, RoomY;
    public int numberOfRoom;                    // 방의 수

    int[,] map;                    // 0 = 방이 할당되지 않았음, 1 = 방이 할당 되어있음, -1 = 버려짐
    List<RoomIndex> roomList = new List<RoomIndex>();
    List<int> mapTypeList = new List<int>();

    void BoardInit()
    {
        map = new int[BoardRows, BoardColums];

        MakeRoomList();

        for (int i = 0; i < BoardRows; i++)
            for (int j = 0; j < BoardColums; j++)
                map[i, j] = -1;
    }

    void MakeRoomList()
    {
        /*
         * 0 = 사용 불가지점
         * 1 = 일반방
         * 2 = 시작지점
         * 3 = 보스방
         * 4 = 상점
         * 5 = 비밀방
         * 6 = 퀘스트방
         * 7 = 비밀 보스방
         */
        bool Shop = (Random.value > 0.5f);
        bool Quest = (Random.value > 0.5f);

        mapTypeList.Add(2);
        mapTypeList.Add(3);

        while (mapTypeList.Count < numberOfRoom)
        {
            if (Shop)
            {
                mapTypeList.Add(4);
                Shop = false;
                continue;
            }
            if (Quest)
            {
                mapTypeList.Add(6);
                Quest = false;
                continue;
            }

            mapTypeList.Add(1);
        }

        mapTypeList.Add(5);
    }

    void RandomSelect()
    {
        while(mapTypeList.Count > 0)
        {
            int pickX = Random.Range(0, BoardRows);
            int pickY = Random.Range(0, BoardColums);

            if (map[pickX, pickY] == 1)
            {
                continue;
            }
            
            int pickType = Random.Range(0, mapTypeList.Count);
            mapTypeList.RemoveAt(pickType);

            map[pickX, pickY] = 1;
            roomList.Add(new RoomIndex(pickX, pickY, pickType));
        }

        Debug.Log("방의 수 " + roomList.Count);
    }

    public struct RoomIndex
    {
        public int indexX;
        public int indexY;
        public int type;
        public RoomIndex(int x, int y, int t)
        {
            indexX = x;
            indexY = y;
            type = t;
        }
    }

    public void RouteMaker()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            int xMin, xMax;
            int yMin, yMax;

            RoomIndex nowRoom, nextRoom;

            Debug.Log("나는 " + i + "번 방");

            nowRoom = roomList[i];

            if (i != roomList.Count - 1)
                nextRoom = roomList[i + 1];
            else
                nextRoom = roomList[0];

            Debug.Log("좌표 출력" + nowRoom.indexX + " " + nextRoom.indexX + " " + nowRoom.indexY + " " + nextRoom.indexY);

            xMin = Mathf.Min(nowRoom.indexX, nextRoom.indexX);
            xMax = Mathf.Max(nowRoom.indexX, nextRoom.indexX);
            yMin = Mathf.Min(nowRoom.indexY, nextRoom.indexY);
            yMax = Mathf.Max(nowRoom.indexY, nextRoom.indexY);


            for (int x = xMin; x <= xMax; x++)
            {
                if (map[x, nowRoom.indexY] != -1)
                    continue;
                map[x, nowRoom.indexY] = 0;
                Debug.Log("x" + x + nowRoom.indexY + " " + map[x, nowRoom.indexY]);
            }

            for (int y = yMin; y <= yMax; y++)
            {
                if (map[nextRoom.indexX, y] != -1)
                    continue;
                map[nextRoom.indexX, y] = 0;
                Debug.Log("y" + nextRoom.indexX + y + " " + map[nextRoom.indexX, y]);
            }
        }

        for (int i = 0; i < BoardRows; i++)
            for (int j = 0; j <BoardColums; j++)
                Debug.Log(map[i, j]);
    }

    // Start is called before the first frame update
    void Start()
    {
        BoardInit();
        RandomSelect();
        RouteMaker();
    }
}
