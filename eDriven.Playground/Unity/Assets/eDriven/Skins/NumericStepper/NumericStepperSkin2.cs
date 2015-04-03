using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    [HostComponent(typeof(NumericStepper))]

    [Style(Name = "textFieldStyle", Type = typeof(GUIStyle), ProxyType = typeof(NumericStepperTextFieldStyle))]
    [Style(Name = "font", Type = typeof(Font))]
    [Style(Name = "upTextColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "overTextColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "downTextColor", Type = typeof(Color), Default = 0x333333)]

    public class NumericStepperSkin2 : Skin
    {
        public NumericStepperSkin2()
        {
            MinWidth = 80;
            MinHeight = 80;
        }

        /**
         * Skin parts
         * */

#pragma warning disable 1591
        // ReSharper disable MemberCanBePrivate.Global

        public Button IncrementButton;

        public Button DecrementButton;

        public TextField TextDisplay;

        // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 1591

        protected override void CreateChildren()
        {
            base.CreateChildren();

            // Create the down-arrow button, layered above the track.
            if (null == IncrementButton)
            {
                IncrementButton = new Button
                                      {
                                          Id = "increment",
                                          SkinClass = typeof(NumericStepperButtonSkin),
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          Right = 0,
                                          Top = 0,
                                          Width = 40,
                                          PercentHeight = 50,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow-090")
                                      };
                AddChild(IncrementButton);
            }

            // Create the up-arrow button, layered above the track.
            if (null == DecrementButton)
            {
                DecrementButton = new Button
                                      {
                                          Id = "decrement",
                                          AutoRepeat = true,
                                          FocusEnabled = false,
                                          SkinClass = typeof(NumericStepperButtonSkin),
                                          Right = 0,
                                          Bottom = 0,
                                          Width = 40,
                                          PercentHeight = 50,
                                          Icon = (Texture2D)Resources.Load("eDriven/Editor/Icons/arrow-270")
                                      };
                AddChild(DecrementButton);
            }

            if (null == TextDisplay)
            {
                TextDisplay = new TextField
                                  {
                                      Id = "textDisplay",
                                      Left = 0,
                                      Top = 0,
                                      Bottom = 0,
                                      Right = 40,
                                      MinWidth = 30
                                  };
                TextDisplay.SetStyle("textFieldStyle", NumericStepperTextFieldStyle.Instance);
                AddChild(TextDisplay);
            }
        }

        //public override void StyleChanged(string styleName, object s)
        //{
        //    base.StyleChanged(styleName, s);

        //    switch (styleName)
        //    {
        //        case "textFieldStyle":
        //            if (null != TextDisplay)
        //                TextDisplay.SetStyle("textFieldStyle", new GUIStyle((GUIStyle)s));
        //            break;
        //            //case "font":
        //            //    if (null != TextDisplay)
        //            //        TextDisplay.SetStyle("font", s);
        //            //    break;
        //    }
        //}

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            if (null != TextDisplay)
            {
                //TextDisplay.SetStyle("textFieldStyle", GetStyle("textFieldStyle"));
                //TextDisplay.SetStyle("textColor", GetStyle("textColor"));
                ////TextDisplay.SetStyle("labelStyle", GetLabelStyle());
                //if (null != GetStyle("font"))
                //{
                //    /* We have to clone the GUIStyle because we don't want all the numeric steppers to use this font */
                //    TextDisplay.SetStyle("textFieldStyle", new GUIStyle((GUIStyle)GetStyle("textFieldStyle"))); // CLONE! (TEMP)
                //    TextDisplay.SetStyle("font", GetStyle("font"));
                //}
                if (null != GetStyle("font"))
                    TextDisplay.SetStyle("font", GetStyle("font"));

                TextDisplay.SetStyle("upTextColor", GetStyle("upTextColor"));
                TextDisplay.SetStyle("overTextColor", GetStyle("overTextColor"));
                TextDisplay.SetStyle("downTextColor", GetStyle("downTextColor"));
            }
        }

        public override void SetFocus()
        {
            base.SetFocus();

            if (null != TextDisplay)
                TextDisplay.SetFocus();
        }
    }
}