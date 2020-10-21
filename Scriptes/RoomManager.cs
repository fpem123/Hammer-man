using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public int level;
    public int Xindex, Yindex;               // 맵매니저에게 받을 정보, 방의 인덱스
    public int roomX, roomY;                 // 맵매니저에게 받을 정보, 방의 크기
    public GameObject FloorTiles;
    public GameObject Monsters;
    public GameObject Gift;

    int startX, startY;     // 방이 시작되는 좌표의 초기점
    int centerX, centerY;   // 방의 중앙 지점
    int maxMonster;         // 방에 배치될 몬스터의 양의 최대값
    int numberOfMonster;    // 방의 몬스터 수와 상호작용해서 보상을 주기 위해 필요한 변수

    bool given = true; // 플레이어에게 보상을 단 1번만 주기 위함

    private List<Vector3> roomGrid = new List<Vector3>();   // 몬스터와 오브젝트들을 겹치지 않게 배치하기 위한 백터 리스트

    // 사용할 변수들을 초기화 하는 함수
    private void VarInit()
    {
        startX = Xindex * roomX;
        startY = Yindex * roomY;

        centerX = startX + (startX + roomX) / 2;
        centerY = startY + (startY + roomY) / 2;

        maxMonster = roomX * roomY / 4;    // 4칸마다 몬스터가 1마리씩 증가

        roomGrid.Clear();
    }


    // 방의 타일 배치 및 리스트에 좌표 정보들을 roomGrid에 ADD
    public void RoomLoarder()
    {
        for (int i = startX; i < startX + roomX; i++)
            for (int j = startY; j < startY + roomY; j++)
            {
                roomGrid.Add(new Vector3(i, j, 0f));

                GameObject instance = Instantiate(FloorTiles,
                            new Vector3(i, j, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(transform);
            }
    }

    // roomGrid에서 좌표값 하나를 뽑아서 몬스터를 배치하고 해당 좌표를 리스트에서 삭제
    public void MonsterSet()
    {
        // 몬스터방은 항상 최소 1마리의 몬스터를 보장한다.
        numberOfMonster = Random.Range(1, maxMonster);

        Debug.Log("maxMonster " + maxMonster);
        Debug.Log("numberOfMonster " + numberOfMonster);

        for (int i = 0; i < numberOfMonster; i++)
        {
            int randomIndex = Random.Range(0, roomGrid.Count);
            Vector3 randomPosition = roomGrid[randomIndex];

            GameObject instance = Instantiate(Monsters, randomPosition, Quaternion.identity);

            instance.transform.SetParent(transform);

            roomGrid.RemoveAt(randomIndex);
        }
    }

    public void GiftForYou()
    {
        GameObject instance = Instantiate(Gift, new Vector3(centerX, centerY, 0f), Quaternion.identity);

        instance.transform.SetParent(transform);
    }

    // Start is called before the first frame update
    void Start()
    {
        VarInit();
        RoomLoarder();
        MonsterSet();
    }

    public void ChangeNumberOfMonster(int amount)
    {
        numberOfMonster = Mathf.Clamp(numberOfMonster + amount, 0, maxMonster);

        Debug.Log("남은 몬스터 " + numberOfMonster);

        if (numberOfMonster == 0 && given)
        {
            GiftForYou();
            given = false;
        }
    }
}
