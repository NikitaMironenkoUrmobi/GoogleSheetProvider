using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Editor.Core;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class GoogleSheetsProviderPresenter
  {
    private readonly VisualTreeAsset _tableContainerView;
    private readonly GoogleSheetsFacade _googleSheetsFacade;
    private readonly ProfilesContainer _profilesContainer;
    private readonly List<VisualElement> _containers = new();
    private VisualElement _view;
    private VisualElement _folder;
    private readonly SerializationSettingPresenter _serializationSettingPresenter;

    public GoogleSheetsProviderPresenter(
      VisualTreeAsset tableContainerView,
      GoogleSheetsFacade googleSheetsFacade,
      ProfilesContainer profilesContainer)
    {
      _googleSheetsFacade = googleSheetsFacade;
      _tableContainerView = tableContainerView;
      _profilesContainer = profilesContainer;
      _serializationSettingPresenter = new SerializationSettingPresenter(_profilesContainer);
    }

    public void ModelViewLink(VisualElement view)
    {
      _view = view;
      DropdownGroupsSetup(view);
      SetupContainers(view);
      _serializationSettingPresenter.ModelViewLink(view);
    }

    public void RecreateContainers()
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
      for (var i = 0; i < _googleSheetsFacade.SpreadSheetDataTypes.Count; i++)
      {
        CreateGroupButton(i, _folder);
      }
    }

    private void CreateGroupButton(int i, VisualElement folder)
    {
      var view = _tableContainerView.Instantiate();
      var dataType = _googleSheetsFacade.SpreadSheetDataTypes[i];
      var containerSheetModel = new SheetContainerPresenter(
        view,
        _googleSheetsFacade.SpreadSheetDataTypes[i],
        _profilesContainer,
        _googleSheetsFacade.SpreadSheetSoList.Find(x => x.SheetDataType == dataType),
        _googleSheetsFacade.SerializeContainer
      );
      view.userData = containerSheetModel;
      _containers.Add(view);
      folder.Add(view);
    }

    private void DropdownGroupsSetup(VisualElement view)
    {
      var dropdownField = view.Q<DropdownField>("ProfileDropdown");
      dropdownField.choices = _profilesContainer.ProfileModels.Select(x => x.profileName).ToList();

      dropdownField.index = _profilesContainer.ProfileModels.IndexOf(_profilesContainer.CurrentProfile);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;

      dropdownField.RegisterValueChangedCallback(x => { OnChangeDropdownValue(dropdownField); });
      EditorUtility.SetDirty(_profilesContainer);
    }

    private void OnChangeDropdownValue(DropdownField dropdownField)
    {
      _profilesContainer.SetAsCurrent(_profilesContainer.ProfileModels[dropdownField.index]);
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;

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