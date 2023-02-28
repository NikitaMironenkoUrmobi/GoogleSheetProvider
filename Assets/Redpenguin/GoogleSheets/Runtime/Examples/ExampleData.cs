using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Attributes;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets.Examples
{
  [SpreadSheet("Example")] //Get values from Example sheet. Uncomment attribute for create example container
  [Serializable]
  public class ExampleData : ISheetData
  {
    public string myString;         // string
    public int myInt;               // 1
    public bool myBool;             // true
    public List<int> myInts;        // [1,2,3]
    public List<string> myStrings;  // ["a","b","c"]
    public JsonExample jsonExample; // {"id":1,"myString":"string"}
    public ExampleEnum exampleEnum; // Example1
  }
  [SpreadSheet("Example")] //Get values from Example sheet. Uncomment attribute for create example container
  [Serializable]
  public class ExampleData2 : ISheetData
  {
    public string myString;         // string
    public int myInt;               // 1
    public bool myBool;             // true
    public List<int> myInts;        // [1,2,3]
    public List<string> myStrings;  // ["a","b","c"]
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

  public static class ExampleDataProviderExtension
  {
    public static bool HasBoolTrue(this ISheetDataProvider<ExampleData> provider)
    {
      var data = provider.Data.Find(x => x.myBool);
      return data != null;
    }
  }
}