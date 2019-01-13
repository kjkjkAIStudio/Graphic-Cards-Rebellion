using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class Editor_StagePacker : Editor
{
    [MenuItem("Assets/打包所有Stage")]
    public static void Pack()
    {
        string assetbundlePath = Application.streamingAssetsPath;
        if (!Directory.Exists(assetbundlePath))
            Directory.CreateDirectory(assetbundlePath);
#if UNITY_STANDALONE_WIN
        BuildPipeline.BuildAssetBundles(assetbundlePath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
#endif
#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(assetbundlePath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
#endif
    }
}
