using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMover : MonoBehaviour
{
    public Boss main;
    public Collider2D hitBox;
    public SpriteMask mask_hp;
    public SpriteRenderer img;
    [System.Serializable]
    public class ImageData
    {
        [Tooltip("待机图片（至少要有一张）")]
        public Sprite[] img_idle;
        [Tooltip("待机图片更换间隔（即使只有一张图片，也尽量不要设为0）")]
        public float idleDelay;
        [Tooltip("左转图片（可以为空）")]
        public Sprite[] img_left;
        [Tooltip("左转图片更换间隔（即使没有图片，也尽量不要设为0）")]
        public float leftDelay;
        [Tooltip("右转图片（可以为空）")]
        public Sprite[] img_right;
        [Tooltip("右转图片更换间隔（即使没有图片，也尽量不要设为0）")]
        public float rightDelay;
        [Tooltip("失去判定点时切换到的图片")]
        public Sprite img_loseHitBox;
    }
    [Tooltip("与boss图片有关的数据")]
    public ImageData imageData;
    [System.Serializable]
    public class StateData
    {
        [Tooltip("boss的默认位置，在符卡开始时移动到这个位置，以下所有移动都相对于这个位置")]
        public Vector2 defaultPos;
        [Tooltip("需要让boss移动到其他位置就勾选，否则站在默认位置")]
        public bool move;
        [Tooltip("移动的最大速度")]
        public float maxSpeed;
        [Tooltip("移动的平滑速度")]
        public float smoothTime;
        [Tooltip("保持X坐标与玩家相同")]
        public bool AimingX;
        [Tooltip("在一个方形区域内随机移动（如果AimingX开启，则在它的基础上增加）")]
        public bool randomMove;
        [Tooltip("上面的方形区域除以2")]
        public Vector2 halfRandomMove;
        [Tooltip("让boss跟随一个gameObject。此状态下上面的参数都无效")]
        public Transform dynamicPos;
        [Tooltip("开启后boss会直接瞬移到上面那些参数所决定的目标点，而不是移动")]
        public bool flashMove;
        [Tooltip("这个状态的持续时间，如果这是最后一个状态，则从头开始")]
        public float lifetime;
    }
    [Tooltip("与移动有关的数据（请至少设计一个，作为boss的初始状态）")]
    public StateData[] stateData;
    [Tooltip("是否隐藏判定点")]
    public bool hideHitBox;

    [HideInInspector]
    public Vector3 pos;
    [HideInInspector]
    public ItemSpawner.ItemDrop itemDrop;
    [HideInInspector]
    public bool isTime;

    public void BeginBattle(int maxHp, StateData[] stateData)
    {
        this.stateData = stateData;
        mask_hp.alphaCutoff = 1.0f;
        cMaxHp = maxHp;
        cHp = cMaxHp;
        isFillHp = true;
        isItemDrop = false;
        SetState();
    }

    public void Damage(int damage)
    {
        if (isBattle && !isFillHp && !isTime)
        {
            cHp -= damage;
            float percent = (float)cHp / cMaxHp;
            mask_hp.alphaCutoff = 1.0f - percent;
            isFlash = true;
            isLessThan20Percent = percent < 0.2f;
            if (cHp <= 0)
            {
                if (isItemDrop) //当两颗子弹同时击中对象，并且都致死的话，会导致掉落两遍，故增加
                    return;
                isItemDrop = true;
                Defeat();
                ItemSpawner.Instance.Spawn(transform.localPosition, itemDrop);
                main.Defeat();
            }
        }
    }

    public void Defeat()
    {
        cState = 0;
        stateTime = 0.0f;
        pos = transform.localPosition;
        hideHitBox = true;
        isBattle = false;
    }

    public void SetState()
    {
        if (stateData[cState].dynamicPos == null)
        {
            pos = stateData[cState].defaultPos;
            if (stateData[cState].move)
            {
                if (stateData[cState].AimingX)
                    pos.x = GameMgr.Instance.player.playerPos.localPosition.x;
                if (stateData[cState].randomMove)
                    pos += new Vector3(Random.Range(-stateData[cState].halfRandomMove.x, stateData[cState].halfRandomMove.x), Random.Range(-stateData[cState].halfRandomMove.y, stateData[cState].halfRandomMove.y));
            }
            if (stateData[cState].flashMove)
                transform.localPosition = pos;
        }
    }

    Vector3 cSpeed;
    float imgTime, stateTime, flashTime;
    int cIndex, cHp, cMaxHp, cState;
    bool isBattle, isFillHp, isFlash, isLessThan20Percent, isItemDrop;

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "player_bullet" && !hideHitBox && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            Mover temp = target.GetComponent<Mover>();
            Damage(temp.initData.damage);
            temp.QueryRecycle();
        }
    }

    void Awake()
    {
        cSpeed = new Vector3();
        imgTime = 0.0f;
        stateTime = 0.0f;
        flashTime = 0.0f;
        cIndex = 0;
        cHp = 0;
        cMaxHp = 0;
        cState = 0;
        isBattle = false;
        isFillHp = false;
        isFlash = false;
        isLessThan20Percent = false;
        isItemDrop = false;
    }

    void Start()
    {
        img.sprite = imageData.img_idle[0];
    }

    void Update()
    {
        imgTime += Time.deltaTime;
        if (cSpeed.x < -5.0f && imageData.img_left.Length != 0)
        {
            if (imgTime > imageData.leftDelay)
            {
                imgTime = 0.0f;
                ++cIndex;
                if (cIndex >= imageData.img_left.Length) cIndex = 0;
                img.sprite = imageData.img_left[cIndex];
            }
        }
        else
        {
            if (cSpeed.x > 5.0f && imageData.img_right.Length != 0)
            {
                if (imgTime > imageData.rightDelay)
                {
                    imgTime = 0.0f;
                    ++cIndex;
                    if (cIndex >= imageData.img_right.Length) cIndex = 0;
                    img.sprite = imageData.img_right[cIndex];
                }
            }
            else
            {
                if (imgTime > imageData.idleDelay)
                {
                    imgTime = 0.0f;
                    ++cIndex;
                    if (cIndex >= imageData.img_idle.Length) cIndex = 0;
                    img.sprite = imageData.img_idle[cIndex];
                }
            }
        }
        if (hideHitBox)
            if (imageData.img_loseHitBox != null)
                img.sprite = imageData.img_loseHitBox;

        if (stateData[cState].dynamicPos != null)
            pos = stateData[cState].dynamicPos.localPosition;

        if (isFlash)
        {
            flashTime += Time.deltaTime;
            if (flashTime > 0.02f && flashTime < 0.04f)
            {
                img.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                GameMgr.Instance.ui.img_enemyTag.color = img.color;
            }
            if (flashTime > 0.04f)
            {
                flashTime = 0.0f;
                isFlash = false;
                img.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                GameMgr.Instance.ui.img_enemyTag.color = img.color;
                if (main.se_hit != null && isLessThan20Percent)
                    SoundMgr.Instance.PlaySE(main.se_hit);
            }
        }

        if (isBattle)
        {
            stateTime += Time.deltaTime;
            if (stateTime > stateData[cState].lifetime)
            {
                stateTime = 0.0f;
                ++cState;
                if (cState >= stateData.Length) cState = 0;
                SetState();
            }
        }

        if (isFillHp)
        {
            mask_hp.alphaCutoff -= main.fillHpSpeed * Time.deltaTime;
            if (mask_hp.alphaCutoff <= 0.0f)
            {
                isFillHp = false;
                isBattle = true;
            }
        }
    }

    void FixedUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, pos, ref cSpeed, stateData[cState].smoothTime, stateData[cState].maxSpeed, Time.fixedDeltaTime);
    }
}
