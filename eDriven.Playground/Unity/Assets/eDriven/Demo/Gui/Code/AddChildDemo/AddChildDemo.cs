using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;

public class AddChildDemo : Gui
{
    private Button _btnAddRect;
    private Button _btnAddText;
    
    private readonly TweenFactory _tweenFactory = new TweenFactory(typeof(FadeInUp));

    private int _count;

    private Panel _panel;

    #region Dummy text

    private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam non urna purus. Suspendisse tincidunt scelerisque euismod. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent ipsum elit, consectetur ac scelerisque vitae, rhoncus porta nulla. In semper placerat sem nec consectetur. Donec mi arcu, tristique at viverra eget, accumsan at erat. Nulla ut ligula nibh, sit amet consequat neque. Aliquam a turpis sem, at dictum leo. Sed ut lacinia quam. Aenean facilisis vehicula lorem a rutrum.

Nam dignissim consectetur sem, in ultricies turpis elementum at. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nullam venenatis massa id velit eleifend ultrices. Cras sed arcu nec nisi hendrerit sollicitudin vel non eros. Sed nunc eros, auctor ut laoreet et, sodales non justo. Nulla id varius tortor. Sed ac malesuada lectus. Praesent lobortis mauris est. Nunc convallis ultrices augue vitae bibendum. Aenean non diam lacus. Quisque quis dui at erat viverra dictum. Vestibulum dolor risus, iaculis non laoreet vitae, laoreet nec ante. Integer rutrum sagittis lacus et vulputate. Nulla id fringilla tortor. Proin commodo nisl ut tortor tempor condimentum quis ut nibh. Sed lacinia sapien dolor, dapibus semper arcu.

Nulla facilisi. Aenean interdum porta nisl non blandit. Nunc interdum faucibus nisi, eu iaculis libero consequat consectetur. Pellentesque tempus, neque facilisis tristique porta, dolor nisi elementum sapien, id auctor sem justo vel metus. Nulla non mi sit amet est imperdiet accumsan id ac odio. Aliquam semper lacinia lorem, eu sollicitudin libero ultricies vel. Proin a ultricies elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; In pretium lorem in leo pretium non convallis magna ullamcorper. In bibendum erat a sem iaculis sit amet sollicitudin enim molestie. Vestibulum ut massa eget sem tempus varius ac in dolor. Nunc vitae nisl arcu. Fusce facilisis diam eu lectus posuere semper. Nullam metus nulla, malesuada in pharetra sit amet, dapibus sit amet enim. Etiam semper, felis quis venenatis hendrerit, mauris velit hendrerit lacus, sit amet semper arcu lacus eget mauris. Suspendisse vitae varius velit.";

    #endregion

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Layout = new VerticalLayout
        {
            PaddingLeft = 10,
            PaddingRight = 10,
            PaddingTop = 10,
            PaddingBottom = 10,
            HorizontalAlign = HorizontalAlign.Left,
            Gap = 10
        };
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Controls

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        _btnAddRect = new Button
        {
            Text = "New button",
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
            FocusEnabled = false,
            AutoRepeat = true
        };
        _btnAddRect.ButtonDown += delegate { AddButton(); };
        toolbar.AddContentChild(_btnAddRect);

        _btnAddText = new Button
                          {
                              Text = "New text",
                              FocusEnabled = false,
                              SkinClass = typeof(ImageButtonSkin),
                              Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
                              AutoRepeat = true
                          };
        _btnAddText.ButtonDown += delegate { AddTextField(); };
        toolbar.AddContentChild(_btnAddText);

        Button btnClear = new Button
        {
            Text = "Clear",
            SkinClass = typeof(ImageButtonSkin),
            Icon = Resources.Load<Texture>("Icons/cancel"),
            FocusEnabled = false
        };
        btnClear.Click += delegate
        {
            _panel.RemoveAllContentChildren();
            _count = 0;
        };
        toolbar.AddContentChild(btnClear);

        #endregion

        #region Panel

        _panel = new Panel
                     {
                         Title = "Add children to panel",
                         Icon = Resources.Load<Texture>("Icons/star"),
                         SkinClass = typeof(PanelSkin2),
                         Width = 600,
                         Height = 600,
                         Layout = new VerticalLayout
                         {
                             PaddingLeft = 10,
                             PaddingRight = 10,
                             PaddingTop = 10,
                             PaddingBottom = 10,
                             HorizontalAlign = HorizontalAlign.Left,
                             Gap = 10
                         }
                     };
        AddChild(_panel);

        CheckBox chkClip = new CheckBox
        {
            Text = "Clip and enable scrolling",
            //SkinClass = typeof(ImageButtonSkin),
            //Icon = Resources.Load<Texture>("Icons/scroll"),
            FocusEnabled = false,
            Selected = true
        };
        chkClip.Click += delegate
        {
            _panel.ContentGroup.ClipAndEnableScrolling = chkClip.Selected;
        };
        _panel.ControlBarGroup.AddChild(chkClip);

        #endregion
    }

    #region Helper

    private void AddButton()
    {
        _count++;
        Button btn = new Button
                         {
                             SkinClass = typeof(ButtonSkin5),
                             Icon = ImageLoader.Instance.Load("Icons/star_big"),
                             FocusEnabled = false,
                             Width = 200,
                             Height = 200,
                             MinWidth = 200,
                             MinHeight = 200,
                             Text = "Button " + _count,
                             Tooltip = "Resizable Button"
                         };

        btn.SetStyle("addedEffect", _tweenFactory);
        _panel.AddContentChild(btn);
    }

    private void AddTextField()
    {
        TextArea field = new TextArea
                              {
                                  Width = 300,
                                  Height = 200,
                                  MinWidth = 100,
                                  MinHeight = 100,
                                  Text = LoremIpsum,
                                  Tooltip = "Resizable TextField"
                              };

        field.SetStyle("addedEffect", _tweenFactory);
        _panel.AddContentChild(field);
    }

    #endregion

}