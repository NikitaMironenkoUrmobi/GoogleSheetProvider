#if ENABLE_ZENJECT
#if ENABLE_NAUGHTYATTRIBUTES
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Settings;
#endif
using Redpenguin.GoogleSheets.Core;
using UnityEngine;
using Zenject;

namespace Redpenguin.GoogleSheets.Installer
{
  public class SpreadSheetProvidersInstaller : MonoInstaller
  {
#if ENABLE_NAUGHTYATTRIBUTES
    [HideInInspector] public List<string> profiles;
    [Dropdown(nameof(profiles))]
#endif
    [SerializeField] private string profileName;

    public override void InstallBindings()
    {
      var databaseProvider = new SpreadSheetsDatabaseProvider();
      var database = databaseProvider.Load(profileName);
      foreach (var sheetDataContainer in database.Containers)
      {
        Container.BindInterfacesTo(sheetDataContainer.GetType()).FromInstance(sheetDataContainer).AsSingle();
      }
    }

#if ENABLE_NAUGHTY_ATTRIBUTES
    [Button()]
    private void LoadProfiles()
    {
      profiles = Resources.Load<SerializeSettingsContainer>("SerializeSettingsContainer")
        .SerializationRuleSetting
        .Select(x => x.profile)
        .ToList();
    }
#endif
  }
}
#endif