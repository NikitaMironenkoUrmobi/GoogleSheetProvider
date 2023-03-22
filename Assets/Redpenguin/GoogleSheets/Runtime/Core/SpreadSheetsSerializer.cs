using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    private readonly SerializeSettingsContainer _serializeSettings;
    private readonly string _profileName;
    private readonly SpreadSheetsSerializerUtils _utils;
    public SpreadSheetsSerializer(string profileName, SerializeSettingsContainer serializeSettings)
    {
      _profileName = profileName;
      _serializeSettings = serializeSettings;
      _utils = new SpreadSheetsSerializerUtils(_profileName);
    }

    public void SerializationByRule(List<ISheetDataContainer> sheetDataContainers)
    {
      var serializationRuleSetting = _serializeSettings.GetSerializeRuleSetting(_profileName);
      if (_utils.RuleTypeEmpty(serializationRuleSetting.serializationRuleType)) return;
      var type = Type.GetType(serializationRuleSetting.serializationRuleType);
      _utils.TypeError(type);

      var serializationRule =
        type == null ? new JsonSerializationRule() : Activator.CreateInstance(type) as SerializationRule;

      var container = new SpreadSheetsDatabase();
      foreach (var sheetDataContainer in sheetDataContainers)
      {
        var metaData =
          _serializeSettings.GetSerializeSetting(_profileName, sheetDataContainer.SheetDataType.ToString());
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
      var serializationRuleSetting = _serializeSettings.GetSerializeRuleSetting(_profileName);
      var serializeSettings = _serializeSettings.GetSerializeSetting(_profileName);
      if (_utils.RuleTypeEmpty(serializationRuleSetting.serializationRuleType)) return null;
      var type = Type.GetType(serializationRuleSetting.serializationRuleType);
      _utils.TypeError(type);
      var serializationRule =
        type == null ? new JsonSerializationRule() : Activator.CreateInstance(type) as SerializationRule;

      var container = DeserializeTogether(serializeSettings, serializationRuleSetting, serializationRule);
      container = DeserializeSeparately(serializeSettings, container, serializationRule);

      return container;
    }

    private SpreadSheetsDatabase DeserializeTogether(
      List<SerializeSetting> serializeSettings,
      SerializationRuleSetting serializationRuleSetting,
      SerializationRule serializationRule)
    {
      var container = new SpreadSheetsDatabase();
      if (!serializeSettings.Exists(x => !x.saveSeparately)) return container;
      var path = _utils.FixResourcesPath(serializationRuleSetting.savePath);
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
          $"Load {serializationRuleSetting.fileName} from Resources\\{loadPath}".WithColor(
            ColorExt.CompletedColor));
      }
      return container;
    }

    private SpreadSheetsDatabase DeserializeSeparately(
      List<SerializeSetting> serializeSettings, 
      SpreadSheetsDatabase container,
      SerializationRule serializationRule)
    {
      
      foreach (var serializeSetting in serializeSettings.Where(x => x.saveSeparately))
      {
        var path = _utils.FixResourcesPath(serializeSetting.savePath);
        var loadPath = Path.Combine(path, serializeSetting.fileName);
        var textAsset = Resources.Load<TextAsset>(loadPath);
        if (textAsset == null)
        {
          Debug.LogError(
            $"{serializeSetting.fileName} cant load from path {loadPath}. Make sure that file under Resource folder.");
          continue;
        }

        container.AddContainer(serializationRule?.Deserialization<ISheetDataContainer>(textAsset.text));
        Debug.Log($"Load {serializeSetting.fileName} from Resources\\{loadPath}".WithColor(ColorExt.CompletedColor));
      }

      return container;
    }
  }
}