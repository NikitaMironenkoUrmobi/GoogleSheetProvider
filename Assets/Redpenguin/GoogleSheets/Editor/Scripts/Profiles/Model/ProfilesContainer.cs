using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using Redpenguin.GoogleSheets.Settings;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Model
{
  public interface IProfilesContainer
  {
    List<ProfileModel> ProfileModels { get; set; }
    SerializeSettingsContainer SerializeSettingsContainer { get; }
    ProfileModel CurrentProfile { get; }
    public event Action<ProfileModel> OnChangeCurrentProfile;
    void SetAsCurrent(ProfileModel profileModel);
    void ClearMeta(string profileName);
  }

  [CreateAssetMenu(menuName = "GoogleSheetsProvider/Create ProfilesModel", fileName = "ProfilesModel", order = 0)]
  public class ProfilesContainer : ScriptableObject, IProfilesContainer
  {
    [SerializeField] private List<ProfileModel> profileModels;
    [SerializeField] private SerializeSettingsContainer serializeSettingsContainer;
    public SerializeSettingsContainer SerializeSettingsContainer => serializeSettingsContainer;
    public ProfileModel CurrentProfile => GetCurrentProfile();
    public event Action<ProfileModel> OnChangeCurrentProfile;

    public List<ProfileModel> ProfileModels
    {
      get => profileModels;
      set => profileModels = value;
    }

    private ProfileModel GetCurrentProfile()
    {
      if (profileModels.Count == 0)
      {
        throw new Exception($"Can't find profiles. You need to have at least one");
      }
      var current = profileModels.Find(x => x.selected);
      if (current == null)
      {
        current = profileModels[0];
        current.selected = true;
      }
      return current;
    }

    public void SetAsCurrent(ProfileModel profileModel)
    {
      foreach (var model in profileModels)
      {
        model.selected = profileModel == model;
      }
      OnChangeCurrentProfile?.Invoke(profileModel);
    }

    public void ClearMeta(string profileName)
    {
      serializeSettingsContainer.Clear(profileName);
      foreach (var model in profileModels)
      {
        if (model.profileName == profileName)
        {
          model.metaData = new ProfileMetaData();
          Debug.Log($"{model.profileName} profile meta was deleted!".WithColor(ColorExt.CompletedColor));
          return;
        }
      }
    }
  }
}