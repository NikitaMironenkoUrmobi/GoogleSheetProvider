using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Editor.Core;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class GoogleSheetsProviderPresenter
  {
    private readonly VisualTreeAsset _tableContainerView;
    private readonly GoogleSheetsProviderService _googleSheetsProviderService;
    private readonly ProfilesContainer _profilesContainer;
    private readonly List<VisualElement> _containers = new();
    private VisualElement _view;
    private VisualElement _folder;

    public GoogleSheetsProviderPresenter(VisualTreeAsset tableContainerView, GoogleSheetsProviderService googleSheetsProviderService)
    {
      _googleSheetsProviderService = googleSheetsProviderService;
      _tableContainerView = tableContainerView;
      _profilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
    }

    public void ModelViewLink(VisualElement view)
    {
      _view = view;
      DropdownGroupsSetup(view);
      SetupContainers(view);
    }

    private void RecreateContainers()
    {
      foreach (var visualElement in _containers)
      {
        _folder.Remove(visualElement);
      }
      SetupContainers(_view);
    }

    private void SetupContainers(VisualElement view)
    {
      _containers.Clear();
      _folder = view.Q<VisualElement>("Containers");
      for (var i = 0; i < _googleSheetsProviderService.SpreadSheetDataTypes.Count; i++)
      {
        CreateGroupButton(i, _folder);
      }
    }

    private void CreateGroupButton(int i, VisualElement folder)
    {
      var view = _tableContainerView.Instantiate();
      var dataType = _googleSheetsProviderService.SpreadSheetDataTypes[i];
      var containerSheetModel = new SheetContainerPresenter(
        view,
        _googleSheetsProviderService.SpreadSheetDataTypes[i],
        _profilesContainer,
        _googleSheetsProviderService.SpreadSheetSoList.Find(x => x.SheetDataType == dataType),
        _googleSheetsProviderService.SerializeContainer
      );
      view.userData = containerSheetModel;
      _containers.Add(view);
      folder.Add(view);
    }
    
    private void DropdownGroupsSetup(VisualElement view)
    {
      var dropdownField = view.Q<DropdownField>("ProfileDropdown");
      dropdownField.choices = _profilesContainer.profileModels.Select(x => x.profileName).ToList();
      
      dropdownField.index = _profilesContainer.profileModels.IndexOf(_profilesContainer.CurrentProfile);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;
      
      dropdownField.RegisterValueChangedCallback(x =>
      {
        OnChangeDropdownValue(dropdownField);
      });
      EditorUtility.SetDirty(_profilesContainer);
    }

    private void OnChangeDropdownValue(DropdownField dropdownField)
    {
      _profilesContainer.SetCurrentProfile(_profilesContainer.profileModels[dropdownField.index]);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;
      
      _googleSheetsProviderService.OnProfileChange();
      RecreateContainers();
      EditorUtility.SetDirty(_profilesContainer);
    }
    
    public bool IsTableIDAndCredentialSetup(VisualElement view)
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      if (currentProfile.tableID == string.Empty)
      {
        var csharpHelpBox = new HelpBox("Current profile table ID doesn't setup", HelpBoxMessageType.Warning);
        view.Add(csharpHelpBox);
      }

      if (currentProfile.credential == null)
      {
        var csharpHelpBox = new HelpBox("Current profile credential doesn't setup", HelpBoxMessageType.Warning);
        view.Add(csharpHelpBox);
      }

      if (currentProfile.tableID == string.Empty || currentProfile.credential == null)
      {
        var button = new Button(() => EditorApplication.ExecuteMenuItem("GoogleSheets/Profiles"))
        {
          text = "Setup Profile"
        };
        view.Add(button);
        return false;
      }
      
      return true;
    }
  }
}