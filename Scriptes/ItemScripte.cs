using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScripte : MonoBehaviour
{
    public int itemIndex;
    protected HeroController player;

    private void Start()
    {
        itemIndex = 0;
    }

    protected virtual void EffectToPlayer()
    {
        // 상속했을 때 플레이어에게 줄 효과를 정의하기
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.GetComponent<HeroController>();
            EffectToPlayer();
        }
    }
}
