using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageCleaner : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D target)
    {
        if (target.tag == "enemy_bullet" || target.tag == "player_bullet" || target.tag == "item")
            target.gameObject.GetComponent<Mover>().Recycle();
        if (target.tag == "enemy")
        {
            target.gameObject.GetComponent<Mover>().Recycle();
            GameMgr.Instance.UnregistEnemy(target.transform);
        }
    }
}
