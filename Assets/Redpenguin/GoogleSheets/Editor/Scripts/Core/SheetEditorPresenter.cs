using System.Collections.Generic;
using Redpenguin.GoogleSheets.Editor;
using Redpenguin.GoogleSheets.Runtime.Core;
using Redpenguin.GoogleSheets.Scripts.Runtime.Core;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Scripts.Editor.Core
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
    private SerializationGroup _currentGroup;

    public SheetEditorPresenter(VisualElement view, ScriptableObject model, List<SerializationGroup> rules)
    {
      SetView(view);
      ModelViewLink(model, rules);
      LoadMeta();
    }

    private void ModelViewLink(ScriptableObject scriptableObject, List<SerializationGroup> serializationGroups)
    {
      _containerObject.SetValueWithoutNotify(scriptableObject);
      _rulesContainer.Clear();
      _buttons.Clear();
      _spreadSheetSo = ((ISpreadSheetSO) scriptableObject);
      foreach (var serializationGroup in serializationGroups)
      {
        var button = new Button(() => OnButtonClick(serializationGroup.tag));
        var ruleButton = new RuleButton(button, serializationGroup);
        ruleButton.AddListener(OnButtonClick);
        ruleButton.SetDarker(_spreadSheetSo.SerializationGroupTag == serializationGroup.tag ? 0 : 50);
        if (_spreadSheetSo.SerializationGroupTag == serializationGroup.tag)
        {
          _currentGroup = serializationGroup;
          _pathContainer.style.display =
            serializationGroup.serializationRule.PackSeparately
              ? DisplayStyle.Flex
              : DisplayStyle.None;
        }

        _rulesContainer.Add(button);
        _buttons.Add(ruleButton);
      }
    }

    private void OnButtonClick(string tag)
    {
      foreach (var button in _buttons)
      {
        button.SetDarker(button.Group.tag == tag ? 0 : 50);
        if (tag == button.Group.tag)
        {
          _currentGroup = button.Group;
          _pathContainer.style.display =
            button.Group.serializationRule.PackSeparately
              ? DisplayStyle.Flex
              : DisplayStyle.None;
        }
      }

      _spreadSheetSo.SerializationGroupTag = tag;
      LoadMeta();
    }

    private void SetView(VisualElement visualElement)
    {
      _containerObject = visualElement.Q<ObjectField>("ContainerObject");
      _rulesContainer = visualElement.Q<VisualElement>("RulesContainer");
      _pathContainer = visualElement.Q<VisualElement>("PathContainer");
      _savePath = visualElement.Q<TextField>("SavePath");
      _fileName = visualElement.Q<TextField>("FileName");
      _savePath.RegisterValueChangedCallback(SaveMeta);
      _fileName.RegisterValueChangedCallback(SaveMeta);
    }

    private void SaveMeta(ChangeEvent<string> evt)
    {
      var metaData = _currentGroup.serializationRule.GoogleSheetSerializeConfig.Get(_spreadSheetSo.SheetDataTypeName);
      metaData.fileName = _fileName.value;
      metaData.savePath = _savePath.value;
    }

    private void LoadMeta()
    {
      var metaData = _currentGroup.serializationRule.GoogleSheetSerializeConfig.Get(_spreadSheetSo.SheetDataTypeName);
      _fileName.value = metaData.fileName;
      _savePath.value = metaData.savePath;
    }
  }
}