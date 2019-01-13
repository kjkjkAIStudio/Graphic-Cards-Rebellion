using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlPad : MonoBehaviour
{
    public GameObject handle_trigger;
    public RectTransform joystick_bottom, joystick;
    [Tooltip("虚拟摇杆的最大圆周半径")]
    public int maxJoystickLength;
    [Tooltip("是否保持虚拟摇杆在最大速度")]
    public bool controlPadMax;

    public class ControlInput
    {
        public Vector2 pos;
        public Vector2Int posRaw;
        public bool isZDown, isXDown, isCDown;
        public bool isZHold, isXHold, isCHold;
        public bool isEscapeDown;

        public void ResetInputDown()
        {
            isZDown = false;
            isXDown = false;
            isCDown = false;
            isEscapeDown = false;
        }
    }

    public static ControlPad Instance { get; private set; }
    public static ControlInput Input { get; private set; }

    public void JoystickDown(BaseEventData pointer)
    {
        Vector2 result;
        Vector2 offset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick_bottom, ((PointerEventData)pointer).position, Camera.main, out offset);
        if (controlPadMax)
            result = offset.normalized;
        else
            result = Vector2.ClampMagnitude(offset, maxJoystickLength) / maxJoystickLength;
        Input.pos = result;
        Input.posRaw = new Vector2Int();
        if (Mathf.Abs(result.x) > Mathf.Abs(result.y))
            Input.posRaw.x = result.x > 0.0f ? 1 : -1;
        else
            Input.posRaw.y = result.y > 0.0f ? 1 : -1;
        joystick.anchoredPosition = maxJoystickLength * result;
    }

    public void JoystickUp()
    {
        Input.pos = new Vector2();
        Input.posRaw = new Vector2Int();
        joystick.anchoredPosition = new Vector2();
    }

    public void TriggerChange()
    {
        Global.savePack.controlPad = !Global.savePack.controlPad;
        gameObject.SetActive(Global.savePack.controlPad);
    }

    public void ApplySettings()
    {
        gameObject.SetActive(Global.savePack.controlPad);
        controlPadMax = Global.savePack.controlPadMax;
    }

    public void TriggerSave()
    {
        Global.SavePack.Save();
    }

    public void ZDown()
    {
        Input.isZHold = true;
        Input.isZDown = true;
    }

    public void ZUp()
    {
        Input.isZHold = false;
    }

    public void XDown()
    {
        Input.isXHold = true;
        Input.isXDown = true;
    }

    public void XUp()
    {
        Input.isXHold = false;
    }

    public void CDown()
    {
        Input.isCHold = true;
        Input.isCDown = true;
    }

    public void CUp()
    {
        Input.isCHold = false;
    }

    public void EscapeDown()
    {
        Input.isEscapeDown = true;
    }

    void Awake()
    {
        Instance = this;
        Input = new ControlInput();
    }

    void Start()
    {
        gameObject.SetActive(false);
        if (Global.savePack != null)
        {
            ApplySettings();
        }
    }

    void LateUpdate()
    {
        Input.ResetInputDown();
    }
}
