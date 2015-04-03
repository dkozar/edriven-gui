using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>
     
    [HostComponent(typeof(Panel))]

    #region Style metadata

    [Style(Name = "showBackground", Type = typeof (bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof (Color), Default = 0xffffff)]
    [Style(Name = "backgroundTexture", Type = typeof (Texture))]

    [Style(Name = "showHeaderBackground", Type = typeof (bool), Default = true)]
    [Style(Name = "headerBackgroundColor", Type = typeof (Color), Default = 0x4488fc)]
    [Style(Name = "headerBackgroundRollOverColor", Type = typeof (Color), Default = 0xff0000)]

    [Style(Name = "headerLabelStyle", Type = typeof (GUIStyle), ProxyType = typeof (LabelStyle))]
    [Style(Name = "headerLabelColor", Type = typeof (Color), Default = 0xffffff)]

    [Style(Name = "showContentBackground", Type = typeof (bool), Default = true)]
    [Style(Name = "contentBackgroundColor", Type = typeof (Color), Default = 0xffffff)]

    [Style(Name = "showControlBarBackground", Type = typeof (bool), Default = true)]
    [Style(Name = "controlGroupBackgroundColor", Type = typeof (Color), Default = 0x4488fc)]

    [Style(Name = "scrollerSkin", Type = typeof (Type), Default = typeof (ScrollerSkin2))]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    #endregion

    public class PanelSkin3 : Skin
    {
        public PanelSkin3()
        {
            States = new List<State>(new[]
            {
                new State("normal"),
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new global::eDriven.Gui.States.SetProperty(this, "BackgroundColor", Color.gray)
                    }
                }
            });

            AddEventListener(MouseEvent.ROLL_OVER, RollOverHandler, EventPhase.Capture | EventPhase.Target);
            AddEventListener(MouseEvent.ROLL_OUT, RollOutHandler, EventPhase.Capture | EventPhase.Target);
        }

        private void RollOverHandler(Event e)
        {
            if (e.Target != this) // skin
                return;

            /*_tween = new Sequence(
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "Height"),
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseInOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                    new TweenOption(TweenOptionType.EndValue, 400f),
                    new TweenOption(TweenOptionType.Target, this)
                    ),
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "Width"),
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseInOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Width")),
                    new TweenOption(TweenOptionType.EndValue, 300f),
                    new TweenOption(TweenOptionType.Target, this)
                    )
                );
            _tween.Play();*/
        }

        private void RollOutHandler(Event e)
        {
            if (e.Target != this) // skin
                return;

            /*_tween = new Sequence(
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "Height"),
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseInOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                    new TweenOption(TweenOptionType.EndValue, 300f),
                    new TweenOption(TweenOptionType.Target, this)
                    ),
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "Width"),
                    new TweenOption(TweenOptionType.Duration, 1f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseInOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Width")),
                    new TweenOption(TweenOptionType.EndValue, 200f),
                    new TweenOption(TweenOptionType.Target, this)
                    )
                );

            _tween.Play();*/
        }

        #region Members

        private RectShape _background;
        private RectShape _headerBackground;
        private RectShape _controlBarBackground;
        private Image _backgroundImage;
        private Scroller _scroller;
        private RectShape _border;

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
        ///<summary>Title label
        ///</summary>
        public Label TitleDisplay;

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
                                  Color = UnityEngine.Color.white,
                                  Left = 0, Right = 0, Top = 0, Bottom = 0
                              };
            AddChild(_background);

            _backgroundImage = new Image
                                   {
                                       Id = "background_image",
                                       Left = 0,
                                       Right = 0,
                                       Top = 0,
                                       Bottom = 0,
                                       Visible = false,
                                       //Rotation = 30,
                                       //AspectRatio = 4,
                                       ScaleMode = ImageScaleMode.ScaleToFill
                                   };
            AddChild(_backgroundImage);

            #endregion

            #region Header background

            _headerBackground = new RectShape
                                    {
                                        Id = "headerBackground",
                                        //Color = (Color)GetStyle("headerBackgroundColor"),
                                        Left = 0, Right = 0, Bottom = 0,
                                        Height = 80
                                    };
            AddChild(_headerBackground);

            #endregion

            #region Header group

            HeaderGroup = new Group
                              {
                                  Id = "headerGroup",
                                  Layout = new AbsoluteLayout(),
                                  Left = 0,
                                  Right = 0,
                                  Bottom = 0,
                                  Height = 80
                              };
            AddChild(HeaderGroup);

            #endregion

            #region Title label

            TitleDisplay = new Label
                             {
                                 Id = "titleLabel",
                                 Left = 10,
                                 VerticalCenter = 0
                             };
            //TitleLabel.SetStyle("textColor", UnityEngine.Color.white);
            HeaderGroup.AddChild(TitleDisplay);

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

            #region Scroller

            _scroller = new Scroller
            {
                SkinClass = EvaluateSkinClassFromStyle("scrollerSkin"),
                Left = 0,
                Right = 0,
                Top = 50,
                Bottom = 80
            };
            AddChild(_scroller);

            #endregion

            #region Content group

            ContentGroup = new Group { Id = "contentGroup" };
            //AddChild(ContentGroup);
            _scroller.Viewport = ContentGroup;

            #endregion

            #region Control bar background

            _controlBarBackground = new RectShape
                                        {
                                            Id = "controlBarBackground",
                                            Left = 0,
                                            Right = 0,
                                            Top = 0,
                                            Height = 50,
                                            //Alpha = 0.5f
                                        };
            AddChild(_controlBarBackground);

            #endregion

            #region Control bar

            ControlBarGroup = new Group
                                  {
                                      Id = "controlBar",
                                      Layout = new HorizontalLayout
                                                   {
                                                       HorizontalAlign = HorizontalAlign.Right,
                                                       VerticalAlign = VerticalAlign.Middle,
                                                       Gap = 4,
                                                       PaddingLeft = 6, PaddingRight = 6, PaddingTop = 6, PaddingBottom = 6
                                                   },
                                      Left = 0,
                                      Right = 0,
                                      Top = 0,
                                      Height = 50,
                                      MouseEnabled = true // not draggable when clicking space between buttons --- false // to be draggable on possible tools label click
                                  };
            AddChild(ControlBarGroup);

            #endregion

            #region Border

            _border = new RectShape
            {
                Id = "border",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                MouseEnabled = false
            };
            AddChild(_border);

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _backgroundImage)
            {
                _backgroundImage.Texture = (Texture) GetStyle("backgroundTexture");
                _backgroundImage.Visible = true;
            }

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