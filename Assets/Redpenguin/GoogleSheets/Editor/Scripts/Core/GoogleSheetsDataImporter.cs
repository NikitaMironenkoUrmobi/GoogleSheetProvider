using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public interface IGoogleSheetsDataImporter
  {
    void LoadDataToContainers(List<ISheetDataContainer> loadContainers, string tableID, string profileName);
  }

  public class GoogleSheetsDataImporter : IGoogleSheetsDataImporter
  {
    private readonly IGoogleSheetsReader _googleSheetsReader;

    public GoogleSheetsDataImporter(IGoogleSheetsReader googleSheetsReader)
    {
      _googleSheetsReader = googleSheetsReader;
    }

    public void LoadDataToContainers(List<ISheetDataContainer> loadContainers, string tableID, string profileName)
    {
      foreach (var sheetDataContainer in loadContainers)
      {
        var sheetValues = GetSheetValues(sheetDataContainer.SheetDataType, tableID, profileName);
        var dataList = sheetDataContainer.GetType().GetFields().FirstOrDefault(x => (x.GetValue(sheetDataContainer) is IList));
        if(dataList == null) return;
        sheetDataContainer.SetListCount(sheetValues.First().Value.Count);
        SetValues(dataList.GetValue(sheetDataContainer) as IList, sheetDataContainer.SheetDataType.Name, sheetValues);
      }

      Debug.Log($"Load data from Google Sheets was completed!".WithColor(ColorExt.CompletedColor));
    }

    private void SetValues(IList dataList, string sheetName, IReadOnlyDictionary<string, List<object>> sheetValues)
    {
      var emptyData = new List<EmptyObject>();
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
          if (fieldData == null || fieldData.ToString() == string.Empty)
          {
            
            var data = new EmptyObject(sheetName, field.Name, i + 1, dataClass);
            if(!emptyData.Contains(data))
              emptyData.Add(data);
            continue;
          }
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
            Debug.LogError($"Sheet {sheetName}, field {field.Name} format isn't correct!");
            throw;
          }
        }
      }
      foreach (var objectToDelete in emptyData)
      {
        Debug.LogWarning($"Sheet {objectToDelete.SheetName}, field {objectToDelete.FieldName} row {objectToDelete.Row} is empty!. Skip object this object.");
        dataList.Remove(objectToDelete.Obj);
      }
    }

    private struct EmptyObject
    {
      public bool Equals(EmptyObject other)
      {
        return this == other;
      }

      public override bool Equals(object obj)
      {
        return obj is EmptyObject other && Equals(other);
      }

      public override int GetHashCode()
      {
        return HashCode.Combine(Row, SheetName, FieldName, Obj);
      }

      public readonly int Row;
      public readonly string SheetName;
      public readonly string FieldName;
      public readonly object Obj;

      public EmptyObject(string sheetName, string fieldName, int row, object obj)
      {
        SheetName = sheetName;
        Row = row;
        Obj = obj;
        FieldName = fieldName;
      }

      public static bool operator ==(EmptyObject obj1, EmptyObject obj2)
      {
        return obj1.Obj == obj2.Obj && obj1.Row == obj2.Row;
      }

      public static bool operator !=(EmptyObject obj1, EmptyObject obj2)
      {
        return !(obj1 == obj2);
      }
    }

    private Dictionary<string, List<object>> GetSheetValues(Type databaseType, string tableID, string profileName)
    {
      var spreadSheetRange = databaseType.GetCustomAttributes(typeof(SpreadSheetAttribute), true) as SpreadSheetAttribute[];
      var range = spreadSheetRange.First(x => string.IsNullOrEmpty(x.Profile) || x.Profile == profileName);
      return _googleSheetsReader.GetValuesOnRange(tableID, range.Range);
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