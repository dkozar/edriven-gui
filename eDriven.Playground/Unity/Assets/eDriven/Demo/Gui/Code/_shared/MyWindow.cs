using System.Collections;
using System.Collections.Generic;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.States;
using UnityEngine;

namespace Assets.eDriven.Demo.Gui.Code
{
    public class MyWindow : Window
    {
        #region Layouts

        /* A few layouts to play with */
        private readonly HorizontalLayout _horizontalLayout = new HorizontalLayout {PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10, Gap = 10};
        private readonly VerticalLayout _verticalLayout = new VerticalLayout {PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10, Gap = 10};
        private readonly TileLayout _tileLayout = new TileLayout
        {
            HorizontalGap = 6,
            VerticalGap = 6,
            HorizontalAlign = HorizontalAlign.Center,
            VerticalAlign = VerticalAlign.Middle
        };

        #endregion

        readonly Hashtable _buttonStyles = new Hashtable {{"cursor", "pointer"}};

        readonly System.Random _random = new System.Random();

        private Button _btnHoriz;
        private Button _btnVert;
        private Button _btnTile;

        public MyWindow()
        {
            Width = 400;
            Height = 600;

            CurrentState = "tile";

            States = new List<State>(new []
            {
                new State("tile")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Layout", _tileLayout),
                        new SetProperty("BackgroundColor", Color.gray)
                    }
                },
                new State("horizontal")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Layout", _horizontalLayout),
                        new SetProperty("BackgroundColor", Color.yellow)
                        //new SetStyle("backgroundColor", Color.yellow)
                    }
                }, 
                new State("vertical")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Layout", _verticalLayout),
                        new SetProperty("BackgroundColor", Color.green)
                    }
                }
            });
        }

        override protected void CreateChildren()
        {
            base.CreateChildren();

            Button button;
            for (var i = 0; i < 14; i++)
            {
                button = new Button
                {
                    Text = "Button " + (i + 1),
                    SkinClass = typeof(ImageButtonSkin),
                    FocusEnabled = false,
                    Icon = Resources.Load<Texture>("Icons/shape_square_add"),
                    Width = (_random.Next(2) == 1) ? 400 : 200,
                    Height = (_random.Next(2) == 1) ? 200 : 150
                };
                AddContentChild(button);
            }
            
            button = new Button
            {
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/cancel"),
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            button.Click += delegate
            {
                DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
            };
            ToolGroup.AddChild(button);

            _btnHoriz = new Button
            {
                Text = "Horizontal",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                ToggleMode = true,
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            _btnHoriz.Click += delegate
            {
                //AnimateToLayout(_horizontalLayout);
                SetCurrentState("horizontal");
                Defer(HandleButtons, 1);
            };
            ControlBarGroup.AddChild(_btnHoriz);

            _btnVert = new Button
            {
                Text = "Vertical",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                ToggleMode = true,
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            _btnVert.Click += delegate
            {
                //AnimateToLayout(_verticalLayout);
                SetCurrentState("vertical");
                Defer(HandleButtons, 1);
            };
            ControlBarGroup.AddChild(_btnVert);

            _btnTile = new Button
            {
                Text = "Tile",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                ToggleMode = true,
                FocusEnabled = false,
                Styles = _buttonStyles,
                Selected = true
            };
            _btnTile.Click += delegate
            {
                SetCurrentState("tile"); //tile
                Defer(HandleButtons, 1);
            };
            ControlBarGroup.AddChild(_btnTile);

            ControlBarGroup.AddChild(new Spacer {PercentWidth = 100});

            button = new Button
            {
                Text = "Close window",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                FocusEnabled = false,
                Styles = _buttonStyles
            };
            button.Click += delegate
                                {
                                    DispatchEvent(new CloseEvent(CloseEvent.CLOSE));
                                };
            ControlBarGroup.AddChild(button);

            List<string> icons = new List<string> { "lock", "star", "accept" };
            for (int i = 0; i < 3; i++)
            {
                button = new Button
                {
                    Text = UppercaseFirst(icons[i]),
                    SkinClass = typeof(ImageButtonSkin),
                    FocusEnabled = false,
                    Styles = _buttonStyles,
                    Icon = ImageLoader.Instance.Load("Icons/" + icons[i])
                };
                ControlBarGroup.AddChild(button);
            }
        }

        private void HandleButtons(params object[] args)
        {
            _btnHoriz.Selected = CurrentState == "horizontal";
            _btnVert.Selected = CurrentState == "vertical";
            _btnTile.Selected = CurrentState == "tile";
        }

        static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}