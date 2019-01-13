using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[PreferBinarySerialization]
public class AIAVGEng_Script : MonoBehaviour
{
    [Serializable]
    public class Arg_Text
    {
        public bool isRight;
        public string str;
    }
    [Serializable]
    public class Arg_Img
    {
        public bool isRight;
        public int img;
    }
    [Serializable]
    public class Arg_DecorationImg
    {
        public float posX, posY;
        public int img;
    }
    public enum Command { Text, Img, DecorationImg };
    [Serializable]
    public class CommandAndArg
    {
        public Command command;
        public object arg;

        public CommandAndArg(Command command, object arg)
        {
            this.command = command;
            this.arg = arg;
        }
    }
    public CommandAndArg[] commandAndArgs;

    public byte[] bin;
    public Sprite[] res_img;

    void Awake()
    {
        MemoryStream ms = new MemoryStream(bin);
        BinaryFormatter bf = new BinaryFormatter();
        commandAndArgs = (CommandAndArg[])bf.Deserialize(ms);
        ms.Close();
        ms.Dispose();
        bin = null;
    }
}
