using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingLoader : MonoBehaviour
{
    public Animator ani_loading;
    [Tooltip("角色续关通关的结局")]
    public AIAVGEng_Script[] BE;
    [Tooltip("角色不续关通关的结局")]
    public AIAVGEng_Script[] GE;
    [Tooltip("Extra角色续关通关的结局")]
    public AIAVGEng_Script[] extraBE;
    [Tooltip("Extra角色不续关通关的结局")]
    public AIAVGEng_Script[] extraGE;

    public void DialogEnd()
    {
        SceneManager.LoadSceneAsync(0);
        ani_loading.SetBool("loading", true);
    }

    void Awake()
    {
        Time.timeScale = 1.0f;
    }

    void Start()
    {
        AIAVGEng.Instance.OnScriptEnd = DialogEnd;
        if (Global.mainPassPack.isExtra)
        {
            if (Global.mainPassPack.isDieContinue)
                AIAVGEng.Instance.Run(extraBE[(int)Global.titlePassPack.character]);
            else
                AIAVGEng.Instance.Run(extraGE[(int)Global.titlePassPack.character]);
        }
        else
        {
            if (Global.mainPassPack.isDieContinue)
                AIAVGEng.Instance.Run(BE[(int)Global.titlePassPack.character]);
            else
                AIAVGEng.Instance.Run(GE[(int)Global.titlePassPack.character]);
        }
        Global.titlePassPack = null;
        Global.mainPassPack = null;
    }
}
