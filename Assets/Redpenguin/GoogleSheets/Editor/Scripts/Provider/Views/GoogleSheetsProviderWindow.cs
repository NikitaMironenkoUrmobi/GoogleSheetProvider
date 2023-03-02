using System;
using System.Linq;
using System.Threading.Tasks;
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

    private GoogleSheetsFacade _googleSheetsFacade;
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
      Debug.Log("OnEnable");
      _googleSheetsFacade ??= new GoogleSheetsFacade(
        _profilesContainer,
        AssetDatabaseHelper.FindAssetsByType<SpreadSheetSoWrapper>().Cast<ISpreadSheetSoWrapper>().ToList());
      _googleSheetsFacade.SearchForSpreadSheets();
      _googleSheetsProviderPresenter ??= new GoogleSheetsProviderPresenter(containerView, _googleSheetsFacade, _profilesContainer);
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
      Debug.Log("OnDisable");
      _googleSheetsFacade?.Dispose();
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    private void CreateGUI()
    {
      Debug.Log("CreateGUI");
      tree.CloneTree(rootVisualElement);
      if (!_googleSheetsProviderPresenter.IsTableIDAndCredentialSetup(rootVisualElement))
        return;
      if (_googleSheetsFacade.SpreadSheetDataTypes.Count == 0)
      {
        var container = rootVisualElement.Q<VisualElement>("Containers");
        var csharpHelpBox = new HelpBox("Cant find class with SpreadSheet attribute. Look in the console",
          HelpBoxMessageType.Warning);
        container.Add(csharpHelpBox);
        _googleSheetsFacade.CantFindClassWithAttribute();
      }

      ButtonActionLink();
      _googleSheetsProviderPresenter.ModelViewLink(rootVisualElement);
    }

    private bool WarningsSetup()
    {
      if (_googleSheetsFacade.SpreadSheetDataTypes.Count == 0)
      {
        if (!_googleSheetsFacade.CanCreateContainers())
        {
          var csharpHelpBox = new HelpBox("Cant find class with SpreadSheet attribute. Look in the console",
            HelpBoxMessageType.Warning);
          rootVisualElement.Add(csharpHelpBox);
          _googleSheetsFacade.CantFindClassWithAttribute();
          return true;
        }

        var button =
          new Button(() => { _googleSheetsFacade.CreateAdditionalScripts(v => _isCreatingScripts = v); })
          {
            text = "Create containers"
          };
        button.style.height = 70;
        button.style.marginTop = 20;
        rootVisualElement.Add(button);
        return true;
      }

      return false;
    }


    private void ButtonActionLink()
    {
      ButtonCreateSoSetup();

      rootVisualElement.Q<Button>("ButtonClear").clickable.clicked += _googleSheetsFacade.ClearAllData;
      rootVisualElement.Q<Button>("ButtonSaveAll").clickable.clicked += _googleSheetsFacade.SerializeSheetDataContainers;
      rootVisualElement.Q<Button>("OpenProfiles").clickable.clicked +=
        () => EditorApplication.ExecuteMenuItem("GoogleSheets/Profiles");
      //rootVisualElement.Q<Button>("ButtonSave").clickable.clicked += _googleSheetsFacade.SerializeFromScriptableObjectContainers;
    }

    private void ButtonCreateSoSetup()
    {
      var createSO = rootVisualElement.Q<Button>("ButtonCreateSO");
      createSO.style.display = _googleSheetsFacade.CanCreateContainers()
        ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
        : new StyleEnum<DisplayStyle>(DisplayStyle.None);
      createSO.clickable.clicked += () => { _googleSheetsFacade.CreateAdditionalScripts(v => _isCreatingScripts = v); };
    }


    private async void OnAfterAssemblyReload()
    {
      if (_isCreatingScripts == false) return;
      _isCreatingScripts = false;
      _googleSheetsFacade.CreateScriptableObjects();
      await Task.Delay(TimeSpan.FromSeconds(0.1f));
      //_googleSheetsFacade.SearchForSpreadSheets();
      _googleSheetsProviderPresenter.RecreateContainers();
    }
  }
}