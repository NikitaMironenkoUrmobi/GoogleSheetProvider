using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Profiles.Presenters
{
  public class BoxContainerPresenter
  {
    private bool _isGroupBoxSelect;
    private readonly Color _grayBoxColor;
    private readonly GroupBox _groupBox;
    private VisualElement _view;

    public BoxContainerPresenter(VisualElement view, string profileName)
    {
      _view = view;
      _groupBox = view.Q<GroupBox>();
      _grayBoxColor = new Color32(0, 0, 0, 52);
      _groupBox.SetEnabled(profileName != "ExampleProfile");
    }

    public void SetProfileClickEvent(Action<VisualElement> onClick)
    {
      _groupBox.AddManipulator(new Clickable(() =>
      {
        onClick.Invoke(_view);
        Select(!_isGroupBoxSelect);
      }));
    }

    public void Select(bool value)
    {
      _isGroupBoxSelect = value;
      if (value)
      {
        if (ColorUtility.TryParseHtmlString("#4f77ab", out var color))
        {
          _groupBox.style.backgroundColor = color;
        }
      }
      else
      {
        _groupBox.style.backgroundColor = _grayBoxColor;
      }
    }
  }
}