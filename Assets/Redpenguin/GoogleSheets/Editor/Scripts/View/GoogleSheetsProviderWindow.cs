using System;
using System.Linq;
using System.Threading.Tasks;
using Redpenguin.GoogleSheets.Editor.Core;
using Redpenguin.GoogleSheets.Editor.Presenter;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.View
{
  public class GoogleSheetsProviderWindow : EditorWindow
  {
    [SerializeField] private VisualTreeAsset tree;
    [SerializeField] private VisualTreeAsset containerView;

    private GoogleSheetsProviderService _googleSheetsProviderService;
    private bool _isCreatingScripts;

    [MenuItem("GoogleSheets/Provider", false, 1)]
    private static void CreateWindows()
    {
      GetWindow<GoogleSheetsProviderWindow>("Google Sheets Provider").Show();
    }

    private void Awake()
    {
      Debug.Log("Awake");
    }

    private void OnDestroy()
    {
      Debug.Log("OnDestroy");
    }

    private void OnEnable()
    {
      _googleSheetsProviderService ??= new GoogleSheetsProviderService();
      _googleSheetsProviderService.FindAllContainers();
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
      _googleSheetsProviderService?.Dispose();
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    private void CreateGUI()
    {
      _googleSheetsProviderService ??= new GoogleSheetsProviderService();
      _googleSheetsProviderService.FindAllContainers();
      if (!_googleSheetsProviderService.IsSettingsSetup())
      {
        ShowIfSettingsDoesntSetup();
        return;
      }

      if (WarningsSetup()) return;

      tree.CloneTree(rootVisualElement);

      ButtonActionLink();
      DropdownGroupsSetup();

      var folder = rootVisualElement.Q<VisualElement>("Containers");
      for (var i = 0; i < _googleSheetsProviderService.SpreadSheetContainers.Count; i++)
      {
        CreateGroupButton(i, folder);
      }
    }

    private bool WarningsSetup()
    {
      if (_googleSheetsProviderService.SpreadSheetContainers.Count == 0)
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

    private void ShowIfSettingsDoesntSetup()
    {
      if (!_googleSheetsProviderService.IsGoogleSheetIdSetup())
      {
        var csharpHelpBox = new HelpBox("Google Sheet ID doesn't setup", HelpBoxMessageType.Warning);
        rootVisualElement.Add(csharpHelpBox);
      }

      if (!_googleSheetsProviderService.IsCredentialSetup())
      {
        var csharpHelpBox = new HelpBox("Credential doesn't setup", HelpBoxMessageType.Warning);
        rootVisualElement.Add(csharpHelpBox);
      }

      var button = new Button(GoogleSheetsProviderAssetMenu.SelectSettingsAsset)
      {
        text = "Setup Settings"
      };
      rootVisualElement.Add(button);
    }

    private void DropdownGroupsSetup()
    {
      var dropdownField = rootVisualElement.Q<DropdownField>("DropdownGroups");
      dropdownField.choices = _googleSheetsProviderService.Settings.SerializationGroups.Select(x => x.tag).ToList();
      var index = _googleSheetsProviderService.Settings.SerializationGroups.FindIndex(x => x.tag ==
        _googleSheetsProviderService
          .Settings.currentGroup.tag);
      dropdownField.index = index;
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor =
        _googleSheetsProviderService.Settings.SerializationGroups[index].color;
      dropdownField.RegisterValueChangedCallback(x => OnChangeDropdownValue(dropdownField));
    }

    private void OnChangeDropdownValue(DropdownField dropdownField)
    {
      var serializationGroup = _googleSheetsProviderService.Settings.SerializationGroups[dropdownField.index];
      dropdownField.style.color = serializationGroup.color;
      dropdownField.Q(className: "unity-base-popup-field__text").style.backgroundColor = serializationGroup.color;
      _googleSheetsProviderService.Settings.currentGroup = serializationGroup;

      //RecreateGUI();
    }

    private void ButtonActionLink()
    {
      ButtonCreateSoSetup();

      rootVisualElement.Q<Button>("ButtonClear").clickable.clicked += _googleSheetsProviderService.Clear;
      rootVisualElement.Q<Button>("ButtonLoad").clickable.clicked += _googleSheetsProviderService.LoadSheetsData;
      rootVisualElement.Q<Button>("ButtonSave").clickable.clicked += _googleSheetsProviderService.SaveToFile;
      rootVisualElement.Q<Button>("ButtonAllSave").clickable.clicked += _googleSheetsProviderService.SaveAllGroups;
      rootVisualElement.Q<Button>("ButtonSettings").clickable.clicked +=
        GoogleSheetsProviderAssetMenu.SelectSettingsAsset;
    }

    private void ButtonCreateSoSetup()
    {
       var createSO = rootVisualElement.Q<Button>("ButtonCreateSO");
       createSO.style.display = _googleSheetsProviderService.CanCreateContainers()
         ? new StyleEnum<DisplayStyle>(DisplayStyle.Flex)
         : new StyleEnum<DisplayStyle>(DisplayStyle.None);
      createSO.clickable.clicked += () => { _googleSheetsProviderService.CreateAdditionalScripts(v => _isCreatingScripts = v); };
    }


    private void CreateGroupButton(int i, VisualElement folder)
    {
      var view = containerView.Instantiate();
      var containerSheetModel = new SheetEditorPresenter(
        view,
        _googleSheetsProviderService.SpreadSheetContainers[i],
        _googleSheetsProviderService.Settings
      );
      view.userData = containerSheetModel;
      folder.Add(view);
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