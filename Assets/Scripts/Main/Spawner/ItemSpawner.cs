using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObjectPool itemPool_power;
    public GameObjectPool itemPool_score;
    public GameObjectPool itemPool_scoreSmall;
    public GameObjectPool itemPool_lifePiece;
    public GameObjectPool itemPool_bombPiece;
    public GameObjectPool scorePool;
    [Tooltip("道具Mover的初始化参数")]
    public Mover.InitData initData;
    [Tooltip("道具Mover的基本参数")]
    public Mover.StateData[] stateData;
    [Tooltip("道具Mover在全屏收点时的基本参数")]
    public Mover.StateData collectStateData;
    [Tooltip("随机偏移最小值")]
    public Vector2 randomSpawnOffsetMin;
    [Tooltip("随机偏移最大值")]
    public Vector2 randomSpawnOffsetMax;
    [System.Serializable]
    public class ItemDrop
    {
        public int power, score, scoreSmall, lifePiece, bombPiece;
    }

    public static ItemSpawner Instance { get; private set; }

    public void Spawn(Vector3 pos, ItemDrop itemDrop)
    {
        ItemMover temp;
        for (int i = 0; i < itemDrop.power; ++i)
        {
            if (GameMgr.Instance.IsFullPower)
                temp = (ItemMover)itemPool_score.Get();
            else
                temp = (ItemMover)itemPool_power.Get();
            temp.stateData = stateData;
            temp.transform.localPosition = pos + new Vector3(Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x), Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y));
            temp.SetState(0);
        }
        for (int i = 0; i < itemDrop.score; ++i)
        {
            temp = (ItemMover)itemPool_score.Get();
            temp.stateData = stateData;
            temp.transform.localPosition = pos + new Vector3(Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x), Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y));
            temp.SetState(0);
        }
        for (int i = 0; i < itemDrop.scoreSmall; ++i)
        {
            temp = (ItemMover)itemPool_scoreSmall.Get();
            temp.stateData = stateData;
            temp.transform.localPosition = pos + new Vector3(Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x), Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y));
            temp.SetState(0);
        }
        for (int i = 0; i < itemDrop.lifePiece; ++i)
        {
            temp = (ItemMover)itemPool_lifePiece.Get();
            temp.stateData = stateData;
            temp.transform.localPosition = pos + new Vector3(Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x), Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y));
            temp.SetState(0);
        }
        for (int i = 0; i < itemDrop.bombPiece; ++i)
        {
            temp = (ItemMover)itemPool_bombPiece.Get();
            temp.stateData = stateData;
            temp.transform.localPosition = pos + new Vector3(Random.Range(randomSpawnOffsetMin.x, randomSpawnOffsetMax.x), Random.Range(randomSpawnOffsetMin.y, randomSpawnOffsetMax.y));
            temp.SetState(0);
        }
    }

    void Awake()
    {
        Instance = this;
    }
}
