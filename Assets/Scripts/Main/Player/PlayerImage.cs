using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImage : MonoBehaviour
{
    public SpriteRenderer img_c, img_l, img_r;
    public float alpha;
    public bool isGod;

    float time;

    void Start()
    {
        time = 0.0f;
    }

    void Update()
    {
        if (isGod)
        {
            time += Time.deltaTime;
            if (time > 0.1f && time < 0.2f)
            {
                img_c.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
                img_l.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
                img_r.color = new Color(1.0f, 1.0f, 1.0f, 0.2f);
            }
            if (time > 0.2f)
            {
                time = 0.0f;
                img_c.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                img_l.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                img_r.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
        else
        {
            img_c.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            img_l.color = new Color(1.0f, 1.0f, 1.0f, alpha);
            img_r.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }
}
