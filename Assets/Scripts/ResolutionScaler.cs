using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionScaler : MonoBehaviour
{
    [Tooltip("当前设计的分辨率")]
    public Vector2Int designResolution;
    [Tooltip("用于充当黑边")]
    public RectTransform up, down, left, right;

    Vector2Int prevResolution;
    float designWPH;

    void Awake()
    {
        prevResolution = new Vector2Int(designResolution.x, designResolution.y);
        designWPH = (float)designResolution.x / designResolution.y;
    }
    
    void Update()
    {
        if (Screen.width != prevResolution.x || Screen.height != prevResolution.y)
        {
            //根据设计标准放缩比例
            prevResolution = new Vector2Int(Screen.width, Screen.height);
            float scale = (float)Screen.width / Screen.height / designWPH;
            if (scale > 1.0f) scale = 1.0f;
            transform.localScale = new Vector3(scale, scale, 1.0f);
            //填充黑边
            float x = Mathf.Abs(Screen.height - (float)Screen.width / designResolution.x * designResolution.y);
            float y = Mathf.Abs(Screen.width - (float)Screen.height / designResolution.y * designResolution.x);
            if(left != null) left.sizeDelta = new Vector2(x, 0.0f);
            if (right != null) right.sizeDelta = new Vector2(x, 0.0f);
            if (up != null) up.sizeDelta = new Vector2(0.0f, y);
            if (down != null) down.sizeDelta = new Vector2(0.0f, y);
        }
    }
}
