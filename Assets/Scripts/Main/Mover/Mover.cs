using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : GameObjectPool.PoolElement
{
    public SpriteRenderer img;
    public BoxCollider2D hitBox;
    public CircleCollider2D hitPoint;
    [System.Serializable]
    public class InitData
    {
        [Tooltip("伤害（如果击中玩家的话，此值毫无意义）")]
        public int damage;
        [Tooltip("销毁时放缩目标")]
        public Vector2 recycleScaleTarget;
        [Tooltip("销毁时放缩速度")]
        public Vector2 recycleScaleSpeed;
        [Tooltip("上面那个参数的平滑时间")]
        public float recycleScaleSpeedSmoothTime;
        [Tooltip("销毁时淡出速度（线性的，淡出结束时开始回收。例如：1.0表示1秒淡出，0.5表示2秒淡出）")]
        public float recycleAlphaSpeed;
        [Tooltip("是否使用子弹出现时的特效（注意：以下所有刷新器会无视间隔立刻发射一次子弹。以下所有刷新器只会执行一遍stateData）")]
        public bool spawnFxBullet;
        [Tooltip("上面的特效在DataBase的序号")]
        public int spawnFxBulletIndex;
        [Tooltip("是否使用子弹回收时的特效（其实和死尸弹是同一个东西）")]
        public bool dieFxBullet;
        [Tooltip("上面的特效在DataBase的序号")]
        public int dieFxBulletIndex;
        [Tooltip("是否使用死尸弹（使用死尸子弹刷新器）")]
        public bool dieBullet;
        [Tooltip("死尸弹刷新器在DataBase的序号")]
        public int dieBulletIndex;
    }
    [Tooltip("在子弹生命周期内有效的数据")]
    public InitData initData;
    [System.Serializable]
    public class BaseData
    {
        [Tooltip("绝对速度")]
        public Vector2 speed;
        [Tooltip("绝对加速度")]
        public Vector2 acc;
        [Tooltip("沿着对象朝向的速度")]
        public Vector2 forwardSpeed;
        [Tooltip("沿着对象朝向的加速度")]
        public Vector2 forwardAcc;
        [Tooltip("旋转速度")]
        public float rotationSpeed;
        [Tooltip("旋转加速度")]
        public float rotationAcc;
        [Tooltip("放缩速度")]
        public Vector2 scaleSpeed;
        [Tooltip("放缩加速度")]
        public Vector2 scaleAcc;
        [Tooltip("颜色")]
        public Color color;
        [Tooltip("图片")]
        public Sprite img;
        [Tooltip("方形碰撞器的大小（若其中一项为0，方形碰撞器会关闭）")]
        public Vector2 triggerSize;
        [Tooltip("圆形碰撞器的大小（若为0，圆形碰撞器会关闭）")]
        public float triggerRadius;
        [Tooltip("此状态的持续时间。时间到后进行下一个状态（若没有下一个状态则正常销毁）")]
        public float lifetime;

        public static BaseData operator+(BaseData left, BaseData right)
        {
            BaseData result = new BaseData();
            result.speed = left.speed + right.speed;
            result.acc = left.acc + right.acc;
            result.forwardSpeed = left.forwardSpeed + right.forwardSpeed;
            result.rotationSpeed = left.rotationSpeed + right.rotationSpeed;
            result.rotationAcc = left.rotationAcc + right.rotationAcc;
            result.color = left.color + right.color;
            result.img = left.img;
            result.triggerSize = left.triggerSize;
            result.triggerRadius = left.triggerRadius;
            result.lifetime = left.lifetime + right.lifetime;
            return result;
        }
    }
    [System.Serializable]
    public class Special
    {
        [Tooltip("打钩设置目标为敌机，否则目标为自机")]
        public bool aimingEnemy;
        [Tooltip("自机狙：出现时朝着目标的位置")]
        public bool spawnForward;
        [Tooltip("跟踪：出现时获取最近的目标，每帧旋转朝着目标的位置")]
        public bool homing;
        [Tooltip("阶段开始时根据向前的速度和朝向转换为绝对速度，此时绝对速度无效，向前速度清零（适用于旋转前进的子弹）")]
        public bool forwardSpeedToSpeed;
        [Tooltip("是否保留上一状态的绝对速度，开启后此状态的绝对速度无效，但是随机数有效")]
        public bool persistantSpeed;
    }
    [System.Serializable]
    public class RandomData
    {
        [Tooltip("如果需要启动随机数的话，必须打开。否则不要打开")]
        public bool enabled;
        [Tooltip("绝对随机速度最小值")]
        public Vector2 speedMin;
        [Tooltip("绝对随机速度最大值")]
        public Vector2 speedMax;
        [Tooltip("沿着对象朝向的随机速度最小值")]
        public Vector2 forwardSpeedMin;
        [Tooltip("沿着对象朝向的随机速度最大值")]
        public Vector2 forwardSpeedMax;
        [Tooltip("旋转随机速度最小值")]
        public float rotationSpeedMin;
        [Tooltip("旋转随机速度最大值")]
        public float rotationSpeedMax;
        [Tooltip("放缩随机速度最小值")]
        public Vector2 scaleSpeedMin;
        [Tooltip("放缩随机速度最大值")]
        public Vector2 scaleSpeedMax;
    }
    [System.Serializable]
    public class StateData
    {
        [Tooltip("移动transform的固定参数")]
        public BaseData baseData;
        [Tooltip("特殊移动方式，在固定参数的基础上添加")]
        public Special special;
        [Tooltip("随机参数，在特殊移动方式的基础上添加。")]
        public RandomData randomData;
    }
    [Tooltip("状态数据。根据时间切换为不同的状态。如果为空，出现时正常销毁")]
    public StateData[] stateData;
    [HideInInspector]
    public bool isGraze;

    public bool IsQueryRecycle { get { return isQueryRecycle; } }

    public override void Reset()
    {
        transform.localPosition = new Vector3();
        transform.localRotation = new Quaternion();
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        cRecycleScaleSpeed = new Vector3();
        time = 0.0f;
        state = 0;
        isGraze = false;
    }

    public virtual void QueryRecycle()
    {
        if (!isQueryRecycle)
        {
            //不知道为什么，设置碰撞箱状态会导致图片颜色强制变为透明
            //if (hitBox != null)
            //    hitBox.enabled = false;
            //if (hitPoint != null)
            //    hitPoint.enabled = false;
            if (initData.dieBullet)
            {
                BulletSpawner dieBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
                dieBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
                dieBulletSpawner.transform.localPosition = transform.localPosition;
                dieBulletSpawner.stateData = StageLoader.Instance.currentStage.bulletDataBase[initData.dieBulletIndex].bulletSpawnerStateData;
                dieBulletSpawner.ShootOnce();
            }
            if (initData.dieFxBullet)
            {
                BulletSpawner dieFxBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
                dieFxBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
                dieFxBulletSpawner.transform.localPosition = transform.localPosition;
                dieFxBulletSpawner.stateData = StageLoader.Instance.currentStage.bulletDataBase[initData.dieFxBulletIndex].bulletSpawnerStateData;
                dieFxBulletSpawner.ShootOnce();
            }
            isQueryRecycle = true;
        }
    }

    public void Recycle()
    {
        pool.Recycle(this);
    }

    public virtual void SetState(int state)
    {
        time = 0.0f;
        this.state = state;
        isQueryRecycle = false;
        SetState();
        //出生特效
        if (initData.spawnFxBullet)
        {
            BulletSpawner spawnFxBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
            spawnFxBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
            spawnFxBulletSpawner.transform.localPosition = transform.localPosition;
            spawnFxBulletSpawner.stateData = StageLoader.Instance.currentStage.bulletDataBase[initData.spawnFxBulletIndex].bulletSpawnerStateData;
            spawnFxBulletSpawner.ShootOnce();
        }
    }

    Vector3 cRecycleScaleSpeed;
    protected Vector2 cSpeed, cForwardSpeed, cScaleSpeed;
    float cRotationSpeed;
    float time;
    int state;
    protected bool isQueryRecycle;

    protected virtual void SetState()
    {
        if (isQueryRecycle || stateData == null)
            return;

        //开始缓存初始数据
        if (stateData[state].special.forwardSpeedToSpeed)
        {
            cSpeed = Quaternion.Euler(transform.eulerAngles) * cForwardSpeed;
            cForwardSpeed = new Vector2();
        }
        else
        {
            cForwardSpeed = stateData[state].baseData.forwardSpeed;
        }
        if (!stateData[state].special.persistantSpeed)
            cSpeed = stateData[state].baseData.speed;
        cRotationSpeed = stateData[state].baseData.rotationSpeed;
        cScaleSpeed = stateData[state].baseData.scaleSpeed;
        if (img != null)
        {
            img.color = stateData[state].baseData.color;
            img.sprite = stateData[state].baseData.img;
        }
        if (hitBox != null)
        {
            if (stateData[state].baseData.triggerSize.x != 0.0f && stateData[state].baseData.triggerSize.y != 0.0f)
            {
                hitBox.size = stateData[state].baseData.triggerSize;
                hitBox.enabled = true;
            }
            else
            {
                hitBox.enabled = false;
            }
        }
        if (hitPoint != null)
        {
            if (stateData[state].baseData.triggerRadius != 0.0f && hitPoint != null)
            {
                hitPoint.radius = stateData[state].baseData.triggerRadius;
                hitPoint.enabled = true;
            }
            else
            {
                hitPoint.enabled = false;
            }
        }
        //特殊数据补正：自机狙
        if (stateData[state].special.spawnForward)
        {
            if (stateData[state].special.aimingEnemy)
            {
                if (GameMgr.Instance.aimingEnemy != null)
                    transform.localRotation = Quaternion.LookRotation(transform.forward, GameMgr.Instance.aimingEnemy.localPosition - transform.localPosition);
            }
            else
            {
                transform.localRotation = Quaternion.LookRotation(transform.forward, GameMgr.Instance.player.playerPos.localPosition - transform.localPosition);
            }
        }
        //随机数据补正
        if (stateData[state].randomData.enabled)
        {
            cSpeed += new Vector2(Random.Range(stateData[state].randomData.speedMin.x, stateData[state].randomData.speedMax.x), Random.Range(stateData[state].randomData.speedMin.y, stateData[state].randomData.speedMax.y));
            cForwardSpeed += new Vector2(Random.Range(stateData[state].randomData.forwardSpeedMin.x, stateData[state].randomData.forwardSpeedMax.x), Random.Range(stateData[state].randomData.forwardSpeedMin.y, stateData[state].randomData.forwardSpeedMax.y));
            cRotationSpeed += Random.Range(stateData[state].randomData.rotationSpeedMin, stateData[state].randomData.rotationSpeedMax);
            cScaleSpeed += new Vector2(Random.Range(stateData[state].randomData.scaleSpeedMin.x, stateData[state].randomData.scaleSpeedMax.x), Random.Range(stateData[state].randomData.scaleSpeedMin.y, stateData[state].randomData.scaleSpeedMax.y));
        }
    }

    protected virtual void Awake()
    {
        cRecycleScaleSpeed = new Vector3();
        time = 0.0f;
        state = 0;
        isQueryRecycle = false;
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (isQueryRecycle)
        {
            //开始淡出
            if (img == null)
            {
                Recycle();
            }
            else
            {
                img.color -= new Color(0.0f, 0.0f, 0.0f, initData.recycleAlphaSpeed * Time.deltaTime);
                transform.localScale = Vector3.SmoothDamp(transform.localScale, initData.recycleScaleTarget, ref cRecycleScaleSpeed, initData.recycleScaleSpeedSmoothTime);
                if (img.color.a <= 0.0f)
                    Recycle();
            }
        }
        else
        {
            time += Time.deltaTime;
            if (time > stateData[state].baseData.lifetime)
            {
                time = 0.0f;
                ++state;
                if (state < stateData.Length)
                {
                    SetState();
                }
                else
                {
                    --state;
                    QueryRecycle();
                    return;
                }
            }
            //开始更新速度
            cSpeed += stateData[state].baseData.acc * Time.deltaTime;
            cForwardSpeed += stateData[state].baseData.forwardAcc * Time.deltaTime;
            cRotationSpeed += stateData[state].baseData.rotationAcc * Time.deltaTime;
            cScaleSpeed += stateData[state].baseData.scaleAcc * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        //if (!isQueryRecycle)
        {
            //按速度更新
            transform.localPosition += (Vector3)cSpeed * Time.fixedDeltaTime;
            transform.localPosition += Quaternion.Euler(0.0f, 0.0f, transform.localEulerAngles.z) * cForwardSpeed * Time.fixedDeltaTime;
            transform.localEulerAngles += new Vector3(0.0f, 0.0f, cRotationSpeed * Time.fixedDeltaTime);
            transform.localScale += (Vector3)cScaleSpeed * Time.fixedDeltaTime;
            //追踪
            if (stateData[state].special.homing)
            {
                if (stateData[state].special.aimingEnemy)
                {
                    if (GameMgr.Instance.aimingEnemy != null)
                        transform.localRotation = Quaternion.LookRotation(transform.forward, GameMgr.Instance.aimingEnemy.localPosition - transform.localPosition);
                }
                else
                {
                    transform.localRotation = Quaternion.LookRotation(transform.forward, GameMgr.Instance.player.playerPos.localPosition - transform.localPosition);
                }
            }
        }
    }
}
