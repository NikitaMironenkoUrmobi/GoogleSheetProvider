using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEditor;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Presenters
{
  public class ProfilesContainerPresenter
  {
    private readonly ProfilesContainer _profilesContainer;
    private readonly VisualTreeAsset _profileView;
    private readonly List<VisualElement> _profileViews = new();
    private VisualElement _selectProfile;

    public ProfilesContainerPresenter(VisualTreeAsset profileView)
    {
      _profileView = profileView;
      _profilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
    }

    public void ModelViewLink(VisualElement view)
    {
      var container = view.Q<VisualElement>("Container");
      SetupButtons(view, container);

      for (var i = 0; i < _profilesContainer.ProfileModels.Count; i++)
      {
        var profileModel = _profilesContainer.ProfileModels[i];
        AddProfileView(container, profileModel, i);
      }
    }

    private void SetupButtons(VisualElement view, VisualElement container)
    {
      view.Q<Button>("ButtonCreateNew").clickable.clicked += () =>
      {
        var profileModel = new ProfileModel{
          profileName = $"Profile_{Random.Range(1, 10000)}", 
          color = Random.ColorHSV(),
          Guid = Guid.NewGuid().ToString()
        };
        _profilesContainer.AddNewProfile(profileModel);
        _profilesContainer.SerializeSettingsContainer.GetSerializeRuleSetting(profileModel.profileName);
        AddProfileView(container, profileModel, _profilesContainer.ProfileModels.Count - 1);
        EditorUtility.SetDirty(_profilesContainer);
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      };
      
      view.Q<Button>("ButtonRemove").clickable.clicked += () =>
      {
        if (_selectProfile == null) return;
        var index = _profileViews.IndexOf(_selectProfile);
        var profile = _profilesContainer.ProfileModels[index];
        var profileName = profile.profileName;
        _profilesContainer.RemoveProfile(profile);
        container.Remove(_profileViews[index]);
        _profileViews.RemoveAt(index);
        _profilesContainer.SerializeSettingsContainer.RemoveSerializeRuleSetting(profileName);
        _selectProfile = null;
        EditorUtility.SetDirty(_profilesContainer);
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      };
    }

    private void AddProfileView(VisualElement view, ProfileModel profileModel, int index)
    {
      var profileView = _profileView.Instantiate();
      var presenter = new ProfilePresenter(profileModel, profileView, _profilesContainer);
      presenter.GroupBoxContainer.SetProfileClickEvent(OnProfileClick);
      profileView.userData = presenter;
      _profileViews.Add(profileView);
      view.Add(profileView);
    }

    private void OnProfileClick(VisualElement profileView)
    {
      foreach (var visualElement in _profileViews)
      {
        if(visualElement == profileView) continue;
        (visualElement.userData as ProfilePresenter).GroupBoxContainer.Select(false);
      }
      _selectProfile = profileView;
    }
  }
}