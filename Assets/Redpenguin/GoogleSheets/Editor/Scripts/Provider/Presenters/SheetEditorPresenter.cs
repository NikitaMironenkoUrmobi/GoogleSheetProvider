using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor.Provider.Views;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class SheetEditorPresenter
  {
    private ObjectField _containerObject;
    private VisualElement _rulesContainer;
    private ISpreadSheetSO _spreadSheetSo;
    private readonly List<RuleButton> _buttons = new();
    private VisualElement _pathContainer;
    private TextField _savePath;
    private TextField _fileName;
    private readonly GoogleSheetProviderSettings _settings;
    private Toggle _loadToggle;
    private Label _containerLabel;

    public SheetEditorPresenter(
      VisualElement view,
      ScriptableObject model,
      GoogleSheetProviderSettings settings)
    {
      _settings = settings;

      SetView(view);
      ModelViewLink(model);
      //LoadMeta();
    }

    private void SetView(VisualElement visualElement)
    {
      _containerObject = visualElement.Q<ObjectField>("ContainerObject");
      _rulesContainer = visualElement.Q<VisualElement>("RulesContainer");
      _pathContainer = visualElement.Q<VisualElement>("PathContainer");
      _loadToggle = visualElement.Q<Toggle>("LoadToggle");
      _savePath = visualElement.Q<TextField>("SavePath");
      _fileName = visualElement.Q<TextField>("FileName");
      _containerLabel = visualElement.Q<Label>("ContainerLabel");
      //_savePath.RegisterValueChangedCallback(SaveMeta);
      //_fileName.RegisterValueChangedCallback(SaveMeta);
      _loadToggle.RegisterValueChangedCallback(SetLoadState);
    }

    private void SetLoadState(ChangeEvent<bool> evt)
    {
      _spreadSheetSo.IsLoad = evt.newValue;
      //var groupData = _settings.groupsMetaData.Get(_spreadSheetSo.SheetDataType.Name);
      //groupData.isLoad = evt.newValue;
      EditorUtility.SetDirty(_settings);
    }


    private void ModelViewLink(ScriptableObject scriptableObject)
    {
      _containerObject.SetValueWithoutNotify(scriptableObject);
      _rulesContainer.Clear();
      _buttons.Clear();
      _spreadSheetSo = ((ISpreadSheetSO) scriptableObject);
      //LoadGroupFromMeta();
      // foreach (var serializationGroup in serializationGroups)
      // {
      //   if (serializationGroup.serializationRule == null)
      //   {
      //     Debug.LogError($"{serializationGroup.tag} has no serialization rule.");
      //     continue;
      //   }
      //
      //   var button = new Button(() => OnButtonClick(serializationGroup.tag));
      //   var ruleButton = new RuleButton(button, serializationGroup);
      //   ruleButton.AddListener(OnButtonClick);
      //   ruleButton.SetDarker(_spreadSheetSo.SerializationGroupTag == serializationGroup.tag ? 0 : 50);
      //   if (_spreadSheetSo.SerializationGroupTag == serializationGroup.tag)
      //   {
      //     // _currentGroup = serializationGroup;
      //     _pathContainer.style.display =
      //       serializationGroup.serializationRule.PackSeparately
      //         ? DisplayStyle.Flex
      //         : DisplayStyle.None;
      //   }
      //
      //   _rulesContainer.Add(button);
      //   _buttons.Add(ruleButton);
      // }

      _containerLabel.text = _spreadSheetSo.SheetDataType.Name;
    }


    private void OnButtonClick(string tag)
    {
      //foreach (var button in _buttons)
      //{
        //button.SetDarker(button.Group.tag == tag ? 0 : 50);
        //if (tag == button.Group.tag)
        //{
          // _currentGroup = button.Group;
          // _pathContainer.style.display =
          //   button.Group.serializationRule.PackSeparately
          //     ? DisplayStyle.Flex
          //     : DisplayStyle.None;
        //}
      //}

      _spreadSheetSo.SerializationGroupTag = tag;
      //SaveGroupToMeta();
      //LoadMeta();
    }

    // private void SaveGroupToMeta()
    // {
    //   var groupData = _settings.groupsMetaData.Get(_spreadSheetSo.SheetDataType.Name);
    //   groupData.group = _spreadSheetSo.SerializationGroupTag;
    //   EditorUtility.SetDirty(_settings);
    // }
    //
    // private void LoadGroupFromMeta()
    // {
    //   var groupData = _settings.groupsMetaData.Get(_spreadSheetSo.SheetDataType.Name);
    //   _spreadSheetSo.SerializationGroupTag = groupData.group;
    //   _spreadSheetSo.IsLoad = groupData.isLoad;
    //   _loadToggle.SetValueWithoutNotify(groupData.isLoad);
    // }
    //
    //
    // private void SaveMeta(ChangeEvent<string> evt)
    // {
    //   var metaData = _currentGroup.serializationRule.GoogleSheetSerializeConfig.Get(_spreadSheetSo.SheetDataType.Name);
    //   metaData.fileName = _fileName.value;
    //   metaData.savePath = _savePath.value;
    //   EditorUtility.SetDirty(_currentGroup.serializationRule);
    // }
    //
    // private void LoadMeta()
    // {
    //   var metaData = _currentGroup.serializationRule.GoogleSheetSerializeConfig.Get(_spreadSheetSo.SheetDataType.Name);
    //   _fileName.value = metaData.fileName;
    //   _savePath.value = metaData.savePath;
    // }
  }
}