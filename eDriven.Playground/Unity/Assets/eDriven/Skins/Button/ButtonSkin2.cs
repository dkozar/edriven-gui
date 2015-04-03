using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using eDriven.Gui.Util;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Panel skin
    /// </summary>

    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonStyle))]
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonLabelStyle))]
    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]

    public class ButtonSkin2 : Skin
    {
        public ButtonSkin2()
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
        }

        #region Members

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

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            #region Background

            _background = new RectShape
                              {
                                  //Id = "background",
                                  //Color = (Color)GetStyle("backgroundColor"),
                                  Left = 0,
                                  Right = 0,
                                  Top = 0,
                                  Bottom = 0
                              };
            _background.SetStyle("backgroundStyle", ButtonStyle.Instance);
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
                MouseEnabled = false
            };
            vGroup.AddChild(LabelDisplay);

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _background)
            {
                _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle")); // ButtonStyle.Instance);
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
            }

            if (null != LabelDisplay)
            {
                LabelDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
                LabelDisplay.SetStyle("color", GetStyle("textColor"));
            }

            base.UpdateDisplayList(width, height);
        }
    }
}