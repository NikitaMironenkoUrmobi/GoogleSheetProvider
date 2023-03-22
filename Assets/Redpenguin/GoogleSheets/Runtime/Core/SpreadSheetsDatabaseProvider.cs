namespace Redpenguin.GoogleSheets.Core
{
    public class SpreadSheetsDatabaseProvider
    {
        private readonly ISerializeSettingsProvider _serializeSettingsProvider;

        public SpreadSheetsDatabaseProvider()
        {
            _serializeSettingsProvider = new SerializeSettingsProvider();
        }

        public virtual SpreadSheetsDatabase Load(string profileName)
        {
            var serialization = new SpreadSheetsSerializer(profileName, _serializeSettingsProvider.SerializeSettings);
            return serialization.DeserializeByRule();
        }
    }
}