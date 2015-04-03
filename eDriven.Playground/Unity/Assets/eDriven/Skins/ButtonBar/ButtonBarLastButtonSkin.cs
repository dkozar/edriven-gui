using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace Assets.eDriven.Skins
{
    /// <summary>
    /// 
    /// </summary>
    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(ButtonStyle))]
    [Style(Name = "showBackground", Type = typeof(bool), Default = true)]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0x00ff00)]
    [Style(Name = "textColor", Type = typeof(Color), Default = 0x333333)]

    public class ButtonBarLastButtonSkin : Skin
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ButtonBarLastButtonSkin()
        {
            States = DefaultButtonStateFactory.CreateStates();
            /*States = new List<State>(new[]
            {
                new State("up"), 
                new State("over"),
                new State("down"),
                new State("disabled"),
                new State("upAndSelected"), 
                new State("overAndSelected"),
                new State("downAndSelected"),
                new State("disabledAndSelected")
            });*/
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
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
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

        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _background)
            {
                _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle"));
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
            }

            if (null != LabelDisplay)
            {
                LabelDisplay.SetStyle("color", GetStyle("textColor"));
            }

            base.UpdateDisplayList(width, height);
        }
    }
}