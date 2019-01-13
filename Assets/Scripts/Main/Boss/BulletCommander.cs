using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCommander : MonoBehaviour
{
    public int stateIndex;

    public void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "enemy_bullet" && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            target.GetComponent<Mover>().SetState(stateIndex);
        }
    }
}
