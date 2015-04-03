using System.Collections;
using Assets.eDriven.Skins;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Styles;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

public class SliderDemo : eDriven.Gui.Gui
{
    /*void Awake()
    {
        Debug.Log("CreateChildren");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }*/

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof(ScrollerSkin2),
            Left = 0,
            Right = 0,
            Top = 0,
            Bottom = 0,
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        Group viewport = new Group
        {
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            }
        };
        scroller.Viewport = viewport;

        #endregion

        TitleLabel titleLabel = new TitleLabel
        {
            Text = "Slider Demo",
            StyleName = "title",
            Right = 20,
            Top = 20
        };
        AddChild(titleLabel);

        var labelStyles = new Hashtable
                              {
                                  {"labelStyle", BlueLabelStyle.Instance}
                              };

        AddChild(new Spacer { Height = 20 });

        #region Vertical sliders

        HGroup hbox = new HGroup
        {
            PercentWidth = 100,
            PercentHeight = 100,
            Gap = 10
        };
        viewport.AddChild(hbox);

        WrapVBox(hbox, new Label {Text = "100% slider", Styles = labelStyles},
            new VSlider {PercentHeight = 100})
            .PercentHeight = 100;

        WrapVBox(hbox, new Label {Text = "400px slider", Styles = labelStyles},
            new VSlider { Width = 30, Height = 400, SkinClass = typeof(VSliderSkin2) });

        WrapVBox(hbox, new Label { Text = "400px slider, disabled", Styles = labelStyles },
            new VSlider { Width = 30, Height = 400, SkinClass = typeof (VSliderSkin2), Enabled = false });

        WrapVBox(hbox, new Label {Text = "50x400 slider", Styles = labelStyles},
            new VSlider {Width = 50, Height = 400, SkinClass = typeof (VSliderSkin2)});

        WrapVBox(hbox, new Label { Text = "80x400 slider", Styles = labelStyles },
            new VSlider { Width = 80, Height = 400, SkinClass = typeof(VSliderSkin3) });

        WrapVBox(hbox, new Label { Text = "80x100% slider", Styles = labelStyles },
            new VSlider { Width = 80, PercentHeight = 100, Maximum = 1000, Value = 500, SkinClass = typeof(VSliderSkin3) })
            .PercentHeight = 100;

        #endregion

        #region Horizontal sliders

        WrapHBox(viewport, new Label {Text = "100% slider", Styles = labelStyles},
            new HSlider {Id = "miki", Maximum = 400, PercentWidth = 100})
            .PercentWidth = 100;

        WrapHBox(viewport, new Label { Text = "400px slider", Styles = labelStyles },
            new HSlider { Width = 400, Maximum = 400, Height = 30, SkinClass = typeof(HSliderSkin2) });

        WrapHBox(viewport, new Label {Text = "400px slider, disabled", Styles = labelStyles},
            new HSlider { Width = 400, Maximum = 400, Height = 30, SkinClass = typeof(HSliderSkin2), Enabled = false });

        WrapHBox(viewport, new Label {Text = "400x50 slider", Styles = labelStyles},
            new HSlider {Width = 400, Maximum = 400, Height = 50, SkinClass = typeof (HSliderSkin2)});

        WrapHBox(viewport, new Label { Text = "80x400 slider", Styles = labelStyles },
            new HSlider { Width = 400, Height = 80, SkinClass = typeof(HSliderSkin3) });

        WrapHBox(viewport, new Label { Text = "80x100% slider", Styles = labelStyles },
            new HSlider { PercentWidth = 100, Maximum = 1000, Value = 500, Height = 80, SkinClass = typeof(HSliderSkin3) })
            .PercentWidth = 100;

        #endregion

        AddChild(new Spacer {Height = 20});
    }

    #region Helper

    private static HGroup WrapHBox(Component parent, params DisplayListMember[] children)
    {
        HGroup wrapper = new HGroup { HorizontalAlign = HorizontalAlign.Left, VerticalAlign = VerticalAlign.Middle };
        parent.AddChild(wrapper);

        foreach (DisplayListMember child in children)
        {
            wrapper.AddChild(child);
        }
        return wrapper;
    }

    private static VGroup WrapVBox(Component parent, params DisplayListMember[] children)
    {
        VGroup wrapper = new VGroup { HorizontalAlign = HorizontalAlign.Center };
        parent.AddChild(wrapper);

        foreach (DisplayListMember child in children)
        {
            wrapper.AddChild(child);
        }
        return wrapper;
    }

    #endregion
}