using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

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


        List<string> bundleInfos = new List<string>();  //文件信息列表

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories); //查找目录下的所有文件
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))  //不打包meta文件
            {
                continue;
            }
            //Debug.LogFormat("file:" + files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            string assetName = PathUtil.GetUnityPath(fileName);  //获取文件相对路径

            assetBundle.assetNames = new string[] { assetName };

            string bundleName = files[i].Replace(PathUtil.BuildResourcesPath, "").ToLower(); //设置bundleName
            bundleName = PathUtil.GetStandardPath(bundleName);
            bundleName = bundleName.Remove(0, 1);

            assetBundle.assetBundleName = bundleName + ".ab";
            assetBundleBuilds.Add(assetBundle);

            //添加文件信息和依赖信息
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + bundleName + ".ab";
            Debug.LogFormat("bundleInfo: {0}", bundleInfo);
            if (dependenceInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo);
            }
            Debug.LogFormat("bundleInfo: {0}", bundleInfo);
            bundleInfos.Add(bundleInfo);
        }

        //保证一定有bundle的输出文件目录，且目录干净
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        //Debug.LogFormat("BundleOutPath :{0}", PathUtil.BundleOutPath);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);  //打包操作

        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);  //创建依赖资源文件并写入

        AssetDatabase.Refresh();  //刷新unity目录
    }

    /// <summary>
    /// 获取依赖文件列表
    /// </summary>
    /// <param name="curFile"></param>
    /// <returns></returns>
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        dependence = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();
        return dependence;
    }
}
