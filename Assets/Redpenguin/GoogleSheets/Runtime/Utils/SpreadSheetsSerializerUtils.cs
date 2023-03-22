using System;
using UnityEngine;

namespace Redpenguin.GoogleSheets
{
    public class SpreadSheetsSerializerUtils
    {
        private string _profileName;

        public SpreadSheetsSerializerUtils(string profileName)
        {
            _profileName = profileName;
        }
        public string FixResourcesPath(string path)
        {
            var newPath = path.Split("Resources")[1];
            if (newPath == String.Empty) return newPath;
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
        public string FixAddressablePath(string path)
        {
            var newPath = path;
            if (newPath == String.Empty) return newPath;
            if (newPath[0] == '/' || newPath[0] == '\\')
            {
                newPath = newPath.Remove(0, 1);
            }

            if (newPath[^1] == '/' || newPath[^1] == '\\')
            {
                newPath = newPath.Remove(newPath.Length - 1, 1);
            }
            return $"Assets/{newPath}";
        }

        public bool RuleTypeEmpty(string serializationRuleSetting)
        {
            if (serializationRuleSetting == string.Empty)
            {
                Debug.LogError($"SerializationRule for {_profileName} doesn't exist.");
                return true;
            }

            return false;
        }

        public void TypeError(Type type)
        {
            if (type == null)
            {
                Debug.LogError(
                    $"SerializationRuleType of {_profileName} profile doesn't exist. Serialize with JsonSerializationRule");
            }
        }
    }
}