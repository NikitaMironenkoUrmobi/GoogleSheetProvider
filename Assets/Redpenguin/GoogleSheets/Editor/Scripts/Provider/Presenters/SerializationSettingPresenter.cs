using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class SerializationSettingPresenter
  {
    private readonly ProfilesContainer _profilesContainer;
    private Toggle _useSoToggle;
    private Toggle _loadFromRemoteToggle;
    private ProfileMetaData _currentMeta;

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
      _useSoToggle.UnregisterValueChangedCallback(OnUseSoToggleValueChange);
      _loadFromRemoteToggle.UnregisterValueChangedCallback(OnLoadFromRemoteToggleValueChange);
      
      _currentMeta = _profilesContainer.CurrentProfile.metaData;
      
      _useSoToggle.SetValueWithoutNotify(_currentMeta.useSoContainers);
      _loadFromRemoteToggle.SetValueWithoutNotify(_currentMeta.loadFromRemote);
      
      _useSoToggle.RegisterValueChangedCallback(OnUseSoToggleValueChange);
      _loadFromRemoteToggle.RegisterValueChangedCallback(OnLoadFromRemoteToggleValueChange);

      SetupLoadFromRemoteToggle(_currentMeta.useSoContainers);
    }

    private void OnLoadFromRemoteToggleValueChange(ChangeEvent<bool> x)
    {
      _currentMeta.loadFromRemote = x.newValue;
      EditorUtility.SetDirty(_profilesContainer);
    }

    private void OnUseSoToggleValueChange(ChangeEvent<bool> x)
    {
      _currentMeta.useSoContainers = x.newValue;
      SetupLoadFromRemoteToggle(_currentMeta.useSoContainers);
      EditorUtility.SetDirty(_profilesContainer);
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