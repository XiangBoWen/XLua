using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

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


        List<string> bundleInfos = new List<string>();  //�ļ���Ϣ�б�

        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories); //����Ŀ¼�µ������ļ�
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))  //�����meta�ļ�
            {
                continue;
            }
            //Debug.LogFormat("file:" + files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            string assetName = PathUtil.GetUnityPath(fileName);  //��ȡ�ļ����·��

            assetBundle.assetNames = new string[] { assetName };

            string bundleName = files[i].Replace(PathUtil.BuildResourcesPath, "").ToLower(); //����bundleName
            bundleName = PathUtil.GetStandardPath(bundleName);
            bundleName = bundleName.Remove(0, 1);

            assetBundle.assetBundleName = bundleName + ".ab";
            assetBundleBuilds.Add(assetBundle);

            //����ļ���Ϣ��������Ϣ
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

        //��֤һ����bundle������ļ�Ŀ¼����Ŀ¼�ɾ�
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        //Debug.LogFormat("BundleOutPath :{0}", PathUtil.BundleOutPath);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);  //�������

        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);  //����������Դ�ļ���д��

        AssetDatabase.Refresh();  //ˢ��unityĿ¼
    }

    /// <summary>
    /// ��ȡ�����ļ��б�
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
