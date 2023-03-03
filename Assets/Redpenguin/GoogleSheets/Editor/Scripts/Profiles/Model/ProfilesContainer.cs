using System;
using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using Redpenguin.GoogleSheets.Settings;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Model
{
  public interface IProfilesContainer
  {
    public event Action<ProfileModel> OnNewProfileAdd;
    List<ProfileModel> ProfileModels { get; set; }
    SerializeSettingsContainer SerializeSettingsContainer { get; }
    ProfileModel CurrentProfile { get; }
    void SetAsCurrent(ProfileModel profileModel);
    void ClearMeta(string profileName);
    event Action<ProfileModel> OnRemoveProfile;
    void RemoveProfile(ProfileModel profileModel);
  }

  [CreateAssetMenu(menuName = "GoogleSheetsProvider/Create ProfilesModel", fileName = "ProfilesModel", order = 0)]
  public class ProfilesContainer : ScriptableObject, IProfilesContainer
  {
    [SerializeField] private List<ProfileModel> profileModels;
    [SerializeField] private SerializeSettingsContainer serializeSettingsContainer;
    public SerializeSettingsContainer SerializeSettingsContainer => serializeSettingsContainer;
    public ProfileModel CurrentProfile => GetCurrentProfile();

    public event Action<ProfileModel> OnNewProfileAdd;
    public event Action<ProfileModel> OnRemoveProfile;

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

    public void AddNewProfile(ProfileModel profileModel)
    {
      if(profileModels.Contains(profileModel)) return;
      profileModels.Add(profileModel);
      OnNewProfileAdd?.Invoke(profileModel);
    }
    public void RemoveProfile(ProfileModel profileModel)
    {
      if(!profileModels.Contains(profileModel)) return;
      profileModels.Remove(profileModel);
      OnNewProfileAdd?.Invoke(profileModel);
    }

    public void SetAsCurrent(ProfileModel profileModel)
    {
      foreach (var model in profileModels)
      {
        model.selected = profileModel == model;
      }
    }

    public void ClearMeta(string profileName)
    {
      foreach (var model in profileModels)
      {
        if (model.profileName == profileName)
        {
          model.metaData.tableSheetsNames.Clear();
          Debug.Log($"{model.profileName} profile meta was deleted!".WithColor(ColorExt.CompletedColor));
          return;
        }
      }
    }
  }
}