using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins.LoadingMask
{
    /// <summary>
    /// 
    /// </summary>
    [HostComponent(typeof(LoadingMaskAnimator))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskBackgroundStyle))]
    [Style(Name = "labelStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskLabelStyle))]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0xaaaaaa)]
    [Style(Name = "backgroundAlpha", Type = typeof(float), Default = 0.7f)]

    [Style(Name = "boxShowBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "boxBackgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(LoadingMaskBoxStyle))]

    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]
    
    public class LoadingMaskSkin2 : Skin
    {
        public LoadingMaskSkin2()
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
        private RectShape _box;

        #endregion

        #region Skin parts

        // ReSharper disable MemberCanBePrivate.Global

        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Image IconDisplay;

        /* skin part */
        ///<summary>Label display
        ///</summary>
        public Label LabelDisplay;

        // ReSharper restore MemberCanBePrivate.Global

        #endregion

        protected override void CreateChildren()
        {
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
            AddChild(_background);

            #endregion

            #region Box

            _box = new RectShape
            {
                MinWidth = 300,
                MinHeight = 100,
                HorizontalCenter = 0,
                VerticalCenter = 0
            };
            AddChild(_box);

            #endregion

            HGroup hGroup = new HGroup
            {
                HorizontalCenter = 0,
                VerticalCenter = 0
            };
            AddChild(hGroup);

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
                                   Width = 100
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

            if (null != _box)
            {
                _box.Visible = (bool) GetStyle("boxShowBackground");
                _box.SetStyle("backgroundStyle", GetStyle("boxBackgroundStyle"));
            }

            if (null != LabelDisplay) {
                LabelDisplay.SetStyle("labelStyle", GetStyle("labelStyle"));
                LabelDisplay.Color = (Color)GetStyle("textColor");
            }

            base.UpdateDisplayList(width, height);
        }
    }
}