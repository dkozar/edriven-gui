using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Layout;
using eDriven.Gui.Mappers;
using eDriven.Gui.Reflection;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;
using SetProperty = eDriven.Gui.States.SetProperty;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// Button skin having 8 background images for 8 states
    /// </summary>
    [HostComponent(typeof(Button))]

    [Style(Name = "upTexture", Type = typeof(Texture))]
    [Style(Name = "overTexture", Type = typeof(Texture))]
    [Style(Name = "downTexture", Type = typeof(Texture))]
    [Style(Name = "disabledTexture", Type = typeof(Texture))]

    [Style(Name = "upAndSelectedTexture", Type = typeof(Texture))]
    [Style(Name = "overAndSelectedTexture", Type = typeof(Texture))]
    [Style(Name = "downAndSelectedTexture", Type = typeof(Texture))]
    [Style(Name = "disabledAndSelectedTexture", Type = typeof(Texture))]

    [Style(Name = "labelUpColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "labelOverColor", Type = typeof(Color), Default = 0x222222)]
    [Style(Name = "labelDownColor", Type = typeof(Color), Default = 0xffffff)]

    [Style(Name = "font", Type = typeof(Font))]

    public class ButtonSkin6 : Skin
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

        //private RectShape _background;
        private Image _backgroundImage;

        #endregion

        public ButtonSkin6()
        {
            MouseEnabled = false;
            MouseChildren = false;
        }

        public override void StylesInitialized()
        {
            base.StylesInitialized();

            /* Note: We are initilizing states here and not in the constructor
             * We are doing so because we are using styles here (calling GetStyle() method) and thus have to wait until styles are loaded from the stylesheet.
             * If we've been using the default styles only (from class attributes), we could have initialize states in the constructor
             */
            States = new List<State>(new[]
            {
                new State("up"){
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("upTexture"))
                    }
                }, 
                new State("over")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("overTexture"))
                    }
                },
                new State("down") {
                    //BasedOn = "up",
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("downTexture"))
                    }
                },
                new State("disabled") {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("disabledTexture"))
                    }
                },
                new State("upAndSelected") { 
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("upAndSelectedTexture"))
                    }
                },
                new State("overAndSelected") { 
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("overAndSelectedTexture"))
                    }
                },
                new State("downAndSelected") { 
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("downAndSelectedTexture"))
                    }
                },
                new State("disabledAndSelected") { 
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_backgroundImage", "Texture", GetStyle("disabledAndSelectedTexture"))
                    }
                }
            });
        }

        #region Methods

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            #region Background image

            /* just in case that no image is visible */
            /*_background = new RectShape
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            _background.SetStyle("backgroundStyle", ButtonSingleStateStyle.Instance);
            AddChild(_background);*/

            /* Tiled image */
            _backgroundImage = new Image
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                Mode = ImageMode.Tiled,
                Visible = true
            };
            AddChild(_backgroundImage);

            #endregion

            #region Icon + label group

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

            #endregion

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
        
        protected override void UpdateDisplayList(float width, float height)
        {
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

            return _labelStyle;
        }

        #endregion
    }
}