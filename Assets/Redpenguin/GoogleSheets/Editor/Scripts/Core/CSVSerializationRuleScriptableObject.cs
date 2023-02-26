using Redpenguin.GoogleSheets.Runtime.Core;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
{
  [CreateAssetMenu(menuName = "GoogleSheetProvider/CSVSerializationRule", fileName = "CSVSerializationRule",
    order = 4)]
  public class CSVSerializationRuleScriptableObject : SerializationRuleScriptableObject<CSVSerializationRule>
  {
  }
}