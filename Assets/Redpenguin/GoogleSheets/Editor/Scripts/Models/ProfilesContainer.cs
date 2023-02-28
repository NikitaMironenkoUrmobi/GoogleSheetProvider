using System.Collections.Generic;
using UnityEngine;

namespace Redpenguin.GoogleSheets.Editor.Models
{
  
  [CreateAssetMenu(menuName = "Create ProfilesModel", fileName = "ProfilesModel", order = 0)]
  public class ProfilesContainer : ScriptableObject
  {
    public List<ProfileModel> profileModels;

    public ProfileModel CurrentProfile => GetCurrentProfile();

    private ProfileModel GetCurrentProfile()
    {
      ProfileModel profile = null;
      foreach (var profileModel in profileModels)
      {
        if (profile != null && profileModel.selected)
        {
          profileModel.selected = false;
        }

        if (profile == null && profileModel.selected)
        {
          profile = profileModel;
        }
      }

      if (profile == null && profileModels.Count != 0)
      {
        profile = profileModels[0];
      }
      return profile;
    }
    
    public void SetCurrentProfile(ProfileModel profileModel)
    {
      foreach (var model in profileModels)
      {
        model.selected = profileModel == model;
      }
    }
  }
  
  
  
  
}