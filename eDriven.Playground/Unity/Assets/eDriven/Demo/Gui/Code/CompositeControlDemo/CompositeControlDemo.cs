using System.Collections;
using Assets.eDriven.Demo.Gui.Code.CompositeControlDemo;
using Assets.eDriven.Demo._shared.Code.Util;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using eDriven.Gui.Util;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Styles;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

public class CompositeControlDemo : eDriven.Gui.Gui
{
    override protected void CreateChildren()
    {
        base.CreateChildren();

        TitleLabel label = new TitleLabel
        {
            StyleName = "title",
            Left = 10,
            Top = 20
        };
        AddChild(label);

        var labelStyles = new Hashtable
        {
            {"labelStyle", BlueLabelStyle.Instance}
        };

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
            {
                "Composite Control Demo", 
                "Created with eDriven.Gui",
                //"Author: Danko Kozar",
                "Composite controls could be built using child controls",
                "These controls could treated as simple controls...",
                "... and used to build even more complex controls"
            },
            Callback = delegate(string line) { label.Text = line; }
        }.Start();

        AddChild(new Spacer { Height = 20 });

        HGroup hbox = new HGroup
        {
            HorizontalCenter = 0,
            VerticalCenter = 0,
            Gap = 50
        };
        AddChild(hbox);

        WrapVBox(hbox, new Label { Text = "RGB Mixer", Styles = labelStyles, PercentWidth = 100 },
            new RgbMixer { RgbColor = ColorMixer.FromHex(0xFBAF5C).ToColor() });

        WrapVBox(hbox, new Label { Text = "RGB Mixer", Styles = labelStyles, PercentWidth = 100 },
            new RgbMixer { RgbColor = ColorMixer.FromHex(0x855FA8).ToColor() });

        WrapVBox(hbox, new Label { Text = "RGB Mixer", Styles = labelStyles, PercentWidth = 100 },
            new RgbMixer { RgbColor = ColorMixer.FromHex(0x438CCA).ToColor() });
    }

    #region Helper

    protected HGroup WrapHBox(Component parent, params DisplayListMember[] children)
    {
        HGroup wrapper = new HGroup { HorizontalAlign = HorizontalAlign.Left, VerticalAlign = VerticalAlign.Middle };
        parent.AddChild(wrapper);

        foreach (DisplayListMember child in children)
        {
            wrapper.AddChild(child);
        }
        return wrapper;
    }

    protected VGroup WrapVBox(Component parent, params DisplayListMember[] children)
    {
        VGroup wrapper = new VGroup { HorizontalAlign = HorizontalAlign.Center, Gap = 10 };
        parent.AddChild(wrapper);

        foreach (DisplayListMember child in children)
        {
            wrapper.AddChild(child);
        }
        return wrapper;
    }

    #endregion
}