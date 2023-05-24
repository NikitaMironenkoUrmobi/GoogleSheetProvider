using System;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Editor.Profiles.ProfilesMetaData;
using Redpenguin.GoogleSheets.Editor.Provider.Views;
using Redpenguin.GoogleSheets.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static System.String;

namespace Redpenguin.GoogleSheets.Editor.Provider.Presenters
{
  public class SheetContainerPresenter
  {
    private readonly SheetContainerView _view;
    private readonly ProfilesContainer _profilesContainer;
    private readonly ISpreadSheetSoWrapper _scriptableObject;
    private readonly Action<Type> _onLoadClick;
    private readonly Type _modelType;

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
      _view = new SheetContainerView(view);
      ModelViewLink(modelType);
    }

    private void ModelViewLink(Type dataType)
    {
      var currentProfile = _profilesContainer.CurrentProfile;
      var metaData =
        _profilesContainer.SerializeSettingsContainer.GetSerializeSetting(currentProfile.profileName,
          dataType.ToString());
      var metaDataEditor = currentProfile.metaData.GetMeta(dataType.ToString());
      var serializationRuleSetting = _profilesContainer.SerializeSettingsContainer.GetSerializeRuleSetting(currentProfile.profileName);

      ContainerObjectSetup();
      ContainerLabelSetup(dataType, metaData, serializationRuleSetting);
      LoadSetup(metaDataEditor);
      SaveSeparatelyToggleSetup(metaData, serializationRuleSetting);
      FileNameToggleSetup(metaData);
      SavePathSetup(metaData);
      FileNameSetup(metaData);
      EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      EditorUtility.SetDirty(_profilesContainer);

      _view.ButtonSave.clickable.clicked += () => { _onLoadClick.Invoke(dataType); };
    }

    private void ContainerObjectSetup()
    {
      _view.ContainerObject.SetEnabled(_scriptableObject != null);
      _view.ContainerObject.style.visibility = _scriptableObject != null ? Visibility.Visible : Visibility.Hidden;
      _view.ContainerObject.objectType = typeof(SpreadSheetSoWrapper);
      _view.ContainerObject.RegisterValueChangedCallback(x => { _view.ContainerObject.SetValueWithoutNotify(x.previousValue); });

      if (_scriptableObject != null)
        _view.ContainerObject.SetValueWithoutNotify(_scriptableObject as SpreadSheetSoWrapper);
    }

    private void ContainerLabelSetup(Type dataType, SerializeSetting metaData,
      SerializationRuleSetting serializationRuleSetting)
    {
      _view.ContainerLabel.text = dataType.Name;
      _view.ContainerLabel.RemoveManipulator(SelectFileOnLableClick(metaData, serializationRuleSetting));
      _view.ContainerLabel.AddManipulator(SelectFileOnLableClick(metaData, serializationRuleSetting));
    }

    private Clickable SelectFileOnLableClick(SerializeSetting metaData, SerializationRuleSetting serializationRuleSetting)
    {
      return new Clickable(() =>
      {
        var path = metaData.saveSeparately ? metaData.savePath : serializationRuleSetting.savePath;
        var name = metaData.saveSeparately ? metaData.fileName : serializationRuleSetting.fileName;
        path = $"Assets/{path}/{name}.json";
        var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if(obj == null) return;
        Selection.activeObject = obj;
      });
    }

    private void SavePathSetup(SerializeSetting metaData)
    {
      if (metaData.savePath != Empty)
        _view.SavePath.SetValueWithoutNotify(metaData.savePath);
      _view.SavePath.RegisterValueChangedCallback(x =>
      {
        metaData.savePath = x.newValue;
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void FileNameSetup(SerializeSetting metaData)
    {
      if (metaData.fileName != Empty)
        _view.FileName.SetValueWithoutNotify(metaData.fileName);
      _view.FileName.RegisterValueChangedCallback(x =>
      {
        metaData.fileName = x.newValue;
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void SaveSeparatelyToggleSetup(SerializeSetting metaData, SerializationRuleSetting serializationRuleSetting)
    {
      if (metaData.saveSeparately && metaData.fileName == Empty)
      {
        _view.FileName.value = _modelType.Name;
        metaData.fileName = _modelType.Name;
      }

      if (metaData.saveSeparately && metaData.savePath == Empty)
      {
        _view.SavePath.value = serializationRuleSetting.savePath;
        metaData.savePath = serializationRuleSetting.savePath;
      }
      _view.AdditionalFoldout.value = metaData.saveSeparately;
      _view.SaveSeparatelyToggle.SetValueWithoutNotify(metaData.saveSeparately);
      _view.SavePath.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _view.FileNameContainer.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      _view.ButtonSave.style.display = metaData.saveSeparately ? DisplayStyle.Flex : DisplayStyle.None;
      

      _view.SaveSeparatelyToggle.RegisterValueChangedCallback(x =>
      {
        metaData.saveSeparately = x.newValue;
        if (metaData.saveSeparately && metaData.fileName == Empty)
        {
          _view.FileName.value = _modelType.Name;
          metaData.fileName = _modelType.Name;
        }

        if (metaData.saveSeparately && metaData.savePath == Empty)
        {
          _view.SavePath.value = serializationRuleSetting.savePath;
          metaData.savePath = serializationRuleSetting.savePath;
        }
        _view.FileNameToggle.value = false;
        _view.SavePath.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _view.FileNameContainer.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        _view.ButtonSave.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        

        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void FileNameToggleSetup(SerializeSetting metaData)
    {
      if (!metaData.saveSeparately)
      {
        metaData.overrideName = false;
        _view.FileNameToggle.SetValueWithoutNotify(metaData.overrideName);
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      }
      else
      {
        _view.FileNameToggle.SetValueWithoutNotify(metaData.overrideName);
      }

      _view.FileName.style.display = metaData.overrideName ? DisplayStyle.Flex : DisplayStyle.None;
      _view.FileNameToggle.RegisterValueChangedCallback(x =>
      {
        metaData.overrideName = x.newValue;
        _view.FileName.style.display = x.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        EditorUtility.SetDirty(_profilesContainer.SerializeSettingsContainer);
      });
    }

    private void LoadSetup(SheetContainerMetaData metaData)
    {
      _view.LoadToggle.value = metaData.isLoad;
      if (!metaData.isLoad)
      {
        if (ColorUtility.TryParseHtmlString("#996237", out var color))
        {
          _view.GroupBox.style.backgroundColor = color;
        }
      }
      else
      {
        _view.GroupBox.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 52));
      }

      _view.LoadToggle.RegisterValueChangedCallback(x =>
      {
        metaData.isLoad = x.newValue;
        if (!metaData.isLoad)
        {
          if (ColorUtility.TryParseHtmlString("#996237", out var color))
          {
            _view.GroupBox.style.backgroundColor = color;
          }
        }
        else
        {
          _view.GroupBox.style.backgroundColor = new StyleColor(new Color32(0, 0, 0, 52));
        }

        EditorUtility.SetDirty(_profilesContainer);
      });
    }
  }
}