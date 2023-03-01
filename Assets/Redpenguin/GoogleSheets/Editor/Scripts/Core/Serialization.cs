using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Utils;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Core
{
  public class Serialization
  {
    private readonly ProfileModel _profileModel;
    private readonly List<ISheetDataContainer> _sheetDataContainers;

    public Serialization(ProfileModel profileModel, List<ISheetDataContainer> sheetDataContainers)
    {
      _sheetDataContainers = sheetDataContainers;
      _profileModel = profileModel;
    }

    public void SerializationByRule()
    {
      if (RuleTypeEmpty(_profileModel)) return;
      var type = Type.GetType(_profileModel.serializationRuleType);
      TypeError(type, _profileModel);
      if (type == null)
      {
        var container = new SpreadSheetsDatabase();
        _sheetDataContainers.ForEach(x => container.AddContainer(x));
        var serializationRule = new JsonSerializationRule();
        serializationRule.Serialization(_profileModel.savePath, _profileModel.fileName, _sheetDataContainers);
      }
      else
      {
        var serializationRule = Activator.CreateInstance(type) as SerializationRule;
        var container = new SpreadSheetsDatabase();
        foreach (var sheetDataContainer in _sheetDataContainers)
        {
          var metaData = _profileModel.metaData.GetMeta(sheetDataContainer.SheetDataType.ToString());
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
          serializationRule?.Serialization(_profileModel.savePath, _profileModel.fileName, container);
          Debug.Log($"{_profileModel.fileName} save to {_profileModel.savePath}".WithColor(ColorExt.CompletedColor));
        }
      }

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    private static bool RuleTypeEmpty(ProfileModel currentProfile)
    {
      if (currentProfile.serializationRuleType == string.Empty)
      {
        Debug.LogError($"SerializationRule for {currentProfile.profileName} doesn't exist.");
        return true;
      }

      return false;
    }

    private static void TypeError(Type type, ProfileModel currentProfile)
    {
      if (type == null)
      {
        Debug.LogError(
          $"SerializationRuleType of {currentProfile.profileName} profile doesn't exist. Serialize with JsonSerializationRule");
      }
    }
  }
}