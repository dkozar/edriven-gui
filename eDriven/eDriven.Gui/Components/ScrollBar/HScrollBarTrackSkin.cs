using System.Collections.Generic;
using eDriven.Gui.Components.Rendering;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// Numeric stepper increment button skin
    ///</summary>
    [HostComponent(typeof(Button))]

    [Style(Name = "backgroundStyle", Type = typeof(GUIStyle), ProxyType = typeof(HScrollBarTrackStyle))]

    public class HScrollBarTrackSkin : Skin
    {
        public HScrollBarTrackSkin()
        {
            States = new List<State>(new[]
            {
                new State("up"), 
                new State("over"),
                new State("down"),
                new State("disabled")
            });
        }

        #region Members

        private RectShape _background;

        #endregion

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            _background = new RectShape
            {
                MinWidth = 16,
                MinHeight = 30,
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
            };
            AddChild(_background);
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            _background.SetStyle("backgroundStyle", GetStyle("backgroundStyle"));

            base.UpdateDisplayList(width, height);
        }
    }
}