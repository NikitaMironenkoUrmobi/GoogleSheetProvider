using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  public class SpreadSheetScriptableObjectFactory
  {
    private const string SavePath = "Assets/GoogleSheets/Resources/SheetsData";
    private string _savePath = $"{Application.dataPath}/GoogleSheets/Resources/SheetsData";
    private const string ConfigSavePath = "Assets/GoogleSheets/Resources/ConfigurationDatabase";
    private string _configSavePath = $"{Application.dataPath}/GoogleSheets/Resources/ConfigurationDatabase";

    public SpreadSheetScriptableObjectFactory()
    {
      CreateDirectories();
    }

    private void CreateDirectories()
    {
      if (!Directory.Exists(_savePath))
      {
        Directory.CreateDirectory(_savePath);
      }

      if (!Directory.Exists(_configSavePath))
      {
        Directory.CreateDirectory(_configSavePath);
      }
    }

    public void CreateScriptableObjects(List<Type> types)
    {
      if(types.Count == 0) return;
      //DeleteAllAssets();
      foreach (var type in types)
      {
        var asset = ScriptableObject.CreateInstance(type);
        AssetDatabase.CreateAsset(asset, $"{SavePath}/{type.Name}.asset");
      }
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      
      Debug.Log("Scriptable Objects was created!");
    }

    public void CreatConfigDatabase(Type type)
    {
      return;
      var asset = ScriptableObject.CreateInstance(type);
      AssetDatabase.CreateAsset(asset, $"{ConfigSavePath}/ConfigDatabase.asset");
      EditorUtility.SetDirty(asset);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      //Debug.Log("<color=green>ConfigDatabase was created</color>");
    }
    
    public void DeleteAllAssets()
    {
      var di = new DirectoryInfo(SavePath);
      foreach (var file in di.EnumerateFiles())
      {
        Debug.Log($"{file.Name} delete");
        file.Delete(); 
      }
      AssetDatabase.Refresh();
      AssetDatabase.SaveAssets();
    }
  }
}