using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Core.Events;
using eDriven.Core.Util;
using eDriven.Gui;
using eDriven.Gui.Animation;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using Assets.eDriven.Demo.Components;
using UnityEngine;

public class LoadingMaskDemo : Gui
{
    protected override void OnInitialize()
    {
        base.OnInitialize();

        //LayoutDescriptor = LayoutDescriptor.Absolute;
        Layout = new AbsoluteLayout();
    }

    override protected void CreateChildren()
    {
        base.CreateChildren();

        #region Top label

        Label label = new TitleLabel { HorizontalCenter = 0, Top = 20, StyleName = "title" };
        AddChild(label);

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
            {
                "Loading Mask Demo",
                "Created with eDriven.Gui"
                //"Author: Danko Kozar"
            },
            Callback = delegate(string line) { label.Text = line; }
        }
        .Start();

        #endregion

        #region VBox

        VGroup vbox = new VGroup
                        {
                            HorizontalCenter = 0,
                            VerticalCenter = 0,
                            HorizontalAlign = HorizontalAlign.Center, 
                            Gap = 20
                        };
        AddChild(vbox);

        #endregion

        HGroup hbox = new HGroup { Gap = 20, VerticalAlign = VerticalAlign.Middle };
        vbox.AddChild(hbox);

        // 3 example mask, just for fun
        hbox.AddChild(new LoadingMaskAnimator { Width = 250, Height = 100, Message = "Loading something..." });
        hbox.AddChild(new LoadingMaskAnimator { Width = 250, Height = 150, Message = "Loading something else..." });
        hbox.AddChild(new LoadingMaskAnimator { Width = 250, Height = 200, Message = "And yet something else..." });

        //vbox.AddChild(new Spacer {Height = 40});

        hbox = new HGroup { Gap = 20, VerticalAlign = VerticalAlign.Middle };
        vbox.AddChild(hbox);

        // create 3 buttons
        CreateButton(hbox);
        CreateButton(hbox);
        CreateButton(hbox);

        Button btn = new Button
        {
            Text = @"Click to show a global mask for 3 seconds",
            Icon = Resources.Load<Texture>("IconsBig/oxyblue-address-book-new"),
            SkinClass = typeof(ButtonSkin5),
            Left = 100,
            Top = 100,
            Width = 250,
            Height = 250,
            FocusEnabled = false
        };
        btn.Click += new EventHandler(delegate
        {
            int count = 0;

            GlobalLoadingMask.Show("");

            Timer t = new Timer(1, 3) { TickOnStart = true };
            t.Tick += delegate
            {
                GlobalLoadingMask.SetMessage(string.Format("Masking... {0} seconds", count));
                count++;
            };
            t.Complete += delegate { GlobalLoadingMask.Hide(); };
            t.Start();
        });
        vbox.AddChild(btn);
    }

    private static void CreateButton(Group parent)
    {
        Button btn1 = new Button
        {
            Text = @"Click to mask me for 3 seconds :)",
            Icon = Resources.Load<Texture>("Icons/star_big"),
            SkinClass = typeof (ButtonSkin5),
            Left = 100,
            Top = 100,
            Width = 250,
            Height = 250,
            FocusEnabled = false
        };
        btn1.Click += new EventHandler(delegate
        {
            int count = 0;

            LoadingMask mask = new LoadingMask(btn1);
            
            Timer t = new Timer(1, 3) {TickOnStart = true};
            t.Tick += delegate
            {
                mask.SetMessage(string.Format("Masking... {0} seconds", count));
                count++;
            };
            t.Complete += delegate { mask.Unmask(); };
            t.Start();
        });
        parent.AddChild(btn1);
    }
}