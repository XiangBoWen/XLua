using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildTool : Editor  //继承子Editor的不会被打包打进去
{
    [MenuItem("Tools/Build Windows Bundle")]
    static void BundleBuildWindows()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Tools/Build Android Bundle")]
    static void BundleBuildAndroid()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("Tools/Build iPhone Bundle")]
    static void BundleBuildIPhone()
    {
        Build(BuildTarget.iOS);
    }

    private static void Build(BuildTarget target)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories); //查找目录下的所有文件
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))  //不打包meta文件
            {
                continue;
            }
            Debug.LogFormat("file:" + files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            string assetName = PathUtil.GetUnityPath(fileName);  //获取文件相对路径

            assetBundle.assetNames = new string[] { assetName };
            Debug.LogFormat("assetNames: {0}", assetBundle.assetNames);

            string bundleName = files[i].Replace(PathUtil.BuildResourcesPath, "").ToLower(); //设置bundleName
            bundleName = bundleName.Remove(0, 1);
            Debug.LogFormat("bundleName: {0}",bundleName);

            assetBundle.assetBundleName = bundleName + ".ab";
            Debug.LogFormat("assetBundleName: {0}", assetBundle.assetBundleName);
            assetBundleBuilds.Add(assetBundle);
        }

        //保证一定有bundle的输出文件目录，且目录干净
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        Debug.LogFormat("BundleOutPath :{0}", PathUtil.BundleOutPath);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);  //打包操作
    }
}
