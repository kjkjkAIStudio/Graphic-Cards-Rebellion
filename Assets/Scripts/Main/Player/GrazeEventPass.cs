using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrazeEventPass : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "enemy_bullet" && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            Mover temp = target.GetComponent<Mover>();
            if (!temp.isGraze)
            {
                temp.isGraze = true;
                GameMgr.Instance.Graze();
            }
        }
    }
}
