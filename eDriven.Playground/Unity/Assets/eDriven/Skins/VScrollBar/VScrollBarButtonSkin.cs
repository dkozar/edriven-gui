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
    /// Vertical scrollbar button skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x4488fc)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0x439dde)]

    public class VScrollBarButtonSkin : Skin
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
            
            /* Color */
            switch (state)
            {
                case "over":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, (Color)GetStyle("backgroundOverColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color)GetStyle("backgroundOverColor");
                    }
                    break;

                case "up":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, (Color)GetStyle("backgroundUpColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color)GetStyle("backgroundUpColor");
                    }
                    break;

                case "down":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                //new TweenOption(TweenOptionType.Interpolator, new ColorInterpolator()),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, GetStyle("backgroundDownColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color)GetStyle("backgroundDownColor");
                    }
                    break;
            }
        } 

        protected override void CreateChildren()
        {
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