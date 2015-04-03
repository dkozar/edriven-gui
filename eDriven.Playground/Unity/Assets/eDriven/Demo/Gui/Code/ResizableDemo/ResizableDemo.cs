using Assets.eDriven.Demo;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Core.Caching;
using eDriven.Core.Events;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using Assets.eDriven.Demo.Tweens;
using Component=eDriven.Gui.Components.Component;
using Event = eDriven.Core.Events.Event;
using Random = System.Random;

public class ResizableDemo : Gui
{
    #region Members

    private Group _viewport;
    private readonly TweenFactory _tweenFactory = new TweenFactory(typeof (FadeInUp));
    private readonly Random _random = new Random();
    private int _count;
    private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam non urna purus. Suspendisse tincidunt scelerisque euismod. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent ipsum elit, consectetur ac scelerisque vitae, rhoncus porta nulla. In semper placerat sem nec consectetur. Donec mi arcu, tristique at viverra eget, accumsan at erat. Nulla ut ligula nibh, sit amet consequat neque. Aliquam a turpis sem, at dictum leo. Sed ut lacinia quam. Aenean facilisis vehicula lorem a rutrum.

Nam dignissim consectetur sem, in ultricies turpis elementum at. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nullam venenatis massa id velit eleifend ultrices. Cras sed arcu nec nisi hendrerit sollicitudin vel non eros. Sed nunc eros, auctor ut laoreet et, sodales non justo. Nulla id varius tortor. Sed ac malesuada lectus. Praesent lobortis mauris est. Nunc convallis ultrices augue vitae bibendum. Aenean non diam lacus. Quisque quis dui at erat viverra dictum. Vestibulum dolor risus, iaculis non laoreet vitae, laoreet nec ante. Integer rutrum sagittis lacus et vulputate. Nulla id fringilla tortor. Proin commodo nisl ut tortor tempor condimentum quis ut nibh. Sed lacinia sapien dolor, dapibus semper arcu.

Nulla facilisi. Aenean interdum porta nisl non blandit. Nunc interdum faucibus nisi, eu iaculis libero consequat consectetur. Pellentesque tempus, neque facilisis tristique porta, dolor nisi elementum sapien, id auctor sem justo vel metus. Nulla non mi sit amet est imperdiet accumsan id ac odio. Aliquam semper lacinia lorem, eu sollicitudin libero ultricies vel. Proin a ultricies elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; In pretium lorem in leo pretium non convallis magna ullamcorper. In bibendum erat a sem iaculis sit amet sollicitudin enim molestie. Vestibulum ut massa eget sem tempus varius ac in dolor. Nunc vitae nisl arcu. Fusce facilisis diam eu lectus posuere semper. Nullam metus nulla, malesuada in pharetra sit amet, dapibus sit amet enim. Etiam semper, felis quis venenatis hendrerit, mauris velit hendrerit lacus, sit amet semper arcu lacus eget mauris. Suspendisse vitae varius velit.";
    
    #endregion

    protected override void OnInitialize()
    {
        base.OnInitialize();

        Layout = new VerticalLayout { Gap = 0 };

        AddEventListener(MouseEvent.MOUSE_DOWN, delegate(Event e)
        {
            Component doc = e.Target as Component;
            if (null != doc)
                doc.BringToFront();
        });
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Controls

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        Button btnAddRect = new Button
        {
            Text = "New button",
            Icon = ImageLoader.Instance.Load("Icons/shape_square_add"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false
        };
        btnAddRect.Click += delegate { AddButton(); };
        toolbar.AddContentChild(btnAddRect);

        Button btnAddText = new Button
        {
            Text = "New text",
            Icon = ImageLoader.Instance.Load("Icons/page_white_text"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false
        };
        btnAddText.Click += delegate { AddTextField(); };
        toolbar.AddContentChild(btnAddText);

        Button btnClear = new Button
        {
            Text = "Clear",
            Icon = ImageLoader.Instance.Load("Icons/cancel"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false
        };
        btnClear.Click += delegate
        {
            _viewport.RemoveAllChildren();
            _count = 1;
        };
        toolbar.AddContentChild(btnClear);

        #endregion

        #region Scroller

        Scroller scroller = new Scroller
        {
            SkinClass = typeof(ScrollerSkin2),
            PercentWidth = 100,
            PercentHeight = 100
        };
        //scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.On);
        //scroller.SetStyle("verticalScrollPolicy", ScrollPolicy.On);
        AddChild(scroller);

        _viewport = new Group
        {
            MouseEnabled = true,
            Layout = new AbsoluteLayout()
        };
        scroller.Viewport = _viewport;

        #endregion

        #region Default controls

        var button = new Button
        {
            X = 200,
            Y = 200,
            Width = 300,
            Height = 200,
            MinWidth = 200,
            MinHeight = 200,
            FocusEnabled = false,
            Text = "Resize me!",
            Tooltip = "Resizable Button"
        };
        button.Plugins.Add(new Resizable()/* { ShowOverlay = false }*/);
        _viewport.AddChild(button);

        var textArea = new TextArea
        {
            X = 600,
            Y = 400,
            Width = 300,
            Height = 200,
            MinWidth = 100,
            MinHeight = 100,
            Text = LoremIpsum,
            Tooltip = "Resizable TextArea",
            Optimized = true
        };
        textArea.Plugins.Add(new Resizable()/* { ShowOverlay = false }*/);
        _viewport.AddChild(textArea);

        #endregion
    }

    #region Helper

    private void AddButton()
    {
        _count++;
        Button btn = new Button
                         {
                             FocusEnabled = false,
                             Width = 200 + _random.Next((int)(Stage.Width / 2 - 200)),
                             Height = 200 + _random.Next((int)(Stage.Height / 2 - 200)),
                             MinWidth = 200,
                             MinHeight = 200,
                             Text = "Button " + _count,
                             Tooltip = "Resizable Button"
                         };
        btn.X = _random.Next((int)(Stage.Width - btn.Width));
        btn.Y = _random.Next((int)(Stage.Height - btn.Height));

        btn.Plugins.Add(new Resizable()/* { ShowOverlay = false }*/);
        btn.SetStyle("addedEffect", _tweenFactory);
        _viewport.AddChild(btn);
    }

    private void AddTextField()
    {
        var textArea = new TextArea
                              {
                                  X = _random.Next((int)(Stage.Width)),
                                  Y = _random.Next((int)(Stage.Height)),
                                  Width = 100 + _random.Next(400),
                                  Height = 100 + _random.Next(400),
                                  MinWidth = 100,
                                  MinHeight = 100,
                                  Text = LoremIpsum,
                                  Tooltip = "Resizable TextArea",
                                  Optimized = true
                              };

        textArea.SetStyle("addedEffect", _tweenFactory);
        textArea.Plugins.Add(new Resizable());
        _viewport.AddChild(textArea);
    }

    #endregion
}