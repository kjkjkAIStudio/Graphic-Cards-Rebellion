using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ani_eventPass : MonoBehaviour
{
    public UnityEvent[] OnTriggerEvent;

    public void Send(int index)
    {
        OnTriggerEvent[index].Invoke();
    }
}
