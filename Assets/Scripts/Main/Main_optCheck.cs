using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main_optCheck : MonoBehaviour
{
    public Title_opt opt_pause, opt_die, opt_ask, opt_skipScore;
    public GameObject handle_ask, handle_skipScore, handle_txt_noBreak, handle_txt_dieContinue, handle_endBonus;
    public Text txt_score;
    public Animator ani_score;
    public InputField input_name;
    [HideInInspector]
    public bool isDie;

    public void Select()
    {
        if (handle_ask.activeSelf)
        {
            opt_ask.isFocused = false;
            handle_ask.SetActive(false);
            if (opt_ask.selected == 1)
                StageLoader.Instance.LoadScene(isResetOrTitle ? 1 : 0);
            else
                opt_pause.isFocused = true;
        }
        else
        {
            if (isDie)
            {
                opt_die.isFocused = false;
                switch (opt_die.selected)
                {
                    case 0: //续关
                        isDie = false;
                        GameMgr.Instance.DieContinue();
                        break;
                    case 1: //重来
                        StageLoader.Instance.LoadScene(1);
                        break;
                    case 2: //主菜单
                        StageLoader.Instance.LoadScene(0);
                        break;
                }
            }
            else
            {
                opt_pause.isFocused = false;
                switch (opt_pause.selected)
                {
                    case 0: //恢复
                        opt_pause.isFocused = false;
                        Time.timeScale = 1.0f;
                        GameMgr.Instance.isFocused = true;
                        GameMgr.Instance.ui.ani_pause.SetBool("enabled", false);
                        break;
                    case 1: //重来
                        isResetOrTitle = true;
                        handle_ask.SetActive(true);
                        opt_ask.SetSelected(0);
                        opt_ask.isFocused = true;
                        break;
                    case 2: //主菜单
                        isResetOrTitle = false;
                        handle_ask.SetActive(true);
                        opt_ask.SetSelected(0);
                        opt_ask.isFocused = true;
                        break;
                }
            }
        }
    }

    public void Cancel()
    {
        if (handle_ask.activeSelf)
        {
            opt_ask.isFocused = false;
            handle_ask.SetActive(false);
            opt_pause.isFocused = true;
        }
        else
        {
            if (isDie)
            {
                opt_die.SetSelected(2);
            }
            else
            {
                opt_pause.isFocused = false;
                Time.timeScale = 1.0f;
                GameMgr.Instance.isFocused = true;
                GameMgr.Instance.ui.ani_pause.SetBool("enabled", false);
            }
        }
    }

    public void UpdateScoreAndDisplay(uint score)
    {
        opt_die.isFocused = false;
        //登陆分数
        if (GameMgr.Instance.IsDieContinue)
        {
            handle_skipScore.SetActive(true);
            handle_txt_dieContinue.SetActive(true);
            handle_txt_noBreak.SetActive(false);
            opt_skipScore.isFocused = true;
        }
        else
        {
            newScoreIndex = -1;
            int scoreCount = 0;
            switch (Global.titlePassPack.difficulty)
            {
                case Global.TitlePassPack.Difficulty.Normal:
                    scoreCount = Global.savePack.normalHighscore.Length;
                    for (int i = 0; i < Global.savePack.normalHighscore.Length; ++i)
                    {
                        if (score >= Global.savePack.normalHighscore[i].score)
                        {
                            newScoreIndex = i;
                            break;
                        }
                    }
                    break;
                case Global.TitlePassPack.Difficulty.Toxic:
                    scoreCount = Global.savePack.toxicHighscore.Length;
                    for (int i = 0; i < Global.savePack.toxicHighscore.Length; ++i)
                    {
                        if (score >= Global.savePack.toxicHighscore[i].score)
                        {
                            newScoreIndex = i;
                            break;
                        }
                    }
                    break;
                case Global.TitlePassPack.Difficulty.Extra:
                    scoreCount = Global.savePack.extraHighscore.Length;
                    for (int i = 0; i < Global.savePack.extraHighscore.Length; ++i)
                    {
                        if (score >= Global.savePack.extraHighscore[i].score)
                        {
                            newScoreIndex = i;
                            break;
                        }
                    }
                    break;
            }
            if (newScoreIndex == -1)
            {
                if (scoreCount < 10)
                {
                    newScoreIndex = scoreCount;
                }
                else
                {
                    handle_skipScore.SetActive(true);
                    handle_txt_dieContinue.SetActive(false);
                    handle_txt_noBreak.SetActive(true);
                    opt_skipScore.isFocused = true;
                }
            }
            else
            {
                //保存分数
                ++scoreCount;
                if (scoreCount > 10) scoreCount = 10;
                newHighscore = new Global.SavePack.Highscore[scoreCount];
                bool isInsert = false;
                for (int i = 0; i < newHighscore.Length; ++i)
                {
                    if (newScoreIndex == i)
                    {
                        newHighscore[i] = new Global.SavePack.Highscore { score = score };
                        isInsert = true;
                        continue;
                    }
                    switch (Global.titlePassPack.difficulty)
                    {
                        case Global.TitlePassPack.Difficulty.Normal:
                            newHighscore[i] = Global.savePack.normalHighscore[isInsert ? (i - 1) : i];
                            break;
                        case Global.TitlePassPack.Difficulty.Toxic:
                            newHighscore[i] = Global.savePack.toxicHighscore[isInsert ? (i - 1) : i];
                            break;
                        case Global.TitlePassPack.Difficulty.Extra:
                            newHighscore[i] = Global.savePack.extraHighscore[isInsert ? (i - 1) : i];
                            break;
                    }
                }
                input_name.interactable = true;
            }
        }
        //开始显示最终分数
        txt_score.text = "";
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                for (int i = 0; i < Global.savePack.normalHighscore.Length; ++i)
                {
                    txt_score.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.normalHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.normalHighscore[i].name + "\n";
                }
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                for (int i = 0; i < Global.savePack.toxicHighscore.Length; ++i)
                {
                    txt_score.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.toxicHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.toxicHighscore[i].name + "\n";
                }
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                for (int i = 0; i < Global.savePack.extraHighscore.Length; ++i)
                {
                    txt_score.text += (i + 1).ToString() + ". <color=#00ff00>" + Global.savePack.extraHighscore[i].score.ToString("000,000,000") + "</color> " + Global.savePack.extraHighscore[i].name + "\n";
                }
                break;
        }
        if (newScoreIndex != -1)
        {
            txt_score.text += "<color=#ffffff>New score at " + (newScoreIndex + 1).ToString() + ". " + newHighscore[newScoreIndex].score.ToString("000,000,000") + " " + newHighscore[newScoreIndex].name + "</color>";
        }
        ani_score.SetBool("enabled", true);
    }

    public void UpdateName(string name)
    {
        newHighscore[newScoreIndex].name = name;
        switch (Global.titlePassPack.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                Global.savePack.normalHighscore = newHighscore;
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                Global.savePack.toxicHighscore = newHighscore;
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                Global.savePack.extraHighscore = newHighscore;
                break;
        }
        Global.SavePack.Save();
        HideScore();
    }

    public void HideScore()
    {
        input_name.interactable = false;
        opt_skipScore.isFocused = false;
        ani_score.SetBool("enabled", false);
        if (!GameMgr.Instance.IsEndBonus)
        {
            opt_die.isFocused = true;
        }
        else
        {
            Global.mainPassPack = new Global.MainPassPack(Global.titlePassPack.difficulty == Global.TitlePassPack.Difficulty.Extra, GameMgr.Instance.IsDieContinue);
            StageLoader.Instance.LoadScene(2);
        }
    }

    Global.SavePack.Highscore[] newHighscore;
    int newScoreIndex;
    bool isResetOrTitle;
}
