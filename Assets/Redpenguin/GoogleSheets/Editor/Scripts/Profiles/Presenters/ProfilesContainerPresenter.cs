using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Presenters
{
  public class ProfilesContainerPresenter
  {
    private readonly ProfilesContainer _model;
    private readonly VisualTreeAsset _profileView;
    private readonly List<VisualElement> _profileViews = new();
    private VisualElement _selectProfile;

    public ProfilesContainerPresenter(VisualTreeAsset profileView)
    {
      _profileView = profileView;
      _model = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
    }

    public void ModelViewLink(VisualElement view)
    {
      var container = view.Q<VisualElement>("Container");
      SetupButtons(view, container);

      for (var i = 0; i < _model.profileModels.Count; i++)
      {
        var profileModel = _model.profileModels[i];
        AddProfileView(container, profileModel, i);
      }
    }

    private void SetupButtons(VisualElement view, VisualElement container)
    {
      view.Q<Button>("ButtonCreateNew").clickable.clicked += () =>
      {
        var profileModel = new ProfileModel();
        _model.profileModels.Add(profileModel);
        AddProfileView(container, profileModel, _model.profileModels.Count - 1);
      };
      view.Q<Button>("ButtonRemove").clickable.clicked += () =>
      {
        if (_selectProfile == null) return;
        var index = _profileViews.IndexOf(_selectProfile);
        _model.profileModels.RemoveAt(index);
        container.Remove(_profileViews[index]);
        _profileViews.RemoveAt(index);
        _selectProfile = null;
      };
    }

    private void AddProfileView(VisualElement view, ProfileModel profileModel, int index)
    {
      var profileView = _profileView.Instantiate();
      var presenter = new ProfilePresenter(profileModel, profileView, _model);
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