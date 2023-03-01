using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class DataImporter
  {
    private const string SheetsData = "SheetsData";
    private readonly GoogleSheetsReader _sheetsReader;

    public DataImporter(GoogleSheetsReader sheetsReader)
    {
      _sheetsReader = sheetsReader;
    }

    public void LoadAndFillDataContainers(List<ISheetDataContainer> databaseScriptObj)
    {
      foreach (var database in databaseScriptObj)
      {
        //var databaseSO = (databaset as ISpreadSheetSO);
        //var database = databaseSO.SheetDataContainer;
        //if(databaseSO.IsLoad == false) continue;
        var sheetValues = GetSheetValues(database.SheetDataType);
        var dataList = database.GetType().GetFields().FirstOrDefault(x => (x.GetValue(database) is IList));
        if(dataList == null) return;
        database.SetListCount(sheetValues.First().Value.Count);
        SetValues(dataList.GetValue(database) as IList, database.SheetDataType.Name, sheetValues);
        //EditorUtility.SetDirty(databaset);
      }

      Debug.Log($"Load data from Google Sheets was completed!".WithColor(ColorExt.CompletedColor));
    }

    private void SetValues(IList dataList, string soName,
      IReadOnlyDictionary<string, List<object>> sheetValues)
    {
      for (var i = 0; i < dataList.Count; i++)
      {
        var dataClass = dataList[i];
        var dataClassFields = dataClass.GetType().GetFields();
        foreach (var field in dataClassFields)
        {
          var fieldName = field.Name;
          if (!sheetValues.ContainsKey(fieldName)) continue;
          if (sheetValues[fieldName].Count <= i) continue;

          var fieldData = sheetValues[fieldName][i];
          var isJson = IsJson(field.FieldType, fieldData);
          var isEnum = IsEnum(field.FieldType, fieldData);
          try
          {
            if (isJson != null)
            {
              field.SetValue(dataClass, isJson);
            }
            else if (isEnum != null)
            {
              field.SetValue(dataClass, isEnum);
            }
            else
            {
              field.SetValue(dataClass, Convert.ChangeType(sheetValues[fieldName][i], field.FieldType));
            }
          }
          catch
          {
            Debug.LogError($"Table {soName}, field {field.Name} format isn't correct!");
            throw;
          }
        }
      }
    }

    private void SetValues(FieldInfo list, ISheetDataContainer database,
      IReadOnlyDictionary<string, List<object>> sheetValues)
    {
      if (!(list.GetValue(database) is IList dataList)) return;
      for (var i = 0; i < dataList.Count; i++)
      {
        var dataClass = dataList[i];
        var dataClassFields = dataClass.GetType().GetFields();
        foreach (var field in dataClassFields)
        {
          var fieldName = field.Name;
          if (!sheetValues.ContainsKey(fieldName)) continue;
          if (sheetValues[fieldName].Count <= i) continue;

          var fieldData = sheetValues[fieldName][i];
          var isJson = IsJson(field.FieldType, fieldData);
          var isEnum = IsEnum(field.FieldType, fieldData);
          if (isJson != null)
          {
            field.SetValue(dataClass, isJson);
          }
          else if (isEnum != null)
          {
            field.SetValue(dataClass, isEnum);
          }
          else
          {
            field.SetValue(dataClass, Convert.ChangeType(sheetValues[fieldName][i], field.FieldType));
          }
        }
      }
    }

    private Dictionary<string, List<object>> GetSheetValues(Type databaseType)
    {
      var spreadSheetRange = databaseType.GetAttributeValue((SpreadSheet st) => st.Range);
      return _sheetsReader.GetValuesOnRange(spreadSheetRange);
    }

    private object IsJson(Type type, object value)
    {
      try
      {
        var result = JsonConvert.DeserializeObject(value.ToString(), type);
        return result;
      }
      catch
      {
        return null;
      }
    }

    private object IsEnum(Type type, object value)
    {
      try
      {
        var result = Enum.Parse(type, value.ToString());
        return result;
      }
      catch
      {
        return null;
      }
    }
  }
}