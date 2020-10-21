using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int BoardRows, BoardColums;       // 맵의 크기
    public int numberOfSplit;
    public int numberOfRoom;
    public GameObject floorTile;
    public GameObject corridorTile;
    public GameObject wallTile;
    public GameObject unBreakableTile;

    private int[,] GameBoard;

    int roomX, roomY;
    List<int> mapList = new List<int>();

    public void MapInfoInit()
    {
        GameBoard = new int[numberOfSplit, numberOfSplit];

        for (int i = 0; i < numberOfSplit; i++)
            for (int j = 0; j < numberOfSplit; j++)
                GameBoard[i, j] = 0;
    }

    public void MakeRoomList()
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

        while(mapList.Count < numberOfRoom - 1)
        {
            if (Shop)
            {
                mapList.Add(4);     // 4 = 상점
                Shop = false;
                continue;
            }
            if (Quest)
            {
                mapList.Add(6);
                Quest = false;
                continue;
            }

            mapList.Add(1);
        }
    }

    public bool AbleSecret(int Xnow, int Ynow)
    {
        int count = 0;

        if (GameBoard[Xnow - 1, Ynow] == 0) count++;
        if (GameBoard[Xnow + 1, Ynow] == 0) count++;
        if (GameBoard[Xnow, Ynow - 1] == 0) count++;
        if (GameBoard[Xnow, Ynow + 1] == 0) count++;

        if (count > 1)  return true;
        else            return false;
    }

    public bool NextCheck(int Xnext, int Ynext)
    {
        if (Xnext < 0 || Xnext > numberOfSplit - 1)
            return false;

        if (Ynext < 0 || Ynext > numberOfSplit - 1)
            return false;

        if (GameBoard[Xnext, Ynext] != 0)
            return false;

        return true;
    }

    public void MapLinker()
    { 
        int Xnow = Random.Range(1, numberOfSplit - 1);
        int Ynow = Random.Range(1, numberOfSplit - 1);
        int Xnext;
        int Ynext;
        int ExceptionCount = 0;     // 다음방 사용 불가능 포착 횟수
        int Xfirst = Xnow;          // 에러 처리용 X의 초기값 저장
        int Yfirst = Ynow;          // 에러 처리용 Y의 초기값 저장

        GameBoard[Xnow, Ynow] = 2;

        for (int i = 1; i < numberOfRoom; i++)
        {
            // 맵생성이 구석에서 ㄷ자로되서 dead 해버리는 거 방지용
            // 3번이상 충돌하면 초기 노드에서 확장
            if (ExceptionCount > 2)
            {
                Xnow = Xfirst;
                Ynow = Yfirst;
                ExceptionCount = 0;
            }

            if (Random.value > 0.5f)
            {
                Xnext = Random.value > 0.5f ? Xnow + 1 : Xnow - 1;

                if (!NextCheck(Xnext, Ynow))
                {
                    i--;
                    ExceptionCount++;
                    continue;
                }

                Xnow = Xnext;
            }
            else
            {
                Ynext = Random.value > 0.5f ? Ynow + 1 : Ynow - 1;

                if (!NextCheck(Xnow, Ynext))
                {
                    i--;
                    ExceptionCount++;
                    continue;
                }

                Ynow = Ynext;
            }

            if (i == numberOfRoom - 1)
                GameBoard[Xnow, Ynow] = 3;
            else
            {
                int pick = Random.Range(0, mapList.Count);
                GameBoard[Xnow, Ynow] = mapList[pick];
                mapList.RemoveAt(pick);
            }
        }

        for (int i = 0; i < numberOfSplit; i++)
            for (int j = 0; j < numberOfSplit; j++)
                Debug.Log(GameBoard[i, j]);
    }

    public void MapLoarder()
    {
        roomX = BoardRows / numberOfSplit;
        roomY = BoardColums / numberOfSplit;

        for (int i = 0; i < numberOfSplit; i++)
            for (int j = 0; j < numberOfSplit; j++)
                if (GameBoard[i, j] != 0)
                {
                    SetTile(i, j);
                }
    }

    public void SetTile(int X, int Y)
    {
        for (int i = (roomX + 1) * X; i < (roomX + 1) * X + (roomX); i++)
            for (int j = (roomY + 1) * Y; j < (roomY + 1) * Y + (roomY); j++)
            {
                GameObject instance = Instantiate(floorTile,
                            new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(transform);
            }
    }

    private void Start()
    {
        MapInfoInit();
        MakeRoomList();
        MapLinker();
        MapLoarder();
    }
}
