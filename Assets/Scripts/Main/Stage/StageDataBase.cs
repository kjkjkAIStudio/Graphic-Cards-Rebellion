using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataBase : MonoBehaviour
{
    [Serializable]
    public class EnemyDataBase
    {
        [Tooltip("针对敌机移动本身的初始数据")]
        public Mover.InitData initData;
        [Tooltip("针对敌机移动本身的状态数据")]
        public Mover.StateData[] stateData;
        [Tooltip("敌机的其他数据")]
        public EnemyMover.EnemyStateData enemyStatData;
        [Tooltip("敌机的掉落")]
        public ItemSpawner.ItemDrop itemDrop;
    }
    [Tooltip("当前关卡中所用的所有敌机（非boss）。尽量不要把当前关卡用不到的敌机数据放入。")]
    public EnemyDataBase[] enemyDataBase;
    [Serializable]
    public class BulletDataBase
    {
        [Tooltip("子弹刷新器的参数")]
        public BulletSpawner.StateData[] bulletSpawnerStateData;
    }
    [Tooltip("当前关卡中所用的所有子弹刷新器。")]
    public BulletDataBase[] bulletDataBase;

    [Tooltip("场景开始时播放的BGM")]
    public AudioClip bgm_stage;
    [Tooltip("场景开始时播放的BGM的名字")]
    public string bgm_stage_name;
    [Tooltip("boss开始时播放的BGM")]
    public AudioClip bgm_boss;
    [Tooltip("boss开始时播放的BGM的名字")]
    public string bgm_boss_name;
    [Tooltip("通关该场景的奖分")]
    public int stageBonus;

    void Start()
    {
        StageLoader.Instance.LoadStageFinish(this);
    }
}
