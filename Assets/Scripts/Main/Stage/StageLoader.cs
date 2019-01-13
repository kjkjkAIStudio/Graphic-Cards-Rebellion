using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageLoader : MonoBehaviour
{
    public GameObjectPool enemyPool, enemyBulletPool, fxBulletSpawnerPool;
    public Animator ani_loading, ani_bgmName;
    public Text txt_bgmName;
    [Tooltip("Stage的总数，非Extra难度。")]
    public int totalNoExtraStage;
    [Tooltip("Stage的总数，Extra难度")]
    public int totalExtraStage;
    [Tooltip("测试用场景，请务必在生成前清空")]
    public GameObject[] prefab_stages_test;
    [Tooltip("测试模式，使用测试Stage，请务必在生成前关闭")]
    public bool testMode;
    [Tooltip("测试模式下的power值")]
    public int testPower;

    public static StageLoader Instance { get; private set; }
    [HideInInspector]
    public StageDataBase currentStage;

    public bool IsLoading { get; private set; }

    public void LoadStage(int index)
    {
        IsLoading = true;
        Time.timeScale = 0.0f;
        int cDifficultyStageCount = 0;
        if (testMode)
        {
            cDifficultyStageCount = prefab_stages_test.Length;
        }
        else
        {
            cDifficultyStageCount = cTotalStage;
        }
        if (index < cDifficultyStageCount)
        {
            cStage = index;
            GameMgr.Instance.isFocused = false;
            if (currentStage != null)
            {
                DestroyImmediate(currentStage.gameObject, true);
                currentStage = null;
            }
            ani_loading.SetBool("loading", true);
        }
        else
        {
            //最后一个场景完成。解锁ex，登记额外加分，登记最终分数
            if (!GameMgr.Instance.IsDieContinue)
                Global.savePack.characterCanExtra[(int)Global.titlePassPack.character] = true;
            GameMgr.Instance.AllClearScore();
            GameMgr.Instance.optCheck.handle_endBonus.SetActive(true);
        }
    }

    public void LoadNextStage()
    {
        LoadStage(cStage + 1);
    }

    public void Ani_Loading_End()
    {
        if (loadScene == -1)
        {
            if (testMode)
            {
                Instantiate(prefab_stages_test[cStage], transform);
            }
            else
            {
                StartCoroutine(LoadStageFromPack());
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(loadScene);
        }
    }


    public void LoadStageFinish(StageDataBase database)
    {
        currentStage = database;
        IsLoading = false;
        Time.timeScale = 1.0f;
        GameMgr.Instance.isFocused = true;
        ani_loading.SetBool("loading", false);
        LoadBGMWithName(database.bgm_stage, database.bgm_stage_name);
    }

    public void LoadScene(int index)
    {
        loadScene = index;
        IsLoading = true;
        Time.timeScale = 0.0f;
        GameMgr.Instance.isFocused = false;
        if (currentStage != null)
        {
            DestroyImmediate(currentStage.gameObject, true);
            currentStage = null;
            if (assetBundle != null)
                assetBundle.Unload(true);
        }
        ani_loading.SetBool("loading", true);
    }

    public void LoadBGMWithName(AudioClip bgm, string name)
    {
        SoundMgr.Instance.PlayBGM(bgm);
        txt_bgmName.text = "♪BGM：" + name;
        ani_bgmName.SetTrigger("show");
    }

    AssetBundle assetBundle;
    int cTotalStage;
    int cStage;
    int loadScene;

    IEnumerator LoadStageFromPack()
    {
        if (assetBundle != null)
            assetBundle.Unload(true);
        AssetBundleCreateRequest createRequest;
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                createRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/stage_" + (cStage + 1).ToString());
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                createRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/stage_" + (cStage + 1).ToString());
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                createRequest = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/stage_extra_" + (cStage + 1).ToString());
                break;
            default:
                createRequest = null;
                break;
        }
        yield return createRequest;

        assetBundle = createRequest.assetBundle;
        AssetBundleRequest assetBundleRequest;
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                assetBundleRequest = assetBundle.LoadAssetAsync<GameObject>((cStage + 1).ToString() + "_normal");
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                assetBundleRequest = assetBundle.LoadAssetAsync<GameObject>((cStage + 1).ToString() + "_toxic");
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                assetBundleRequest = assetBundle.LoadAssetAsync<GameObject>((cStage + 1).ToString());
                break;
            default:
                assetBundleRequest = null;
                break;
        }
        yield return assetBundleRequest;

        Instantiate(assetBundleRequest.asset, transform);
        System.GC.Collect();
    }

    void Awake()
    {
        cStage = 0;
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
            case Global.TitlePassPack.Difficulty.Toxic:
                cTotalStage = totalNoExtraStage;
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                cTotalStage = totalExtraStage;
                break;
        }
        loadScene = -1;
        Instance = this;
    }

    void Start()
    {
        ani_loading.Play("loading_show");
        if (testMode)
            GameMgr.Instance.PowerChange(testPower);
    }
}
