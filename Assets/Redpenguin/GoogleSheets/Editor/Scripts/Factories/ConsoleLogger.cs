using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Factories
{
  public class ConsoleLogger : IConsoleLogger
  {
    private const string SpreadSheetAttributeLink
      = "<a href=\"Assets/Redpenguin/GoogleSheets/Runtime/Attributes/SpreadSheet.cs\" line=\"6\">SpreadSheet</a>";

    private const string ExampleAttributeUsageLink
      = "<a href=\"Assets/Redpenguin/GoogleSheets/Runtime/Examples/ExampleData.cs\" line=\"8\">Example</a>";
    
    public void LogCantFindClassesWithSpreadSheetAttribute()
    {
      Debug.Log($"Can't find classes with {SpreadSheetAttributeLink} attribute. Click on link to see example({ExampleAttributeUsageLink})"
        .WithColor(ColorExt.ErrorColor));
    }
    public void LogCantFindClassesWithSpreadSheetAttribute(string profileName)
    {
      Debug.LogError(
        $"Can't find class with SpreadSheet attribute that has SheetName which contains in {profileName} profile table");
    }
  }
}