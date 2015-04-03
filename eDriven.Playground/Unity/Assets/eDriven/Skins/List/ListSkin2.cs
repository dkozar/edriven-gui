using System;
using System.Collections.Generic;
using eDriven.Gui.Components;
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
    /// <summary>
    /// List skin
    /// </summary>

    [HostComponent(typeof(List))]

    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xdadada)]
    [Style(Name = "backgroundTexture", Type = typeof(Texture))]

    [Style(Name = "borderStyle", Type = typeof(GUIStyle), ProxyType = typeof(OnePxBorderStyle))]
    [Style(Name = "borderColor", Type = typeof(Color), Default = 0x222222)]

    [Style(Name = "scrollerSkin", Type = typeof(Type), Default = typeof(ScrollerSkin2))]

    public class ListSkin2 : Skin
    {
        public ListSkin2()
        {
            States = new List<State>(new[]
            {
                new State("normal"), 
                new State("disabled")
            });
        }

        #region Members

        private RectShape _background;
        private RectShape _border;
        private Scroller _scroller;

        #endregion

        #region Skin parts

        // ReSharper disable MemberCanBePrivate.Global
        /* skin part */
        ///<summary>Content group
        ///</summary>
        public DataGroup DataGroup;

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        protected override void CreateChildren()
        {
            base.CreateChildren();

            #region Background

            _background = new RectShape
            {
                Id = "background",
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            AddChild(_background);

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
            AddChild(_border);

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
                HasFocusableChildren = false
            };
            AddChild(_scroller);

            #endregion

            #region Data group

            DataGroup = new DataGroup
            {
                Id = "data_group",
                ItemRenderer = new ItemRendererFactory<DefaultItemRenderer>(),
                Layout = new VerticalLayout
                             {
                                 Gap = 0, HorizontalAlign = HorizontalAlign.ContentJustify, RequestedMinRowCount = 5
                             }
            };
            //AddChild(DataGroup);
            _scroller.Viewport = DataGroup;

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            //if (changed)
            //{
            //    changed = false;
            //    Debug.Log("headerBackgroundColor changed to: " + GetStyle("headerBackgroundColor"));
            //}

            if (null != _background)
            {
                _background.Visible = (bool)GetStyle("showBackground");
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
            }

            if (null != _scroller)
            {
                _scroller.SkinClass = EvaluateSkinClassFromStyle("scrollerSkin");
            }

            //if (null != _border)
            //{
            //    _border.SetStyle("backgroundStyle", GetStyle("borderStyle"));
            //    _border.BackgroundColor = (Color)GetStyle("borderColor");
            //}

            base.UpdateDisplayList(width, height);
        }
    }
}