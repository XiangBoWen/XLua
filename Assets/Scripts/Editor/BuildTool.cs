using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildTool : Editor  //�̳���Editor�Ĳ��ᱻ������ȥ
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
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories); //����Ŀ¼�µ������ļ�
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))  //�����meta�ļ�
            {
                continue;
            }
            Debug.LogFormat("file:" + files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            string assetName = PathUtil.GetUnityPath(fileName);  //��ȡ�ļ����·��

            assetBundle.assetNames = new string[] { assetName };
            Debug.LogFormat("assetNames: {0}", assetBundle.assetNames);

            string bundleName = files[i].Replace(PathUtil.BuildResourcesPath, "").ToLower(); //����bundleName
            bundleName = bundleName.Remove(0, 1);
            Debug.LogFormat("bundleName: {0}",bundleName);

            assetBundle.assetBundleName = bundleName + ".ab";
            Debug.LogFormat("assetBundleName: {0}", assetBundle.assetBundleName);
            assetBundleBuilds.Add(assetBundle);
        }

        //��֤һ����bundle������ļ�Ŀ¼����Ŀ¼�ɾ�
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        Debug.LogFormat("BundleOutPath :{0}", PathUtil.BundleOutPath);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);  //�������
    }
}
