using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// Panel skin
    /// </summary>
     
    [HostComponent(typeof(Window))]

    /**
     * The idea here is to have a separate style mapper for a skin
     * This style mapper should be tightly bound to the skin and provide resources (textures etc.)
     * Since the skin first looks upon a particular style at its cache, and then at the Owner component's
     * values loaded via the skin style proxy are overriding those from the component itself
     * Also, since it these styles are tied to the skin, only one instance of a mapper is needed (no IDs)
     * If no mapper exists in the scene, we should provide defaults
     * Also, there's no inheritance needed (the mapper doesn't need to inherit anything)
     * Additionally, we could provide multiple mappers (an array of mappers)
     * At the end, the mapper (MonoBehaviour) should be bundled into a prefab, having referenced resources (textures, styles etc.)
     * If the prefab present on the screen, every panel skin should load resources from this prefab
     * It should be signalized if multiple prefabs are being present on screen
     * */
    //[StyleProxy(typeof(PanelSkinStyleProxy))] // <- notice: not a panel proxy!

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xdadada)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "showHeaderBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "headerBackgroundColor", Type = typeof(Color), Default = 0xdadada)]
    //[Style(Name = "headerBackgroundRollOverColor", Type = typeof(Color), Default = 0xff0000)]

    [Style(Name = "headerLabelStyle", Type = typeof(GUIStyle), ProxyType = typeof(LabelStyle))]
    [Style(Name = "headerLabelColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "showContentBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "contentBackgroundColor", Type = typeof(Color), Default = 0xffffff)]

    [Style(Name = "showControlBarBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "controlGroupBackgroundColor", Type = typeof(Color), Default = 0xdadada)]

    [Style(Name = "scrollerSkin", Type = typeof(Type), Default = typeof(ScrollerSkin))]

    public class WindowSkin : Skin
    {
        public WindowSkin()
        {
            States = new List<State>(new[]
            {
                new State("normal"),
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty(this, "BackgroundColor", Color.gray)
                    }
                }
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

// ReSharper disable MemberCanBePrivate.Global
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
// ReSharper disable MemberCanBePrivate.Global
        public Image HeaderIconDisplay;
// ReSharper restore MemberCanBePrivate.Global

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

// ReSharper restore MemberCanBePrivate.Global

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Background

            _background = new RectShape
                              {
                                  Id = "background",
                                  Left = 0, Right = 0, Top = 0, Bottom = 0
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

            #region Header background

            _headerBackground = new RectShape
                                    {
                                        Id = "headerBackground",
                                        Left = 1, Right = 1, Top = 1,
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

            HeaderIconDisplay = new Image
            {
                /*Left = 10,
                VerticalCenter = 0*/
            };
            _labelGroup.AddChild(HeaderIconDisplay);

            #endregion

            #region Label display

            TitleDisplay = new Label
                             {
                                 //Id = "titleLabel",
                                 //Left = 10,
                                 //VerticalCenter = 0
                             };
            //TitleLabel.SetStyle("textColor", UnityEngine.Color.white);
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
                                Layout = new HorizontalLayout {
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

            ContentGroup = new Group
                               {
                                   Id = "contentGroup",
                                   //Left = 6,
                                   //Right = 6,
                                   //Top = 50,
                                   //Bottom = 50
                               };

            /**
             * Panel: in designer, children not focusable (textarea)
             * Problem je bio u WindowSkinu:
             * ContentGroup = new Group{ Id = "contentGroup" };
             * AddChild(ContentGroup); // -> ova linija radi problem (ako se izbaci, sve je ok)
             * _scroller.Viewport = ContentGroup;
             * */

            // AddChild(ContentGroup);
            _scroller.Viewport = ContentGroup;

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
                ClipAndEnableScrolling = true,
                Layout = new HorizontalLayout
                {
                    HorizontalAlign = HorizontalAlign.Right,
                    VerticalAlign = VerticalAlign.Middle,
                    Gap = 4,
                    PaddingLeft = 6,
                    PaddingRight = 6,
                    PaddingTop = 6,
                    PaddingBottom = 6
                },
                Left = 1,
                Right = 1,
                Bottom = 1,
                Height = 50,
                MouseEnabled = true
            };
            AddChild(ControlBarGroup);

            #endregion

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            HeaderIconDisplay.Visible = HeaderIconDisplay.IncludeInLayout = null != HeaderIconDisplay.Texture;
            TitleDisplay.Visible = TitleDisplay.IncludeInLayout = !string.IsNullOrEmpty(TitleDisplay.Text);

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

            if (null != TitleDisplay) {
                TitleDisplay.SetStyle("labelStyle", GetStyle("headerLabelStyle"));
                TitleDisplay.SetStyle("color", GetStyle("headerLabelColor"));
            }

            if (null != _scroller)
            {
                _scroller.SkinClass = EvaluateSkinClassFromStyle("scrollerSkin");
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