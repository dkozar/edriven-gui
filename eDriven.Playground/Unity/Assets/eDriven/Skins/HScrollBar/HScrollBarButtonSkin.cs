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

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Horizontal scrollbar button skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0x4488fc)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0x439dde)]

    public class HScrollBarButtonSkin : Skin
    {
        public HScrollBarButtonSkin()
        {
            States = new List<State>
            {
                new State("up", new StateTable()), 
                new State("over", new StateTable()), 
                new State("down", new StateTable()), 
                new State("selected", new StateTable())
            };
        }

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
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction) Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, (Color)GetStyle(state == "over" ? "backgroundOverColor" : "backgroundOverAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                                )
                            ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color)GetStyle(state == "over" ? "backgroundOverColor" : "backgroundOverAndSelectedColor");
                    }
                    break;

                case "up":
                    if (playTransition)
                    {
                        new Parallel(
                            Tween.New().SetOptions(
                                new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                                new TweenOption(TweenOptionType.Duration, 3f),
                                new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                                new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                                new TweenOption(TweenOptionType.EndValue, (Color)GetStyle(state == "up" ? "backgroundUpColor" : "backgroundUpAndSelectedColor")),
                                new TweenOption(TweenOptionType.Target, _background)
                                )
                            ).Play();
                    }
                    else
                    {
                        _background.BackgroundColor = (Color)GetStyle(state == "up" ? "backgroundUpColor" : "backgroundUpAndSelectedColor");
                    }
                    break;

                case "down":
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
                        _background.BackgroundColor = (Color)GetStyle(state == "down" ? "backgroundDownColor" : "backgroundDownAndSelectedColor");
                        IconDisplay.Scale = new Vector2(1.15f, 1.15f);
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

            //_shine = new RectShape
            //{
            //    Left = 0,
            //    Right = 0,
            //    Top = 0,
            //    Height = GetCurrentShineHeight(),
            //    MouseEnabled = false
            //};
            //_shine.SetStyle("backgroundStyle", ButtonSingleStateStyle.Instance);
            //_shine.Color = (Color) GetStyle("shineColor");
            //AddChild(_shine);

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