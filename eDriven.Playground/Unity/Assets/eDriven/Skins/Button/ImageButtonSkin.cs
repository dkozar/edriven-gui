using System.Collections.Generic;
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

    // background
    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonStyle))]

    [Style(Name = "backgroundUpColor", Type = typeof(Color), Default = 0xeeeeee)]
    [Style(Name = "backgroundOverColor", Type = typeof(Color), Default = 0xf9f9f9)]
    [Style(Name = "backgroundDownColor", Type = typeof(Color), Default = 0xdadada)]
    [Style(Name = "backgroundDisabledColor", Type = typeof(Color), Default = 0xeeeeee)]

    [Style(Name = "backgroundUpAndSelectedColor", Type = typeof(Color), Default = 0x439dde)]
    [Style(Name = "backgroundOverAndSelectedColor", Type = typeof(Color), Default = 0x4488fc)]
    [Style(Name = "backgroundDownAndSelectedColor", Type = typeof(Color), Default = 0x1261c1)]
    [Style(Name = "backgroundDisabledAndSelectedColor", Type = typeof(Color), Default = 0x439dde)]

    [Style(Name = "disabledAlpha", Type = typeof(float), Default = 0.6f)]

    // text
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]
    [Style(Name = "font", Type = typeof(Font))]

    // should the content be placed horizontally or vertically
    [Style(Name = "vertical", Type = typeof(bool), Default = false)]

    // padding & gap
    [Style(Name = "paddingLeft", Type = typeof(int), Default = 8)]
    [Style(Name = "paddingRight", Type = typeof(int), Default = 8)]
    [Style(Name = "paddingTop", Type = typeof(int), Default = 4)]
    [Style(Name = "paddingBottom", Type = typeof(int), Default = 4)]
    [Style(Name = "gap", Type = typeof(int), Default = 4)]

    public class ImageButtonSkin : Skin
    {
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

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Image IconDisplay;
        // ReSharper restore MemberCanBePrivate.Global

        private Group _group;

        /// <summary>
        /// 
        /// </summary>
        public ImageButtonSkin()
        {
            MinHeight = 35;
            MouseEnabled = false;
            MouseChildren = false;
        }

        public override void StylesInitialized()
        {
            base.StylesInitialized();

            // state change kick-in when the button is being mouse-overed etc.
            States = new List<State>
            {
                new State("up"),
                new State("over")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", GetStyle("backgroundOverColor"))
                        #region Just for test
                        
                        // Can set child property
                        //new SetProperty("IconDisplay", "Color", Color.red),
                        //new SetProperty("IconDisplay", "Scale", new Vector2(2f, 2f)),
                        
                        // Can access private part
                        //new SetProperty("_hGroup", "Rotation", 30)
                        
                        #endregion
                    }
                },
                new State("down") {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", GetStyle("backgroundDownColor"))
                    }
                },
                new State("disabled")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("Alpha", GetStyle("disabledAlpha")),
                        new SetStyle("backgroundColor", GetStyle("backgroundDisabledColor"))
                    }
                },
                new State("upAndSelected") {
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("textColor", Color.white),
                        new SetStyle("backgroundColor", GetStyle("backgroundUpAndSelectedColor"))
                    }
                }, 
                new State("overAndSelected") {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", GetStyle("backgroundOverAndSelectedColor"))
                    }
                }, 
                new State("downAndSelected") {
                    BasedOn = "upAndSelected",
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", GetStyle("backgroundDownAndSelectedColor"))
                    }
                }, 
                new State("disabledAndSelected")
                {
                    BasedOn = "disabled",
                    Overrides = new List<IOverride>
                    {
                        new SetStyle("backgroundColor", GetStyle("backgroundDisabledAndSelectedColor"))
                    }
                }
            };
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Background

            _background = new RectShape { Left = 0, Right = 0, Top = 0, Bottom = 0 };
            _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle"));
            AddChild(_background);

            #endregion

            _group = new Group
            {
                Left = 0, Right = 0, Top = 0, Bottom = 0,
                Layout = GetLayout((bool)GetStyle("vertical"))
            };
            AddChild(_group);

            #region Icon

            IconDisplay = new Image
            {
                MouseEnabled = false
            };
            _group.AddChild(IconDisplay);

            #endregion

            #region Label

            LabelDisplay = new Label();
            _group.AddChild(LabelDisplay);

            #endregion
        }

        public override void StyleChanged(string styleProp)
        {
            base.StyleChanged(styleProp);

            if (null != styleProp && styleProp.StartsWith("padding"))
                InvalidateDisplayList(); // we should update display
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            IconDisplay.Visible = IconDisplay.IncludeInLayout = null != IconDisplay.Texture;
            LabelDisplay.Visible = LabelDisplay.IncludeInLayout = !string.IsNullOrEmpty(LabelDisplay.Text);

            if (null != _background)
            {
                _background.Visible = (bool)GetStyle("showBackground");
                object color;
                // kicks-in with the total style rebuild (when resizing the screen etc.)
                // important: beware of the alpha for colors set in style declaration (the alpha tends to be 0f)
                switch (CurrentState)
                {
                    //case "up":
                    default:
                        color = GetStyle("backgroundUpColor");
                        break;
                    case "over":
                        color = GetStyle("backgroundOverColor");
                        break;
                    case "down":
                        color = GetStyle("backgroundDownColor");
                        break;
                    case "disabled":
                        color = GetStyle("backgroundDisabledColor");
                        break;
                    case "upAndSelected":
                        color = GetStyle("backgroundUpAndSelectedColor");
                        break;
                    case "overAndSelected":
                        color = GetStyle("backgroundOverAndSelectedColor");
                        break;
                    case "downAndSelected":
                        color = GetStyle("backgroundDownAndSelectedColor");
                        break;
                    case "disabledAndSelected":
                        color = GetStyle("backgroundDisabledAndSelectedColor");
                        break;
                }
                _background.BackgroundColor = (Color)color;
            }

            if (null != LabelDisplay) {
                /* Note: creating the "labelStyle" prior to applying a font, because we don't want to change it globally */
                LabelDisplay.SetStyle("labelStyle", GetLabelStyle());
                LabelDisplay.SetStyle("font", GetStyle("font"));
                LabelDisplay.SetStyle("color", GetStyle("textColor"));
            }

            _group.Layout = GetLayout((bool) GetStyle("vertical"));

            base.UpdateDisplayList(width, height);
        }

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

        private HorizontalLayout _horizCached;
        private VerticalLayout _vertCached;

        private LayoutBase GetLayout(bool isVertical)
        {
            if (isVertical)
            {
                if (null == _vertCached) // create once
                {
                    _vertCached = new VerticalLayout
                    {
                        HorizontalAlign = HorizontalAlign.Center,
                        VerticalAlign = VerticalAlign.Middle
                    };
                }
                _vertCached.Gap = (int)GetStyle("gap"); /*Gap = (IconDisplay.Visible && null != LabelDisplay && !string.IsNullOrEmpty(LabelDisplay.Text)) ?  (int)GetStyle("gap") : 0*/
                _vertCached.PaddingLeft = (int)GetStyle("paddingLeft");
                _vertCached.PaddingRight = (int)GetStyle("paddingRight");
                _vertCached.PaddingTop = (int)GetStyle("paddingTop");
                _vertCached.PaddingBottom = (int)GetStyle("paddingBottom");
                return _vertCached;
            }

            if (null == _horizCached) // create once
            {
                _horizCached = new HorizontalLayout
                {
                    HorizontalAlign = HorizontalAlign.Center,
                    VerticalAlign = VerticalAlign.Middle
                };
            }
            _horizCached.Gap = (int)GetStyle("gap");
            _horizCached.PaddingLeft = (int)GetStyle("paddingLeft");
            _horizCached.PaddingRight = (int)GetStyle("paddingRight");
            _horizCached.PaddingTop = (int)GetStyle("paddingTop");
            _horizCached.PaddingBottom = (int)GetStyle("paddingBottom");
            return _horizCached;
        }

        #endregion

    }
}