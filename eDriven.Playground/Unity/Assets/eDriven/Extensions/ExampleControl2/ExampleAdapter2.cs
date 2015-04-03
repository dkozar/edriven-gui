using System;
using eDriven.Extensions.ExampleControl2;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using Component=eDriven.Gui.Components.Component;

[Toolbox(Label = "ExampleControl2", Group = "My controls", Icon = "eDriven/Editor/Controls/example")]

public class ExampleAdapter2 : ComponentAdapter
{
    #region Saveable values

    [Saveable]
    public int LabelWidth = 150;

    [Saveable]
    public SendMode SimulateSending = SendMode.SimulateSending;

    #endregion

    public ExampleAdapter2()
    {
        // setting default values   
        MinWidth = 400;
    }

    public override Type ComponentType
    {
        get { return typeof(ExampleControl2); }
    }

    public override Component NewInstance()
    {
        return new ExampleControl2();
    }

    public override void Apply(Component component)
    {
        base.Apply(component);

        ExampleControl2 exampleControl = (ExampleControl2)component;
        exampleControl.LabelWidth = LabelWidth;
        exampleControl.SendMode = SimulateSending;
    }
}