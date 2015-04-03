using System.Collections;
using Assets.eDriven.Demo._shared.Code.Util;
using Assets.eDriven.Skins;
using eDriven.Audio;
using eDriven.Core.Util;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using eDriven.Gui.Managers;
using Assets.eDriven.Demo.Components;
using UnityEngine;

public class CursorDemo : Gui
{
    private int _selectedButton;

    override protected void CreateChildren()
    {
        base.CreateChildren();

        TitleLabel titleLabel = new TitleLabel
        {
            StyleName = "title",
            HorizontalCenter = 0,
            Top = 20
        };
        AddChild(titleLabel);

        new TextRotator
        {
            Delay = 5, // 5 seconds delay
            Lines = new[]
                        {
                            "Cursor Demo", 
                            "Created with eDriven.Gui",
                            //"Author: Danko Kozar",
                            "Cursor packages are loaded dynamically",
                            "You can set the priority to each cursor operation"
                        },
            Callback = delegate(string line) { titleLabel.Text = line; }
        }
        .Start();

        var buttonStyles = new Hashtable
                                      {
                                          {"cursor", "pointer"}
                                      };
        #region VBox

        var vbox = new VGroup
                        {
                            HorizontalCenter = 0,
                            VerticalCenter = 0,
                            Gap = 10
                        };
        AddChild(vbox);

        #endregion

        vbox.AddChild(new Label { Text = "Load cursor package:" });

        #region HBox

        var hbox = new HGroup();
        /*hbox.Click += delegate(Event e)
        {
            var me = (MouseEvent)e;
            if (me.Target is Button)
                AudioPlayerMapper.GetDefault().PlaySound("button_click");
        };*/
        vbox.AddChild(hbox);

        #endregion

        #region Cursor package 1

        var btnCursor1 = new Button
        {
            Text = "Load cursor package 1",
            Icon = (Texture)Resources.Load("Icons/drive_disk"),
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Styles = buttonStyles,
            MinWidth = 200, 
            MinHeight = 120,
            ToggleMode = true,
            Selected = true
        };
        hbox.AddChild(btnCursor1);

        #endregion

        #region Cursor package 2

        var btnCursor2 = new Button
        {
            Text = "Load cursor package 2",
            Icon = (Texture)Resources.Load("Icons/drive_disk"),
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Styles = buttonStyles,
            MinWidth = 200,
            MinHeight = 120,
            ToggleMode = true,
            Selected = false
        };
        hbox.AddChild(btnCursor2);

        // button 1 press
        btnCursor1.Click += delegate
        {
            if (_selectedButton != 0)
                AudioPlayerMapper.GetDefault().PlaySound("button_click");

            _selectedButton = 0;
            DeferManager.Instance.Defer(delegate
            {
                btnCursor1.Selected = _selectedButton == 0;
                btnCursor2.Selected = _selectedButton == 1;
                CursorManager.Instance.LoadPackage("Cursors/antialiased-classic/package");
            }, 1);
        };

        // button 2 press
        btnCursor2.Click += delegate
        {
            if (_selectedButton != 1)
                AudioPlayerMapper.GetDefault().PlaySound("button_click");

            _selectedButton = 1;
            DeferManager.Instance.Defer(delegate
            {
                btnCursor1.Selected = _selectedButton == 0;
                btnCursor2.Selected = _selectedButton == 1;
                CursorManager.Instance.LoadPackage("Cursors/blueglass-vista/package");
            }, 1);
        };
        
        #endregion

        #region Spacer

        vbox.AddChild(new Spacer {Height = 30});

        #endregion

        vbox.AddChild(new Label { Text = "Hover cursor over buttons:" });

        TileGroup tileGroup = new TileGroup { PercentWidth = 100, RequestedRowCount = 2 };
        vbox.AddChild(tileGroup);

        #region Crosshair

        var btnCrosshair = new Button
                                  {
                                      Text = "Crosshair",
                                      Icon = (Texture)Resources.Load("Icons/star"),
                                      SkinClass = typeof(ImageButtonSkin),
                                      FocusEnabled = false,
                                      Styles = new Hashtable { { "cursor", "crosshair" } },
                                      MinWidth = 200,
                                      MinHeight = 120
                                  };
        tileGroup.AddChild(btnCrosshair);

        #endregion

        #region Move

        var btnMove = new Button
        {
            Text = "Move",
            Icon = (Texture)Resources.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false,
            Styles = new Hashtable { { "cursor", "move" } },
            MinWidth = 200,
            MinHeight = 120
        };
        tileGroup.AddChild(btnMove);

        #endregion

        #region Help

        var btnHelp = new Button
        {
            Text = "Help",
            Icon = (Texture)Resources.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false,
            Styles = new Hashtable { { "cursor", "help" } },
            MinWidth = 200,
            MinHeight = 120
        };
        tileGroup.AddChild(btnHelp);

        #endregion

        #region E-Resize

        var btnEResize = new Button
        {
            Text = "E-Resize",
            Icon = (Texture)Resources.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false,
            Styles = new Hashtable { { "cursor", "e-resize" } },
            MinWidth = 200,
            MinHeight = 120
        };
        tileGroup.AddChild(btnEResize);

        #endregion

        #region Spacer

        vbox.AddChild(new Spacer { Height = 30 });

        #endregion

        vbox.AddChild(new Label { Text = "Click the button for a high-priority cursor:" });

        #region Cursor progress

        var btnProgress = new Button
        {
            Text = "Show progress cursor (5 sec)",
            Icon = (Texture)Resources.Load("Icons/star"),
            SkinClass = typeof(ImageButtonSkin),
            FocusEnabled = false,
            Styles = buttonStyles,
            MinWidth = 200,
            MinHeight = 120,
            PercentWidth = 100
        };
        btnProgress.Press += new eDriven.Core.Events.EventHandler(delegate
        {
            int id = CursorManager.Instance.SetCursor("wait", 1);
            var t = new Timer(5, 1);
            t.Complete += delegate
            {
                t.Dispose();
                CursorManager.Instance.RemoveCursor(id);
            };
            t.Start();
        });
        vbox.AddChild(btnProgress);

        #endregion
    }
}