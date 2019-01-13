using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //SceneManager所在的命名空间

public class Title_optCheck : MonoBehaviour
{
    public Title_opt opt, opt_difficulty, opt_character, opt_score;
    public Animator ani_title, ani_difficulty, ani_character, ani_loading;
    public GameObject handle_opening, handle_settings, handle_score, handle_replay, handle_help;
    public GameObject handle_2080ti_cannotExtra, handle_radeon_cannotExtra, handle_i9_cannotExtra;
    public AudioClip bgm;
    public Slider slider_bgmVol, slider_seVol;
    public Toggle toggle_fullscreen, toggle_controlPadMax;
    public Text txt_score_normal, txt_score_toxic, txt_score_extra;
    
    public void OptCheck()
    {
        opt.isFocused = false;
        switch (opt.selected)
        {
            case 0: //start
                exMode = false;
                handle_2080ti_cannotExtra.SetActive(false);
                handle_radeon_cannotExtra.SetActive(false);
                handle_i9_cannotExtra.SetActive(false);
                ani_difficulty.SetTrigger("show");
                opt_difficulty.isFocused = true;
                break;
            case 1: //ex
                exMode = true;
                if (!Global.savePack.characterCanExtra[0])
                    handle_2080ti_cannotExtra.SetActive(true);
                if (!Global.savePack.characterCanExtra[1])
                    handle_radeon_cannotExtra.SetActive(true);
                if (!Global.savePack.characterCanExtra[2])
                    handle_i9_cannotExtra.SetActive(true);
                ani_character.SetTrigger("show");
                opt_character.isFocused = true;
                break;
            case 2: //score
                opt_score.isFocused = true;
                handle_score.SetActive(true);
                opt_score.SetSelected(0);
                break;
            case 3: //replay
                handle_replay.SetActive(true);
                break;
            case 4: //setting
                handle_settings.SetActive(true);
                break;
            case 5: //quit
                Application.Quit();
                break;
        }
    }

    public void CharacterReturn()
    {
        opt_character.isFocused = false;
        ani_character.SetTrigger("hide");
        if (exMode)
        {
            opt.isFocused = true;
        }
        else
        {
            ani_difficulty.SetTrigger("show");
            opt_difficulty.isFocused = true;
        }
    }

    public void ApplySettings()
    {
        handle_settings.SetActive(false);
        opt.isFocused = true;

        Global.savePack.fullscreen = toggle_fullscreen.isOn;
        Global.savePack.controlPadMax = toggle_controlPadMax.isOn;
        Global.savePack.bgmVol = slider_bgmVol.value;
        Global.savePack.seVol = slider_seVol.value;
        ApplyFromSavePack();
        Global.SavePack.Save();
    }

    public void ApplyFromSavePack()
    {
        Screen.SetResolution(1920, 1080, Global.savePack.fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
        if (ControlPad.Instance != null)
            ControlPad.Instance.ApplySettings();
        SoundMgr.Instance.RefreshBGMVol();
    }

    public void SkipOpening()
    {
        Destroy(handle_opening);
        opt.SetFocused(true);
        ani_title.SetTrigger("begin");
    }
    
    public void GameStart()
    {
        if (exMode)
        {
            if (Global.savePack.characterCanExtra[opt_character.selected])
            {
                Global.titlePassPack = new Global.TitlePassPack
                    (Global.TitlePassPack.Difficulty.Extra, (Global.TitlePassPack.Character)opt_character.selected);
                ani_loading.SetBool("loading", true);
            }
            else
            {
                opt_character.isFocused = true;
            }
        }
        else
        {
            Global.titlePassPack = new Global.TitlePassPack((Global.TitlePassPack.Difficulty)opt_difficulty.selected, (Global.TitlePassPack.Character)opt_character.selected);
            ani_loading.SetBool("loading", true);
        }
    }

    public void BeginLoad()
    {
        SceneManager.LoadSceneAsync(1);
    }

    bool exMode;

    void Awake()
    {
        exMode = false;
        //读档
        if (Global.savePack == null)
        {
            if (Global.SavePack.Load())
            {
                handle_settings.SetActive(true);
                handle_help.SetActive(true);
            }
        }
        toggle_fullscreen.isOn = Global.savePack.fullscreen;
        toggle_controlPadMax.isOn = Global.savePack.controlPadMax;
        slider_bgmVol.value = Global.savePack.bgmVol;
        slider_seVol.value = Global.savePack.seVol;
    }

    void Start()
    {
        Time.timeScale = 1.0f;
        //获取排行榜
        txt_score_normal.text = "";
        txt_score_toxic.text = "";
        txt_score_extra.text = "";
        for (int i = 0; i < Global.savePack.normalHighscore.Length; ++i)
        {
            txt_score_normal.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.normalHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.normalHighscore[i].name;
            if (i < Global.savePack.normalHighscore.Length - 1) txt_score_normal.text += "\n";
        }
        for (int i = 0; i < Global.savePack.toxicHighscore.Length; ++i)
        {
            txt_score_toxic.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.toxicHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.toxicHighscore[i].name;
            if (i < Global.savePack.toxicHighscore.Length - 1) txt_score_normal.text += "\n";
        }
        for (int i = 0; i < Global.savePack.extraHighscore.Length; ++i)
        {
            txt_score_extra.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.extraHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.extraHighscore[i].name;
            if (i < Global.savePack.extraHighscore.Length - 1) txt_score_normal.text += "\n";
        }
        //播放音乐
        SoundMgr.Instance.PlayBGM(bgm);
        //判断是否跳过片头
        if (!Global.notFirstBoot)
        {
            ApplyFromSavePack();
            Global.notFirstBoot = true;
        }
        else
        {
            SkipOpening();
        }
    }
}
