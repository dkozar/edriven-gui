using System;
using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Animation.Interpolators;
using eDriven.Gui.Components;
using eDriven.Gui.Graphics.Base;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Mappers;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;
using Rect = eDriven.Gui.Graphics.Rect;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x00ff66)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0xeeeeee)]

    [Style(Name = "labelUpColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "labelOverColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "labelDownColor", Type = typeof(Color), Default = 0xffffff)]

    [Style(Name = "font", Type = typeof(Font))]

    [Style(Name = "shineColor", Type = typeof(Color), Default = 0x4488fc)]

    public class ButtonSkin5 : Skin
    {
        #region Skin parts

        /* skin part */
        /// <summary>
        /// Label display
        /// </summary>
        public Label LabelDisplay;
        
        /* skin part */
        /// <summary>
        /// Icon display
        /// </summary>
        public Image IconDisplay;
        
        #endregion

        #region Members

        private RectShape _background;
        private RectShape _shine;

        #endregion

        #region States

        public ButtonSkin5()
        {
            MouseEnabled = false;
            MouseChildren = false;

            States = new List<State>(new[]
            {
                new State("up"),
                new State("over"),
                new State("down"),
                new State("disabled"),
                new State("upAndSelected"),
                new State("overAndSelected"),
                new State("downAndSelected"),
                new State("disabledAndSelected")
            });
        }

        public override void SetCurrentState(string state, bool playTransition)
        {
            //var st = "SetCurrentState: " + state;
            //if (playTransition)
            //    st += " [transition]";
            //Debug.Log(state);

            /* 1. Icon */
            switch (state)
            {
                case "up":
                case "upAndSelected":
                    new Parallel(
                        Tween.New().SetOptions( // background
                            new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                            new TweenOption(TweenOptionType.Duration, 1.5f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentBgColor(state)),
                            new TweenOption(TweenOptionType.Target, _background)
                            ),
                        Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Scale"),
                            new TweenOption(TweenOptionType.Duration, 1f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Scale")),
                            new TweenOption(TweenOptionType.EndValue, new Vector2(1f, 1f)),
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                            )
                        ).Play();
                    break;
                case "over":
                case "overAndSelected":
                    new Parallel(
                        Tween.New().SetOptions( // background
                            new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                            new TweenOption(TweenOptionType.Duration, 1.5f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentBgColor(state)),
                            new TweenOption(TweenOptionType.Target, _background)
                            ),
                        Tween.New().SetOptions( // shine
                            new TweenOption(TweenOptionType.Property, "Height"),
                            new TweenOption(TweenOptionType.Duration, 1f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Bounce.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentShineHeight(state)),
                            new TweenOption(TweenOptionType.Target, _shine)
                            ),
                        Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Scale"),
                            new TweenOption(TweenOptionType.Duration, 0.6f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Scale")),
                            new TweenOption(TweenOptionType.EndValue, new Vector2(1.15f, 1.15f)),
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                            ),
                        /*Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Rotation"),
                            new TweenOption(TweenOptionType.Duration, 1.2f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Quad.EaseInOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Rotation")),
                            new TweenOption(TweenOptionType.EndValue, 0f), // note: "f" is mandatory
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                            ),*/
                        Tween.New().SetOptions( // label
                            new TweenOption(TweenOptionType.Property, "Color"),
                            new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                            new TweenOption(TweenOptionType.Duration, 2f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Color")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentLabelColor(state)),
                            new TweenOption(TweenOptionType.Target, LabelDisplay)
                            )
                        ).Play();
                    break;
                case "down":
                case "downAndSelected":
                    new Parallel(
                        Tween.New().SetOptions( // background
                            new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                            new TweenOption(TweenOptionType.Duration, 1.5f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentBgColor(state)),
                            new TweenOption(TweenOptionType.Target, _background)
                            ),
                        Tween.New().SetOptions( // shine
                            new TweenOption(TweenOptionType.Property, "Height"),
                            new TweenOption(TweenOptionType.Duration, 1f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Bounce.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Height")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentShineHeight(state)),
                            new TweenOption(TweenOptionType.Target, _shine)
                            ),
                        Tween.New().SetOptions( // label
                            new TweenOption(TweenOptionType.Property, "Color"),
                            new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                            new TweenOption(TweenOptionType.Duration, 2f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Color")),
                            new TweenOption(TweenOptionType.EndValue, GetCurrentLabelColor(state)),
                            new TweenOption(TweenOptionType.Target, LabelDisplay)
                            )/*,
                        Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Rotation"),
                            new TweenOption(TweenOptionType.Duration, 1.2f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Quad.EaseInOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Rotation")),
                            new TweenOption(TweenOptionType.EndValue, 360f),
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                            )*/
                        ).Play();
                    break;
            }
        }

        #endregion

        #region Methods

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            #region Background

            _background = new RectShape
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                BackgroundColor = (Color) GetStyle("backgroundUpColor")
            };
            _background.SetStyle("backgroundStyle", ButtonSingleStateStyle.Instance);
            AddChild(_background);

            #endregion

            #region Shine

            _shine = new RectShape
            {
                Left = 1,
                Right = 1,
                Top = 1,
                Height = GetCurrentShineHeight("up"),
                MouseEnabled = false
            };
            _shine.SetStyle("backgroundStyle", GetShineStyle());
            _shine.BackgroundColor = (Color) GetStyle("shineColor");
            AddChild(_shine);

            #endregion

            VGroup vGroup = new VGroup
            {
                Gap = 10,
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle,
                HorizontalCenter = 0,
                VerticalCenter = 0,
                Right = 10,
                Left = 10,
                Top = 10,
                Bottom = 10,
            };
            AddChild(vGroup);

            #region Label

            LabelDisplay = new Label {
                MouseEnabled = false,
                Multiline = true, 
                Color = (Color) GetStyle("labelUpColor")
            };
            vGroup.AddChild(LabelDisplay);

            #endregion

            #region Icon

            IconDisplay = new Image {MouseEnabled = false};
            vGroup.AddChild(IconDisplay);

            #endregion
        }

        /*public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);

            if (styleProp == "backgroundUpColor" && null != _background)
            {
                _background.BackgroundColor = (Color) GetStyle("backgroundUpColor");
            }

            if (styleProp == "labelUpColor" && null != LabelDisplay)
            {
                LabelDisplay.Color = (Color) GetStyle("labelUpColor");
            }
        }*/

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _shine)
            {
                _shine.Color = (Color) GetStyle("shineColor");
            }

            if (null != LabelDisplay)
            {
                LabelDisplay.SetStyle("labelStyle", GetLabelStyle());
                if (null != GetStyle("font"))
                    LabelDisplay.SetStyle("font", GetStyle("font"));
                //LabelDisplay.Color = GetCurrentLabelColor("up");
            }

            base.UpdateDisplayList(width, height);
        }

        #endregion

        #region Helper methods

        public Color GetCurrentBgColor(string state)
        {
            var color = "backgroundUpColor";
            switch (state)
            {
                case "over":
                case "overAndSelected":
                    color = "backgroundOverColor";
                    break;
                case "down":
                case "downAndSelected":
                    color = "backgroundDownColor";
                    break;
            }
            return (Color)GetStyle(color);
        }

        public Color GetCurrentLabelColor(string state)
        {
            var color = "labelUpColor";
            switch (state)
            {
                case "over":
                case "overAndSelected":
                    color = "labelOverColor";
                    break;
                case "down":
                case "downAndSelected":
                    color = "labelDownColor";
                    break;
            }
            return (Color)GetStyle(color);
        }

        private float GetCurrentShineHeight(string state)
        {
            var height = 0;
            switch (state)
            {
                case "over":
                case "overAndSelected":
                    height = 0;
                    break;
                case "down":
                case "downAndSelected":
                    height = /*200*/ Convert.ToInt32(Height - 2);
                    break;
            }
            return height;
        }

        private static GUIStyle _labelStyle;
        private static GUIStyle GetLabelStyle()
        {
            if (null == _labelStyle)
            {
                _labelStyle = new GUIStyle();

                ProgramaticStyle style = new ProgramaticStyle
                {
                    Style = _labelStyle,
                    Alignment = TextAnchor.MiddleCenter,
                    Font = FontMapper.GetWithFallback("pixel").Font,
                    NormalTextColor = Color.white
                };

                style.Commit();
            }

            _labelStyle.richText = true; // -> for rendering HTML

            return _labelStyle;
        }

        private static GUIStyle _shineStyle;
        private static GUIStyle GetShineStyle()
        {
            if (null == _shineStyle)
            {
                _shineStyle = new GUIStyle();

                ProgramaticStyle style = new ProgramaticStyle
                {
                    Style = _shineStyle,
                    Alignment = TextAnchor.MiddleCenter,
                    NormalGraphics = new Rect(1, 1, new Fill(Color.white))
                };

                style.Commit();
            }

            return _shineStyle;
        }

        #endregion
    }
}