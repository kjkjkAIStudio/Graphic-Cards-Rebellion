using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIAVGEng : MonoBehaviour
{
    public Animator ani;
    public Image img_left, img_right, img_decoration;
    public Text txt_left, txt_right;

    public static AIAVGEng Instance { get; private set; }
    public delegate void ScriptEndHandle();
    public ScriptEndHandle OnScriptEnd;

    public void Run(AIAVGEng_Script script)
    {
        if (isRun)
            return;
        
        line = 0;
        cScript = script;
        if (img_decoration != null)
            img_decoration.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img_left.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        if (img_right != null)
            img_right.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        isRun = true;
        Next();
        ani.SetBool("enabled", true);
    }

    public void Next()
    {
        waitTime = 0.0f;
        while (true)
        {
            if (line >= cScript.commandAndArgs.Length)
            {
                isRun = false;
                ani.SetBool("enabled", false);
                OnScriptEnd();
                return;
            }
            LineAction();
            if (cScript.commandAndArgs[line].command == AIAVGEng_Script.Command.Text)
            {
                ++line;
                return;
            }
            ++line;
        }
    }

    AIAVGEng_Script cScript;
    float waitTime;
    int line;
    bool isRun;

    void LineAction()
    {
        switch (cScript.commandAndArgs[line].command)
        {
            case AIAVGEng_Script.Command.Text:
                {
                    AIAVGEng_Script.Arg_Text arg = (AIAVGEng_Script.Arg_Text)cScript.commandAndArgs[line].arg;
                    if (arg.isRight)
                        txt_right.text = arg.str;
                    else
                        txt_left.text = arg.str;
                    if (img_right != null)
                        ani.SetBool("right", arg.isRight);
                    break;
                }
            case AIAVGEng_Script.Command.Img:
                {
                    AIAVGEng_Script.Arg_Img arg = (AIAVGEng_Script.Arg_Img)cScript.commandAndArgs[line].arg;
                    if (arg.isRight)
                    {
                        img_right.sprite = cScript.res_img[arg.img];
                        if (arg.img == -1)
                            img_right.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                        else
                            img_right.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    else
                    {
                        img_left.sprite = cScript.res_img[arg.img];
                        if (arg.img == -1)
                            img_left.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                        else
                            img_left.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    break;
                }
            case AIAVGEng_Script.Command.DecorationImg:
                {
                    AIAVGEng_Script.Arg_DecorationImg arg = (AIAVGEng_Script.Arg_DecorationImg)cScript.commandAndArgs[line].arg;
                    img_decoration.transform.localPosition = new Vector3(arg.posX, arg.posY);
                    if (arg.img == -1)
                        img_decoration.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    else
                        img_decoration.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    img_decoration.sprite = cScript.res_img[arg.img];
                    img_decoration.SetNativeSize();
                    break;
                }
        }
    }

    void Awake()
    {
        Instance = this;
        isRun = false;
    }

    void Update()
    {
        if (isRun)
        {
            if (Input.GetButtonDown("Z") || ControlPad.Input.isZDown)
                Next();
            if (Input.GetButton("C") || ControlPad.Input.isCHold)
                Next();
            waitTime += Time.deltaTime;
            if (waitTime > 10.0f)
            {
                waitTime = 0.0f;
                Next();
            }
        }
    }
}
