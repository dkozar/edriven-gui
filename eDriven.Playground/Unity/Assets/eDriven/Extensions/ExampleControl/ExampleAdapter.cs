using System;
using eDriven.Extensions.ExampleControl;
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using Component=eDriven.Gui.Components.Component;

[Toolbox(Label = "ExampleControl", Group = "My controls", Icon = "eDriven/Editor/Controls/example")]

public class ExampleAdapter : ComponentAdapter
{
    #region Saveable values

    [Saveable]
    public string Text = "This is a text";

    [Saveable]
    public string ButtonText = "Click me";

    [Saveable]
    public bool BoolExample;

    [Saveable]
    public Direction EnumExample = Direction.Horizontal;

    #endregion

    public ExampleAdapter()
    {
        // setting default values   
        MinWidth = 400;
    }

    public override Type ComponentType
    {
        get { return typeof(ExampleControl); }
    }

    public override Component NewInstance()
    {
        return new ExampleControl();
    }

    public override void Apply(Component component)
    {
        base.Apply(component);

        ExampleControl exampleControl = (ExampleControl)component;
        exampleControl.Text = Text;
        exampleControl.ButtonText = ButtonText;
    }
}