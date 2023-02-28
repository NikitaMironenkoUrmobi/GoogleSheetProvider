using Redpenguin.GoogleSheets.Editor.Profiles.Presenters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Views
{
  public class ProfilesWindow : EditorWindow
  {
    [SerializeField] private VisualTreeAsset profilesContainerView;
    [SerializeField] private VisualTreeAsset profileView;
    private ProfilesContainerPresenter _profilesContainerPresenter;

    [MenuItem("GoogleSheets/Profiles", false, 2)]
    private static void CreateWindows()
    {
      GetWindow<ProfilesWindow>("Google Sheets Profiles").Show();
    }

    private void OnEnable()
    {
      _profilesContainerPresenter ??= new ProfilesContainerPresenter(profileView);
    }

    private void CreateGUI()
    {
      profilesContainerView.CloneTree(rootVisualElement);
      _profilesContainerPresenter.ModelViewLink(rootVisualElement);
    }
  }
}