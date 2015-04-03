using System.Collections.Generic;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Alert skin
    /// </summary>

    [HostComponent(typeof(AlertInstance))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0x439dde)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "showHeaderBackground", Type = typeof(bool), Default = false)]
    [Style(Name = "headerBackgroundColor", Type = typeof(Color), Default = 0xdadada)]
    
    [Style(Name = "headerLabelStyle", Type = typeof(GUIStyle), ProxyType = typeof(LabelStyle))]
    [Style(Name = "headerLabelColor", Type = typeof(Color), Default = 0xffffff)]

    [Style(Name = "showContentBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "contentBackgroundColor", Type = typeof(Color), Default = 0xfffffe)]

    [Style(Name = "showControlBarBackground", Type = typeof(bool), Default = false)]
    [Style(Name = "controlGroupBackgroundColor", Type = typeof(Color), Default = 0xdadada)]

    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(AlertLabelStyle))]
    [Style(Name = "labelColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "labelFont", Type = typeof(Font))]

    public class AlertSkin2 : Skin
    {
        public AlertSkin2()
        {
            States = new List<State>(new[]
            {
                new State("normal"),
                new State("disabled")
            });
        }

        #region Members

        private RectShape _background;
        private RectShape _border;
        private RectShape _headerBackground;
        private RectShape _contentGroupBackground;
        private RectShape _controlBarBackground;
        private Scroller _scroller;
        private HGroup _labelGroup;

        #endregion

        #region Skin parts

        /* skin part */
        ///<summary>Content group
        ///</summary>
        public Group ContentGroup;

        /* skin part */
        ///<summary>Header group
        ///</summary>
        public Group HeaderGroup;

        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Image HeaderIconDisplay;

        /* skin part */
        ///<summary>
        /// Title label
        ///</summary>
        public Label TitleDisplay;

        /* skin part */
        ///<summary>Tool group
        ///</summary>
        public Group MoveArea;

        /* skin part */
        ///<summary>Tool group
        ///</summary>
        public Group ToolGroup;

        /* skin part */
        ///<summary>Control bar group
        ///</summary>
        public Group ControlBarGroup;

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Image IconDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        /* skin part */
        ///<summary>Title label
        ///</summary>
        public Label MessageDisplay;

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Background

            _background = new RectShape
            {
                Id = "background",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            AddChild(_background);

            #endregion

            #region Border

            _border = new RectShape
            {
                Id = "overlay",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                MouseEnabled = false
            };
            AddChild(_border);

            #endregion

            #region Header background

            _headerBackground = new RectShape
            {
                Id = "headerBackground",
                //Color = (Color?)GetStyle("headerBackgroundColor"),
                Left = 1,
                Right = 1,
                Top = 1,
                Height = 50
            };
            AddChild(_headerBackground);

            #endregion

            #region Header group

            HeaderGroup = new Group
            {
                Id = "headerGroup",
                Layout = new AbsoluteLayout(),
                Left = 1,
                Right = 1,
                Top = 1,
                Height = 50,
                MouseEnabled = true
            };
            AddChild(HeaderGroup);

            #endregion

            #region Icon + label group

            _labelGroup = new HGroup
            {
                Left = 10,
                VerticalCenter = 0,
                Gap = 6,
                VerticalAlign = VerticalAlign.Middle,
                ClipAndEnableScrolling = true
            };
            HeaderGroup.AddChild(_labelGroup);

            #endregion

            #region Icon display

            HeaderIconDisplay = new Image();
            _labelGroup.AddChild(HeaderIconDisplay);

            #endregion

            #region Label display

            TitleDisplay = new Label();
            _labelGroup.AddChild(TitleDisplay);

            #endregion

            #region Move area

            MoveArea = new Group
            {
                Id = "move_area",
                Layout = new AbsoluteLayout(),
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                MouseEnabled = true
            };
            HeaderGroup.AddChild(MoveArea);

            #endregion

            #region Tools

            ToolGroup = new Group
            {
                Id = "toolGroup",
                Layout = new HorizontalLayout
                {
                    HorizontalAlign = HorizontalAlign.Right,
                    VerticalAlign = VerticalAlign.Middle,
                    Gap = 4
                },
                Right = 6,
                VerticalCenter = 0,
                MouseEnabled = true // not draggable when clicking space between buttons --- false // to be draggable on possible tools label click
            };
            HeaderGroup.AddChild(ToolGroup);

            #endregion

            #region Content background

            _contentGroupBackground = new RectShape
            {
                Id = "contentGroupBackground",
                //Color = Color.white,
                Left = 6,
                Right = 6,
                Top = 50,
                Bottom = 50
            };
            AddChild(_contentGroupBackground);

            #endregion

            #region Scroller

            _scroller = new Scroller
            {
                Left = 6,
                Right = 6,
                Top = 50,
                Bottom = 50
            };
            AddChild(_scroller);

            #endregion

            #region Content group

            ContentGroup = new HGroup
            {
                Id = "contentGroup",
                Gap = 10,
                PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle
            };
            AddChild(ContentGroup);
            _scroller.Viewport = ContentGroup;

            #endregion

            #region Icon

            IconDisplay = new Image();
            ContentGroup.AddChild(IconDisplay);

            #endregion

            #region Message display

            MessageDisplay = new Label {
                Multiline = true, 
                MaxWidth = 800,
                MaxHeight = 600,
                Color = Color.black,
            };
            MessageDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
            MessageDisplay.SetStyle("font", GetStyle("labelFont"));
            ContentGroup.AddChild(MessageDisplay);

            #endregion

            #region Control bar background

            _controlBarBackground = new RectShape
            {
                Id = "controlBarBackground",
                Left = 1,
                Right = 1,
                Bottom = 1,
                Height = 50,
                Alpha = 0.5f,
                MouseEnabled = false
            };
            AddChild(_controlBarBackground);

            #endregion

            #region Control bar

            ControlBarGroup = new Group
            {
                Id = "controlBar",
                //ClipAndEnableScrolling = true, // this introduces a child positioning bug
                Layout = new HorizontalLayout
                {
                    HorizontalAlign = HorizontalAlign.Right,
                    VerticalAlign = VerticalAlign.Bottom,
                    Gap = 4,
                    PaddingLeft = 6,
                    PaddingRight = 6,
                    PaddingTop = 6,
                    PaddingBottom = 6
                },
                ClipAndEnableScrolling = true,
                Left = 1,
                Right = 1,
                Bottom = 1,
                Height = 50,
                MouseEnabled = true // not draggable when clicking space between buttons --- false // to be draggable on possible tools label click
            };
            AddChild(ControlBarGroup);

            #endregion

        }

        protected override void UpdateDisplayList(float width, float height)
        {
            HeaderIconDisplay.Visible = HeaderIconDisplay.IncludeInLayout = null != HeaderIconDisplay.Texture;
            TitleDisplay.Visible = TitleDisplay.IncludeInLayout = !string.IsNullOrEmpty(TitleDisplay.Text);
            
            IconDisplay.Visible = IconDisplay.IncludeInLayout = null != IconDisplay.Texture;

            _labelGroup.Right = ToolGroup.Width + 10;

            if (null != _background)
            {
                _background.Visible = (bool)GetStyle("showBackground");
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
            }

            if (null != _headerBackground)
            {
                _headerBackground.Visible = (bool)GetStyle("showHeaderBackground");
                _headerBackground.BackgroundColor = (Color)GetStyle("headerBackgroundColor");
            }

            if (null != _contentGroupBackground)
            {
                _contentGroupBackground.Visible = (bool)GetStyle("showContentBackground");
                _contentGroupBackground.BackgroundColor = (Color)GetStyle("contentBackgroundColor");
            }

            if (null != _controlBarBackground)
            {
                _controlBarBackground.Visible = (bool)GetStyle("showControlBarBackground");
                _controlBarBackground.BackgroundColor = (Color)GetStyle("controlGroupBackgroundColor");
            }

            if (null != TitleDisplay)
            {
                TitleDisplay.SetStyle("labelStyle", GetStyle("headerLabelStyle"));
                TitleDisplay.SetStyle("color", GetStyle("headerLabelColor"));
            }

            if (null != _border)
            {
                _border.SetStyle("backgroundStyle", GetStyle("borderStyle"));
                _border.BackgroundColor = (Color)GetStyle("borderColor");
            }

            base.UpdateDisplayList(width, height);
        }
    }
}