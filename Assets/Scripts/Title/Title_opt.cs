using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; //UnityEvent所在的命名空间

public class Title_opt : MonoBehaviour
{
    [Tooltip("判断是否接受用户的输入")]
    public bool isFocused = false;
    [Tooltip("用户当前选择的项")]
    public int selected = 0;
    [Tooltip("当用户按住方向键时，判定为连续按下的间隔")]
    public float pressDelay = 1.0f;
    [Tooltip("判定为连续按下时，连按间隔")]
    public float holdDelay = 0.2f;
    [Tooltip("所有opt对象的Animator，可以不填")]
    public Animator[] ani_opt;
    [Tooltip("闪动效果的Animator，可以不填")]
    public Animator[] ani_txt_flash;
    public AudioClip se_move, se_select, se_cancel;
    public UnityEvent OnSelect, OnCancel;

    //将字段方法化，以便在Unity事件中添加。
    public void SetFocused(bool value)
    {
        isFocused = value;
    }

    public void SetSelected(int value)
    {
        prevSelected = selected;
        selected = value;
        ani_opt[prevSelected].SetBool("select", false);
        ani_opt[selected].SetBool("select", true);
    }
    
    float countTime;
    int prevSelected;
    bool isCountTime;
    bool isHold;
    bool isUnlocked;

    void MoveDown()
    {
        prevSelected = selected;
        ++selected;
        if (selected >= ani_opt.Length)
            selected = 0;
        //设置Animator中的bool参数
        ani_opt[prevSelected].SetBool("select", false);
        ani_opt[selected].SetBool("select", true);
        SoundMgr.Instance.PlaySE(se_move);
    }

    void MoveUp()
    {
        prevSelected = selected;
        --selected;
        if (selected < 0)
            selected = ani_opt.Length - 1;
        ani_opt[prevSelected].SetBool("select", false);
        ani_opt[selected].SetBool("select", true);
        SoundMgr.Instance.PlaySE(se_move);
    }

    void Awake()
    {
        countTime = 0.0f;
        prevSelected = selected;
        isCountTime = false;
        isHold = false;
        isUnlocked = isFocused;
    }

    void OnEnable()
    {
        if (ani_opt.Length > 0)
            ani_opt[selected].SetBool("select", true);
    }

    void Update()
    {
        if (isUnlocked)
        {
            //在用户按下“右”或“下”时为true
            if (ani_opt.Length > 0)
            {
                if (Input.GetAxisRaw("Horizontal") == 1 || ControlPad.Input.posRaw.x == 1 || Input.GetAxisRaw("Vertical") == -1 || ControlPad.Input.posRaw.y == -1)
                {
                    //按方向键的瞬间启动计时器，并判定为一次MoveDown
                    if (!isCountTime)
                    {
                        isCountTime = true;
                        MoveDown();
                    }
                    countTime += Time.deltaTime;
                    if (!isHold)
                    {
                        //按住方向键指定时间后判定为连按
                        if (countTime > pressDelay)
                        {
                            countTime = 0.0f;
                            isHold = true;
                        }
                    }
                    else
                    {
                        //连按时根据指定的间隔MoveDown
                        if (countTime > holdDelay)
                        {
                            countTime = 0.0f;
                            MoveDown();
                        }
                    }
                }
                else
                {
                    //在用户按下“左”或“上”时为true
                    if (Input.GetAxisRaw("Horizontal") == -1 || ControlPad.Input.posRaw.x == -1 || Input.GetAxisRaw("Vertical") == 1 || ControlPad.Input.posRaw.y == 1)
                    {
                        if (!isCountTime)
                        {
                            isCountTime = true;
                            MoveUp();
                        }
                        countTime += Time.deltaTime;
                        if (!isHold)
                        {
                            if (countTime > pressDelay)
                            {
                                countTime = 0.0f;
                                isHold = true;
                            }
                        }
                        else
                        {
                            if (countTime > holdDelay)
                            {
                                countTime = 0.0f;
                                MoveUp();
                            }
                        }
                    }
                    else //用户松开方向键的场合，计时器复位
                    {
                        isCountTime = false;
                        isHold = false;
                        countTime = 0.0f;
                    }
                }
            }

            //在用户按下“选择”键的那一帧时为true
            //而GetButton在按住时的每一帧都为true
            if (Input.GetButtonDown("Z") || ControlPad.Input.isZDown)
            {
                if (selected < ani_txt_flash.Length)
                    ani_txt_flash[selected].SetTrigger("flash");
                OnSelect.Invoke();
                SoundMgr.Instance.PlaySE(se_select);
            }

            //“取消”键
            if (Input.GetButtonDown("C") || ControlPad.Input.isCDown || Input.GetKeyDown(KeyCode.Escape) || ControlPad.Input.isEscapeDown)
            {
                OnCancel.Invoke();
                SoundMgr.Instance.PlaySE(se_cancel);
            }
        }

        isUnlocked = isFocused;
    }
}
