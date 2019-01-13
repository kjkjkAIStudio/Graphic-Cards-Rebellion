using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMover : Mover
{
    public MeshRenderer mr;
    public TextMesh txt;

    protected override void Start()
    {
        base.Start();
        mr.sortingLayerName = "Item";
        mr.sortingOrder = 1;
        SetState(0);
    }

    protected override void Update()
    {
        base.Update();
        txt.color = img.color;
    }
}
