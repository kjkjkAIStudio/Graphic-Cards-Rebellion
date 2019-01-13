using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : GameObjectPool.PoolElement
{
    [Tooltip("使用的子弹所对的缓冲池")]
    public GameObjectPool bulletPool;
    [System.Serializable]
    public class StateData
    {
        [Tooltip("子弹的初始参数")]
        public Mover.InitData bulletInitData;
        [Tooltip("子弹的状态参数")]
        public Mover.StateData[] bulletStateData;
        [Tooltip("子弹出现间隔（在第一次启动时立刻刷出子弹）")]
        public float spawnDelay;
        [Tooltip("子弹一次性出现个数")]
        public float spawnCount;
        [Tooltip("子弹出现的位置偏移（相对于自己的朝向），每个状态都会重置")]
        public Vector2 spawnOffset;
        [Tooltip("子弹是否按照自己的方向出现")]
        public bool spawnInRotation;
        [Tooltip("子弹是否按照自己的放缩出现")]
        public bool spawnInScale;
        [Tooltip("每一颗子弹出现时，位置偏移累计改变的量")]
        public Vector2 deltaSpawnOffset;
        [Tooltip("每一颗子弹出现时，自己的位移")]
        public Vector2 deltaPosition;
        [Tooltip("每一颗子弹出现时，自己旋转的量")]
        public float deltaRotation;
        [Tooltip("每一颗子弹出现时，自己放缩的量")]
        public Vector2 deltaScale;
        [Tooltip("每轮射击开始时使自己朝向自机，适用于复数自机狙")]
        public bool spawnForward;
        [Tooltip("上面那个参数打开时，决定朝向自机的方向的偏移量")]
        public float spawnForwardOffset;
        [Tooltip("该状态结束时是否复位自己（包含local位置，local旋转，local放缩）")]
        public bool reset;
        [System.Serializable]
        public struct DeltaBulletBaseData
        {
            [Tooltip("所操作的状态数据序号")]
            public int stateIndex;
            [Tooltip("对移动数据的改变量")]
            public Mover.BaseData deltaBulletBaseData;
        }
        [System.Serializable]
        public class RandomData
        {
            [Tooltip("是否启动随机（没有填就不要勾）")]
            public bool enabled;
            [Tooltip("位置偏移的随机偏移最小值")]
            public Vector2 randomSpawnOffsetMin;
            [Tooltip("位置偏移的随机偏移最大值")]
            public Vector2 randomSpawnOffsetMax;
            [Tooltip("位置偏移累计改变的量的随机偏移最小值")]
            public Vector2 randomDeltaSpawnOffsetMin;
            [Tooltip("位置偏移累计改变的量的随机偏移最大值")]
            public Vector2 randomDeltaSpawnOffsetMax;
            [Tooltip("自己的位移的随机偏移最小值")]
            public Vector2 randomDeltaPositionMin;
            [Tooltip("自己的位移的随机偏移最大值")]
            public Vector2 randomDeltaPositionMax;
            [Tooltip("自己旋转的量的随机偏移最小值")]
            public float randomDeltaRotationMin;
            [Tooltip("自己旋转的量的随机偏移最大值")]
            public float randomDeltaRotationMax;
            [Tooltip("自己放缩的量的随机偏移最小值")]
            public Vector2 randomDeltaScaleMin;
            [Tooltip("自己放缩的量的随机偏移最大值")]
            public Vector2 randomDeltaScaleMax;
        }
        [Tooltip("随机数据补正")]
        public RandomData randomData;
        [Tooltip("每一颗子弹出现时，对子弹的移动数据的补正（请注意：图片，碰撞大小是不会补正的）")]
        public DeltaBulletBaseData[] deltaBaseData;
        [Tooltip("子弹出现时的音效（可以为空）")]
        public AudioClip se;
        [Tooltip("此状态的持续时间。时间到后进行下一个状态（若没有下一个状态则从第0个重新开始）")]
        public float lifetime;
    }
    [Tooltip("状态数据")]
    public StateData[] stateData;

    StateData.DeltaBulletBaseData[] cDeltaBulletBaseData;
    Vector2 cSpawnOffset;
    float time, bulletTime;
    int state;
    bool isOnce;

    public override void Reset()
    {
        transform.localPosition = new Vector3();
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        state = 0;
        time = 0.0f;
        bulletTime = 0.0f;
        isOnce = false;
    }

    public void ShootOnce()
    {
        SetState(0);
        bulletTime = stateData[state].spawnDelay;
        isOnce = true;
    }

    public void SetState(int index)
    {
        state = index;
        cDeltaBulletBaseData = new StateData.DeltaBulletBaseData[stateData[state].deltaBaseData.Length];
        for (int i = 0; i < cDeltaBulletBaseData.Length; ++i)
        {
            cDeltaBulletBaseData[i].stateIndex = stateData[state].deltaBaseData[i].stateIndex;
            cDeltaBulletBaseData[i].deltaBulletBaseData = new Mover.BaseData();
        }
        cSpawnOffset = stateData[state].spawnOffset;
    }

    void Awake()
    {
        time = 0.0f;
        bulletTime = 0.0f;
        state = 0;
        isOnce = false;
        if (stateData.Length == 0)
        {
            stateData = new StateData[1];
            stateData[0] = new StateData();
        }
    }

    void Start()
    {
        SetState(0);
    }

    void Update()
    {
        bulletTime += Time.deltaTime;
        if (bulletTime > stateData[state].spawnDelay)
        {
            bulletTime = 0.0f;
            if (stateData[state].spawnForward)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.forward, GameMgr.Instance.player.playerPos.localPosition - transform.position);
                transform.eulerAngles += new Vector3(0.0f, 0.0f, stateData[state].spawnForwardOffset);
            }
            for (int i = 0; i < stateData[state].spawnCount; ++i)
            {
                //开始初始化子弹
                Mover temp = (Mover)bulletPool.Get();
                temp.initData = stateData[state].bulletInitData;
                if (cDeltaBulletBaseData.Length == 0)
                {
                    temp.stateData = stateData[state].bulletStateData; //引用赋值
                }
                else
                {
                    temp.stateData = new Mover.StateData[stateData[state].bulletStateData.Length];
                    for (int j = 0; j < stateData[state].bulletStateData.Length; ++j)
                    {
                        temp.stateData[j] = new Mover.StateData();
                        temp.stateData[j].baseData = new Mover.BaseData();
                        temp.stateData[j].baseData.speed = stateData[state].bulletStateData[j].baseData.speed;
                        temp.stateData[j].baseData.acc = stateData[state].bulletStateData[j].baseData.acc;
                        temp.stateData[j].baseData.forwardSpeed = stateData[state].bulletStateData[j].baseData.forwardSpeed;
                        temp.stateData[j].baseData.forwardAcc = stateData[state].bulletStateData[j].baseData.forwardAcc;
                        temp.stateData[j].baseData.rotationSpeed = stateData[state].bulletStateData[j].baseData.rotationSpeed;
                        temp.stateData[j].baseData.rotationAcc = stateData[state].bulletStateData[j].baseData.rotationAcc;
                        temp.stateData[j].baseData.scaleSpeed = stateData[state].bulletStateData[j].baseData.scaleSpeed;
                        temp.stateData[j].baseData.scaleAcc = stateData[state].bulletStateData[j].baseData.scaleAcc;
                        temp.stateData[j].baseData.color = stateData[state].bulletStateData[j].baseData.color;
                        temp.stateData[j].baseData.img = stateData[state].bulletStateData[j].baseData.img;
                        temp.stateData[j].baseData.triggerRadius = stateData[state].bulletStateData[j].baseData.triggerRadius;
                        temp.stateData[j].baseData.triggerSize = stateData[state].bulletStateData[j].baseData.triggerSize;
                        temp.stateData[j].baseData.lifetime = stateData[state].bulletStateData[j].baseData.lifetime;
                        temp.stateData[j].randomData = stateData[state].bulletStateData[j].randomData;
                        temp.stateData[j].special = stateData[state].bulletStateData[j].special;
                    }
                    for (int j = 0; j < cDeltaBulletBaseData.Length; ++j)
                        temp.stateData[cDeltaBulletBaseData[j].stateIndex].baseData += cDeltaBulletBaseData[j].deltaBulletBaseData;
                }
                Vector3 randomPosition = stateData[state].randomData.enabled ?
                    new Vector3(Random.Range(stateData[state].randomData.randomSpawnOffsetMin.x, stateData[state].randomData.randomSpawnOffsetMax.x),
                    Random.Range(stateData[state].randomData.randomSpawnOffsetMin.y, stateData[state].randomData.randomSpawnOffsetMax.y))
                    : new Vector3();
                temp.transform.position =
                    transform.position + Quaternion.Euler(0.0f, 0.0f, transform.eulerAngles.z) * ((Vector3)cSpawnOffset + randomPosition);
                if (stateData[state].spawnInRotation)
                    temp.transform.rotation = transform.rotation;
                if (stateData[state].spawnInScale)
                    temp.transform.localScale = transform.localScale;
                temp.SetState(0);
                //改变量累计
                for (int j = 0; j < cDeltaBulletBaseData.Length; ++j)
                    cDeltaBulletBaseData[j].deltaBulletBaseData += stateData[state].deltaBaseData[j].deltaBulletBaseData;
                if (stateData[state].randomData.enabled)
                {
                    cSpawnOffset += stateData[state].deltaSpawnOffset + new Vector2(Random.Range(stateData[state].randomData.randomDeltaSpawnOffsetMin.x, stateData[state].randomData.randomDeltaSpawnOffsetMax.x), Random.Range(stateData[state].randomData.randomDeltaSpawnOffsetMin.y, stateData[state].randomData.randomDeltaSpawnOffsetMax.y));
                    transform.localPosition += (Vector3)stateData[state].deltaPosition + new Vector3(Random.Range(stateData[state].randomData.randomDeltaPositionMin.x, stateData[state].randomData.randomDeltaPositionMax.x), Random.Range(stateData[state].randomData.randomDeltaPositionMin.y, stateData[state].randomData.randomDeltaPositionMax.y));
                    transform.eulerAngles += new Vector3(0.0f, 0.0f, stateData[state].deltaRotation + Random.Range(stateData[state].randomData.randomDeltaRotationMin, stateData[state].randomData.randomDeltaRotationMax));
                    transform.localScale += (Vector3)stateData[state].deltaScale + new Vector3(Random.Range(stateData[state].randomData.randomDeltaScaleMin.x, stateData[state].randomData.randomDeltaScaleMax.x), Random.Range(stateData[state].randomData.randomDeltaScaleMin.y, stateData[state].randomData.randomDeltaScaleMax.y));
                }
                else
                {
                    cSpawnOffset += stateData[state].deltaSpawnOffset;
                    transform.localPosition += (Vector3)stateData[state].deltaPosition;
                    transform.eulerAngles += new Vector3(0.0f, 0.0f, stateData[state].deltaRotation);
                    transform.localScale += (Vector3)stateData[state].deltaScale;
                }
            }
            //一次子弹出现播放一次音效
            if (stateData[state].se != null)
                SoundMgr.Instance.PlaySE(stateData[state].se);
        }
        time += Time.deltaTime;
        if (time > stateData[state].lifetime)
        {
            time = 0.0f;
            bulletTime = 0.0f;
            ++state;
            if (state >= stateData.Length)
            {
                state = 0;
                //是否被设置为一次性
                if (isOnce)
                {
                    pool.Recycle(this);
                    return;
                }
            }
            if (stateData[state].reset)
            {
                transform.localPosition = new Vector3();
                transform.localRotation = new Quaternion();
                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
            SetState(state);
        }
    }
}
