using System;
using System.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using Assets.eDriven.Demo.Components;

[Obfuscation(Exclude = true)]
[Toolbox(Label = "Animated label", Group = "My controls", Icon = "eDriven/Editor/Controls/label")]
public class AnimatedLabelAdapter : LabelAdapter
{
    public override Type ComponentType
    {
        get { return typeof (TitleLabel); }
    }

    public override Component NewInstance()
    {
        return new TitleLabel();
    }
}