using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMover : Mover
{
    [Tooltip("道具类型")]
    public GameMgr.ItemType itemType;
    [HideInInspector]
    public bool isCollect;

    public override void Reset()
    {
        base.Reset();
    }

    public override void SetState(int state)
    {
        base.SetState(state);
        isCollect = false;
        if (itemType == GameMgr.ItemType.ScoreSmall)
        {
            stateData = new StateData[1];
            stateData[0] = ItemSpawner.Instance.collectStateData;
            isCollect = true;
        }
    }
}
