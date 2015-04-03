using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    [HostComponent(typeof(DropDownList))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = false)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xdadada)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "scrollerSkin", Type = typeof(Type), Default = typeof(ScrollerSkin2))]

    public class DropDownListSkin2 : Skin
    {
        public DropDownListSkin2()
        {
            States = new List<State>(new[]
            {
                new State("normal")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_anchor", "DisplayPopup", false)
                    }
                }, 
                new State("open")
                {
                    Overrides = new List<IOverride>
                    {
                        new SetProperty("_anchor", "DisplayPopup", true)
                    }
                }, 
                new State("disabled")
            });
        }

        #region Members

        private PopUpAnchor _anchor;
        private RectShape _background;
        private RectShape _border;
        private Scroller _scroller;

        #endregion

        #region Skin parts

        // ReSharper disable MemberCanBePrivate.Global
        
        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Group DropDown;
        
        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Button OpenButton;
        
        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Label LabelDisplay;

        /* skin part */
        ///<summary>Content group
        ///</summary>
        public DataGroup DataGroup;

        // ReSharper disable MemberCanBePrivate.Global
        
        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Popup anchor

            _anchor = new PopUpAnchor
            {
                Id = "pop_up",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                PopupPosition = PopupPosition.Below,
                //PopupWidthMatchesAnchorWidth = true
            };
            AddChild(_anchor);

            #endregion

            #region DropDown

            DropDown = new Group
            {
                Id = "drop_down",
                MaxHeight= 134, MinHeight= 22,
                Width = 300,
                Height = 350
            };
            _anchor.Popup = DropDown;

            #endregion

            #region Background

            _background = new RectShape
            {
                Id = "background",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            DropDown.AddChild(_background);

            #endregion

            #region Border

            _border = new RectShape
            {
                Id = "border",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                MouseEnabled = false
            };
            DropDown.AddChild(_border);

            #endregion

            #region Scroller

            _scroller = new Scroller
            {
                Id = "scroller",
                SkinClass = EvaluateSkinClassFromStyle("scrollerSkin"),
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                MinViewportInset = 1,
                HasFocusableChildren = false,
                //Width = 300
            };
            _scroller.SetStyle("horizontalScrollPolicy", ScrollPolicy.Off);
            DropDown.AddChild(_scroller);

            #endregion
            
            #region Data group

            DataGroup = new DataGroup
            {
                Id = "data_group",
                ItemRenderer = new ItemRendererFactory<DefaultItemRenderer>(),
                Layout = new TileLayout { HorizontalGap = 0, VerticalGap = 0, RequestedRowCount = 4, RequestedColumnCount = 5 }                
            };
            _scroller.Viewport = DataGroup;

            #endregion

            #region OpenButton

            OpenButton = new Button
            {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                FocusEnabled = false,
                SkinClass = typeof(DropDownListButtonSkin)
            };
            AddChild(OpenButton);

            #endregion
            
            #region Label

            LabelDisplay = new Label
            {
                VerticalCenter = 0,
                Left = 7,
                Right = 32,
                Top = 2,
                Bottom = 2,
                Width = 75,
                MouseEnabled = false,
                Color = Color.black
            };
            AddChild(LabelDisplay);

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            _border.SetStyle("backgroundStyle", GetStyle("borderStyle"));
            _border.SetStyle("backgroundColor", GetStyle("borderColor"));
        }
    }
}