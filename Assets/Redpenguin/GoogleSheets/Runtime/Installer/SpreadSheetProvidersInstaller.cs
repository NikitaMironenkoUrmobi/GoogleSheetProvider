#if ZENJECT
using Zenject;
using Redpenguin.GoogleSheets.Provider;

namespace Redpenguin.GoogleSheets.Installer
{
  public class SpreadSheetProvidersInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      var databaseProvider = new SpreadSheetsDatabaseProvider();
      databaseProvider.Load();
      foreach (var sheetDataContainer in databaseProvider.Database.Containers)
      {
        Container.BindInterfacesTo(container.GetType()).FromInstance(container).AsSingle();
      }
    }
  }
}
#endif