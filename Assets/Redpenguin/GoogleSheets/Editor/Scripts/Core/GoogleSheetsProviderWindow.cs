using System;
using System.Linq;
using System.Threading.Tasks;
using Redpenguin.GoogleSheets.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
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
    
    private void OnEnable()
    {
      _googleSheetsProviderService ??= new GoogleSheetsProviderService();
      _googleSheetsProviderService.FindAllContainers();
      AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    private void OnDisable()
    {
      _googleSheetsProviderService.Dispose();
      AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    private void CreateGUI()
    {
      tree.CloneTree(rootVisualElement);

      ButtonActionLink();
      DropdownGroupsSetup();
      
      var folder = rootVisualElement.Q<VisualElement>("Containers");
      for (var i = 0; i < _googleSheetsProviderService.SpreadSheetContainers.Count; i++)
      {
        CreateGroupButton(i, folder);
      }
    }

    private void DropdownGroupsSetup()
    {
      var dropdownField = rootVisualElement.Q<DropdownField>("DropdownGroups");
      dropdownField.choices = _googleSheetsProviderService.Settings.SerializationGroups.Select(x => x.tag).ToList();
      var index = _googleSheetsProviderService.Settings.SerializationGroups.FindIndex(x => x.tag == _googleSheetsProviderService
        .Settings.currentGroup.tag);
      dropdownField.index = index;
      dropdownField.Q(className:"unity-base-popup-field__text").style.backgroundColor = _googleSheetsProviderService.Settings.SerializationGroups[index].color;
      dropdownField.RegisterValueChangedCallback(x => OnChangeDropdownValue(dropdownField));
    }

    private void OnChangeDropdownValue(DropdownField dropdownField)
    {
      var serializationGroup = _googleSheetsProviderService.Settings.SerializationGroups[dropdownField.index];
      dropdownField.style.color = serializationGroup.color;
      dropdownField.Q(className:"unity-base-popup-field__text").style.backgroundColor = serializationGroup.color;
      _googleSheetsProviderService.Settings.currentGroup = serializationGroup;

      //RecreateGUI();
    }

    private void ButtonActionLink()
    {
      rootVisualElement.Q<Button>("ButtonCreateSO").clickable.clicked += () =>
      {
        _isCreatingScripts = _googleSheetsProviderService.CreateAdditionalScripts();
      };
      rootVisualElement.Q<Button>("ButtonClear").clickable.clicked += _googleSheetsProviderService.Clear;
      rootVisualElement.Q<Button>("ButtonLoad").clickable.clicked += _googleSheetsProviderService.LoadSheetsData;
      rootVisualElement.Q<Button>("ButtonSave").clickable.clicked += _googleSheetsProviderService.SaveToFile;
      rootVisualElement.Q<Button>("ButtonAllSave").clickable.clicked += _googleSheetsProviderService.SaveAllGroups;
      rootVisualElement.Q<Button>("ButtonSettings").clickable.clicked += GoogleSheetsProviderAssetMenu.SelectSettingsAsset;
    }

    private void CreateGroupButton(int i, VisualElement folder)
    {
      var view = containerView.Instantiate();
      var containerSheetModel = new SheetEditorPresenter(
        view, 
        _googleSheetsProviderService.SpreadSheetContainers[i],  
        _googleSheetsProviderService.Settings.SerializationGroups
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