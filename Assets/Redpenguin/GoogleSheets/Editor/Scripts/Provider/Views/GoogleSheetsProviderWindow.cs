using System;
using System.Linq;
using System.Threading.Tasks;
using Redpenguin.GoogleSheets.Editor.Core;
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

    private GoogleSheetsProviderService _googleSheetsProviderService;
    private bool _isCreatingScripts;
    private GoogleSheetsProviderPresenter _googleSheetsProviderPresenter;

    [MenuItem("GoogleSheets/Provider", false, 1)]
    private static void CreateWindows()
    {
      GetWindow<GoogleSheetsProviderWindow>("Google Sheets Provider").Show();
    }

    private void OnEnable()
    {
      SetupGoogleSheetsProvider();
      _googleSheetsProviderPresenter ??= new GoogleSheetsProviderPresenter(containerView, _googleSheetsProviderService);
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void SetupGoogleSheetsProvider()
    {
      _googleSheetsProviderService ??= new GoogleSheetsProviderService();
      _googleSheetsProviderService.FindAllContainers();
    }

    private void OnDisable()
    {
      _googleSheetsProviderService?.Dispose();
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    private void CreateGUI()
    {
      SetupGoogleSheetsProvider();
      tree.CloneTree(rootVisualElement);
      if(!_googleSheetsProviderPresenter.IsTableIDAndCredentialSetup(rootVisualElement))
        return;
      if (_googleSheetsProviderService.SpreadSheetDataTypes.Count == 0)
      {
        var container = rootVisualElement.Q<VisualElement>("Containers");
        var csharpHelpBox = new HelpBox("Cant find class with SpreadSheet attribute. Look in the console",
          HelpBoxMessageType.Warning);
        container.Add(csharpHelpBox);
        Debug.Log(_googleSheetsProviderService.CantFindClassWithAttribute());
      }
      ButtonActionLink();
      _googleSheetsProviderPresenter.ModelViewLink(rootVisualElement);
    }

    private bool WarningsSetup()
    {
      if (_googleSheetsProviderService.SpreadSheetDataTypes.Count == 0)
      {
        if (!_googleSheetsProviderService.CanCreateContainers())
        {
          var csharpHelpBox = new HelpBox("Cant find class with SpreadSheet attribute. Look in the console",
            HelpBoxMessageType.Warning);
          rootVisualElement.Add(csharpHelpBox);
          Debug.Log(_googleSheetsProviderService.CantFindClassWithAttribute());
          return true;
        }

        var button =
          new Button(() => { _googleSheetsProviderService.CreateAdditionalScripts(v => _isCreatingScripts = v); })
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

      rootVisualElement.Q<Button>("ButtonClear").clickable.clicked += _googleSheetsProviderService.Clear;
      rootVisualElement.Q<Button>("ButtonLoad").clickable.clicked += _googleSheetsProviderService.LoadSheetsData;
      rootVisualElement.Q<Button>("OpenProfiles").clickable.clicked += () => EditorApplication.ExecuteMenuItem("GoogleSheets/Profiles");
      rootVisualElement.Q<Button>("ButtonSave").clickable.clicked += _googleSheetsProviderService.Serialization;
    }

    private void ButtonCreateSoSetup()
    {
       var createSO = rootVisualElement.Q<Button>("ButtonCreateSO");
       createSO.style.display = _googleSheetsProviderService.CanCreateContainers()
         ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
         : new StyleEnum<DisplayStyle>(DisplayStyle.None);
      createSO.clickable.clicked += () => { _googleSheetsProviderService.CreateAdditionalScripts(v => _isCreatingScripts = v); };
    }


    

    private async void OnAfterAssemblyReload()
    {
      if (_isCreatingScripts == false) return;
      _isCreatingScripts = false;
      _googleSheetsProviderService.CreateScriptableObjects();
      await Task.Delay(TimeSpan.FromSeconds(0.1f));
      _googleSheetsProviderService.FindAllContainers();
      RecreateGUI();
    }

    private void RecreateGUI()
    {
      rootVisualElement.Clear();
      CreateGUI();
    }
  }
}