using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_version : MonoBehaviour
{
    public Text txt;

    void Start()
    {
        txt.text = Global.AISTGEngVersion + " " + Application.companyName + " " + Application.version;
    }
}
