using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public void Spawn(int index)
    {
        EnemyMover temp = (EnemyMover)StageLoader.Instance.enemyPool.Get();
        temp.transform.position = transform.position;
        temp.transform.rotation = transform.rotation;
        temp.initData = StageLoader.Instance.currentStage.enemyDataBase[index].initData;
        temp.stateData = StageLoader.Instance.currentStage.enemyDataBase[index].stateData;
        //对敌弹刷新器的特殊配置
        temp.bulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
        temp.enemyStateData = StageLoader.Instance.currentStage.enemyDataBase[index].enemyStatData;
        temp.itemDrop = StageLoader.Instance.currentStage.enemyDataBase[index].itemDrop;
        temp.SetState(0);
    }
}
