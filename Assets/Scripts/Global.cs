using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//这是游戏的全局类，即使切换场景也不会消失
public static class Global
{
    //Main场景接受来自Title场景的数据包
    public class TitlePassPack
    {
        public enum Difficulty { Normal, Toxic, Extra };
        public Difficulty difficulty;
        public enum Character { G_2080TI, G_radeon, G_i9 };
        public Character character;

        public TitlePassPack(Difficulty difficulty = Difficulty.Normal, Character character = Character.G_2080TI)
        {
            this.difficulty = difficulty;
            this.character = character;
        }
    }
    public static TitlePassPack titlePassPack = null;

    //Ending场景接受来自Main场景的数据包
    public class MainPassPack
    {
        public bool isExtra;
        public bool isDieContinue;

        public MainPassPack(bool isExtra, bool isDieContinue)
        {
            this.isExtra = isExtra;
            this.isDieContinue = isDieContinue;
        }
    }
    public static MainPassPack mainPassPack = null;

    //存档数据包
    [Serializable]
    public class SavePack
    {
        public string identifier;
        [Serializable]
        public class Highscore
        {
            public string name;
            public uint score;

            public Highscore()
            {
                name = "未命名";
                score = 0u;
            }
        }
        public Highscore[] normalHighscore;
        public Highscore[] toxicHighscore;
        public Highscore[] extraHighscore;
        public bool[] characterCanExtra;
        public float bgmVol, seVol;
        public bool fullscreen, controlPad, controlPadMax;

        public SavePack(float bgmVol = 1.0f, float seVol = 1.0f)
        {
            identifier = AISTGEngVersion + "." + Application.companyName + "." + Application.productName;
            normalHighscore = new Highscore[1];
            normalHighscore[0] = new Highscore();
            toxicHighscore = new Highscore[1];
            toxicHighscore[0] = new Highscore();
            extraHighscore = new Highscore[1];
            extraHighscore[0] = new Highscore();
            characterCanExtra = new bool[3];
            this.bgmVol = bgmVol;
            this.seVol = seVol;
            fullscreen = false;
            controlPad = false;
            controlPadMax = false;
        }

        public static void Save()
        {
            FileStream fs;
            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);
            if (!File.Exists(SaveFileName))
                fs = File.Create(SaveFileName);
            else
                fs = new FileStream(SaveFileName, FileMode.Truncate, FileAccess.Write);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, savePack);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        public static bool Load()
        {
            savePack = null;
            if (File.Exists(SaveFileName))
            {
                try
                {
                    FileStream fs = new FileStream(SaveFileName, FileMode.Open);
                    BinaryFormatter bf = new BinaryFormatter();
                    savePack = (SavePack)bf.Deserialize(fs);
                    fs.Close();
                    fs.Dispose();
                }
                catch
                {
                    if (savePack == null)
                    {
                        savePack = new SavePack();
                        Save();
                        Debug.LogError("存档数据不匹配！读取失败！新建一个默认存档并覆盖...");
                    }
                    else
                    {
                        Debug.LogWarning("读取存档时发生未知的错误...");
                    }
                }
                if (savePack.identifier != AISTGEngVersion + "." + Application.companyName + "." + Application.productName)
                {
                    savePack = new SavePack();
                    Save();
                    Debug.LogError("存档认证失败！新建一个默认存档并覆盖...");
                }
                return false;
            }
            else
            {
                savePack = new SavePack();
                Save();
                return true;
            }
        }
    }
    public static SavePack savePack = null;

    public static bool notFirstBoot = false;

    public static readonly string AISTGEngVersion = "AISTGEng v2.0";
    public static readonly string SaveDirectory = Application.persistentDataPath + "/AISTGEng";
    public static readonly string SaveFileName = Application.persistentDataPath + "/AISTGEng/score.aisave";
    public static readonly string LogDirectory = Application.persistentDataPath + "/AISTGEng/Log";
    public static readonly string LogFileName = Application.persistentDataPath + "/AISTGEng/Log/{0}.log";
    public static readonly string ScreenshotDirectory = Application.persistentDataPath + "/AISTGEng/Screenshot";
    public static readonly string ScreenshotFileName = Application.persistentDataPath + "/AISTGEng/Screenshot/{0}.png";
    public static readonly string RecordDirectory = Application.persistentDataPath + "/AISTGEng/Record";
    public static readonly string RecordFileName = Application.persistentDataPath + "/AISTGEng/Record/{0}.aireplay";
}
