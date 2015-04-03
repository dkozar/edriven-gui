using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonStyle))]
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonLabelStyle))]
    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]
    
    public class ButtonSkin : Skin
    {
        public ButtonSkin()
        {
            MouseEnabled = false;
            MouseChildren = false;

            States = DefaultButtonStateFactory.CreateStates();
            /*States = new List<State>
            {
                new State("up"), 
                new State("over")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xffffff).ToColor()),
                    }
                }, 
                new State("down")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0xcccccc).ToColor())
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
                        new SetProperty("BackgroundColor", Color.yellow),
                    }
                }, 
                new State("overAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", ColorMixer.FromHex(0xff9900).ToColor()), // orange
                    }
                }, 
                new State("downAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("BackgroundColor", ColorMixer.FromHex(0xffbb00).ToColor()) // more orange
                    }
                },
                new State("disabledAndSelected")
                {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", 0.6f)
                    }
                },
            };*/
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
                                  Left = 0, Right = 0, Top = 0, Bottom = 0
                              };
            //_background.SetStyle("backgroundStyle", ButtonStyle.Instance);
            AddChild(_background);

            #endregion

            #region Label

            LabelDisplay = new Label
                               {
                                   HorizontalCenter = 0,
                                   VerticalCenter = 0,
                                   Right = 10,
                                   Left = 10,
                                   Top = 10,
                                   Bottom = 10,
                                   MouseEnabled = false
                               };
            AddChild(LabelDisplay);

            #endregion
        }

        /*public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);

            if (styleProp == "backgroundColor")
            {
                Debug.Log("Skin, backgroundColor: " + GetStyle("backgroundColor"));
                Debug.Log("InheritingStyles: " + InheritingStyles);
                Debug.Log("NonInheritingStyles: " + NonInheritingStyles);
            }
        }*/

        protected override void UpdateDisplayList(float width, float height)
        {
            //Debug.Log("ButtonSkin backgroundColor: " + GetStyle("backgroundColor"));
            //Debug.Log("Inheriting? : " + StyleManager.Instance.InheritingStyles.Contains("backgroundStyle"));
            //Debug.Log("Inheriting: " + InheritingStyles.GetValue("backgroundStyle"));
            //Debug.Log("NonInheritingStyles: " + NonInheritingStyles.GetValue("backgroundStyle"));
            //Debug.Log("ButtonSkin backgroundStyle: " + GetStyle("backgroundStyle"));
            if (null != _background)
            {
                _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle")); // ButtonStyle.Instance);
                _background.BackgroundColor =(Color)GetStyle("backgroundColor");
            }

            if (null != LabelDisplay) {
                LabelDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
                LabelDisplay.SetStyle("color", GetStyle("textColor"));
            }

            base.UpdateDisplayList(width, height);
        }
    }
}