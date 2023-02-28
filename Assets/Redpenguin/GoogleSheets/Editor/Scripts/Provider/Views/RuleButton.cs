using System;
using Redpenguin.GoogleSheets.Editor.Utils;
using Redpenguin.GoogleSheets.Settings;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Views
{
  public class RuleButton
  {
    private readonly Button _button;
    //public readonly SerializationGroup Group;
      
    public RuleButton(Button button)
    {
      _button = button;
      //Group = group;
      //_button.text = Group.tag;
    }
    public void AddListener(Action<string> onClick)
    {
      //_button.clickable.clicked += () => onClick.Invoke(Group.tag);
    }

    public void SetDarker(int percent)
    {
      //_button.style.backgroundColor = Group.color.MakeDarker(percent);
    }
  }
}