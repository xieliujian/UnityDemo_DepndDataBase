using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(DependDatabaseTest))]
public class DependDatabaseTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (DependDatabaseTest)target;
        if (script == null)
            return;

        if (GUILayout.Button("导出依懒文件"))
        {
            var path = "Assets/Scenes/SampleScene_depend.json";
            var absPath = EditorUtils.AssetsPath2ABSPath(path);

            var curScene = EditorSceneManager.GetActiveScene();
            var scenePath = curScene.path;
            var dependArray = AssetDatabase.GetDependencies(scenePath);

            DependDataBase dababase = new DependDataBase(absPath);
            var isNeedUpdate = dababase.IsNeedUpdate("SampleScene", dependArray, "SampleScene");
            if (isNeedUpdate)
            {
                Debug.Log("需要刷新");
            }
        }
    }
}
