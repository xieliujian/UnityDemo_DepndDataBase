using System;
using System.IO;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEditor;

public class FileMD5Cache
{
    [Serializable]
    class Info
    {
        public string md5;
        public long lastWriteTime;
    }
    
    const string FILE_MD5_CACHE_LIBRARY_PATH = "Library/FileMD5Cache.json";
    
    static FileMD5Cache s_Instance;

    Dictionary<string, Info> m_InfoMap = new Dictionary<string, Info>();

    public static FileMD5Cache S
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new FileMD5Cache();
                s_Instance.Load();
            }

            return s_Instance;
        }
    }

    public string GetMD5(string absPath)
    {
        Info info;
        long curLastWriteTime = File.GetLastWriteTime(absPath).Ticks;
        
        if (!m_InfoMap.TryGetValue(absPath, out info))
        {
            info = new Info();
            info.md5 = EditorUtils.GetFileMD5Value(absPath);
            info.lastWriteTime = curLastWriteTime;
            m_InfoMap[absPath] = info;
        }
        else
        {
            if (info.lastWriteTime != curLastWriteTime)
            {
                info.md5 = EditorUtils.GetFileMD5Value(absPath);
                info.lastWriteTime = curLastWriteTime;
            }
        }
        
        return info.md5;
    }

    public void Save()
    {
        try
        {
            StreamWriter writer = new StreamWriter(FILE_MD5_CACHE_LIBRARY_PATH, false);
            JsonWriter jw = new JsonWriter(writer);
            jw.PrettyPrint = true;
            JsonMapper.ToJson(m_InfoMap, jw);
            writer.Close();
        }
        catch (System.Exception ex)
        {
            throw new Exception("Could not save file md5 cache " + FILE_MD5_CACHE_LIBRARY_PATH + " " + ex.Message);
        }
    }

    void Load()
    {
        m_InfoMap.Clear();

        try
        {
            using (StreamReader reader = File.OpenText(FILE_MD5_CACHE_LIBRARY_PATH))
            {
                m_InfoMap = JsonMapper.ToObject<Dictionary<string, Info>>(reader);
            }
        }
        catch
        {
            m_InfoMap.Clear();
        }
    }
}
