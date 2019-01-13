using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DebugMgr : MonoBehaviour
{
    public GameObject handle_icon;
    public Text txt_screenshot;

    public static DebugMgr Instance { get; private set; }

    public void Log(string condition, string stackTrace, LogType type)
    {
        if (logCount >= 10)
            return;
        switch (type)
        {
            case LogType.Assert:
            case LogType.Exception:
            case LogType.Error:
            case LogType.Warning:
                if (!Directory.Exists(Global.SaveDirectory))
                    Directory.CreateDirectory(Global.SaveDirectory);
                if (!Directory.Exists(Global.LogDirectory))
                    Directory.CreateDirectory(Global.LogDirectory);
                File.AppendAllLines(string.Format(Global.LogFileName, Global.AISTGEngVersion + " " + System.DateTime.Today.ToString("yyyy.MM.dd")), new string[] { condition, stackTrace, "" });
                Screenshot();
                handle_icon.SetActive(true);
                break;
        }
        ++logCount;
    }

    public void Screenshot()
    {
        if (Camera.main == null)
            return;
        RenderTexture rTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = rTex;
        Camera.main.Render();
        RenderTexture.active = rTex;
        Texture2D tempTex = new Texture2D(Screen.width, Screen.height);
        tempTex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tempTex.Apply();
        RenderTexture.active = null;
        Camera.main.targetTexture = null;
        RenderTexture.ReleaseTemporary(rTex);

        if (!Directory.Exists(Global.ScreenshotDirectory))
            Directory.CreateDirectory(Global.ScreenshotDirectory);
        string date = System.DateTime.Today.ToString("yyyy.MM.dd") + "_";
        int index = 1;
        while (true)
        {
            if (!File.Exists(string.Format(Global.ScreenshotFileName, date + index)))
                break;
            ++index;
        }
        File.WriteAllBytes(string.Format(Global.ScreenshotFileName, date + index), tempTex.EncodeToPNG());
        txt_screenshot.text = "截图保存为：" + date + index;
        txt_screenshot.gameObject.SetActive(true);
    }

    int logCount;

    void Awake()
    {
        logCount = 0;
        Instance = this;
        Application.logMessageReceived += Log;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= Log;
    }
}
