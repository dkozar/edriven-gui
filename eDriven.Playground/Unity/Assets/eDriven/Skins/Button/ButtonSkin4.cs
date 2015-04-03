using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Mappers;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x439dde)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0x1261c1/*4488fc*/)]

    [Style(Name = "backgroundUpAndSelectedColor", Type = typeof(Color), Default = 0xfff000)]
    [Style(Name = "backgroundOverAndSelectedColor", Type = typeof(Color), Default = 0xfff000)]
    [Style(Name = "backgroundDownAndSelectedColor", Type = typeof(Color), Default = 0xff6600)]

    [Style(Name = "labelUpColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "labelOverColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "labelDownColor", Type = typeof(Color), Default = 0xffffff)]
    [Style(Name = "font", Type = typeof(Font))]

    public class ButtonSkin4 : Skin
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

        #endregion

        #region States

        public ButtonSkin4()
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
            //Debug.Log(st);

            /* 1. Icon */
            switch (state)
            {
                case "up":
                case "upAndSelected":
                    if (playTransition)
                    {
                        Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Rotation"),
                            new TweenOption(TweenOptionType.Duration, 1.1f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Quad.EaseInOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Rotation")),
                            new TweenOption(TweenOptionType.EndValue, 0f),
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                        ).Play();
                    }
                    else
                    {
                        IconDisplay.Rotation = 0f;
                    }
                    break;
                case "over":
                case "overAndSelected":
                    if (playTransition)
                    {
                        Tween.New().SetOptions( // icon
                            new TweenOption(TweenOptionType.Property, "Rotation"),
                            new TweenOption(TweenOptionType.Duration, 1.1f),
                            new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Quad.EaseInOut),
                            new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Rotation")),
                            new TweenOption(TweenOptionType.EndValue, 360f),
                            new TweenOption(TweenOptionType.Target, IconDisplay)
                        ).Play();
                    }
                    else
                    {
                        IconDisplay.Rotation = 360f;
                    }
                    break;
                case "down":
                case "downAndSelected":
                    //Debug.Log(state);
                    Tween.New().SetOptions( // icon
                        new TweenOption(TweenOptionType.Property, "Scale"),
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Scale")),
                        new TweenOption(TweenOptionType.EndValue, new Vector2(1.15f, 1.15f)),
                        new TweenOption(TweenOptionType.Target, IconDisplay)
                    );
                    break;
                case "disabled":
                case "disabledAndSelected":
                    Tween.New().SetOptions( // icon
                        new TweenOption(TweenOptionType.Property, "Scale"),
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Scale")),
                        new TweenOption(TweenOptionType.EndValue, new Vector2(0, 0)),
                        new TweenOption(TweenOptionType.Target, IconDisplay)
                    );
                    break;
            }

            /* 2. Color */
            switch (state)
            {
                case "up":
                case "upAndSelected":
                    //Debug.Log("backgroundUpColor: " + GetStyle("backgroundUpColor"));
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions( // background
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue,
                                    state == "up"
                                        ? GetStyle("backgroundUpColor")
                                        : GetStyle("backgroundUpAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            ),
                            Tween.New().SetOptions( // label
                                new TweenOption(TweenOptionType.Property, "Color"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 1f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Color")),
                                new TweenOption(TweenOptionType.EndValue, GetStyle("labelUpColor")),
                                new TweenOption(TweenOptionType.Target, LabelDisplay)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color) GetStyle("backgroundUpColor");
                        LabelDisplay.Color = (Color) GetStyle("labelUpColor");
                    }
                    break;

                case "over":
                case "overAndSelected":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions( // background
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue,
                                    state == "over"
                                        ? GetStyle("backgroundOverColor")
                                        : GetStyle("backgroundOverAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            ),
                            Tween.New().SetOptions( // label
                                new TweenOption(TweenOptionType.Property, "Color"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 1f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Color")),
                                new TweenOption(TweenOptionType.EndValue, GetStyle("labelOverColor")),
                                new TweenOption(TweenOptionType.Target, LabelDisplay)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color) GetStyle("backgroundOverColor");
                        LabelDisplay.Color = (Color) GetStyle("labelOverColor");
                    }
                    break;

                case "down":
                case "downAndSelected":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions( // background
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue,
                                    GetStyle(state == "down" ? "backgroundDownColor" : "backgroundDownAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            ),
                            Tween.New().SetOptions( // color
                                new TweenOption(TweenOptionType.Property, "Color"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 1f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Color")),
                                new TweenOption(TweenOptionType.EndValue, GetStyle("labelDownColor")),
                                new TweenOption(TweenOptionType.Target, LabelDisplay)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color) GetStyle("backgroundDownColor");
                        LabelDisplay.Color = (Color) GetStyle("labelUpColor");
                        IconDisplay.Scale = new Vector2(1.15f, 1.15f);
                    }
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

            #region Icon

            IconDisplay = new Image
            {
                MouseEnabled = false
            };
            vGroup.AddChild(IconDisplay);

            #endregion

            #region Label

            LabelDisplay = new Label
            {
                MouseEnabled = false,
                Multiline = true,
                Color = (Color) GetStyle("labelUpColor")
            };
            vGroup.AddChild(LabelDisplay);

            #endregion
        }

        public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);
            if (styleProp == "backgroundUpColor")
            {
                Debug.Log("backgroundUpColor changed to: " + GetStyle("upAndSelected"));
                if (CurrentState == "up")
                {
                    _background.BackgroundColor = (Color) GetStyle("upAndSelected");
                }
            }
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            /*if (CurrentState == "up")
            {
                _background.BackgroundColor = (Color)GetStyle("upAndSelected");
            }*/
            if (null != LabelDisplay)
            {
                LabelDisplay.SetStyle("labelStyle", GetLabelStyle());
                LabelDisplay.SetStyle("font", GetStyle("font"));
                GetLabelStyle().font = (Font) GetStyle("font");
            }

            base.UpdateDisplayList(width, height);
        }

        #endregion

        #region Helper methods

        private static GUIStyle _labelStyle;

        private GUIStyle GetLabelStyle()
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
            return _labelStyle;
        }

        #endregion
    }
}