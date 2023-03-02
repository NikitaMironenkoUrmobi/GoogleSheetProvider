using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using UnityEditor;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public interface IGoogleSheetsReader : IDisposable
  {
    Dictionary<string, List<object>> GetValuesOnRange(string spreadSheetId, string range);
    TableModel GetTableModel(string sheetID);
  }

  public class GoogleSheetsReader : IGoogleSheetsReader
  {
    private static readonly string[] Scopes = {SheetsService.Scope.Spreadsheets};
    private static readonly string ApplicationName = PlayerSettings.productName;
    private static SheetsService _service;

    public GoogleSheetsReader(string clientSecrets)
    {
      var credential = GoogleCredential.FromJson(clientSecrets).CreateScoped(Scopes);
      _service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName
      });
    }

    public Dictionary<string, List<object>> GetValuesOnRange(string spreadSheetId, string range)
    {
      var request = _service.Spreadsheets.Values.Get(spreadSheetId, range);
      request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;
      var values = request.Execute().Values;
      return values.ToDictionary(k => k.First().ToString(), list => list.ToList().GetRange(1, list.Count - 1));
    }

    public TableModel GetTableModel(string sheetID)
    {
      var request = _service.Spreadsheets.Get(sheetID);
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