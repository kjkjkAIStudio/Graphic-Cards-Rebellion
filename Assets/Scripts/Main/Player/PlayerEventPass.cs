using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventPass : MonoBehaviour
{
    public void OnTriggerStay2D(Collider2D target)
    {
        if (target.tag == "item")
        {
            ItemMover mover = target.GetComponent<ItemMover>();
            GameMgr.Instance.GetItem(mover.itemType);
            mover.Recycle();
        }

        if ((target.tag == "enemy_bullet" || target.tag == "enemy") && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            if (!GameMgr.Instance.IsGod)
            {
                target.GetComponent<Mover>().QueryRecycle();
                GameMgr.Instance.QueryDie();
            }
        }

        if (target.tag == "boss" && GameMgr.Instance.bossHitPlayer)
        {
            if (!GameMgr.Instance.IsGod && !target.GetComponent<BossMover>().hideHitBox)
            {
                GameMgr.Instance.QueryDie();
            }
        }
    }
}
