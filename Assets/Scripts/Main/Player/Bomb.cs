using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("此次消弹是否将子弹变为小得点道具")]
    public bool bulletToSmallScore;
    [Tooltip("是否有伤害")]
    public bool isDamage;
    [Tooltip("此次消弹对敌机（包括boss）造成的伤害")]
    public int damage;
    [Tooltip("造成伤害的间隔")]
    public float damageTime;

    ItemSpawner.ItemDrop smallScore;
    float time;

    public void Begin()
    {
        time = damageTime;
    }

    public void End()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        time = damageTime;
        smallScore = new ItemSpawner.ItemDrop();
        smallScore.scoreSmall = 1;
    }

    void Update()
    {
        if (isDamage)
        {
            time += Time.deltaTime;
            if (time > damageTime) time = damageTime;
        }
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if (target.tag == "enemy_bullet" && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            Mover temp = target.GetComponent<Mover>();
            temp.QueryRecycle();
            if (bulletToSmallScore)
                ItemSpawner.Instance.Spawn(temp.transform.localPosition, smallScore);
        }
        if (isDamage)
        {
            if (time >= damageTime)
            {
                if (target.tag == "enemy" && !target.GetComponent<EnemyMover>().IsQueryRecycle)
                {
                    time = 0.0f;
                    target.GetComponent<EnemyMover>().Damage(damage);
                }
                if (target.tag == "boss" && !target.GetComponent<BossMover>().hideHitBox)
                {
                    time = 0.0f;
                    target.GetComponent<BossMover>().Damage(damage);
                }
            }
        }
    }
}
