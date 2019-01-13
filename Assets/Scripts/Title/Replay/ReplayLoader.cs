/*
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class ReplayLoader : MonoBehaviour
{
    public Transform content;
    public GameObject handle_info;
    public Text txt_info;
    public InputField input_stageIndex;
    public Animator ani_loading;
    public GameObject prefab_element;

    public void Refresh()
    {
        for (int i = 0; i < content.childCount; ++i)
            Destroy(content.GetChild(i).gameObject);
        replayFilenames = Directory.GetFiles(Global.RecordDirectory, "*.aireplay");
        for (int i = 0; i < replayFilenames.Length; ++i)
        {
            ReplayLoaderElement temp = Instantiate(prefab_element, content).GetComponent<ReplayLoaderElement>();
            temp.main = this;
            temp.index = i;
            temp.txt.text = Path.GetFileNameWithoutExtension(replayFilenames[i]);
        }
    }

    public void Click(int index)
    {
        input_stageIndex.interactable = false;
        txt_info.text = Path.GetFileNameWithoutExtension(replayFilenames[index]);
        handle_info.SetActive(true);
        try
        {
            FileStream fs = new FileStream(replayFilenames[index], FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            recordInfo = (Recorder.RecordInfo)bf.Deserialize(fs);
            fs.Close();
            fs.Dispose();
        }
        catch
        {
            if (recordInfo == null)
            {
                txt_info.text += "\n回放读取失败！";
                return;
            }
            else
            {
                txt_info.text += "\n回放读取时发生未知错误！";
            }
        }
        if (recordInfo.identifier != Global.AISTGEngVersion + "." + Application.companyName + "." + Application.productName)
        {
            txt_info.text += "\n回放认证失败！";
            return;
        }
        string info = "\n难度：";
        switch (recordInfo.info.difficulty)
        {
            case Global.TitlePassPack.Difficulty.Normal:
                info += "Normal";
                break;
            case Global.TitlePassPack.Difficulty.Toxic:
                info += "Toxic";
                break;
            case Global.TitlePassPack.Difficulty.Extra:
                info += "Extra";
                break;
        }
        info += "\n所用角色：";
        switch (recordInfo.info.character)
        {
            case Global.TitlePassPack.Character.G_2080TI:
                info += "Nuidiu Geforce RTX 2080TI";
                break;
            case Global.TitlePassPack.Character.G_radeon:
                info += "AWD Radeon RX Vega Liquid Edition";
                break;
            case Global.TitlePassPack.Character.G_i9:
                info += "Xtel HD Graphics 2000";
                break;
        }
        info += "\n输入要开始回放的场景序号（非Extra时输入1~6，Extra时输入1即可）";
        txt_info.text += info;
        input_stageIndex.interactable = true;
    }

    public void ReplayStart()
    {
        if (!int.TryParse(input_stageIndex.text, out int value) || value < 1 || value - 1 >= recordInfo.recordInputFrames.Length)
        {
            input_stageIndex.text = "找不到场景";
            return;
        }
        Global.titlePassPack = new Global.TitlePassPack(recordInfo.info.difficulty, recordInfo.info.character, recordInfo, value - 1);
        ani_loading.SetBool("loading", true);
    }
    
    Recorder.RecordInfo recordInfo;
    string[] replayFilenames;
}
*/