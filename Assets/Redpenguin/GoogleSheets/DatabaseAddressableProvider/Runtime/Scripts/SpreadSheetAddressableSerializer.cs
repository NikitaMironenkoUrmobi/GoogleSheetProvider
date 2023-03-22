using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Redpenguin.GoogleSheets.Core;
using Redpenguin.GoogleSheets.Settings;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.DatabaseAddressableProvider
{
    public class SpreadSheetAddressableSerializer
    {
        private readonly string _profileName;
        private readonly SerializeSettingsContainer _serializeSettings;
        private readonly SpreadSheetsSerializerUtils _utils;
        private readonly AssetsProvider _assetProvider;

        public SpreadSheetAddressableSerializer(string profileName, SerializeSettingsContainer serializeSettings)
        {
            _serializeSettings = serializeSettings;
            _profileName = profileName;
            _utils = new SpreadSheetsSerializerUtils(_profileName);
            _assetProvider = new AssetsProvider();
        }

        public async UniTask<SpreadSheetsDatabase> DeserializeByRuleAsync()
        {
            var serializationRuleSetting = _serializeSettings.GetSerializeRuleSetting(_profileName);
            var serializeSettings = _serializeSettings.GetSerializeSetting(_profileName);
            if (_utils.RuleTypeEmpty(serializationRuleSetting.serializationRuleType)) return new SpreadSheetsDatabase();
            var type = Type.GetType(serializationRuleSetting.serializationRuleType);
            _utils.TypeError(type);
            var serializationRule =
                type == null ? new JsonSerializationRule() : Activator.CreateInstance(type) as SerializationRule;

            var container = await DeserializeTogether(serializeSettings, serializationRuleSetting, serializationRule);
            container = await DeserializeSeparately(serializeSettings, container, serializationRule);

            return container;
        }

        private async UniTask<SpreadSheetsDatabase> DeserializeTogether(List<SerializeSetting> serializeSettings,
            SerializationRuleSetting serializationRuleSetting, SerializationRule serializationRule)
        {
            var container = new SpreadSheetsDatabase();
            if (!serializeSettings.Exists(x => !x.saveSeparately)) return container;
            var path = _utils.FixAddressablePath(serializationRuleSetting.savePath);
            var loadPath = Path.Combine(path, $"{serializationRuleSetting.fileName}.json").Replace("\\", "/");
            var (isLoaded, operationHandle) = await _assetProvider.LoadAsset<TextAsset>(loadPath);
            if (!isLoaded)
            {
                CantFindFileLog(serializationRuleSetting.fileName, loadPath);
            }
            else
            {
                container = serializationRule?.Deserialization<SpreadSheetsDatabase>(operationHandle.Result.text);
                LoadLog(serializationRuleSetting.fileName, loadPath);
            }

            return container;
        }

        private async UniTask<SpreadSheetsDatabase> DeserializeSeparately(List<SerializeSetting> serializeSettings,
            SpreadSheetsDatabase container, SerializationRule serializationRule)
        {
            foreach (var serializeSetting in serializeSettings.Where(x => x.saveSeparately))
            {
                var path = _utils.FixAddressablePath(serializeSetting.savePath);
                var loadPath = Path.Combine(path, $"{serializeSetting.fileName}.json").Replace("\\", "/");;
                var (isLoaded, operationHandle) = await _assetProvider.LoadAsset<TextAsset>(loadPath);
                if (!isLoaded)
                {
                    CantFindFileLog(serializeSetting.fileName, loadPath);
                    continue;
                }

                container.AddContainer(
                    serializationRule?.Deserialization<ISheetDataContainer>(operationHandle.Result.text));
                LoadLog(serializeSetting.fileName, loadPath);
            }

            return container;
        }

        private static void LoadLog(string fileName, string loadPath)
        {
            Debug.Log($"Load {fileName} from {loadPath}".WithColor(ColorExt.CompletedColor));
        }

        private void CantFindFileLog(string fileName, string loadPath)
        {
            Debug.LogError(
                $"{fileName} cant load from path {loadPath}. Make sure that file in Addressable Group with name {loadPath} and NOT under Resource folder.");
        }
    }
}