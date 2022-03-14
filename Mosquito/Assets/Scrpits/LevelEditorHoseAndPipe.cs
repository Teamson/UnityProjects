using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;
using System;
using System.Reflection;

public class LevelEditorHoseAndPipe : MonoBehaviour
{
    [MenuItem("Nigel/保存关卡-蚊子快冲")]
    private static void SaveLevel()
    {
        var levelManager = GameObject.Find("LevelManager");
        var childCount = levelManager.transform.childCount;
        Debug.Log("关卡数：" + childCount);


        for (var i = 0; i < childCount; i++)
        {
            var jsonStr = "[\n";

            //根据索引获取关卡
            var level = levelManager.transform.GetChild(i);
            var levelChildCount = level.transform.childCount;

            var dataLevel = new JsonData();

            //dataLevel["itemX"] = new JsonData();
            //dataLevel["itemX"] = level.localScale.x.ToString("0.00");
            //jsonStr += dataLevel.ToJson();
            //jsonStr += ",\n";

            for (var j = 0; j < levelChildCount; j++)
            {
                var data = new JsonData();

                //处理名字
                var levelItem = level.transform.GetChild(j);

                data["name"] = levelItem.gameObject.name.IndexOf(" ") != -1 ? levelItem.gameObject.name.Split(' ')[0] : data["name"] = levelItem.gameObject.name;

                var obj = getRealRotation(levelItem.gameObject);

                data["position"] = new JsonData();
                data["position"]["x"] = (-levelItem.position.x).ToString("0.00");
                data["position"]["y"] = (levelItem.position.y).ToString("0.00");
                data["position"]["z"] = levelItem.position.z.ToString("0.00");

                data["rotation"] = new JsonData();
                data["rotation"]["x"] = levelItem.eulerAngles.x.ToString("0.00");
                //data["rotation"]["x"] = obj.x.ToString("0.00");
                data["rotation"]["y"] = levelItem.eulerAngles.y > 180 ? (360 - levelItem.eulerAngles.y).ToString("0.00") : (-levelItem.eulerAngles.y).ToString("0.00");
                data["rotation"]["z"] = levelItem.eulerAngles.z.ToString("0.00");

                data["scale"] = new JsonData();
                data["scale"]["x"] = levelItem.localScale.x.ToString("0.00");
                data["scale"]["y"] = levelItem.localScale.y.ToString("0.00");
                data["scale"]["z"] = levelItem.localScale.z.ToString("0.00");

                data["value"] = new JsonData();
                data["value"]["v"] = levelItem.name.Contains("Slice") ? levelItem.GetChild(0).name : "0";

                jsonStr += data.ToJson();

                jsonStr += (j < levelChildCount - 1 ? ",\n" : "\n]");
            }
            Debug.Log(jsonStr);

            SaveJson(level.gameObject.name, jsonStr);
        }
    }

    private static void SaveJson(string fileName, string text)
    {
        //流写入器
        StreamWriter sw;
        //文件信息操作类
        FileInfo info = new FileInfo("E:/FDProjects/Projects/Mosquito/bin/res/config" + "/" + fileName + ".json");
        //判断路径是否存在
        if (!info.Exists)
        {
            sw = info.CreateText();
        }
        else
        {
            //先删除再创建
            info.Delete();
            sw = info.CreateText();
        }

        sw.WriteLine(text);
        sw.Close();

        Debug.Log(Application.dataPath + "/" + fileName + ".json" + "  保存成功!");
    }


    private static Vector3 getRealRotation(GameObject target)
    {
        // 获取原生值
        System.Type transformType = target.transform.GetType();
        PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(target.transform, null);
        MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        object value = m_methodInfo_GetLocalEulerAngles.Invoke(target.transform, new object[] { m_OldRotationOrder });

        //Debug.LogError("反射调用GetLocalEulerAngles方法获得的值：" + value.ToString());
        //Debug.LogError("transform.localEulerAngles获取的值：" + target.transform.localEulerAngles.ToString());

        return (Vector3)value;
    }
}