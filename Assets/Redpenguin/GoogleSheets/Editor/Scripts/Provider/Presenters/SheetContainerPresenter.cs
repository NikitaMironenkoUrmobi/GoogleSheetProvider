using System;
using Redpenguin.GoogleSheets.Editor.Models;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static System.String;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class SheetContainerPresenter
  {
    private ObjectField _containerObject;
    private TextField _savePath;
    private TextField _fileName;
    private Toggle _loadToggle;
    private Label _containerLabel;
    private Toggle _saveSeparatelyToggle;
    private readonly ProfilesContainer _profilesContainer;
    private GroupBox _groupBox;
    private Foldout _additionalFoldout;
    private VisualElement _fileNameContainer;
    private Toggle _fileNameToggle;
    private ScriptableObject _scriptableObject;
    private Button _buttonLoad;
    private Action<Type> _onLoadClick;

    public SheetContainerPresenter(
      VisualElement view,
      Type model,
      ProfilesContainer profilesContainer,
      ScriptableObject scriptableObject, 
      Action<Type> onLoadClick)
    {
      _onLoadClick = onLoadClick;
      _scriptableObject = scriptableObject;
      _profilesContainer = profilesContainer;
      SetView(view);
      ModelViewLink(model);
    }

    private void SetView(VisualElement view)
    {
      _containerObject = view.Q<ObjectField>("ContainerObject");
      _loadToggle = view.Q<Toggle>("LoadToggle");
      _savePath = view.Q<TextField>("SavePath");
      _fileName = view.Q<TextField>("FileName");
      _containerLabel = view.Q<Label>("ContainerLabel");
      _saveSeparatelyToggle = view.Q<Toggle>("SaveSeparatelyToggle");
      _additionalFoldout = view.Q<Foldout>("AdditionalFoldout");
      _fileNameContainer = view.Q<VisualElement>("FileNameContainer");
      _fileNameToggle = view.Q<Toggle>("FileNameToggle");
      _buttonLoad = view.Q<Button>("ButtonLoad");
      _groupBox = view.Q<GroupBox>();
    }

    private void ModelViewLink(Type dataType)
    {

      _containerObject.SetEnabled(_scriptableObject != null);
      if(_scriptableObject != null)
        _containerObject.SetValueWithoutNotify(_scriptableObject);
      _containerLabel.text = dataType.Name;

      var metaData = _profilesContainer.CurrentProfile.metaData.GetMeta(dataType.ToString());
      LoadSetup(metaData);
      SaveSeparatelyToggleSetup(metaData);
      FileNameToggleSetup(metaData);
      SavePathSetup(metaData);
      FileNameSetup(metaData);

      _buttonLoad.clickable.clicked += () => {_onLoadClick.Invoke(dataType); };
    }

    private void SavePathSetup(SheetContainerMetaData metaData)
    {
      if (metaData.savePath != Empty)
        _savePath.SetValueWithoutNotify(metaData.savePath);
      _savePath.RegisterValueChangedCallback(x =>
      {
        metaData.savePath = x.newValue;
        EditorUtility.SetDirty(_profilesContainer);
      });
    }

    private void FileNameSetup(SheetContainerMetaData metaData)
    {
      if (metaData.fileName != Empty)
        _fileName.SetValueWithoutNotify(metaData.fileName);
      _fileName.RegisterValueChangedCallback(x =>
      {
        metaData.fileName = x.newValue;
        EditorUtility.SetDirty(_profilesContainer);
      });
    }

    private void SaveSeparatelyToggleSetup(SheetContainerMetaData metaData)
    {
      _additionalFoldout.value = metaData.saveSeparately;
      _saveSeparatelyToggle.SetValueWithoutNotify(metaData.saveSeparately);
      _savePath.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _fileNameContainer.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _buttonLoad.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _saveSeparatelyToggle.RegisterValueChangedCallback(x =>
      {
        metaData.saveSeparately = x.newValue;
        _fileNameToggle.value = false;
        _savePath.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _fileNameContainer.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _buttonLoad.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        EditorUtility.SetDirty(_profilesContainer);
      });
    }
    private void FileNameToggleSetup(SheetContainerMetaData metaData)
    {
      if (!metaData.saveSeparately)
      {
        metaData.overrideName = false;
        _fileNameToggle.SetValueWithoutNotify(metaData.overrideName);
        EditorUtility.SetDirty(_profilesContainer);
      }
      else
      {
        _fileNameToggle.SetValueWithoutNotify(metaData.overrideName);
      }
      _fileName.style.display = metaData.overrideName ? DisplayStyle.Flex : DisplayStyle.None;
      _fileNameToggle.RegisterValueChangedCallback(x =>
      {
        metaData.overrideName = x.newValue;
        _fileName.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        EditorUtility.SetDirty(_profilesContainer);
      });
    }

    private void LoadSetup(SheetContainerMetaData metaData)
    {
      _loadToggle.value = metaData.isLoad;
      if (!metaData.isLoad)
      {
        if(ColorUtility.TryParseHtmlString("#996237", out var color))
        {
          _groupBox.style.backgroundColor = color;
        }
      }
      else
      {
        _groupBox.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 52));
      }
      _loadToggle.RegisterValueChangedCallback(x =>
      {
        metaData.isLoad = x.newValue;
        if (!metaData.isLoad)
        {
          if(ColorUtility.TryParseHtmlString("#996237", out var color))
          {
            _groupBox.style.backgroundColor = color;
          }
        }
        else
        {
          _groupBox.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 52));
        }
        EditorUtility.SetDirty(_profilesContainer);
      });
    }
  }
}