using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE_Recycle : GameObjectPool.PoolElement
{
    public AudioSource se;

    public override void Reset()
    {
        se.clip = null;
    }

    void Update()
    {
        if (!se.isPlaying)
            pool.Recycle(this);
    }
}
