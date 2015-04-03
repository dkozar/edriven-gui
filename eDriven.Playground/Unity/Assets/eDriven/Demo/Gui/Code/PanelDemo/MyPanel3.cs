using System.Collections.Generic;
using Assets.eDriven.Skins;
using eDriven.Core.Caching;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace Assets.eDriven.Demo.Gui.Code.PanelDemo
{
// ReSharper disable UnusedMember.Global
    public class MyPanel3 : Panel
// ReSharper restore UnusedMember.Global
    {
        public MyPanel3()
        {
            Id = "ct";
            Title = "Panel with vertical scrollbar";
        }

        private Group _view;
        private VScrollBar _vScrollBar;
        
        override protected void CreateChildren()
        {
            base.CreateChildren();

            Button button;

            #region View + vertical scrollbar

            _view = new VGroup
                              {
                                  Id = "view",
                                  ClipAndEnableScrolling = true,
                                  Left = 0, /*Right = 16, */Top = 0, Bottom = 0,
                                  PaddingLeft = 10,
                                  PaddingRight = 10,
                                  PaddingTop = 10,
                                  PaddingBottom = 10,
                                  Gap = 6,
                                  StopMouseWheelPropagation = true
                              };
            AddContentChild(_view);

            _vScrollBar = new VScrollBar
            {
                Left = null,
                Right = 0,
                Top = 0,
                Bottom = 0,
                Viewport = _view,
                SkinClass = typeof(VScrollBarSkin3)
            };
            AddContentChild(_vScrollBar);

            //_vScrollBar.ValidateNow();
            //_view.Right = _vScrollBar.MeasuredWidth;

            #endregion

            for (int i = 0; i < 14; i++)
            {
                button = new Button
                             {
                                 Text = "Button " + (i + 1),
                                 SkinClass = typeof(ImageButtonSkin),
                                 FocusEnabled = false,
                                 Icon = ImageLoader.Instance.Load("Icons/shape_square_add")
                             };
                //button.Click += delegate { AddButton(); };
                _view.AddChild(button);
            }

            List<string> icons = new List<string> { "maximize", "cancel" };
            for (int i = 0; i < icons.Count; i++)
            {
                button = new Button
                             {
                                 FocusEnabled = false,
                                 SkinClass = typeof(ImageButtonSkin),
                                 Icon = ImageLoader.Instance.Load("Icons/" + icons[i])
                             };
                button.Click += delegate(Event e) { Debug.Log("Clicked: " + e.Target); };
                ToolGroup.AddChild(button);
            }

            button = new Button
            {
                Text = "Clip",
                SkinClass = typeof(ImageButtonSkin),
                Icon = ImageLoader.Instance.Load("Icons/color_swatch"),
                ToggleMode = true,
                FocusEnabled = false,
                Selected = true
            };
            button.Change += delegate(Event e)
                                 {
                                     Button btn = (Button) e.Target;
                                     //Debug.Log("Selected: " + btn.Selected);
                                     _view.ClipAndEnableScrolling = btn.Selected;
                                 };
            ControlBarGroup.AddChild(button);

            icons = new List<string> { "lock", "star", "accept" };
            for (int i = 0; i < 3; i++)
            {
                button = new Button
                             {
                                 Text = UppercaseFirst(icons[i]),
                                 SkinClass = typeof(ImageButtonSkin),
                                 FocusEnabled = false,
                                 Icon = ImageLoader.Instance.Load("Icons/" + icons[i])
                             };
                //button.Click += delegate { AddButton(); };
                ControlBarGroup.AddChild(button);
            }
        }

        private bool _viewLayoutChanged;
        private LayoutBase _viewLayout;
// ReSharper disable UnusedMember.Global
        public LayoutBase ViewLayout
// ReSharper restore UnusedMember.Global
        {
            get { 
                return _viewLayout;
            }
            set
            {
                if (value == _viewLayout)
                    return;

                _viewLayout = value;
                _viewLayoutChanged = true;
                InvalidateProperties();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_viewLayoutChanged)
            {
                _viewLayoutChanged = false;
                _view.Layout = _viewLayout;
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            _view.Right = _vScrollBar.MeasuredWidth;
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