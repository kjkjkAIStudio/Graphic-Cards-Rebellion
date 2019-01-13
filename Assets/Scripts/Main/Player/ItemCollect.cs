using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollect : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "item")
        {
            ItemMover temp = target.GetComponent<ItemMover>();
            if (!temp.isCollect)
            {
                temp.stateData = new Mover.StateData[1];
                temp.stateData[0] = ItemSpawner.Instance.collectStateData;
                temp.SetState(0);
                temp.isCollect = true;
            }
        }
    }
}
