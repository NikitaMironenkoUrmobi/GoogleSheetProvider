using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Redpenguin.GoogleSheets.DatabaseAddressableProvider
{
    public class AssetsProvider
    {
        public async UniTask<(bool, AsyncOperationHandle<T>)> LoadAsset<T>(string labelOrAddress) where T : class
        {
            var asset = Addressables.LoadAssetAsync<T>(labelOrAddress);
            try
            {
                await asset;
                return (true, asset);
            }
            catch
            {
                return (false, default);
            }
        }
    }
}