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
    private VisualTreeAsset _tableContainerView;
    private GoogleSheetsProviderService _googleSheetsProviderService;
    private readonly ProfilesContainer _profilesContainer;

    public GoogleSheetsProviderPresenter(VisualTreeAsset tableContainerView, GoogleSheetsProviderService googleSheetsProviderService)
    {
      _googleSheetsProviderService = googleSheetsProviderService;
      _tableContainerView = tableContainerView;
      _profilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
    }

    public void ModelViewLink(VisualElement view)
    {
      DropdownGroupsSetup(view);
      var folder = view.Q<VisualElement>("Containers");
      for (var i = 0; i < _googleSheetsProviderService.SpreadSheetContainers.Count; i++)
      {
        CreateGroupButton(i, folder);
      }
    }
    
    private void CreateGroupButton(int i, VisualElement folder)
    {
      var view = _tableContainerView.Instantiate();
      var containerSheetModel = new SheetEditorPresenter(
        view,
        _googleSheetsProviderService.SpreadSheetContainers[i],
        _googleSheetsProviderService.Settings
      );
      view.userData = containerSheetModel;
      folder.Add(view);
    }
    
    private void DropdownGroupsSetup(VisualElement view)
    {
      var dropdownField = view.Q<DropdownField>("ProfileDropdown");
      dropdownField.choices = _profilesContainer.profileModels.Select(x => x.profileName).ToList();
      
      dropdownField.index = _profilesContainer.profileModels.IndexOf(_profilesContainer.CurrentProfile);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;
      
      dropdownField.RegisterValueChangedCallback(x => OnChangeDropdownValue(dropdownField));
      EditorUtility.SetDirty(_profilesContainer);
    }

    private void OnChangeDropdownValue(DropdownField dropdownField)
    {
      _profilesContainer.SetCurrentProfile(_profilesContainer.profileModels[dropdownField.index]);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;
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