# UnityDemo_DepndDataBase

json文件保存依懒文件的md5，用来判断是否需要重新生成数据

代码调用

```C#

DependDataBase dababase = new DependDataBase(absPath);
var isNeedUpdate = dababase.IsNeedUpdate("SampleScene", dependArray, "SampleScene");
if (isNeedUpdate)
{
    Debug.Log("需要刷新");
}

```

生成json文件示例

```json

{
    "SampleScene" : {
        "Assets/Scenes/SampleScene.unity" : "836349a24b7c7f2423b1c57264f1be8c",
        "Assets/Scenes/SampleScene.unity.meta" : "097fac763945c6bda1035d2a0a22dc50",
        "Assets/Scenes/DependDatabaseTest.cs"  : "6c420e2056cd1585a84dbce2112f56eb",
        "Assets/Scenes/DependDatabaseTest.cs.meta" : "0e20b3cf026ca49cdde33c53daffaac1"
    }
}

```