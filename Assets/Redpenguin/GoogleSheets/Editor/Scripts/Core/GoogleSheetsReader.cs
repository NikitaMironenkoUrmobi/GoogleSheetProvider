using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class GoogleSheetsReader : IDisposable
  {
    private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};
    private static readonly string ApplicationName = PlayerSettings.productName;
    private static string _spreadsheetId;
    private static SheetsService _service;

    public GoogleSheetsReader(string spreadSheetId, string clientSecrets)
    {
      _spreadsheetId = spreadSheetId;
      var credential = GoogleCredential.FromJson(clientSecrets).CreateScoped(Scopes);
      _service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName
      });
    }

    public Dictionary<string, List<object>> GetValuesOnRange(string range)
    {
      var request = _service.Spreadsheets.Values.Get(_spreadsheetId, range);
      request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;
      var values = request.Execute().Values;
      DebugLog(values);
      return values.ToDictionary(k => k.First().ToString(), list => list.ToList().GetRange(1, list.Count - 1));
    }

    public TableModel GetTableModel()
    {
      var request = _service.Spreadsheets.Get(_spreadsheetId);
      request.IncludeGridData = false;
      var spreadsheet = request.Execute();
      return new TableModel
      {
        SheetNames = spreadsheet.Sheets.Select(x => x.Properties.Title).ToList()
      };
    }
    
    

    private void DebugLog(IEnumerable<IList<object>> values)
    {
      var sb = new StringBuilder();
      foreach (var item in values)
      {
        foreach (var val in item)
        {
          sb.Append(val);
          sb.Append(" ");
        }
        //Debug.Log(sb.ToString());
        sb.Clear();
      }
    }

    public void Dispose()
    {
      _service.Dispose();
    }
  }
  
  public struct TableModel
  {
    public List<string> SheetNames;
  }
}