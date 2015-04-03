using Assets.eDriven.Skins;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using UnityEngine;

public class ImageDemo : eDriven.Gui.Gui
{
    private Panel _panel;

    protected override void OnInitialize()
    {
        base.OnInitialize();

        //Layout = new VerticalLayout
        //{
        //    HorizontalAlign = HorizontalAlign.Center,
        //    VerticalAlign = VerticalAlign.Middle,
        //    PaddingLeft = 10, PaddingRight = 10,PaddingTop = 10, PaddingBottom = 10,
        //    Gap = 10
        //};

        Layout = new AbsoluteLayout();
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Scroller and viewport

        Scroller scroller = new Scroller
                                {
                                    SkinClass = typeof(ScrollerSkin2),
                                    Left = 0,
                                    Right = 0,
                                    Top = 0,
                                    Bottom = 0,
                                };
        AddChild(scroller);

        Group viewport = new Group
                                {
                                    Id = "viewport",
                                    Layout = new AbsoluteLayout()
                                };
        scroller.Viewport = viewport;

        Group g = new Group();
        viewport.AddChild(g);

        var image = new Image
        {
            Mode = ImageMode.Tiled,
            TilingAnchor = Anchor.BottomRight,
            Texture = (Texture)Resources.Load("stripes3"),
            Left = 10,
            Right = 10,
            Top = 10,
            Bottom = 10,
            Tooltip = "Original size"
        };
        g.AddChild(image);

        #endregion

        #region VGroup

        VGroup vGroup = new VGroup
                            {
                                Id = "vGroup",
                                PaddingLeft = 10,
                                PaddingRight = 10,
                                PaddingTop = 10,
                                PaddingBottom = 10,
                                Left = 10,
                                Right = 10,
                                Top = 10,
                                Bottom = 10,
                                Gap = 10
                            };
        g.AddChild(vGroup);

        #endregion

        #region Images

        var hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        image = new Image
        {
            Texture = (Texture) Resources.Load("eDriven/Editor/Logo/logo"),
            ScaleMode = ImageScaleMode.OriginalSize
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 600,
            Height = 400,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo"),
            ScaleMode = ImageScaleMode.OriginalSize
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 300,
            Height = 150,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo"),
            ScaleMode = ImageScaleMode.OriginalSize
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 150,
            Height = 300,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo"),
            ScaleMode = ImageScaleMode.OriginalSize
        };
        hgroup.AddChild(image);

        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        image = new Image
        {
            Width = 350,
            Height = 200,
            ScaleMode = ImageScaleMode.ScaleToWidth,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 350,
            Height = 200,
            ScaleMode = ImageScaleMode.ScaleToHeight,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 350,
            Height = 200,
            ScaleMode = ImageScaleMode.ScaleToFit,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 350,
            Height = 200,
            ScaleMode = ImageScaleMode.ScaleToFill,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 350,
            Height = 200,
            ScaleMode = ImageScaleMode.StretchToFit,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        image = new Image
        {
            Width = 200,
            Height = 350,
            ScaleMode = ImageScaleMode.ScaleToWidth,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 200,
            Height = 350,
            ScaleMode = ImageScaleMode.ScaleToHeight,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 200,
            Height = 350,
            ScaleMode = ImageScaleMode.ScaleToFit,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 200,
            Height = 350,
            ScaleMode = ImageScaleMode.ScaleToFill,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        image = new Image
        {
            Width = 200,
            Height = 350,
            ScaleMode = ImageScaleMode.StretchToFit,
            Texture = (Texture)Resources.Load("eDriven/Editor/Logo/logo")
        };
        hgroup.AddChild(image);

        hgroup = new HGroup();
        vGroup.AddChild(hgroup);

        #endregion

        #region Panels

        _panel = new Panel
                        {
                            Title = "Panel 1",
                            MinWidth = 200,
                            MinHeight = 350
                        };
        hgroup.AddChild(_panel);

        _panel = new Panel
                        {
                            SkinClass = typeof(PanelSkin3),
                            Title = "Panel 2",
                            MinWidth = 200,
                            MinHeight = 350
                        };
        hgroup.AddChild(_panel);

        _panel = new Panel
                        {
                            SkinClass = typeof(PanelSkin4),
                            Title = "Panel 3",
                            MinWidth = 200,
                            MinHeight = 350
                        };
        hgroup.AddChild(_panel);

        #endregion

    }
}