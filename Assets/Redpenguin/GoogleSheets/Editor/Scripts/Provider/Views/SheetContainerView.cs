using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Redpenguin.GoogleSheets.Editor.Provider.Views
{
    public class SheetContainerView
    {
        public ObjectField ContainerObject { get; set; }
        public Label ContainerLabel { get; set; }
        public Toggle SaveSeparatelyToggle { get; set; }
        public Foldout AdditionalFoldout { get; set; }
        public VisualElement FileNameContainer { get; set; }
        public Toggle FileNameToggle { get; set; }
        public Button ButtonSave { get; set; }
        public TextField FileName { get; set; }
        public TextField SavePath { get; set; }
        public Toggle LoadToggle { get; set; }
        public GroupBox GroupBox { get; set; }
    
        public SheetContainerView(VisualElement view)
        {
            ContainerObject = view.Q<ObjectField>("ContainerObject");
            LoadToggle = view.Q<Toggle>("LoadToggle");
            SavePath = view.Q<TextField>("SavePath");
            FileName = view.Q<TextField>("FileName");
            ContainerLabel = view.Q<Label>("ContainerLabel");
            SaveSeparatelyToggle = view.Q<Toggle>("SaveSeparatelyToggle");
            AdditionalFoldout = view.Q<Foldout>("AdditionalFoldout");
            FileNameContainer = view.Q<VisualElement>("FileNameContainer");
            FileNameToggle = view.Q<Toggle>("FileNameToggle");
            ButtonSave = view.Q<Button>("ButtonSave");
            GroupBox = view.Q<GroupBox>();
        }
    }
}