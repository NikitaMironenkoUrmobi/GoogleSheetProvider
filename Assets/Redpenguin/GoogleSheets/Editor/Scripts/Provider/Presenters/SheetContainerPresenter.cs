using System;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using Redpenguin.GoogleSheets.Settings;
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
    private ISpreadSheetSoWrapper _scriptableObject;
    private Button _buttonSave;
    private Action<Type> _onLoadClick;
    private Type _modelType;

    public SheetContainerPresenter(
      VisualElement view,
      Type modelType,
      ProfilesContainer profilesContainer,
      ISpreadSheetSoWrapper scriptableObject,
      Action<Type> onLoadClick)
    {
      _modelType = modelType;
      _onLoadClick = onLoadClick;
      _scriptableObject = scriptableObject;
      _profilesContainer = profilesContainer;
      SetView(view);
      ModelViewLink(modelType);
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
      _buttonSave = view.Q<Button>("ButtonSave");
      _groupBox = view.Q<GroupBox>();
    }

    private void ModelViewLink(Type dataType)
    {
      _containerObject.SetEnabled(_scriptableObject != null);
      _containerObject.objectType = typeof(SpreadSheetSoWrapper);
      _containerObject.RegisterValueChangedCallback(x =>
      {
        _containerObject.SetValueWithoutNotify(x.previousValue);
      });
     
      if (_scriptableObject != null)
        _containerObject.SetValueWithoutNotify(_scriptableObject as SpreadSheetSoWrapper);
      _containerLabel.text = dataType.Name;
      
      var currentProfile = _profilesContainer.CurrentProfile;
      var metaData =
        _profilesContainer.SerializeSettingsContainer.GetSerializeSetting(currentProfile.profileName,
          dataType.ToString());
      var metaDataEditor = currentProfile.metaData.GetMeta(dataType.ToString());
      LoadSetup(metaDataEditor);
      SaveSeparatelyToggleSetup(metaData, currentProfile.profileName);
      FileNameToggleSetup(metaData);
      SavePathSetup(metaData);
      FileNameSetup(metaData);
      EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      EditorUtility.SetDirty(_profilesContainer);

      _buttonSave.clickable.clicked += () => { _onLoadClick.Invoke(dataType); };
    }
    private void SavePathSetup(SerializeSetting metaData)
    {
      if (metaData.savePath != Empty)
        _savePath.SetValueWithoutNotify(metaData.savePath);
      _savePath.RegisterValueChangedCallback(x =>
      {
        metaData.savePath = x.newValue;
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void FileNameSetup(SerializeSetting metaData)
    {
      if (metaData.fileName != Empty)
        _fileName.SetValueWithoutNotify(metaData.fileName);
      _fileName.RegisterValueChangedCallback(x =>
      {
        metaData.fileName = x.newValue;
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void SaveSeparatelyToggleSetup(SerializeSetting metaData, string profileName)
    {
      var data = _profilesContainer.SerializeSettingsContainer.GetSerializeRuleSetting(profileName);
      if (metaData.saveSeparately && metaData.fileName == Empty)
      {
        _fileName.value = _modelType.Name;
        metaData.fileName = _modelType.Name;
      }

      if (metaData.saveSeparately && metaData.savePath == Empty)
      {
        _savePath.value = data.savePath;
        metaData.savePath = data.savePath;
      }
      _additionalFoldout.value = metaData.saveSeparately;
      _saveSeparatelyToggle.SetValueWithoutNotify(metaData.saveSeparately);
      _savePath.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _fileNameContainer.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _buttonSave.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      

      _saveSeparatelyToggle.RegisterValueChangedCallback(x =>
      {
        metaData.saveSeparately = x.newValue;
        if (metaData.saveSeparately && metaData.fileName == Empty)
        {
          _fileName.value = _modelType.Name;
          metaData.fileName = _modelType.Name;
        }

        if (metaData.saveSeparately && metaData.savePath == Empty)
        {
          _savePath.value = data.savePath;
          metaData.savePath = data.savePath;
        }
        _fileNameToggle.value = false;
        _savePath.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _fileNameContainer.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _buttonSave.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        

        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void FileNameToggleSetup(SerializeSetting metaData)
    {
      if (!metaData.saveSeparately)
      {
        metaData.overrideName = false;
        _fileNameToggle.SetValueWithoutNotify(metaData.overrideName);
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
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
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void LoadSetup(SheetContainerMetaData metaData)
    {
      _loadToggle.value = metaData.isLoad;
      if (!metaData.isLoad)
      {
        if (ColorUtility.TryParseHtmlString("#996237", out var color))
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
          if (ColorUtility.TryParseHtmlString("#996237", out var color))
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