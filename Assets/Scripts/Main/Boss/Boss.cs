using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public BossMover[] element;
    public SpriteRenderer img;
    public Animator ani_spellcards, ani_img;
    [Tooltip("所有boss所用的子弹刷新器都要放在此处")]
    public BulletSpawner[] spellcardBulletSpawners;
    [Tooltip("boss的名字")]
    public string bossName;
    [Tooltip("boss战斗中所启动的特效")]
    public GameObject handle_battleFx;
    [Tooltip("boss填充血条动画的速度，填充血条时无敌（例如：为1时1秒填满，为2时0.5秒填满）")]
    public float fillHpSpeed;
    [Tooltip("准备开始符卡时，等待的时间（在时间内boss无敌）（如果有对话的话，对话结束后立刻开始）")]
    public float spellcardDelay;
    [Tooltip("所有符卡击破时慢动作时间")]
    public float endSlowTime;
    [Tooltip("序号为0的boss在第一次出现时使用什么动作（可以为空，则使用默认移动方式）")]
    public BossMover.StateData[] firstStateData;
    [Tooltip("未进入符卡战斗的boss的默认移动方式")]
    public BossMover.StateData[] defaultStateData;
    [Tooltip("是否启动boss的击破特效（如果想要开始特效，请自己在符卡里加）")]
    public bool defeatFxBullet;
    [Tooltip("boss的击破特效序号")]
    public int defeatFxBulletIndex;
    [Tooltip("boss出现时的对话（按照角色标签对应存放）（可以为空）")]
    public AIAVGEng_Script[] script_begin;
    [Tooltip("boss结束时的对话（按照角色标签对应存放）（可以为空）")]
    public AIAVGEng_Script[] script_end;
    [Tooltip("符卡开始音效（可以为空）")]
    public AudioClip se_begin;
    [Tooltip("boss血量低于20%时播放击中音效（可以为空）")]
    public AudioClip se_hit;
    [Tooltip("符卡击破音效（可以为空）")]
    public AudioClip se_defeat;
    [Tooltip("boss击破音效（可以为空）")]
    public AudioClip se_end;
    [Tooltip("上面音效的间隔时间")]
    public float seDelay;
    [System.Serializable]
    public class Spellcard
    {
        [Tooltip("符卡名")]
        public string name;
        [Tooltip("此符卡开始时的背景立绘")]
        public Sprite img;
        [Tooltip("此符卡开始时启动的背景，符卡结束时重新禁用（可以为空）")]
        public GameObject handle_background;
        [Tooltip("符卡开始时切换bgm，如果为空则保留上一个")]
        public AudioClip bgm;
        [Tooltip("参战的boss的序号。特效，掉落物，以及boss位置标记将会以第0个boss为基础")]
        public int[] indexes;
        [Tooltip("与boss移动有关的数据")]
        public BossMover.StateData[] stateData;
        [Tooltip("此符卡关联的动画所对的Trigger名（符卡击破的Trigger名是Defeat）")]
        public string triggerName;
        [Tooltip("此符卡的限定时间")]
        public int lifetime;
        [Tooltip("此符卡的血条")]
        public int maxHp;
        [Tooltip("符卡击破时的掉落（如果不是时符，限定时间内boss没有击破，则不掉落）")]
        public ItemSpawner.ItemDrop itemDrop;
        [Tooltip("此符卡的收卡奖分最大值")]
        public int maxBonus;
        [Tooltip("此符卡是否是时符（boss无敌且收卡奖分不会减少）")]
        public bool isTime;
        [Tooltip("此符卡每0.1秒所减的收卡奖分")]
        public int loseBonus;
        [Tooltip("禁止boss撞击使自机死亡")]
        public bool disableHitPlayer;
        [Tooltip("boss在自机无敌时失去判定点，此时请不要操作bossMover的hide")]
        public bool loseHitBoxOnGod;
    }
    [Tooltip("该boss所准备的符卡")]
    public Spellcard[] spellcards;

    public void Next()
    {
        if (cSpellcardIndex == 0)
        {
            StageLoader.Instance.LoadBGMWithName(StageLoader.Instance.currentStage.bgm_boss, StageLoader.Instance.currentStage.bgm_boss_name);
        }
        cLifetime = spellcards[cSpellcardIndex].lifetime;
        cBonus = spellcards[cSpellcardIndex].maxBonus;
        GameMgr.Instance.checkLoseSpellcard = false;
        GameMgr.Instance.player.handle_spellcardBomb.SetActive(false);
        GameMgr.Instance.ui.slider_spellcard.value = spellcards.Length - cSpellcardIndex - 1;
        GameMgr.Instance.ui.txt_spellcardName.text = spellcards[cSpellcardIndex].name;
        GameMgr.Instance.ui.txt_spellcardBonus.text = cBonus.ToString();
        GameMgr.Instance.ui.txt_spellcardHistory.text = "未开放";
        if (cLifetime > 99)
            GameMgr.Instance.ui.txt_timeRemain.text = "99";
        else
            GameMgr.Instance.ui.txt_timeRemain.text = cLifetime.ToString("00");
        GameMgr.Instance.ui.ani_spellcardInfo.SetBool("enabled", true);
        img.sprite = spellcards[cSpellcardIndex].img;
        ani_img.SetTrigger("show");
        handle_battleFx.SetActive(true);
        if (spellcards[cSpellcardIndex].handle_background != null)
            spellcards[cSpellcardIndex].handle_background.SetActive(true);
        ani_spellcards.SetTrigger(spellcards[cSpellcardIndex].triggerName);
        GameMgr.Instance.bossHitPlayer = !spellcards[cSpellcardIndex].disableHitPlayer;
        if (!spellcards[cSpellcardIndex].disableHitPlayer)
            GameMgr.Instance.aimingBoss = element[spellcards[cSpellcardIndex].indexes[0]].transform;
        GameMgr.Instance.RefreshAimingEnemy();
        for (int i = 0; i < element.Length; ++i)
        {
            element[i].stateData = defaultStateData;
            element[i].SetState();
        }
        for (int i = 0; i < spellcards[cSpellcardIndex].indexes.Length; ++i)
        {
            element[spellcards[cSpellcardIndex].indexes[i]].isTime = spellcards[cSpellcardIndex].isTime;
            element[spellcards[cSpellcardIndex].indexes[i]].itemDrop = spellcards[cSpellcardIndex].itemDrop;
            element[spellcards[cSpellcardIndex].indexes[i]].hideHitBox = spellcards[cSpellcardIndex].disableHitPlayer;
            element[spellcards[cSpellcardIndex].indexes[i]].BeginBattle(spellcards[cSpellcardIndex].maxHp, spellcards[cSpellcardIndex].stateData);
        }
        if (spellcards[cSpellcardIndex].bgm != null)
            SoundMgr.Instance.PlayBGM(spellcards[cSpellcardIndex].bgm);
        if (se_begin != null)
            SoundMgr.Instance.PlaySE(se_begin);
    }

    public void Defeat()
    {
        cLifetime = 0.0f;
        for (int i = 0; i < spellcards[cSpellcardIndex].indexes.Length; ++i)
            element[spellcards[cSpellcardIndex].indexes[i]].Defeat();
        ani_spellcards.SetTrigger("cancel");
        GameMgr.Instance.player.handle_spellcardBomb.SetActive(true);
        GameMgr.Instance.aimingBoss = null;
        GameMgr.Instance.RefreshAimingEnemy();
        if (!GameMgr.Instance.checkLoseSpellcard)
            GameMgr.Instance.ScoreChange(cBonus);
        GameMgr.Instance.ui.txt_getSpellcardBonus.text = cBonus.ToString("N0");
        if (spellcards[cSpellcardIndex].isTime)
            ItemSpawner.Instance.Spawn(element[spellcards[cSpellcardIndex].indexes[0]].transform.localPosition, spellcards[cSpellcardIndex].itemDrop);

        handle_battleFx.SetActive(false);
        GameMgr.Instance.ui.ani_timeRemain.SetBool("enabled", false);
        GameMgr.Instance.ui.ani_spellcardBonus.SetTrigger(GameMgr.Instance.checkLoseSpellcard ? "failed" : "get");
        GameMgr.Instance.ui.ani_spellcardInfo.SetBool("enabled", false);
        if (spellcards[cSpellcardIndex].handle_background != null)
            spellcards[cSpellcardIndex].handle_background.SetActive(false);
        if (defeatFxBullet)
        {
            BulletSpawner defeatFxBulletSpawner = (BulletSpawner)StageLoader.Instance.fxBulletSpawnerPool.Get();
            defeatFxBulletSpawner.bulletPool = StageLoader.Instance.enemyBulletPool;
            defeatFxBulletSpawner.transform.localPosition = element[spellcards[cSpellcardIndex].indexes[0]].transform.localPosition;
            defeatFxBulletSpawner.stateData = StageLoader.Instance.currentStage.bulletDataBase[defeatFxBulletIndex].bulletSpawnerStateData;
            defeatFxBulletSpawner.ShootOnce();
        }

        ++cSpellcardIndex;
        if (cSpellcardIndex >= spellcards.Length)
        {
            Time.timeScale = 0.2f;
            GameMgr.Instance.bossEnd = true;
            isSlow = true;
            if (se_end != null)
                SoundMgr.Instance.PlaySE(se_end);
        }
        else
        {
            isNextSpellcard = true;
        }
        if (se_defeat != null)
            SoundMgr.Instance.PlaySE(se_defeat);
    }

    public void DialogEnd()
    {
        isWaitForDialog = false;
        if (cSpellcardIndex >= spellcards.Length)
        {
            BossEnd();
        }
        else
        {
            ++cSpellcardIndex;
            Next();
        }
    }

    public void BossEnd()
    {
        GameMgr.Instance.bossEnd = false;
        GameMgr.Instance.ui.img_enemyTag.gameObject.SetActive(false);
        GameMgr.Instance.ScoreChange(StageLoader.Instance.currentStage.stageBonus);
        GameMgr.Instance.ui.txt_stageBonus.text = StageLoader.Instance.currentStage.stageBonus.ToString("N0");
        GameMgr.Instance.ui.ani_stageBonus.SetTrigger("show");
        //todo record
        GameMgr.Instance.ui.handle_bossInfo.SetActive(false);
        isQueryChangeStage = true;
    }

    float nextSpellcardTime, queryChangeStageTime, loseBonusTime, slowTime, lifetimeTime;
    float cLifetime;
    int cSpellcardIndex, cBonus;
    bool isNextSpellcard, isQueryChangeStage, isSlow, isWaitForDialog;

    void Awake()
    {
        nextSpellcardTime = 0.0f;
        queryChangeStageTime = 0.0f;
        loseBonusTime = 0.0f;
        slowTime = 0.0f;
        cLifetime = 0.0f;
        cSpellcardIndex = 0;
        cBonus = 0;
        isNextSpellcard = false;
        isQueryChangeStage = false;
        isSlow = false;
        isWaitForDialog = false;
        GameMgr.Instance.bossHitPlayer = false;
        for (int i = 0; i < spellcardBulletSpawners.Length; ++i)
            spellcardBulletSpawners[i].bulletPool = StageLoader.Instance.enemyBulletPool;
    }

    void Start()
    {
        GameMgr.Instance.ui.txt_bossName.text = bossName;
        GameMgr.Instance.ui.slider_spellcard.value = spellcards.Length;
        GameMgr.Instance.ui.handle_bossInfo.SetActive(true);
        GameMgr.Instance.ui.img_enemyTag.gameObject.SetActive(true);
        if (firstStateData.Length == 0)
        {
            element[0].stateData = defaultStateData;
            element[0].SetState();
        }
        else
        {
            element[0].stateData = firstStateData;
            element[0].SetState();
        }
        for (int i = 1; i < element.Length; ++i)
        {
            element[i].stateData = defaultStateData;
            element[i].SetState();
        }
        if (GameMgr.Instance.CurrentPlayer < script_begin.Length && script_begin[GameMgr.Instance.CurrentPlayer] != null)
        {
            --cSpellcardIndex;
            isWaitForDialog = true;
            AIAVGEng.Instance.OnScriptEnd = DialogEnd;
            AIAVGEng.Instance.Run(script_begin[GameMgr.Instance.CurrentPlayer]);
        }
        else
        {
            Next();
        }
    }

    void Update()
    {
        if (!isWaitForDialog && !isQueryChangeStage && cSpellcardIndex < spellcards.Length)
        {
            GameMgr.Instance.ui.img_enemyTag.transform.localPosition = new Vector3(element[spellcards[cSpellcardIndex].indexes[0]].transform.localPosition.x, GameMgr.Instance.ui.img_enemyTag.transform.localPosition.y);

            if (isNextSpellcard && !isWaitForDialog)
            {
                nextSpellcardTime += Time.deltaTime;
                if (nextSpellcardTime > spellcardDelay)
                {
                    nextSpellcardTime = 0.0f;
                    isNextSpellcard = false;
                    Next();
                }
            }
            else
            {
                if (spellcards[cSpellcardIndex].loseHitBoxOnGod)
                    for (int i = 0; i < spellcards[cSpellcardIndex].indexes.Length; ++i)
                        element[spellcards[cSpellcardIndex].indexes[i]].hideHitBox = GameMgr.Instance.IsGod;
                lifetimeTime += Time.deltaTime;
                if (lifetimeTime > 1.0f)
                {
                    cLifetime -= lifetimeTime;
                    lifetimeTime = 0.0f;
                    if (cLifetime > 99)
                        GameMgr.Instance.ui.txt_timeRemain.text = "99";
                    else
                        GameMgr.Instance.ui.txt_timeRemain.text = cLifetime.ToString("00");
                    if (cLifetime < 10 && cLifetime > 0)
                    {
                        GameMgr.Instance.ui.ani_timeRemain.SetBool("enabled", true);
                        if (GameMgr.Instance.ui.se_timeRemain != null)
                            SoundMgr.Instance.PlaySE(GameMgr.Instance.ui.se_timeRemain);
                    }
                    else
                    {
                        GameMgr.Instance.ui.ani_timeRemain.SetBool("enabled", false);
                    }
                    if (cLifetime <= 0)
                        Defeat();
                }
                loseBonusTime += Time.deltaTime;
                if (loseBonusTime > 0.1f)
                {
                    loseBonusTime = 0.0f;
                    if (GameMgr.Instance.checkLoseSpellcard)
                    {
                        GameMgr.Instance.ui.txt_spellcardBonus.text = "Failed";
                    }
                    else
                    {
                        if (cSpellcardIndex < spellcards.Length) //这里cSpellcardIndex莫名其妙地就会加1，只好加if
                            cBonus -= spellcards[cSpellcardIndex].loseBonus;
                        GameMgr.Instance.ui.txt_spellcardBonus.text = cBonus.ToString();
                    }
                }
            }
        }
        else
        {
            if (!isWaitForDialog)
            {
                queryChangeStageTime += Time.deltaTime;
                if (queryChangeStageTime > 5.0f)
                {
                    queryChangeStageTime = 0.0f;
                    enabled = false;
                    StageLoader.Instance.LoadNextStage();
                }
            }
        }

        if (isSlow)
        {
            slowTime += Time.unscaledDeltaTime;
            if (slowTime > endSlowTime)
            {
                isSlow = false;
                Time.timeScale = 1.0f;
                GameMgr.Instance.player.ani_stageShake.SetTrigger("shake");
                if (GameMgr.Instance.CurrentPlayer < script_end.Length && script_end[GameMgr.Instance.CurrentPlayer] != null)
                {
                    //--cSpellcardIndex;
                    isWaitForDialog = true;
                    AIAVGEng.Instance.OnScriptEnd = DialogEnd;
                    AIAVGEng.Instance.Run(script_end[GameMgr.Instance.CurrentPlayer]);
                }
                else
                {
                    BossEnd();
                }
            }
        }
    }

    void OnDestroy()
    {
        if (GameMgr.Instance != null)
            GameMgr.Instance.player.handle_spellcardBomb.SetActive(false);
    }
}
