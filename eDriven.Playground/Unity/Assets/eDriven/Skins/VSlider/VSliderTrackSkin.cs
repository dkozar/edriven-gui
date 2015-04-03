using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;
using SetProperty = eDriven.Gui.States.SetProperty;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Vertical slider track skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]
    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x00ff66)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0x000fff)]

    public class VSliderTrackSkin : Skin
    {
        public VSliderTrackSkin()
        {
            States = new List<State>(new[]
            {
                new State("up"),
                new State("over"),
                new State("down"),
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Alpha", 0.5f),
                    }
                },
                new State("upAndSelected"),
                new State("overAndSelected"),
                new State("downAndSelected"),
                new State("disabledAndSelected")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Alpha", 1f),
                    }
                }
            });
        }

        #region Members

        private RectShape _background;
        private Image _backgroundImage;
        
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
            base.SetCurrentState(state, playTransition);

            //var st = "SetCurrentState: " + state;
            //if (playTransition)
            //    st += " [transition]";
            //Debug.Log(st);

            /* Color */
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
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, state == "over" ? GetStyle("backgroundOverColor") : GetStyle("backgroundOverAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                            )
                        ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor =(Color)GetStyle("backgroundOverColor");
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

            _backgroundImage = new Image
            {
                //Id = "background_image",
                Left = 1,
                Right = 1,
                Top = 1,
                Bottom = 1,
                Visible = false,
                Mode = ImageMode.Tiled,
                AdjustWidthToTexture = false,
                AdjustHeightToTexture = false
            };
            AddChild(_backgroundImage);
            
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

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _backgroundImage)
            {
                Texture texture = (Texture)GetStyle("backgroundTexture");
                if (null != texture)
                {
                    _backgroundImage.Texture = texture;
                    _backgroundImage.Visible = true;
                    //_background.Visible = false;
                }
                else
                {
                    //_background.Visible = true;
                }
            }

            base.UpdateDisplayList(width, height);
        }
    }
}