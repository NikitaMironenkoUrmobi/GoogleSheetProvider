#if ZENJECT
using Zenject;
using Redpenguin.GoogleSheets.Provider;

namespace Redpenguin.GoogleSheets.Installer
{
  public class SpreadSheetProvidersInstaller : MonoInstaller
  {
    [SerializeField] private string profileName;

    public override void InstallBindings()
    {
      var databaseProvider = new SpreadSheetsDatabaseProvider();
      var database = databaseProvider.Load(profileName);
      foreach (var sheetDataContainer in database.Containers)
      {
        Container.BindInterfacesTo(container.GetType()).FromInstance(container).AsSingle();
      }
    }
  }
}
#endif