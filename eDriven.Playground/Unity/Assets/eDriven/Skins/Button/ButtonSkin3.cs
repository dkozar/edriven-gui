using System.Collections.Generic;
using eDriven.Animation;
using eDriven.Animation.Easing;
using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Geom;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using eDriven.Gui.Util;
using UnityEngine;
using Event = eDriven.Core.Events.Event;
using SetProperty = eDriven.Gui.States.SetProperty;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "padding", Type = typeof(EdgeMetrics)/*, ProxyMemberName = "padding"*/)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]
    [Style(Name = "backgroundRollOverColor", Type = typeof(Color), Default = 0xff0000)]

    public class ButtonSkin3 : Skin
    {
        #region Members

        //private Image _backgroundImage;
        private RectShape _background;

        #endregion

        #region Skin parts

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Label LabelDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Image IconDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Rollovers

        public ButtonSkin3()
        {
            MouseEnabled = false;
            MouseChildren = false;

            States = new List<State>
            {
                new State("up")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xeeeeee).ToColor())
                    }
                }, 
                new State("over")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xf9f9f9).ToColor())
                    }
                }, 
                new State("down")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0xdadada).ToColor())
                    }
                },
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", 0.6f)
                    }
                },
                new State("upAndSelected")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x439dde).ToColor()),
                        new SetStyle("textColor", Color.white)
                    }
                }, 
                new State("overAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x4488fc).ToColor())
                    }
                }, 
                new State("downAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0x1261c1).ToColor())
                    }
                },
                new State("disabledAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", 0.6f)
                    }
                }
            };

            AddEventListener(MouseEvent.ROLL_OVER, RollOverHandler, EventPhase.Capture | EventPhase.Target);
            AddEventListener(MouseEvent.ROLL_OUT, RollOutHandler, EventPhase.Capture | EventPhase.Target);
        }

        private void RollOverHandler(Event e)
        {
            //Debug.Log("RollOverHandler");
            //RollOverEffect.Target = this;
            //RollOverEffect.Play(_headerBackground);
            new Parallel(
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                    new TweenOption(TweenOptionType.Duration, 3f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                    new TweenOption(TweenOptionType.EndValue, GetStyle("backgroundRollOverColor")),
                    new TweenOption(TweenOptionType.Target, _background)
                )
            ).Play();
        }

        private void RollOutHandler(Event e)
        {
            new Parallel(
                Tween.New().SetOptions(
                    new TweenOption(TweenOptionType.Property, "BackgroundColor"),
                    new TweenOption(TweenOptionType.Duration, 3f),
                    new TweenOption(TweenOptionType.Easer, (Tween.EasingFunction)Expo.EaseOut),
                    new TweenOption(TweenOptionType.StartValueReader, new PropertyReader("BackgroundColor")),
                    new TweenOption(TweenOptionType.EndValue, Color.white),
                    new TweenOption(TweenOptionType.Target, _background)
                )
            ).Play();
        }

        #endregion

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
                BackgroundColor = (Color)GetStyle("backgroundColor")
            };
            _background.SetStyle("backgroundStyle", ButtonSingleStateStyle.Instance);
            AddChild(_background);

            /*_backgroundImage = new Image
                              {
                                  Left = 0,
                                  Right = 0,
                                  Top = 0,
                                  Bottom = 0,
                                  //Visible = false,
                                  ScaleMode = ImageScaleMode.ScaleToFill
                              };
            AddChild(_backgroundImage);*/

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
                MouseEnabled = false,
                //Visible = false,
                //IncludeInLayout = false
            };
            vGroup.AddChild(IconDisplay);

            #endregion

            #region Label

            LabelDisplay = new Label
            {
                MouseEnabled = false,
                //Visible = false,
                //IncludeInLayout = false
            };
            vGroup.AddChild(LabelDisplay);

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            /*if (null != _backgroundImage)
            {
                Texture texture = (Texture)GetStyle("backgroundTexture");
                if (null != texture)
                {
                    _backgroundImage.Texture = texture;
                    //_backgroundImage.Visible = true;
                    //_backgroundImage.IncludeInLayout = true;
                }
                _backgroundImage.Color = (Color) GetStyle("backgroundColor");
            }*/

            if (null != _background)
            {
                _background.Color = (Color)GetStyle("backgroundColor");
            }

            if (null != LabelDisplay)
            {
                LabelDisplay.SetStyle("color", GetStyle("textColor"));
            }

            base.UpdateDisplayList(width, height);
        }
    }
}