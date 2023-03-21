using Cysharp.Threading.Tasks;
using Redpenguin.GoogleSheets.Core;

namespace Redpenguin.GoogleSheets.AddressablesLoad.Scripts
{
    public class AddressableSpreadSheetSerializer
    {
        public async UniTask<SpreadSheetsDatabase> Deserialize()
        {
            return new SpreadSheetsDatabase();
        }
    }
}