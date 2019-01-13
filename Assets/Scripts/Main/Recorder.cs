/*
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    [System.Serializable]
    public class RecordInputFrame
    {
        public int frameIndex;
        public float posX, posY;
        public bool isZHold, isXHold, isCHold;

        public RecordInputFrame(int frameIndex = 0, float posX = 0.0f, float posY = 0.0f, bool isZHold = false, bool isXHold = false, bool isCHold = false)
        {
            this.frameIndex = frameIndex;
            this.posX = posX;
            this.posY = posY;
            this.isZHold = isZHold;
            this.isXHold = isXHold;
            this.isCHold = isCHold;
        }
    }
    [System.Serializable]
    public class RecordInfo
    {
        public string identifier;
        [System.Serializable]
        public class InfoType
        {
            public Global.TitlePassPack.Difficulty difficulty;
            public Global.TitlePassPack.Character character;
            public int seed;

            public InfoType(Global.TitlePassPack.Difficulty difficulty, Global.TitlePassPack.Character character, int seed)
            {
                this.difficulty = difficulty;
                this.character = character;
                this.seed = seed;
            }
        }
        public InfoType info;
        public RecordInputFrame[][] recordInputFrames; //第一个数是场景序号，第二个数是帧序号

        public RecordInfo(InfoType info, RecordInputFrame[][] recordInputFrames)
        {
            identifier = Global.AISTGEngVersion + "." + Application.companyName + "." + Application.productName;
            this.info = info;
            this.recordInputFrames = recordInputFrames;
        }
    }
    public class ReplayInputType
    {
        public Vector2 pos;
        public bool isZDown, isXDown, isCDown;
        public bool isZHold, isXHold, isCHold;

        public void ResetInputDown()
        {
            isZDown = false;
            isXDown = false;
            isCDown = false;
        }
    }

    public static Recorder Instance { get; private set; }
    public static ReplayInputType ReplayInput { get; private set; }
    public bool IsRecord { get; private set; }
    public bool IsPlay { get; private set; }

    public void New(int stageCount)
    {
        recordInfo = new RecordInfo(new RecordInfo.InfoType(Global.titlePassPack.difficulty, Global.titlePassPack.character, Random.Range(int.MinValue, int.MaxValue)), new RecordInputFrame[stageCount][]);
        Random.InitState(recordInfo.info.seed);
        cStage = -1;
    }

    public void RecordStage()
    {
        cFrame = 0;
        if (recordInputFrames != null)
        {
            recordInfo.recordInputFrames[cStage] = recordInputFrames.ToArray();
            System.GC.Collect();
        }
        recordInputFrames = new List<RecordInputFrame>();
        recordInputFrames.Add(new RecordInputFrame
        (
            cFrame,
            Input.GetAxis("Horizontal") + ControlPad.Input.pos.x,
            Input.GetAxis("Vertical") + ControlPad.Input.pos.y,
            Input.GetButton("Z") || ControlPad.Input.isZHold,
            Input.GetButton("X") || ControlPad.Input.isXHold,
            Input.GetButton("C") || ControlPad.Input.isCHold
        ));
        ++cStage;
        IsRecord = true;
    }

    public void RecordPause()
    {
        IsRecord = false;
    }

    public void RecordResume()
    {
        IsRecord = true;
    }

    public void RecordSave(string name)
    {
        if (name == "") name = "未命名";
        IsRecord = false;
        recordInfo.recordInputFrames[cStage] = recordInputFrames.ToArray();

        FileStream fs;
        if (!Directory.Exists(Global.RecordDirectory))
            Directory.CreateDirectory(Global.RecordDirectory);
        int index = 1;
        while (true)
        {
            if (!File.Exists(string.Format(Global.RecordFileName, name + "_" + index.ToString())))
                break;
            ++index;
        }
        if (!File.Exists(string.Format(Global.RecordFileName, name + "_" + index.ToString())))
        {
            fs = File.Create(string.Format(Global.RecordFileName, name + "_" + index.ToString()));
        }
        else
        {
            fs = new FileStream(string.Format(Global.RecordFileName, name + "_" + index.ToString()), FileMode.Truncate, FileAccess.Write);
        }
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, recordInfo);
        fs.Flush();
        fs.Close();
        fs.Dispose();
        fs = null;
        bf = null;
        recordInfo = null;
        recordInputFrames = null;
        System.GC.Collect();
    }

    public void PlayStage()
    {
        cState = 0;
        cFrame = 0;
        if (IsPlay)
        {
            ++cStage;
            return;
        }
        recordInfo = Global.titlePassPack.loadRecord;
        cStage = Global.titlePassPack.loadRecordStageIndex;
        Random.InitState(recordInfo.info.seed);
        IsPlay = true;
    }

    RecordInfo recordInfo;
    List<RecordInputFrame> recordInputFrames;
    int cState;
    int cStage;
    int cFrame;

    void Awake()
    {
        Instance = this;
        ReplayInput = new ReplayInputType();
        IsRecord = false;
        IsPlay = false;
    }

    void Update()
    {
        if (IsRecord)
        {
            ++cFrame;
            if (Input.GetAxis("Horizontal") + ControlPad.Input.pos.x != recordInputFrames[recordInputFrames.Count - 1].posX
                || Input.GetAxis("Vertical") + ControlPad.Input.pos.y != recordInputFrames[recordInputFrames.Count - 1].posY
                || (Input.GetButton("Z") || ControlPad.Input.isZHold) != recordInputFrames[recordInputFrames.Count - 1].isZHold
                || (Input.GetButton("X") || ControlPad.Input.isXHold) != recordInputFrames[recordInputFrames.Count - 1].isXHold
                || (Input.GetButton("C") || ControlPad.Input.isCHold) != recordInputFrames[recordInputFrames.Count - 1].isCHold)
            {
                recordInputFrames.Add(new RecordInputFrame
                (
                    cFrame,
                    Input.GetAxis("Horizontal") + ControlPad.Input.pos.x,
                    Input.GetAxis("Vertical") + ControlPad.Input.pos.y,
                    Input.GetButton("Z") || ControlPad.Input.isZHold,
                    Input.GetButton("X") || ControlPad.Input.isXHold,
                    Input.GetButton("C") || ControlPad.Input.isCHold
                ));
            }
        }
        if (IsPlay)
        {
            if (recordInfo.recordInputFrames[cStage] == null)
                return;
            if (cState >= recordInfo.recordInputFrames[cStage].Length)
                return;
            ++cFrame;
            if (cFrame >= recordInfo.recordInputFrames[cStage][cState].frameIndex)
            {
                ReplayInput.pos = new Vector2(recordInfo.recordInputFrames[cStage][cState].posX, recordInfo.recordInputFrames[cStage][cState].posY);
                ReplayInput.isZHold = recordInfo.recordInputFrames[cStage][cState].isZHold;
                ReplayInput.isXHold = recordInfo.recordInputFrames[cStage][cState].isXHold;
                ReplayInput.isCHold = recordInfo.recordInputFrames[cStage][cState].isCHold;
                ReplayInput.isZDown = recordInfo.recordInputFrames[cStage][cState].isZHold;
                ReplayInput.isXDown = recordInfo.recordInputFrames[cStage][cState].isXHold;
                ReplayInput.isCDown = recordInfo.recordInputFrames[cStage][cState].isCHold;
                ++cState;
            }
        }
    }

    void LateUpdate()
    {
        if (IsPlay)
        {
            ReplayInput.ResetInputDown();
        }
    }
}
*/