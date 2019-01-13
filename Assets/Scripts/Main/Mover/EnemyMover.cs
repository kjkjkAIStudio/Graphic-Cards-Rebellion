using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : Mover
{
    public BulletSpawner bulletSpawner;
    [Serializable]
    public class EnemyStateData
    {
        [Tooltip("血量，为0时正常回收，无论是否无敌")]
        public int maxHp;
        [Tooltip("无敌。仍然有碰撞体积")]
        public bool god;
        [Tooltip("是否使用子弹刷新器")]
        public bool isSpawnBullet;
        [Tooltip("所用的子弹刷新器在DataBase的序号")]
        public int bulletSpawnerIndex;
        [Tooltip("待机图片（至少要有一张）（Mover中的图片会被覆盖，请把Mover中的颜色设为白色不透明）")]
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
        [Tooltip("回收音效（可以为空）")]
        public AudioClip se_die;
    }
    public EnemyStateData enemyStateData;
    public ItemSpawner.ItemDrop itemDrop;

    [HideInInspector]
    public int activeIndex;

    public void Damage(int value)
    {
        if (!enemyStateData.god)
            hp -= value;
        isFlash = true;
        if (hp <= 0)
        {
            if (isItemDrop) //当两颗子弹同时击中对象，并且都致死的话，会导致掉落两遍，故增加
                return;
            isItemDrop = true;
            ItemSpawner.Instance.Spawn(transform.localPosition, itemDrop);
            GameMgr.Instance.ScoreChange(GameMgr.Instance.player.defeatScore);
            QueryRecycle();
        }
    }

    public override void SetState(int state)
    {
        base.SetState(state);
        GameMgr.Instance.RegistEnemy(transform);
    }

    public override void Reset()
    {
        base.Reset();
        bulletSpawner.Reset();
        flashTime = 0.0f;
        imgTime = 0.0f;
        cIndex = 0;
        activeIndex = -1;
        isItemDrop = false;
    }

    protected override void SetState()
    {
        base.SetState();
        hp = enemyStateData.maxHp;
        img.sprite = enemyStateData.img_idle[0];
        if (enemyStateData.isSpawnBullet)
        {
            bulletSpawner.stateData = StageLoader.Instance.currentStage.bulletDataBase[enemyStateData.bulletSpawnerIndex].bulletSpawnerStateData;
            bulletSpawner.SetState(0);
            bulletSpawner.enabled = true;
        }
    }

    public override void QueryRecycle()
    {
        if (!isQueryRecycle)
        {
            GameMgr.Instance.UnregistEnemy(transform);
            bulletSpawner.enabled = false;
            img.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            isFlash = false;
            if (enemyStateData.se_die != null)
                SoundMgr.Instance.PlaySE(enemyStateData.se_die);
            base.QueryRecycle();
        }
    }

    float imgTime, flashTime;
    int cIndex;
    int hp;
    bool isFlash, isItemDrop;

    protected override void Awake()
    {
        base.Awake();
        bulletSpawner.enabled = false;
        flashTime = 0.0f;
        imgTime = 0.0f;
        cIndex = 0;
        activeIndex = -1;
        isFlash = false;
        isItemDrop = false;
    }

    protected override void Start()
    {
        base.Start();
        hp = enemyStateData.maxHp;
        img.sprite = enemyStateData.img_idle[0];
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (!isQueryRecycle && target.tag == "player_bullet" && !target.GetComponent<Mover>().IsQueryRecycle)
        {
            Mover temp = target.GetComponent<Mover>();
            Damage(temp.initData.damage);
            temp.QueryRecycle();
        }
    }

    protected override void Update()
    {
        base.Update();
        imgTime += Time.deltaTime;
        if (cSpeed.x < -5.0f && enemyStateData.img_left.Length != 0)
        {
            if (imgTime > enemyStateData.leftDelay)
            {
                imgTime = 0.0f;
                ++cIndex;
                if (cIndex >= enemyStateData.img_left.Length) cIndex = 0;
                img.sprite = enemyStateData.img_left[cIndex];
            }
        }
        else
        {
            if (cSpeed.x > 5.0f && enemyStateData.img_right.Length != 0)
            {
                if (imgTime > enemyStateData.rightDelay)
                {
                    imgTime = 0.0f;
                    ++cIndex;
                    if (cIndex >= enemyStateData.img_right.Length) cIndex = 0;
                    img.sprite = enemyStateData.img_right[cIndex];
                }
            }
            else
            {
                if (imgTime > enemyStateData.idleDelay)
                {
                    imgTime = 0.0f;
                    ++cIndex;
                    if (cIndex >= enemyStateData.img_idle.Length) cIndex = 0;
                    img.sprite = enemyStateData.img_idle[cIndex];
                }
            }
        }

        if (isFlash)
        {
            flashTime += Time.deltaTime;
            if (flashTime > 0.02f && flashTime < 0.04f)
            {
                img.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            }
            if (flashTime > 0.04f)
            {
                flashTime = 0.0f;
                isFlash = false;
                img.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }
}
