using System.Linq;
using Redpenguin.GoogleSheets.Editor.Core;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Provider.Presenters;
using Redpenguin.GoogleSheets.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Views
{
  public class GoogleSheetsProviderWindow : EditorWindow
  {
    [SerializeField] private VisualTreeAsset tree;
    [SerializeField] private VisualTreeAsset containerView;

    private IGoogleSheetsFacade _googleSheetsFacade;
    private bool _isCreatingScripts;
    private GoogleSheetsProviderPresenter _googleSheetsProviderPresenter;
    private ProfilesContainer _profilesContainer;

    [MenuItem("GoogleSheets/Provider", false, 1)]
    private static void CreateWindows()
    {
      GetWindow<GoogleSheetsProviderWindow>("Google Sheets Provider").Show();
    }

    private void Awake()
    {
      _profilesContainer = AssetDatabaseHelper.FindAssetsByType<ProfilesContainer>()[0];
    }

    private void OnEnable()
    {
      _googleSheetsFacade ??= new GoogleSheetsFacade(
        _profilesContainer,
        AssetDatabaseHelper.FindAssetsByType<SpreadSheetSoWrapper>().Cast<ISpreadSheetSoWrapper>().ToList());
      _googleSheetsFacade.SearchForSpreadSheets();
      _googleSheetsProviderPresenter ??=
        new GoogleSheetsProviderPresenter(containerView, _googleSheetsFacade, _profilesContainer);
      _googleSheetsProviderPresenter.OnChangeCurrentProfile += OnChangeCurrentProfile;
      CreateSoAfterAssemblyReload();
    }

    private void OnChangeCurrentProfile(ProfileModel profileModel)
    {
      _googleSheetsFacade.OnProfileChange(
        profileModel,
        AssetDatabaseHelper.FindAssetsByType<SpreadSheetSoWrapper>().Cast<ISpreadSheetSoWrapper>().ToList());
      var toggle = rootVisualElement.Q<Toggle>("UseSOToggle");
      toggle.UnregisterValueChangedCallback(ModalButtonSetup);
      toggle.RegisterValueChangedCallback(ModalButtonSetup);
      ModalButtonSetup(null);
    }

    private void OnDisable()
    {
      _googleSheetsFacade?.Dispose();
      _googleSheetsProviderPresenter.OnChangeCurrentProfile -= OnChangeCurrentProfile;
      _googleSheetsProviderPresenter?.Dispose();
      AssemblyReloadEvents.afterAssemblyReload -= CreateSoAfterAssemblyReload;
      
    }

    private void CreateGUI()
    {
      tree.CloneTree(rootVisualElement);
      _googleSheetsProviderPresenter.ModelViewLink(rootVisualElement);
      if (!_googleSheetsProviderPresenter.IsTableIDAndCredentialSetup(rootVisualElement))
        return;
      CantFindClasses();
      ButtonActionLink();
    }

    private void CantFindClasses()
    {
      if (_googleSheetsFacade.SpreadSheetDataTypes.Count == 0)
      {
        var container = rootVisualElement.Q<VisualElement>("Containers");
        var csharpHelpBox = new HelpBox("Cant find class with SpreadSheet attribute. Look in the console",
          HelpBoxMessageType.Warning);
        container.Add(csharpHelpBox);
        _googleSheetsFacade.CantFindClassWithAttribute();
      }
    }

    private void ButtonActionLink()
    {
      rootVisualElement.Q<Button>("OpenProfiles").clickable.clicked +=
        () => EditorApplication.ExecuteMenuItem("GoogleSheets/Profiles");
      rootVisualElement.Q<Button>("ButtonClear").clickable.clicked += _googleSheetsFacade.ClearAllData;
      rootVisualElement.Q<Toggle>("UseSOToggle").RegisterValueChangedCallback(ModalButtonSetup);
      ModalButtonSetup(null);
    }

    private void ModalButtonSetup(ChangeEvent<bool> value)
    {
      var button = rootVisualElement.Q<Button>("ButtonSaveAll");
      button.clickable = null;

      if (_googleSheetsFacade.CanCreateContainers())
      {
        button.text = "Create containers";
        button.clickable =
          new Clickable(() => _googleSheetsFacade.CreateAdditionalScripts(v => _isCreatingScripts = v));
      }
      else
      {
        button.text = "Save all";
        button.clickable = new Clickable(_googleSheetsFacade.SerializeSheetData);
      }
    }

    private void CreateSoAfterAssemblyReload()
    {
      if (_isCreatingScripts == false) return;
      _isCreatingScripts = false;
      _googleSheetsFacade.CreateScriptableObjects();
      _googleSheetsFacade.SearchForSpreadSheets();
      _googleSheetsFacade.SetupContainers(
        AssetDatabaseHelper
          .FindAssetsByType<SpreadSheetSoWrapper>()
          .Cast<ISpreadSheetSoWrapper>()
          .ToList()
      );
    }
  }
}