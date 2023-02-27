using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings
{
  public static class GoogleSheetsVariables
  {
    public const string ConfigDatabasePath = "ConfigurationDatabase/ConfigDatabase";
    public const string ConfigDatabaseTemplateName = "ConfigDatabaseTemplate.json";
    public static class SavePaths
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        public static readonly string DATABASE_SAVEPATH = $"{Application.persistentDataPath}/.database";
        public static readonly string TAMPLATE_SAVEPATH = $"{Application.persistentDataPath}/Resources";
#else
      public static readonly string DATABASE_SAVEPATH = $"{Application.persistentDataPath}/.database";
      public static readonly string TAMPLATE_SAVEPATH = $"{Application.dataPath}/Resources";
#endif
    }
  }
}