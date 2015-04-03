using Assets.eDriven.Demo.Gui.Code.AbsoluteLayout2;
using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Components;
using Assets.eDriven.Demo.Styles;

public class AbsoluteLayoutDemo2 : Gui
{
    protected override void OnInitialize()
    {
        base.OnInitialize();

        // not really needed (this is the default layout)
        Layout = new AbsoluteLayout();
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Top label

        Label label = new TitleLabel {Left = 20, Top = 20, StyleName = "title"};
        AddChild(label);

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
            {
                "Absolute Layout Demo 2",
                "Created with eDriven.Gui"/*,
                "Author: Danko Kozar"*/
            },
            Callback = delegate(string line) { label.Text = line; }
        }
            .Start();

        #endregion

        #region Bottom label

        Label label2 = new Label
        {
            HorizontalCenter = 0,
            Bottom = 20,
            Text = "[GuiInspector: Ctrl + click = sticky, Ctrl + Return toggles inspector, Ctrl + arrows for walking the DOM]"
        };
        label2.SetStyle("labelStyle", BlueLabelStyle.Instance);
        AddChild(label2);

        #endregion

        Button btn1 = new Button
        {
            Left = 100,
            Top = 100,
            Width = 300,
            Height = 300,
            FocusEnabled = false,
            Text = @"Left = 100,
Top = 100,
Width = 300,
Height = 300",
            Tooltip = @"This is the tooltip
Nice, eh? :)",
            SkinClass = typeof(ButtonSkin5),
            Icon = ImageLoader.Instance.Load("Icons/star_big")
        };
        AddChild(btn1);

        Button btn2 = new Button
        {
            Left = 500,
            Right = 100,
            Top = 100,
            MinWidth = 300,
            Height = 300,
            FocusEnabled = false,
            Text = @"Left = 400,
Right = 100,
Top = 100,
MinWidth = 300,
Height = 300",
            Tooltip = @"This is the tooltip of the second button
(this one is constrained in width, so resizes with screen width)",
            SkinClass = typeof(ButtonSkin5),
            Icon = ImageLoader.Instance.Load("Icons/star_big")
        };
        AddChild(btn2);

        Button btn3 = new Button
        {
            Left = 100,
            Top = 500,
            Bottom = 100,
            Width = 300,
            MinHeight = 300,
            FocusEnabled = false,
            Text = @"Left = 100,
Top = 500,
Bottom = 100,
Width = 300,
MinHeight = 300",
            Tooltip = @"This is the tooltip of the third button
(this one is constrained in height, so resizes with screen height)",
            SkinClass = typeof(ButtonSkin5),
            Icon = ImageLoader.Instance.Load("Icons/star_big")
        };
        AddChild(btn3);

        Stuff btn4 = new Stuff
        {
            Left = 500,
            Right = 100,
            Top = 500,
            Bottom = 100,
            MinWidth = 300,
            MinHeight = 300
        };
        AddChild(btn4);
    }
}