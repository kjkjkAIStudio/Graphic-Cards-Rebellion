using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Txt_FPS : MonoBehaviour
{
    [Tooltip("要显示FPS的文本")]
    public Text txt;
    [Tooltip("FPS更新周期")]
    public float updateDelay;

    int frame;
    float time;

    void Awake()
    {
        frame = 0;
        time = 0.0f;
        txt.text = "0.0 FPS";
    }
    
    void Update()
    {
        time += Time.unscaledDeltaTime;
        ++frame;
        if (time > updateDelay)
        {
            txt.text = (frame / time).ToString("00.0") + " FPS";
            frame = 0;
            time = 0.0f;
        }
    }
}
