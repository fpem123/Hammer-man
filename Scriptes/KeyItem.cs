using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : ItemScripte
{
    private void Start()
    {
        itemIndex = 1;
    }

    protected override void EffectToPlayer()
    {
        player.setKey(1);
        Destroy(gameObject);
    }
}
