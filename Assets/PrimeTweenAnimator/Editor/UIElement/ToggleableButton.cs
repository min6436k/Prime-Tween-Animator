using UnityEditor;
using UnityEngine.UIElements;

[UxmlElement]
public partial class ToggleableButton : BindableElement,INotifyValueChanged<bool>
{
    private static StyleSheet _style;
    
    public const string UssClassName = "ODY-ToggleButton";
    public const string OnUssClassName = "ODY-ToggleButton_On";

    private bool _value;
    
    [UxmlAttribute] 
    public bool value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            
            using (var evt = ChangeEvent<bool>.GetPooled(_value, value))
            {
                evt.target = this;
                SendEvent(evt);
            }
            _value = value;
            
            SetValueWithoutNotify(value);
        } 
    }
    
    private readonly Label _label;
    
    [UxmlAttribute] 
    public string Text
    {
        get => _label.text;
        set => _label.text = value;
    }
    
    public ToggleableButton()
    {
        _style ??= AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ody.primetween-animator/Editor/Styles/ToggleableButton.uss");

        if(_style) styleSheets.Add(_style);
        
        AddToClassList(Button.ussClassName);
        AddToClassList(UssClassName);
        
        _label = new Label(){name = "label",text = "ToggleButton"};
        Add(_label);
        
        var clickable = new Clickable(OnClick);
        this.AddManipulator(clickable);
    }

    private void OnClick() => value = !value;

    public void SetValueWithoutNotify(bool newValue) => EnableInClassList(OnUssClassName, newValue);
}

