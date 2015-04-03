using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Vertical slider button skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x4488fc)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0x439dde)]

    public class VSliderButtonSkin : Skin
    {
        #region Members

        private RectShape _background;

        //private RectShape _shine;

        #endregion

        #region Skin parts

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Image IconDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        public override void SetCurrentState(string state, bool playTransition)
        {
            //var st = "SetCurrentState: " + state;
            //if (playTransition)
            //    st += " [transition]";
            //Debug.Log(st);

            /* 1. Icon */
            switch (state)
            {
                case "over":
                case "overAndSelected":
                    if (playTransition)
                    {
                        //Tween.New().SetOptions(
                        //    new TweenOption(TweenOptionType.Property, "Y"),
                        //    new TweenOption(TweenOptionType.Duration, 1f),
                        //    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Quad.EaseInOut),
                        //    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Y")),
                        //    new TweenOption(TweenOptionType.EndValue, 4f),
                        //    new TweenOption(TweenOptionType.Target, IconDisplay)/*,
                        //    new TweenOption(TweenOptionType.Interpolator, new FloatInterpolator())*/
                        //).Play();
                    }
                    else
                    {
                        IconDisplay.Rotation = 360f;
                    }
                    break;
                case "up":
                case "upAndSelected":
                    if (playTransition)
                    {
                        Tween.New().SetOptions(
                            new TweenOption(TweenOptionType.Property, "Rotation"),
                            new TweenOption(TweenOptionType.Duration, 2f),
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
                case "down":
                case "downAndSelected":
                    //Debug.Log(state);
                    Tween.New().SetOptions(
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
                    Tween.New().SetOptions(
                        new TweenOption(TweenOptionType.Property, "Scale"),
                        new TweenOption(TweenOptionType.Duration, 1f),
                        new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                        new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("Scale")),
                        new TweenOption(TweenOptionType.EndValue, new Vector2(0, 0)),
                        new TweenOption(TweenOptionType.Target, IconDisplay)
                        );
                    break;
            }

            /* 2. Color */
            switch (state)
            {
                case "over":
                case "overAndSelected":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, state == "over" ? GetStyle("backgroundOverColor") : GetStyle("backgroundOverAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                                )
                            ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor =(Color) GetStyle("backgroundOverColor");
                    }
                    break;

                case "up":
                case "upAndSelected":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, state == "up" ? GetStyle("backgroundUpColor") : GetStyle("backgroundUpAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                                )
                            ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor =(Color)GetStyle("backgroundUpColor");
                    }
                    break;

                case "down":
                case "downAndSelected":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, GetStyle(state == "down" ? "backgroundDownColor" : "backgroundDownAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                                )
                            ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor =(Color)GetStyle("backgroundDownColor");
                        IconDisplay.Scale = new Vector2(1.15f, 1.15f);
                    }
                    break;
            }
        } 

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
                                  Bottom = 0
                              };
            _background.SetStyle("backgroundStyle", ButtonSingleStateStyle.Instance);
            AddChild(_background);

            #endregion

            #region Icon

            IconDisplay = new Image
                              {
                                  HorizontalCenter = 0f,
                                  VerticalCenter = 0f,
                                  MouseEnabled = false
                              };
            AddChild(IconDisplay);

            #endregion
        }
    }
}