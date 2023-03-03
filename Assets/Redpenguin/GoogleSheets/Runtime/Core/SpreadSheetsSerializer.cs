using System;
using System.Collections.Generic;
using System.IO;
using Redpenguin.GoogleSheets.Settings;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Core
{
  public interface ISpreadSheetsSerializer
  {
    void SerializationByRule(List<ISheetDataContainer> sheetDataContainers);
    SpreadSheetsDatabase DeserializeByRule();
  }

  public class SpreadSheetsSerializer : ISpreadSheetsSerializer
  {
    private readonly SerializeSettingsContainer _settingsContainerContainer;
    private readonly string _profileName;

    public SpreadSheetsSerializer(string profileName, SerializeSettingsContainer settingsContainerContainer)
    {
      _profileName = profileName;
      _settingsContainerContainer = settingsContainerContainer;
    }

    public void SerializationByRule(List<ISheetDataContainer> sheetDataContainers)
    {
      var serializationRuleSetting = _settingsContainerContainer.GetSerializeRuleSetting(_profileName);
      if (RuleTypeEmpty(serializationRuleSetting.serializationRuleType)) return;
      var type = Type.GetType(serializationRuleSetting.serializationRuleType);
      TypeError(type);

      var serializationRule =
        type == null ? new JsonSerializationRule() : Activator.CreateInstance(type) as SerializationRule;

      var container = new SpreadSheetsDatabase();
      foreach (var sheetDataContainer in sheetDataContainers)
      {
        var metaData =
          _settingsContainerContainer.GetSerializeSetting(_profileName, sheetDataContainer.SheetDataType.ToString());
        if (!metaData.saveSeparately)
        {
          container.AddContainer(sheetDataContainer);
        }
        else
        {
          var fileName = metaData.overrideName ? metaData.fileName : sheetDataContainer.SheetDataType.Name;
          serializationRule?.Serialization(metaData.savePath, fileName, sheetDataContainer);
          Debug.Log($"{fileName} save to {metaData.savePath}".WithColor(ColorExt.CompletedColor));
        }
      }

      if (container.Containers.Count != 0)
      {
        serializationRule?.Serialization(serializationRuleSetting.savePath, serializationRuleSetting.fileName,
          container);
        Debug.Log(
          $"{serializationRuleSetting.fileName} save to {serializationRuleSetting.savePath}".WithColor(
            ColorExt.CompletedColor));
      }
    }

    public SpreadSheetsDatabase DeserializeByRule()
    {
      var serializationRuleSetting = _settingsContainerContainer.GetSerializeRuleSetting(_profileName);
      var serializeSettings = _settingsContainerContainer.GetSerializeSetting(_profileName);
      if (RuleTypeEmpty(serializationRuleSetting.serializationRuleType)) return null;
      var type = Type.GetType(serializationRuleSetting.serializationRuleType);
      TypeError(type);
      var serializationRule =
        type == null ? new JsonSerializationRule() : Activator.CreateInstance(type) as SerializationRule;

      var container = new SpreadSheetsDatabase();
      if (serializeSettings.Exists(x => !x.saveSeparately))
      {
        var path = FixPath(serializationRuleSetting.savePath);
        var loadPath = Path.Combine(path, serializationRuleSetting.fileName);
        var textAsset = Resources.Load<TextAsset>(loadPath);
        if (textAsset == null)
        {
          Debug.LogError(
            $"{serializationRuleSetting.fileName} cant load from path {loadPath}. Make sure that file under Resource folder.");
        }
        else
        {
          container = serializationRule?.Deserialization<SpreadSheetsDatabase>(textAsset.text);
          Debug.Log(
            $"Load {serializationRuleSetting.fileName} from Resources\\{loadPath}".WithColor(ColorExt.CompletedColor));
        }
      }

      foreach (var serializeSetting in serializeSettings)
      {
        if (serializeSetting.saveSeparately)
        {
          var path = FixPath(serializeSetting.savePath);
          var loadPath = Path.Combine(path, serializeSetting.fileName);
          var textAsset = Resources.Load<TextAsset>(loadPath);
          if (textAsset == null)
          {
            Debug.LogError(
              $"{serializeSetting.fileName} cant load from path {loadPath}. Make sure that file under Resource folder.");
            continue;
          }

          container?.AddContainer(serializationRule?.Deserialization<ISheetDataContainer>(textAsset.text));
          Debug.Log($"Load {serializeSetting.fileName} from Resources\\{loadPath}".WithColor(ColorExt.CompletedColor));
        }
      }

      return container;
    }

    private string FixPath(string path)
    {
      var newPath = path.Split("Resources")[1];
      if (newPath[0] == '/' || newPath[0] == '\\')
      {
        newPath = newPath.Remove(0, 1);
      }

      if (newPath[^1] == '/' || newPath[^1] == '\\')
      {
        newPath = newPath.Remove(newPath.Length - 1, 1);
      }

      return newPath;
    }

    private bool RuleTypeEmpty(string serializationRuleSetting)
    {
      if (serializationRuleSetting == string.Empty)
      {
        Debug.LogError($"SerializationRule for {_profileName} doesn't exist.");
        return true;
      }

      return false;
    }

    private void TypeError(Type type)
    {
      if (type == null)
      {
        Debug.LogError(
          $"SerializationRuleType of {_profileName} profile doesn't exist. Serialize with JsonSerializationRule");
      }
    }
  }
}