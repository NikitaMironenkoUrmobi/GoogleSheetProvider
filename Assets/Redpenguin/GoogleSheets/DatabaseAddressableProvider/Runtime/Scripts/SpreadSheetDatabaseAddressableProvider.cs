using Cysharp.Threading.Tasks;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets.DatabaseAddressableProvider
{
    public interface ISpreadSheetDatabaseAddressableProvider
    {
        UniTask<SpreadSheetsDatabase> LoadAsync(string profileName);
    }

    public class SpreadSheetDatabaseAddressableProvider : ISpreadSheetDatabaseAddressableProvider
    {
        private readonly SerializeSettingsProvider _serializeSettingsProvider;

        public SpreadSheetDatabaseAddressableProvider()
        {
            _serializeSettingsProvider = new SerializeSettingsProvider();
        }

        public async UniTask<SpreadSheetsDatabase> LoadAsync(string profileName)
        {
            var spreadSheetAddressableSerializer =
                new SpreadSheetAddressableSerializer(profileName, _serializeSettingsProvider.SerializeSettings);
            return await spreadSheetAddressableSerializer.DeserializeByRuleAsync();
        }
    }
}