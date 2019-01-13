using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletMover : Mover
{
    public BulletSpawner.StateData[] hitFxBulletSpawnerStateData;

    public override void QueryRecycle()
    {
        if (hitFxBulletSpawnerStateData.Length != 0)
        {
            BulletSpawner hitFxBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
            hitFxBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
            hitFxBulletSpawner.transform.localPosition = transform.localPosition;
            hitFxBulletSpawner.stateData = hitFxBulletSpawnerStateData;
            hitFxBulletSpawner.ShootOnce();
        }
        base.QueryRecycle();
    }
}
