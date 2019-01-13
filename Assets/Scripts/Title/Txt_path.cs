using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Txt_path : MonoBehaviour
{
    public Text txt;

    void Awake()
    {
        txt.text = "存档，日志，截图所在的文件夹：" + Global.SaveDirectory;
    }
}
