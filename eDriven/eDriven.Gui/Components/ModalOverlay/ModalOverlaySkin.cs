using System.Collections.Generic;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Modal overlay skin has no required skin parts
    /// </summary>
    [HostComponent(typeof(ModalOverlay))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(ModalOverlayStyle))]
    [Style(Name = "backgroundColor", Type = typeof(Color), Default = 0x000000)]
    [Style(Name = "backgroundAlpha", Type = typeof(float), Default = 0.25f)]
    
    public class ModalOverlaySkin : Skin
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ModalOverlaySkin()
        {
            States = new List<State>(new[]
            {
                new State("up"), 
                new State("over"),
                new State("down"),
                new State("disabled"),
                new State("upAndSelected"), 
                new State("overAndSelected"),
                new State("downAndSelected"),
                new State("disabledAndSelected")
            });
        }

        #region Members

        private RectShape _background;

        #endregion

        /// <summary>
        /// 
        /// </summary>
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void UpdateDisplayList(float width, float height)
        {
            if (null != _background)
            {
                _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle"));
                _background.BackgroundColor = (Color)GetStyle("backgroundColor");
                _background.Alpha = (float) GetStyle("backgroundAlpha");
            }

            base.UpdateDisplayList(width, height);
        }
    }
}