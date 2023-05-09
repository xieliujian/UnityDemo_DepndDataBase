using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class EditorUtils
{
    static MD5CryptoServiceProvider m_MD5CSP = new MD5CryptoServiceProvider();

    public static string AssetsPath2ABSPath(string assetsPath)
    {
        string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
        return assetRootPath.Substring(0, assetRootPath.Length - 6) + assetsPath;
    }

    public static string ABSPath2AssetsPath(string absPath)
    {
        string assetRootPath = System.IO.Path.GetFullPath(Application.dataPath);
        return "Assets" + System.IO.Path.GetFullPath(absPath).Substring(assetRootPath.Length).Replace("\\", "/");
    }

    public static string GetFileMD5Value(string absPath)
    {
        if (!File.Exists(absPath))
            return "";

        FileStream file = new FileStream(absPath, System.IO.FileMode.Open);
        byte[] retVal = m_MD5CSP.ComputeHash(file);
        file.Close();
        string result = "";

        for (int i = 0; i < retVal.Length; i++)
        {
            result += retVal[i].ToString("x2");
        }

        return result;
    }
}
