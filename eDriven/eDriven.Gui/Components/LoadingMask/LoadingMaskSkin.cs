using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Loading mask skin
    /// </summary>
    [HostComponent(typeof(LoadingMaskAnimator))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskBackgroundStyle))]
    
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "backgroundAlpha", Type = typeof(float), Default = 0.7f)]

    [Style(Name = "boxShowBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "boxBackgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskBoxStyle))]

    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskLabelStyle))]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]
    
    public class LoadingMaskSkin : Skin
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LoadingMaskSkin()
        {
            States = new List<State>
            {
                new State("up"), 
                new State("over"),
                new State("down"),
                new State("disabled"),
                new State("upAndSelected"), 
                new State("overAndSelected"), 
                new State("downAndSelected"),
                new State("disabledAndSelected")
            };
        }

        #region Members

        private RectShape _background;
        private RectShape _boxBg;

        #endregion

        #region Skin parts

        /* skin part */
        ///<summary>Label display
        ///</summary>
        // ReSharper disable MemberCanBePrivate.Global
        public Image IconDisplay;
        // ReSharper restore MemberCanBePrivate.Global

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
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0,
                Alpha = 0.5f
            };
            //_background.SetStyle("backgroundStyle", ButtonStyle.Instance);
            AddChild(_background);

            #endregion

            #region Box

            Group box = new Group
            {
                HorizontalCenter = 0,
                VerticalCenter = 0
            };
            AddChild(box);

            _boxBg = new RectShape
            {
                Left = 0, Right = 0, Top = 0, Bottom = 0,
                HorizontalCenter = 0,
                VerticalCenter = 0,
                /*MinWidth = 300,
                MinHeight = 100*/
            };
            //_background.SetStyle("backgroundStyle", ButtonStyle.Instance);
            _background.SetStyle("backgroundColor", Color.blue);
            box.AddChild(_boxBg);

            #endregion

            HGroup hGroup = new HGroup
            {
                Left = 10, Right = 10, Top = 10, Bottom = 10,
                HorizontalCenter = 0,
                VerticalCenter = 0,
                VerticalAlign = VerticalAlign.Middle
            };
            box.AddChild(hGroup);

            #region Icon

            IconDisplay = new Image {
                MouseEnabled = false
            };
            hGroup.AddChild(IconDisplay);

            #endregion

            #region Label

            LabelDisplay = new Label
                               {
                                   Text = "miki",
                                   MouseEnabled = false,
                                   //Width = 100
                               };
            hGroup.AddChild(LabelDisplay);

            #endregion
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _background)
            {
                _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle")); // ButtonStyle.Instance);
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
                _background.Alpha = (float)GetStyle("backgroundAlpha");
            }

            if (null != _boxBg)
            {
                _boxBg.Visible = (bool) GetStyle("boxShowBackground");
                _boxBg.SetStyle("backgroundStyle", GetStyle("boxBackgroundStyle"));
            }

            if (null != LabelDisplay) {
                LabelDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
                LabelDisplay.Color = (Color)GetStyle("textColor");
            }

            base.UpdateDisplayList(width, height);
        }
    }
}