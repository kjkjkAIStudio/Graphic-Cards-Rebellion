using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(AIAVGEng_Script))]
[ExecuteInEditMode]
public class Editor_AIAVGEng_Script : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.SelectableLabel("警告：Unity 2018.3f2存在一个SetDirty方法的检测bug，即使你按下保存修改，在prefab未保存的情况下会产生异常。保存修改后，随便把prefab中的数据改改再保存。");

        if (GUILayout.Button("保存修改"))
        {
            //编辑数据生效
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, commandAndArgs.ToArray());
            edit.bin = ms.ToArray();
            ms.Close();
            ms.Dispose();
            edit.res_img = res_img.ToArray();
            EditorUtility.SetDirty(target);
        }

        lineId = EditorGUILayout.DelayedIntField("编辑的行号", lineId);
        if (lineId > commandAndArgs.Count - 1)
            lineId = commandAndArgs.Count - 1;

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("在末端添加res_img"))
        {
            res_img.Add(null);
        }
        if (res_img.Count > 0)
        {
            if (GUILayout.Button("删除末端res_img"))
            {
                res_img.RemoveAt(res_img.Count - 1);
            }
        }

        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < res_img.Count; ++i)
            res_img[i] = (Sprite)EditorGUILayout.ObjectField(i.ToString(), res_img[i], typeof(Sprite), false);

        EditorGUILayout.Separator();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("在指定行后添加指令"))
        {
            commandAndArgs.Insert(lineId + 1, new AIAVGEng_Script.CommandAndArg(AIAVGEng_Script.Command.Text, new object()));
        }
        if (commandAndArgs.Count > 1)
        {
            if (GUILayout.Button("删除指定行指令"))
            {
                commandAndArgs.RemoveAt(lineId);
            }
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < commandAndArgs.Count; ++i)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(i.ToString());
            commandAndArgs[i].command = (AIAVGEng_Script.Command)EditorGUILayout.EnumPopup("指令类型", commandAndArgs[i].command);
            switch (commandAndArgs[i].command)
            {
                case AIAVGEng_Script.Command.Text:
                    {
                        if (commandAndArgs[i].arg == null || commandAndArgs[i].arg.GetType() != typeof(AIAVGEng_Script.Arg_Text))
                            commandAndArgs[i].arg = new AIAVGEng_Script.Arg_Text();
                        AIAVGEng_Script.Arg_Text cArgs = (AIAVGEng_Script.Arg_Text)commandAndArgs[i].arg;
                        cArgs.isRight = EditorGUILayout.Toggle("文本框是否在右边", cArgs.isRight);
                        cArgs.str = EditorGUILayout.TextField("对话文本", cArgs.str);
                        commandAndArgs[i].arg = cArgs;
                        break;
                    }
                case AIAVGEng_Script.Command.Img:
                    {
                        if (commandAndArgs[i].arg == null || commandAndArgs[i].arg.GetType() != typeof(AIAVGEng_Script.Arg_Img))
                            commandAndArgs[i].arg = new AIAVGEng_Script.Arg_Img();
                        AIAVGEng_Script.Arg_Img cArgs = (AIAVGEng_Script.Arg_Img)commandAndArgs[i].arg;
                        cArgs.isRight = EditorGUILayout.Toggle("立绘是否在右边", cArgs.isRight);
                        cArgs.img = EditorGUILayout.IntField("立绘的序号（-1为卸载）", cArgs.img);
                        break;
                    }
                case AIAVGEng_Script.Command.DecorationImg:
                    {
                        if (commandAndArgs[i].arg == null || commandAndArgs[i].arg.GetType() != typeof(AIAVGEng_Script.Arg_DecorationImg))
                            commandAndArgs[i].arg = new AIAVGEng_Script.Arg_DecorationImg();
                        AIAVGEng_Script.Arg_DecorationImg cArgs = (AIAVGEng_Script.Arg_DecorationImg)commandAndArgs[i].arg;
                        cArgs.posX = EditorGUILayout.FloatField("装饰图片出现的位置X", cArgs.posX);
                        cArgs.posY = EditorGUILayout.FloatField("装饰图片出现的位置Y", cArgs.posY);
                        cArgs.img = EditorGUILayout.IntField("装饰图片的序号（-1为卸载）", cArgs.img);
                        break;
                    }
            }
        }

        EditorGUI.EndChangeCheck();
    }

    AIAVGEng_Script edit;
    List<AIAVGEng_Script.CommandAndArg> commandAndArgs;
    List<Sprite> res_img;
    int lineId;

    void OnEnable()
    {
        edit = (AIAVGEng_Script)target;
        commandAndArgs = new List<AIAVGEng_Script.CommandAndArg>();
        res_img = new List<Sprite>();
        if (edit.res_img != null)
        {
            for (int i = 0; i < edit.res_img.Length; ++i)
                res_img.Add(edit.res_img[i]);
        }
        if (edit.bin == null)
        {
            commandAndArgs.Add(new AIAVGEng_Script.CommandAndArg(AIAVGEng_Script.Command.Text, new AIAVGEng_Script.Arg_Text()));
        }
        else
        {
            MemoryStream ms = new MemoryStream(edit.bin);
            BinaryFormatter bf = new BinaryFormatter();
            AIAVGEng_Script.CommandAndArg[] temp = (AIAVGEng_Script.CommandAndArg[])bf.Deserialize(ms);
            ms.Close();
            ms.Dispose();
            for (int i = 0; i < temp.Length; ++i)
                commandAndArgs.Add(temp[i]);
        }
        lineId = 0;
    }
}
