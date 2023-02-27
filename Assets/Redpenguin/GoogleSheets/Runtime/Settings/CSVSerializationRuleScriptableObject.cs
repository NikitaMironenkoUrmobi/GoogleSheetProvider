using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Settings
{
  [CreateAssetMenu(menuName = "GoogleSheetProvider/CSVSerializationRule", fileName = "CSVSerializationRule",
    order = 4)]
  public class CSVSerializationRuleScriptableObject : SerializationRuleScriptableObject<CSVSerializationRule>
  {
  }
}