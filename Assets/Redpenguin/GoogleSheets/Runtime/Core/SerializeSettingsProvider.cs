using Redpenguin.GoogleSheets.Settings;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Core
{
    public interface ISerializeSettingsProvider
    {
        SerializeSettingsContainer SerializeSettings { get; }
    }

    public class SerializeSettingsProvider : ISerializeSettingsProvider
    {
        public SerializeSettingsContainer SerializeSettings { get; }

        public SerializeSettingsProvider()
        {
            SerializeSettings = Resources.Load<SerializeSettingsContainer>(SpreadSheets.SerializeSettingsPath);
        }
    }
}