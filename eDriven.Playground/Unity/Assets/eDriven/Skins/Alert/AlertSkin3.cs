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
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]
    [Style(Name = "backgroundImageMode", Type = typeof(ImageMode), Default = ImageMode.Tiled)]

    [Style(Name = "showHeaderBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "headerBackgroundTexture", Type = typeof(Texture))]
    [Style(Name = "headerBackgroundImageMode", Type = typeof(ImageMode), Default = ImageMode.Tiled)]

    [Style(Name = "showContentGroupBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "contentGroupBackgroundTexture", Type = typeof(Texture))]
    [Style(Name = "contentGroupBackgroundImageMode", Type = typeof(ImageMode), Default = ImageMode.Tiled)]

    [Style(Name = "showControlBarBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "controlBarBackgroundTexture", Type = typeof(Texture))]
    [Style(Name = "controlBarBackgroundImageMode", Type = typeof(ImageMode), Default = ImageMode.Tiled)]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "headerLabelStyle", Type = typeof(GUIStyle), ProxyType = typeof(LabelStyle))]
    [Style(Name = "headerLabelColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "headerLabelFont", Type = typeof(Font))]

    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(AlertLabelStyle))]
    [Style(Name = "labelColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "labelFont", Type = typeof(Font))]

    public class AlertSkin3 : Skin
    {
        public AlertSkin3()
        {
            States = new List<State>(new[]
            {
                new State("normal"),
                new State("disabled")
            });
        }

        #region Members

        private Image _backgroundImage;
        private Image _headerBackgroundImage;
        private Image _contentGroupBackgroundImage;
        //private Image _controlBarBackgroundImage;
        
        private RectShape _border;
        
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
        ///<summary>Title label
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
            
            #region Background image

            _backgroundImage = new Image
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                AlphaBlend = true
            };
            AddChild(_backgroundImage);

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

            _headerBackgroundImage = new Image
                                    {
                                        Left = 1,
                                        Right = 1,
                                        Top = 1,
                                        Height = 50,
                                        AlphaBlend = true
                                    };
            AddChild(_headerBackgroundImage);

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

            _contentGroupBackgroundImage = new Image
                                          {
                                              Left = 6,
                                              Right = 6,
                                              Top = 50,
                                              Bottom = 50,
                                              AlphaBlend = true
                                          };
            AddChild(_contentGroupBackgroundImage);

            #endregion

            #region Content group

            ContentGroup = new VGroup
                               {
                                   Id = "contentGroup",
                                   Gap = 10,
                                   PaddingLeft = 10, PaddingRight = 10, PaddingTop = 10, PaddingBottom = 10,
                                   HorizontalAlign = HorizontalAlign.Center,
                                   /* No scroller and viewport with this skin, so using constrains for positioning */
                                   Left = 12, Right = 12, Top = 56, Bottom = 56
                               };
            AddChild(ContentGroup);

            #endregion

            #region Icon

            IconDisplay = new Image();
            ContentGroup.AddChild(IconDisplay);

            #endregion

            #region Message display

            MessageDisplay = new Label
            {
                Multiline = true,
                MaxWidth = 800,
                MaxHeight = 600,
                Color = Color.black,
            };
            MessageDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
            MessageDisplay.SetStyle("font", GetStyle("labelFont"));
            ContentGroup.AddChild(MessageDisplay);

            #endregion

            /*#region Control bar background

            _controlBarBackgroundImage = new Image
                                        {
                                            Left = 1,
                                            Right = 1,
                                            Bottom = 1,
                                            Height = 50,
                                            AlphaBlend = true
                                        };
            AddChild(_controlBarBackgroundImage);

            #endregion*/

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

            _labelGroup.Right = ToolGroup.Width + 10;

            if (null != _backgroundImage)
            {
                _backgroundImage.Visible = (bool)GetStyle("showBackground");
                _backgroundImage.Texture = (Texture)GetStyle("backgroundTexture");
                _backgroundImage.Mode = (ImageMode) GetStyle("backgroundImageMode");
            }

            if (null != _headerBackgroundImage)
            {
                _headerBackgroundImage.Visible = (bool)GetStyle("showHeaderBackground");
                _headerBackgroundImage.Texture = (Texture)GetStyle("headerBackgroundTexture");
                _headerBackgroundImage.Mode = (ImageMode)GetStyle("headerBackgroundImageMode");
            }

            if (null != _contentGroupBackgroundImage)
            {
                _contentGroupBackgroundImage.Visible = (bool)GetStyle("showContentGroupBackground");
                _contentGroupBackgroundImage.Texture = (Texture)GetStyle("contentGroupBackgroundTexture");
                _contentGroupBackgroundImage.Mode = (ImageMode)GetStyle("contentGroupBackgroundImageMode");
            }

            /*if (null != _controlBarBackgroundImage)
            {
                _controlBarBackgroundImage.Visible = (bool)GetStyle("showControlBarBackground");
                _controlBarBackgroundImage.Texture = (Texture) GetStyle("controlBarBackgroundTexture");
                _controlBarBackgroundImage.Mode = (ImageMode)GetStyle("controlBarBackgroundImageMode");
            }*/

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