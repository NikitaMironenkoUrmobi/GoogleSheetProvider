using System;
using System.Collections.Generic;
using System.Linq;
using Redpenguin.GoogleSheets.Editor.Profiles.Model;
using Redpenguin.GoogleSheets.Settings;
using Redpenguin.GoogleSheets.Settings.SerializationRules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static System.String;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Presenters
{
  public class ProfilePresenter
  {
    private TextField _profileNameTextField;
    private TextField _tableFieldID;
    private ObjectField _credentialObjectField;
    private ColorField _profileColorField;
    private TextField _savePathTextField;
    private readonly ProfilesContainer _container;
    private Label _profileLabelTextField;
    private TextField _fileNameTextField;
    private DropdownField _serializationRuleDropdownField;
    private Button _buttonClearMeta;
    public BoxContainerPresenter GroupBoxContainer { get; }
    private SerializeSettingsContainer SerializeSettingsContainer => _container.SerializeSettingsContainer;

    public ProfilePresenter(ProfileModel model, VisualElement view, ProfilesContainer container)
    {
      _container = container;
      GroupBoxContainer = new BoxContainerPresenter(view,model.profileName);
      SetupView(view);
      ModelViewLink(model);
    }

    private void ModelViewLink(ProfileModel model)
    {
      var serializationRuleSettingModel = SerializeSettingsContainer.GetSerializeRuleSetting(model.profileName);
      FileNameLink(serializationRuleSettingModel);
      SavePathLink(serializationRuleSettingModel);
      SerializationRuleLink(serializationRuleSettingModel);

      ProfileNameLink(model, serializationRuleSettingModel);
      ProfileColorLink(model);
      TableIDLink(model);
      CredentialLink(model);
      ButtonClearMetaSetup(model);
    }

    private void ButtonClearMetaSetup(ProfileModel model)
    {
      _buttonClearMeta.clickable.clicked += () =>
      {
        _container.ClearMeta(model.profileName);
        EditorUtility.SetDirty(_container);
      };
    }

    private void ProfileNameLink(ProfileModel model, SerializationRuleSetting serializationRuleSetting)
    {
      if (model.profileName != Empty)
        _profileNameTextField.SetValueWithoutNotify(model.profileName);
      
      _profileLabelTextField.text = model.profileName;
      _profileNameTextField.RegisterValueChangedCallback(x =>
      {
        model.profileName = x.newValue;
        _profileLabelTextField.text = x.newValue;
        serializationRuleSetting.profile = x.newValue;
        EditorUtility.SetDirty(_container);
        EditorUtility.SetDirty(_container.SerializeSettingsContainer);
      });
    }
    private void SerializationRuleLink(SerializationRuleSetting model)
    {
      var list = GetRules();
      var list2 = list.Select(x => $"{x}, {x.Assembly}").ToList();
      _serializationRuleDropdownField.choices = list.Select(x => x.Name).ToList();
      
      if (model.serializationRuleType != Empty)
      {
        var index = list2.FindIndex(x => x == model.serializationRuleType);
        _serializationRuleDropdownField.index = index;
      }

      _serializationRuleDropdownField.RegisterValueChangedCallback(x =>
      {
        model.serializationRuleType = list2[_serializationRuleDropdownField.index];
        EditorUtility.SetDirty(SerializeSettingsContainer);
      });
    }
    private void TableIDLink(ProfileModel model)
    {
      if (model.tableID != Empty)
        _tableFieldID.SetValueWithoutNotify(model.tableID);
      _tableFieldID.RegisterValueChangedCallback(x =>
      {
        model.tableID = x.newValue;
        EditorUtility.SetDirty(_container);
      });
    }
    private void FileNameLink(SerializationRuleSetting model)
    {
      if (model.fileName != Empty)
        _fileNameTextField.SetValueWithoutNotify(model.fileName);
      _fileNameTextField.RegisterValueChangedCallback(x =>
      {
        model.fileName = x.newValue;
        EditorUtility.SetDirty(SerializeSettingsContainer);
      });
    }
    private void SavePathLink(SerializationRuleSetting model)
    {
      if (model.savePath != Empty)
        _savePathTextField.SetValueWithoutNotify(model.savePath);
      _savePathTextField.RegisterValueChangedCallback(x =>
      {
        model.savePath = x.newValue;
        EditorUtility.SetDirty(SerializeSettingsContainer);
      });
    }
    private void CredentialLink(ProfileModel model)
    {
      if (model.credential != null)
        _credentialObjectField.SetValueWithoutNotify(model.credential);
      _credentialObjectField.RegisterValueChangedCallback(x =>
      {
        model.credential = (TextAsset) x.newValue;
        EditorUtility.SetDirty(_container);
      });
    }
    private void ProfileColorLink(ProfileModel model)
    {
      _profileColorField.SetValueWithoutNotify(model.color);
      _profileColorField.RegisterValueChangedCallback(x =>
      {
        model.color = x.newValue;
        EditorUtility.SetDirty(_container);
      });
    }

    private void SetupView(VisualElement view)
    {
      _profileNameTextField = view.Q<TextField>("ProfileName");
      _tableFieldID = view.Q<TextField>("TableID");
      _credentialObjectField = view.Q<ObjectField>("Credential");
      _profileColorField = view.Q<ColorField>("ProfileColor");
      _savePathTextField = view.Q<TextField>("SavePath");
      _profileLabelTextField = view.Q<Label>("ProfileLabel");
      _fileNameTextField = view.Q<TextField>("FileName");
      _serializationRuleDropdownField = view.Q<DropdownField>("SerializationRule");
      _buttonClearMeta = view.Q<Button>("ButtonClearMeta");
    }
    
    private List<Type> GetRules()
    {
      var listOfTypes = new List<Type>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        assembly.GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(SerializationRule)))
          .ToList().ForEach(x => listOfTypes.Add(x));
      }

      return listOfTypes;
    }
  }
}