using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public Main_optCheck optCheck;
    [Tooltip("是否接受用户的输入")]
    public bool isFocused;
    [Serializable]
    public class Player
    {
        public Transform playerPos;
        public Transform bombs;
        public BulletSpawner playerBulletSpawner;
        public GameObject handle_stageCollect, handle_spellcardBomb;
        public SpriteRenderer img_c, img_l, img_r;
        public PlayerImage playerImage;
        [Serializable]
        public class Sub
        {
            public GameObject handle;
            public SpriteRenderer img;
            public Transform sub;
            public BulletSpawner subBulletSpawner;
        }
        public Sub[] subs;
        public Animator ani_player, ani_player_slow, ani_bomb_info, ani_stageShake;
        [Tooltip("自机的出生位置")]
        public Vector2 spawnPoint;
        [Tooltip("自机出生时移动到的目标位置")]
        public Vector2 spawnToPoint;
        [Tooltip("自机出生时移动的最大速度")]
        public float spawnMaxSpeed;
        [Tooltip("自机出生时移动平滑速度")]
        public float spawnSmoothTime;
        [Tooltip("自机出生时到可操控时的时间")]
        public float unlockTime;
        [Tooltip("自机的活动范围除以2")]
        public Vector2 halfRange;
        [Tooltip("移动速度")]
        public float speed;
        [Tooltip("慢速移动速度")]
        public float slowSpeed;
        [Tooltip("切换慢速时自机移到目标点的速度")]
        public float subSpeed;
        [Tooltip("切换慢速时自机移到目标点的速度中的平滑时间")]
        public float subSmoothTime;
        [Tooltip("无敌时间（在自机第一次出现时）")]
        public float godTime;
        [Tooltip("开局时的残机数，开局时减1")]
        public int initLife;
        [Tooltip("复活时的炸弹数")]
        public int initBomb;
        [Tooltip("最大残机碎片数")]
        public int maxLifePiece;
        [Tooltip("最大炸弹碎片数")]
        public int maxBombPiece;
        [Tooltip("最大Power数（例如显示的最大Power为4.00时，此处应该写400。达到1000会造成显示Bug）")]
        public int maxPower;
        [Tooltip("掉残时扣除Power数")]
        public int losePower;
        [Tooltip("全屏收点高度")]
        public float itemCollectY;
        [Tooltip("得点道具最大分值，在全屏收点高度上最大")]
        public int itemScoreMax;
        [Tooltip("得点道具分值在自机的Y坐标每下降1时，损失的分数")]
        public int itemScorePerY;
        [Tooltip("小得点道具（自机消弹时，每个子弹产生）分值")]
        public int itemScoreSmall;
        [Tooltip("击坠分值（不包括boss）")]
        public int defeatScore;
        [Tooltip("擦弹分值")]
        public int grazeScore;
        [Tooltip("全通后每个残机加分")]
        public int lifeScore;
        [Tooltip("全通后每个bomb加分")]
        public int bombScore;
        [Tooltip("自机撞弹时启动的子弹效果")]
        public BulletSpawner.StateData[] dieFxBulletSpawnerStateData;
        [Tooltip("拾取道具时的声音")]
        public AudioClip se_item;
        [Tooltip("擦弹时的声音")]
        public AudioClip se_graze;
        [Tooltip("为了防止声音嘈杂，上面所有声音的发出间隔")]
        public float se_delay;
        [Tooltip("增加子机时的声音")]
        public AudioClip se_addSub;
        [Tooltip("bomb时的声音")]
        public AudioClip se_bomb;
        [Tooltip("增加残机或炸弹时的声音")]
        public AudioClip se_pieceCombine;
        [Tooltip("撞弹的声音")]
        public AudioClip se_queryDie;
    }
    [Tooltip("与自机相关的数据")]
    public Player player;
    [Serializable]
    public class Character
    {
        [Tooltip("角色的中间贴图")]
        public Sprite img_center;
        [Tooltip("角色的左贴图")]
        public Sprite img_left;
        [Tooltip("角色的右贴图")]
        public Sprite img_right;
        [Tooltip("角色的子机贴图")]
        public Sprite img_sub;
        [Tooltip("角色子机相对于角色的Y坐标")]
        public float subYPos;
        [Tooltip("角色子机相对于角色的Y坐标（慢速时）")]
        public float slowSubYPos;
        [Tooltip("角色子机的间距")]
        public float subDistance;
        [Tooltip("角色子机的间距（慢速时）")]
        public float slowSubDistance;
        [Tooltip("相邻角色子级的旋转角度")]
        public float subRotation;
        [Tooltip("相邻角色子级的旋转角度")]
        public float subRotationSlow;
        [Tooltip("如果角色带有自机狙或跟踪型的子弹刷新器的话，勾上")]
        public bool refreshAimingEnemy;
        [Tooltip("计算最近的敌机的频率。（过小会加大算力，过大会使跟踪弹迟钝）")]
        public float refreshAimingEnemyDelay;
        [Tooltip("角色的主炮子弹刷新器的参数（警告：禁止打开任何刷新器）")]
        public BulletSpawner.StateData[] playerBulletState;
        [Tooltip("角色的子机子弹刷新器的参数（警告：禁止打开任何刷新器）")]
        public BulletSpawner.StateData[] subBulletState;
        [Tooltip("无敌时间（释放炸弹时）")]
        public float bombGodTime;
        [Tooltip("角色的bomb")]
        public GameObject prefab_bomb;
        [Tooltip("角色的bomb符卡名")]
        public string bombName;
        [Tooltip("角色的bomb立绘图")]
        public Sprite img_bombImg;
    }
    [Tooltip("根据角色不同，自定义的数据。请按照Global中的角色标签顺序定义")]
    public Character[] characters;
    [Serializable]
    public class UI
    {
        public Animator ani_spellcardInfo, ani_spellcardBonus, ani_timeRemain, ani_stageBonus, ani_pause, ani_die;
        public Text txt_highscore, txt_score, txt_lifePiece, txt_bombPiece, txt_power, txt_graze, txt_bombName, txt_bossName, txt_spellcardName, txt_spellcardBonus, txt_spellcardHistory, txt_timeRemain, txt_getSpellcardBonus, txt_stageBonus;
        public Text txt_endBonus;
        public Slider slider_life, slider_bomb, slider_spellcard;
        public GameObject handle_normal, handle_toxic, handle_extra, handle_bossInfo;
        public Image img_lifePiece, img_bombPiece;
        public Image img_bombImg;
        public Image img_enemyTag;
        public AudioClip se_timeRemain;
    }
    public UI ui;

    [HideInInspector]
    public Transform aimingEnemy; //当前瞄准的敌机
    [HideInInspector]
    public Transform aimingBoss; //当前瞄准的Boss
    [HideInInspector]
    public bool checkLoseSpellcard; //供Boss检查是否收卡
    [HideInInspector]
    public bool bossEnd; //是否开启全屏收点
    [HideInInspector]
    public bool bossHitPlayer; //boss是否可以撞击玩家

    public static GameMgr Instance { get; private set; }
    public int CurrentPlayer { get { return cPlayer; } }
    public bool IsFullPower { get; private set; }
    public bool IsGod { get { return isGod || isBomb; } }
    public bool IsDieContinue { get; private set; }
    public bool IsEndBonus { get; private set; }

    public enum ItemType { Power, Score, ScoreSmall, BombPiece, LifePiece }

    public void RefreshUI()
    {
        ui.txt_score.text = score.ToString("###,###,##0");
        ui.txt_lifePiece.text = lifePiece.ToString() + "/" + player.maxLifePiece;
        ui.txt_bombPiece.text = bombPiece.ToString() + "/" + player.maxBombPiece;
        ui.txt_power.text = power.ToString("000").Insert(1, ".") + powerTail;
        ui.txt_graze.text = graze.ToString();
        ui.slider_life.value = life;
        ui.slider_bomb.value = bomb;
        ui.img_lifePiece.rectTransform.anchoredPosition = new Vector2(ui.img_lifePiece.rectTransform.sizeDelta.x * life, 0.0f);
        ui.img_lifePiece.fillAmount = (float)lifePiece / player.maxLifePiece;
        ui.img_bombPiece.rectTransform.anchoredPosition = new Vector2(ui.img_bombPiece.rectTransform.sizeDelta.x * bomb, 0.0f);
        ui.img_bombPiece.fillAmount = (float)bombPiece / player.maxBombPiece;
    }

    public void GetItem(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Power:
                PowerChange(1);
                break;
            case ItemType.ScoreSmall:
                score += (uint)player.itemScoreSmall;
                RefreshUI();
                break;
            case ItemType.Score:
                int range = Mathf.FloorToInt(player.itemCollectY - player.playerPos.localPosition.y) * player.itemScorePerY;
                if (range < 0) range = 0;
                uint neededScore = (uint)(player.itemScoreMax - range);
                score += neededScore;
                ScoreMover temp = (ScoreMover)ItemSpawner.Instance.scorePool.Get();
                temp.transform.localPosition = player.playerPos.localPosition;
                temp.txt.text = neededScore.ToString();
                if (range == 0)
                    temp.stateData[0].baseData.color = new Color(1.0f, 1.0f, 0.0f, 1.0f);
                else
                    temp.stateData[0].baseData.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                temp.SetState(0);
                RefreshUI();
                break;
            case ItemType.LifePiece:
                ++lifePiece;
                if (lifePiece >= player.maxLifePiece)
                {
                    life += lifePiece / player.maxLifePiece;
                    lifePiece = lifePiece % player.maxLifePiece;
                    if (player.se_pieceCombine != null)
                        SoundMgr.Instance.PlaySE(player.se_pieceCombine);
                }
                RefreshUI();
                break;
            case ItemType.BombPiece:
                ++bombPiece;
                if (bombPiece >= player.maxBombPiece)
                {
                    bomb += bombPiece / player.maxBombPiece;
                    bombPiece = bombPiece % player.maxBombPiece;
                    if (player.se_pieceCombine != null)
                        SoundMgr.Instance.PlaySE(player.se_pieceCombine);
                }
                RefreshUI();
                break;
        }
        if (player.se_item != null && itemSeTime >= player.se_delay)
        {
            itemSeTime = 0.0f;
            SoundMgr.Instance.PlaySE(player.se_item);
        }
    }

    public void PowerChange(int delta)
    {
        int prevSubCount = power / 100;
        power += delta;
        if (power < 0) power = 0;
        if (power > player.maxPower) power = player.maxPower;
        RefreshUI();

        //处理子机变化
        int neededSubCount = power / 100;
        for (int i = 0; i < player.subs.Length; ++i)
        {
            if (i < neededSubCount)
            {
                player.subs[i].handle.SetActive(true);
            }
            else
            {
                player.subs[i].handle.SetActive(false);
                player.subs[i].sub.localPosition = new Vector3();
            }
        }
        //重新计算子机移动方式
        subTarget = new Vector3[neededSubCount];
        subTargetSlow = new Vector3[neededSubCount];
        subRotation = new float[neededSubCount];
        subRotationSlow = new float[neededSubCount];
        subSpeed = new Vector3[neededSubCount];
        for (int i = 0; i < neededSubCount; ++i)
        {
            float offset = (neededSubCount - 1) * -0.5f + i;
            subTarget[i] = new Vector3(offset * characters[cPlayer].subDistance, characters[cPlayer].subYPos, 0.0f);
            subTargetSlow[i] = new Vector3(offset * characters[cPlayer].slowSubDistance, characters[cPlayer].slowSubYPos, 0.0f);
            subRotation[i] = offset * characters[cPlayer].subRotation;
            subRotationSlow[i] = offset * characters[cPlayer].subRotationSlow;
            subSpeed[i] = new Vector3();
        }

        if (neededSubCount > prevSubCount)
            if (player.se_addSub != null)
                SoundMgr.Instance.PlaySE(player.se_addSub);

        //是否满P
        IsFullPower = power == player.maxPower;
    }

    public void Graze()
    {
        ++graze;
        score += (uint)player.grazeScore;
        RefreshUI();
        if (player.se_item != null && grazeSeTime >= player.se_delay)
        {
            grazeSeTime = 0.0f;
            SoundMgr.Instance.PlaySE(player.se_graze);
        }
    }

    public void ScoreChange(int value)
    {
        score = (uint)((int)score + value);
        RefreshUI();
    }

    public void AllClearScore()
    {
        IsEndBonus = true;
        int lifeBonus = life * player.lifeScore;
        int bombBonus = bomb * player.bombScore;
        score = (uint)((int)score + lifeBonus + bombBonus);
        RefreshUI();
        ui.txt_endBonus.text = string.Format("Bomb left:{0}\n{0} * {1} = {2}\nLife left:{3}\n{3} * {4} = {5}\nFinal score:\n{6}", bomb, player.bombScore, bombBonus, life, player.lifeScore, lifeBonus, score);
    }

    public void ScoreUpdate()
    {
        optCheck.UpdateScoreAndDisplay(score);
    }

    public void DieContinue()
    {
        IsDieContinue = true;
        life = player.initLife;
        bomb = player.initBomb;
        Time.timeScale = 1.0f;
        ui.ani_die.SetBool("enabled", false);
    }

    public void Spawn()
    {
        --life;
        player.playerImage.isGod = true;
        player.playerPos.localPosition = player.spawnPoint;
        isQueryDie = false;
        isGod = true;
        isSpawn = true;
        bomb = player.initBomb;
        PowerChange(-player.losePower);
        RefreshUI();
        checkLoseSpellcard = true;
    }

    public void QueryDie()
    {
        if (!isQueryDie)
        {
            isQueryDie = true;
            player.ani_player.SetTrigger("die");
            if (player.dieFxBulletSpawnerStateData.Length != 0)
            {
                BulletSpawner dieFxBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
                dieFxBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
                dieFxBulletSpawner.transform.localPosition = player.playerPos.localPosition;
                dieFxBulletSpawner.stateData = player.dieFxBulletSpawnerStateData;
                dieFxBulletSpawner.ShootOnce();
            }
            if (player.se_queryDie != null)
                SoundMgr.Instance.PlaySE(player.se_queryDie);
        }
    }

    public void Die()
    {
        if (isBomb) return;
        isFocused = false;
        if (life <= 0)
        {
            Time.timeScale = 0.0f;
            optCheck.isDie = true;
            optCheck.opt_die.isFocused = true;
            ui.ani_die.SetBool("enabled", true);
            ScoreUpdate();
        }
    }

    public void RegistEnemy(Transform transform)
    {
        if (characters[cPlayer].refreshAimingEnemy)
        {
            activeEnemy.Add(transform);
        }
    }

    public void UnregistEnemy(Transform transform)
    {
        for (int i = 0; i < activeEnemy.Count; ++i)
            if (transform == activeEnemy[i])
                activeEnemy.RemoveAt(i);
        if (transform == aimingEnemy)
        {
            RefreshAimingEnemy();
        }
    }

    public void RefreshAimingEnemy()
    {
        if (aimingBoss != null)
        {
            aimingEnemy = aimingBoss;
            return;
        }
        if (activeEnemy.Count == 0)
        {
            aimingEnemy = null;
            return;
        }
        int minDistanceIndex = 0;
        float minDistance = (activeEnemy[0].localPosition - player.playerPos.localPosition).magnitude;
        for (int i = 1; i < activeEnemy.Count; ++i)
        {
            float cDistance = (activeEnemy[i].localPosition - player.playerPos.localPosition).magnitude;
            if (cDistance < minDistance)
            {
                minDistanceIndex = i;
                minDistance = cDistance;
            }
        }
        aimingEnemy = activeEnemy[minDistanceIndex];
    }

    List<Transform> activeEnemy;
    Bomb handle_bomb;
    Vector3[] subTarget;
    Vector3[] subTargetSlow;
    Vector3[] subSpeed;
    Vector3 cSpeed, cSpawnSpeed;
    string powerTail;
    float[] subRotation;
    float[] subRotationSlow;
    float itemSeTime, grazeSeTime, godTime, spawnTime, refreshAimingEnemyTime;
    uint score;
    int cPlayer, life, bomb, lifePiece, bombPiece, power, graze;
    bool isSlow, isShoot, isBomb, isGod, isSpawn, isQueryDie;

    void Awake()
    {
        Instance = this;

        //如果没有从Title递送的数据包的话，新建一个默认的
        if (Global.titlePassPack == null)
            Global.titlePassPack = new Global.TitlePassPack();
        if (Global.savePack == null)
            Global.SavePack.Load();

        //角色特性生效
        switch (Global.titlePassPack.character)
        {
            case Global.TitlePassPack.Character.G_2080TI:
                player.maxPower = 500;
                break;
            case Global.TitlePassPack.Character.G_radeon:
                player.itemScoreMax = 15000;
                break;
            case Global.TitlePassPack.Character.G_i9:
                player.maxLifePiece = 2;
                break;
        }

        //变量初始化
        activeEnemy = new List<Transform>();
        subTarget = new Vector3[0];
        subTargetSlow = new Vector3[0];
        subSpeed = new Vector3[0];
        cSpeed = new Vector3();
        cSpawnSpeed = new Vector3();
        powerTail = "<size=40>/" + player.maxPower.ToString("000").Insert(1, ".") + "Hz</size>";
        subRotation = new float[0];
        subRotationSlow = new float[0];
        itemSeTime = 0.0f;
        grazeSeTime = 0.0f;
        godTime = 0.0f;
        spawnTime = player.unlockTime;
        refreshAimingEnemyTime = 0.0f;
        score = 0;
        cPlayer = (int)Global.titlePassPack.character;
        life = player.initLife;
        bomb = player.initBomb;
        lifePiece = 0;
        bombPiece = 0;
        power = 0;
        graze = 0;
        isSlow = false;
        isShoot = false;
        isBomb = false;
        isGod = false;
        isSpawn = false;
        isQueryDie = false;
        IsDieContinue = false;
        IsEndBonus = false;

        //自机初始化
        player.ani_player_slow.Play("player_slow_hide", 0, 1.0f);
        player.img_c.sprite = characters[cPlayer].img_center;
        player.img_l.sprite = characters[cPlayer].img_left;
        player.img_r.sprite = characters[cPlayer].img_right;
        player.playerBulletSpawner.stateData = characters[cPlayer].playerBulletState;
        player.playerBulletSpawner.SetState(0);
        foreach (Player.Sub sub in player.subs)
        {
            sub.img.sprite = characters[cPlayer].img_sub;
            sub.subBulletSpawner.stateData = characters[cPlayer].subBulletState;
            sub.subBulletSpawner.SetState(0);
        }
        handle_bomb = Instantiate(characters[cPlayer].prefab_bomb, player.bombs).GetComponent<Bomb>();
        ui.txt_bombName.text = characters[cPlayer].bombName;
        ui.img_bombImg.sprite = characters[cPlayer].img_bombImg;

        //UI初始化
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                ui.handle_normal.SetActive(true);
                ui.txt_highscore.text = Global.savePack.normalHighscore[0].score.ToString("###,###,##0");
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                ui.handle_toxic.SetActive(true);
                ui.txt_highscore.text = Global.savePack.toxicHighscore[0].score.ToString("###,###,##0");
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                ui.handle_extra.SetActive(true);
                ui.txt_highscore.text = Global.savePack.extraHighscore[0].score.ToString("###,###,##0");
                break;
        }
        RefreshUI();
    }

    void Start()
    {
        StageLoader.Instance.LoadStage(0);
        Spawn();
        isSpawn = false;
        isFocused = false;
    }

    void Update()
    {
        //输入器
        if (isFocused)
        {
            if (isQueryDie)
                cSpeed = new Vector3();
            else
                cSpeed = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal") + ControlPad.Input.pos.x, Input.GetAxis("Vertical") + ControlPad.Input.pos.y), 1.0f);
            isSlow = Input.GetButton("Z") || ControlPad.Input.isZHold;
            if (isSlow) player.ani_player_slow.SetBool("slow", true); else player.ani_player_slow.SetBool("slow", false);
            if (Input.GetButtonDown("C") || ControlPad.Input.isCDown)
            {
                isShoot = !isShoot;
                if (!isShoot)
                {
                    player.playerBulletSpawner.Reset();
                    foreach (Player.Sub sub in player.subs)
                        sub.subBulletSpawner.Reset();
                }
            }
            if ((Input.GetButtonDown("X") || ControlPad.Input.isXDown) && bomb > 0 && !isBomb)
            {
                --bomb;
                RefreshUI();
                godTime = 0.0f;
                isGod = false;
                isBomb = true;
                handle_bomb.transform.localPosition = player.playerPos.localPosition;
                handle_bomb.gameObject.SetActive(true);
                player.playerImage.isGod = true;
                player.ani_bomb_info.SetTrigger("bomb");
                if (player.se_bomb != null)
                    SoundMgr.Instance.PlaySE(player.se_bomb);
                if (isQueryDie)
                {
                    player.ani_player.SetTrigger("cancelDie");
                    isQueryDie = false;
                }
                checkLoseSpellcard = true;
            }
        }
        else
        {
            cSpeed = new Vector3();
            isSlow = false;
            isShoot = false;
        }

        if ((Input.GetKeyDown(KeyCode.Escape) || ControlPad.Input.isEscapeDown) && !StageLoader.Instance.IsLoading && !optCheck.isDie && !IsEndBonus)
        {
            Time.timeScale = 0.0f;
            isFocused = false;
            optCheck.opt_pause.isFocused = true;
            ui.ani_pause.SetBool("enabled", true);
        }

        if (Input.GetKeyDown(KeyCode.F2))
            DebugMgr.Instance.Screenshot();

        //射击
        player.playerBulletSpawner.enabled = isShoot;
        foreach (Player.Sub sub in player.subs)
            sub.subBulletSpawner.enabled = isShoot;

        //全屏收点
        player.handle_stageCollect.SetActive(player.playerPos.localPosition.y > player.itemCollectY || isBomb || bossEnd);

        //自机动画
        if (cSpeed.x < 0.0f) player.ani_player.SetBool("left", true); else player.ani_player.SetBool("left", false);
        if (cSpeed.x > 0.0f) player.ani_player.SetBool("right", true); else player.ani_player.SetBool("right", false);

        //子机动画
        if (subSpeed.Length != 0)
        {
            for (int i = 0; i < subSpeed.Length; ++i)
            {
                player.subs[i].sub.localPosition = Vector3.SmoothDamp(player.subs[i].sub.localPosition, isSlow ? subTargetSlow[i] : subTarget[i], ref subSpeed[i], player.subSmoothTime, player.subSpeed, Time.deltaTime);
                player.subs[i].sub.localEulerAngles = new Vector3(0.0f, 0.0f, isSlow ? subRotationSlow[i] : subRotation[i]);
            }
        }

        //声音间隔处理
        itemSeTime += Time.deltaTime;
        if (itemSeTime > player.se_delay)
            itemSeTime = player.se_delay;
        grazeSeTime += Time.deltaTime;
        if (grazeSeTime > player.se_delay)
            grazeSeTime = player.se_delay;

        //出生处理
        if (isSpawn)
        {
            spawnTime += Time.deltaTime;
            player.playerPos.localPosition = Vector3.SmoothDamp(player.playerPos.localPosition, player.spawnToPoint, ref cSpawnSpeed, player.spawnSmoothTime, player.spawnMaxSpeed, Time.deltaTime);
            if (spawnTime > player.unlockTime)
            {
                spawnTime = 0.0f;
                isSpawn = false;
                isFocused = true;
            }
        }

        //无敌处理
        if (isGod)
        {
            godTime += Time.deltaTime;
            if (godTime > player.godTime)
            {
                godTime = 0.0f;
                isGod = false;
                player.playerImage.isGod = false;
            }
        }

        //bomb处理，仅处理无敌
        if (isBomb)
        {
            godTime += Time.deltaTime;
            if (godTime > characters[cPlayer].bombGodTime)
            {
                godTime = 0.0f;
                isBomb = false;
                player.playerImage.isGod = false;
            }
        }

        //更新目标敌机
        if (characters[cPlayer].refreshAimingEnemy && activeEnemy.Count != 0)
        {
            refreshAimingEnemyTime += Time.deltaTime;
            if (refreshAimingEnemyTime > characters[cPlayer].refreshAimingEnemyDelay)
            {
                refreshAimingEnemyTime = 0.0f;
                RefreshAimingEnemy();
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 temp = player.playerPos.localPosition + cSpeed * (isSlow ? player.slowSpeed : player.speed) * Time.fixedDeltaTime;
        player.playerPos.localPosition = new Vector3(Mathf.Clamp(temp.x, -player.halfRange.x, player.halfRange.x), Mathf.Clamp(temp.y, -player.halfRange.y, player.halfRange.y));
    }
}
