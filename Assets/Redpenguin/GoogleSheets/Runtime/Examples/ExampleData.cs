using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets.Examples
{
  //[SpreadSheet("Example")] //Get values from Example sheet. Uncomment attribute for create example container
  [Serializable]
  public class ExampleData : ISheetData
  {
    public string myString; // string
    public int myInt; // 1
    public bool myBool; // true
    public List<int> myInts; // [1,2,3]
    public List<string> myStrings; // ["a","b","c"]
    public JsonExample jsonExample; // {"id":1,"myString":"string"}
    public ExampleEnum exampleEnum; // Example1
  }
  
  [Serializable]
  public class JsonExample
  {
    public int id;
    public string myString;
  }

  public enum ExampleEnum
  {
    None,
    Example1,
    Example2
  }

  //Access via extension to container with ExampleData
  public static class ExampleDataProviderExtension
  {
    public static bool HasBoolTrue(this ISheetDataProvider<ExampleData> provider)
    {
      var data = provider.Data.Find(x => x.myBool);
      return data != null;
    }
  }

  public class ExampleDatabaseProvider
  {
    public ExampleDatabaseProvider()
    {
      var databaseProvider = new SpreadSheetsDatabaseProvider();
      var database = databaseProvider.Load("ExampleProfile"); //Load all sheets that profile contains 
      var exampleData = database.GetSpreadSheetData<ExampleData>(); //Get list of ExampleData
    }
  }
}