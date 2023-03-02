using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using UnityEditor;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class SerializationSettingPresenter
  {
    private readonly ProfilesContainer _profilesContainer;
    private Toggle _useSoToggle;
    private Toggle _loadFromRemoteToggle;

    public SerializationSettingPresenter(ProfilesContainer profilesContainer)
    {
      _profilesContainer = profilesContainer;
    }

    private void SetupView(VisualElement view)
    {
      _useSoToggle = view.Q<Toggle>("UseSOToggle");
      _loadFromRemoteToggle = view.Q<Toggle>("LoadToggle");
    }

    public void ModelViewLink(VisualElement view)
    {
      SetupView(view);
      var currentMeta = _profilesContainer.CurrentProfile.metaData;

      _useSoToggle.SetValueWithoutNotify(currentMeta.useSoContainers);
      SetupLoadFromRemoteToggle(currentMeta.useSoContainers);
      _useSoToggle.RegisterValueChangedCallback(x =>
      {
        currentMeta.useSoContainers = x.newValue;
        SetupLoadFromRemoteToggle(currentMeta.useSoContainers);
        EditorUtility.SetDirty(_profilesContainer);
      });
      
      _loadFromRemoteToggle.SetValueWithoutNotify(currentMeta.loadFromRemote);
      _loadFromRemoteToggle.RegisterValueChangedCallback(x =>
      {
        currentMeta.loadFromRemote = x.newValue;
        EditorUtility.SetDirty(_profilesContainer);
      });
    }

    private void SetupLoadFromRemoteToggle(bool value)
    {
      _loadFromRemoteToggle.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
      if (!value)
      {
        _loadFromRemoteToggle.value = true;
      }
    }
  }
}