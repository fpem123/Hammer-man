using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomAreaScripte : MonoBehaviour
{
    public int roomSize;
    public GameObject floorTile;

    BossMonsterScripte BM = new BossMonsterScripte();
    
    public void MakeBossRoom(int xmin, int ymin)
    {
        /*
        int xmax = xmin + roomSize;
        int ymax = ymin + roomSize;

        for (int i = xmin; i < xmax; i++)
            for (int j = ymin; j < xmax; j++)
            {
                GameObject instance
                       = Instantiate(floorTile, new Vector3(i, j, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(transform);
            }
        */
    }
}
