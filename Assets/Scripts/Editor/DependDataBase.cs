using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class DependDataBase
{
    string m_FilePath = string.Empty;
    Dictionary<string, Dictionary<string, string>> m_Infos = new Dictionary<string, Dictionary<string, string>>();

    public DependDataBase(string dbAbsPath)
    {
        LoadDataBase(dbAbsPath);
    }

    public bool IsNeedUpdate(string mainFileAssetsPath, string[] dependFileAssetsPathList, string debugStr = null, 
        bool isAutoSave = true, bool isAutoIncludeMeta = true)
    {
        return IsNeedUpdate(mainFileAssetsPath, new List<string>(dependFileAssetsPathList), debugStr, isAutoSave, isAutoIncludeMeta);
    }

    public bool IsNeedUpdate(string mainFileAssetsPath, List<string> dependFileAssetsPathList, string debugStr = null,
        bool isAutoSave = true, bool isAutoIncludeMeta = true)
    {
        if (!m_Infos.ContainsKey(mainFileAssetsPath))
        {
            m_Infos[mainFileAssetsPath] = new Dictionary<string, string>();
        }

        if (debugStr == null)
        {
            debugStr = mainFileAssetsPath;
        }

        Dictionary<string, string> dependDict = new Dictionary<string, string>();


        for (int i = 0; i < dependFileAssetsPathList.Count; ++i)
        {
            if (dependFileAssetsPathList[i].ToLower().EndsWith(".meta"))
            {
                continue;
            }

            string dependFileAbsPath = EditorUtils.AssetsPath2ABSPath(dependFileAssetsPathList[i]);
            dependDict[dependFileAssetsPathList[i]] = FileMD5Cache.S.GetMD5(dependFileAbsPath);
            string metaFileAbsPath = dependFileAbsPath + ".meta";
            string metaFileAssetsPath = dependFileAssetsPathList[i] + ".meta";

            if (isAutoIncludeMeta)
            {
                if (File.Exists(metaFileAbsPath))
                {
                    dependDict[metaFileAssetsPath] = FileMD5Cache.S.GetMD5(metaFileAbsPath);
                }
            }
        }

        bool isNeedUpdate = false;
        foreach (KeyValuePair<string, string> kv in dependDict)
        {
            string dependFileAssetsPath = kv.Key;
            // new depend file
            if (!m_Infos[mainFileAssetsPath].ContainsKey(dependFileAssetsPath))
            {
                isNeedUpdate = true;
                Debug.Log("DependDataBase NeedUpdate " + debugStr + " add new depend file " + dependFileAssetsPath);
                break;
            }

            // depend file changed
            string oldMD5 = m_Infos[mainFileAssetsPath][dependFileAssetsPath];
            string newMD5 = kv.Value;
            if (oldMD5 != newMD5)
            {
                isNeedUpdate = true;
                Debug.Log("DependDataBase NeedUpdate " + debugStr + " depend file change " + dependFileAssetsPath + " oldmd5 " + oldMD5 + " newmd5 " + newMD5);
                break;
            }
        }

        if (dependDict.Count != m_Infos[mainFileAssetsPath].Count)
        {
            isNeedUpdate = true;
        }

        if (isNeedUpdate == true)
        {
            m_Infos[mainFileAssetsPath] = dependDict;
        }

        if (isAutoSave && isNeedUpdate)
        {
            SaveDataBase();
        }

        return isNeedUpdate;
    }

    public void Save()
    {
        SaveDataBase();
    }

    void LoadDataBase(string dbAbsPath)
    {
        m_Infos.Clear();
        m_FilePath = dbAbsPath;

        try
        {
            using (StreamReader reader = File.OpenText(m_FilePath))
            {
                m_Infos = JsonMapper.ToObject<Dictionary<string, Dictionary<string, string>>>(reader);
            }
        }
        catch
        {
            m_Infos.Clear();
        }
    }

    void SaveDataBase()
    {
        try
        {
            Debug.Log("[DependDataBase:SaveDataBase]");
            StreamWriter writer = new StreamWriter(m_FilePath, false);
            JsonWriter jw = new JsonWriter(writer);
            jw.PrettyPrint = true;
            JsonMapper.ToJson(m_Infos, jw);
            writer.Close();
        }
        catch (System.Exception ex)
        {
            throw new Exception("Could not save depend data base " + m_FilePath + " " + ex.Message);
        }
    }
}
