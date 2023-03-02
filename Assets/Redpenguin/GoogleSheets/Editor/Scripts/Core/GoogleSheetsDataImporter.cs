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
    void LoadDataToContainers(List<ISheetDataContainer> loadContainers, string tableID);
  }

  public class GoogleSheetsDataImporter : IGoogleSheetsDataImporter
  {
    private readonly IGoogleSheetsReader _googleSheetsReader;

    public GoogleSheetsDataImporter(IGoogleSheetsReader googleSheetsReader)
    {
      _googleSheetsReader = googleSheetsReader;
    }

    public void LoadDataToContainers(List<ISheetDataContainer> loadContainers, string tableID)
    {
      foreach (var sheetDataContainer in loadContainers)
      {
        //var databaseSO = (databaset as ISpreadSheetSO);
        //var database = databaseSO.SheetDataContainer;
        //if(databaseSO.IsLoad == false) continue;
        var sheetValues = GetSheetValues(sheetDataContainer.SheetDataType, tableID);
        var dataList = sheetDataContainer.GetType().GetFields().FirstOrDefault(x => (x.GetValue(sheetDataContainer) is IList));
        if(dataList == null) return;
        sheetDataContainer.SetListCount(sheetValues.First().Value.Count);
        SetValues(dataList.GetValue(sheetDataContainer) as IList, sheetDataContainer.SheetDataType.Name, sheetValues);
        //EditorUtility.SetDirty(databaset);
      }

      Debug.Log($"Load data from Google Sheets was completed!".WithColor(ColorExt.CompletedColor));
    }

    private void SetValues(IList dataList, string sheetName, IReadOnlyDictionary<string, List<object>> sheetValues)
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
            Debug.LogError($"Sheet {sheetName}, field {field.Name} format isn't correct!");
            throw;
          }
        }
      }
    }

    private Dictionary<string, List<object>> GetSheetValues(Type databaseType, string tableID)
    {
      var spreadSheetRange = databaseType.GetAttributeValue((SpreadSheetAttribute st) => st.Range);
      return _googleSheetsReader.GetValuesOnRange(tableID, spreadSheetRange);
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