using System;
using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Editor.Core;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using UnityEditor;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class GoogleSheetsProviderPresenter : IDisposable

  {
    private readonly VisualTreeAsset _tableContainerView;
    private readonly IGoogleSheetsFacade _googleSheetsFacade;
    private readonly ProfilesContainer _profilesContainer;
    private readonly List<VisualElement> _containers = new();
    private VisualElement _view;
    private VisualElement _folder;
    private readonly SerializationSettingPresenter _serializationSettingPresenter;
    private DropdownField _dropdownField;
    public event Action<ProfileModel> OnChangeCurrentProfile;

    public GoogleSheetsProviderPresenter(
      VisualTreeAsset tableContainerView,
      IGoogleSheetsFacade googleSheetsFacade,
      ProfilesContainer profilesContainer)
    {
      _googleSheetsFacade = googleSheetsFacade;
      _tableContainerView = tableContainerView;
      _profilesContainer = profilesContainer;
      _serializationSettingPresenter = new SerializationSettingPresenter(_profilesContainer);

      _profilesContainer.OnRemoveProfile += ReSetupDropdownGroups;
      _profilesContainer.OnNewProfileAdd += ReSetupDropdownGroups;
    }

    public void ModelViewLink(VisualElement view)
    {
      _view = view;
      DropdownGroupsSetup(view);
      SetupContainers(view);
      _serializationSettingPresenter.ModelViewLink(view);
    }

    private void RecreateContainers()
    {
      foreach (var visualElement in _containers)
      {
        visualElement.RemoveFromHierarchy();
      }

      SetupContainers(_view);
    }

    private void SetupContainers(VisualElement view)
    {
      _containers.Clear();
      _folder = view.Q<ScrollView>("Containers");
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
      _dropdownField = view.Q<DropdownField>("ProfileDropdown");
      _dropdownField.choices = _profilesContainer.ProfileModels.Select(x => x.profileName).ToList();

      _dropdownField.index = _profilesContainer.ProfileModels.IndexOf(_profilesContainer.CurrentProfile);
      _dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _profilesContainer.CurrentProfile.color;

      _dropdownField.RegisterValueChangedCallback(OnChangeDropdownValue);
    }

    private void ReSetupDropdownGroups(ProfileModel profileModel)
    {
      _dropdownField.UnregisterValueChangedCallback(OnChangeDropdownValue);
      DropdownGroupsSetup(_view);
    }

    private void OnChangeDropdownValue(ChangeEvent<string> value)
    {
      _profilesContainer.SetAsCurrent(_profilesContainer.ProfileModels[_dropdownField.index]);
      var currentProfile = _profilesContainer.CurrentProfile;
      _dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        currentProfile.color;

      _serializationSettingPresenter.ModelViewLink(_view);
      OnChangeCurrentProfile?.Invoke(currentProfile);
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

    public void Dispose()
    {
      _profilesContainer.OnRemoveProfile -= ReSetupDropdownGroups;
      _profilesContainer.OnNewProfileAdd -= ReSetupDropdownGroups;
    }
  }
}