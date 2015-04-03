using System.Collections.Generic;
using eDriven.Gui.Reflection;
using eDriven.Gui.Shapes;
using eDriven.Gui.States;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// Numeric stepper increment button skin
    ///</summary>
    [HostComponent(typeof(Button))]

    public class NumericStepperIncrementButtonSkin : Skin
    {
        public NumericStepperIncrementButtonSkin()
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

        protected override void CreateChildren()
        {
            //Debug.Log("Button skin creating children");
            base.CreateChildren();

            _background = new RectShape
                              {
                                  Left = 0, Right = 0, Top = 0, Bottom = 0
                              };
            _background.SetStyle("backgroundStyle", NumericStepperIncrementButtonStyle.Instance);
            AddChild(_background);
        }

        //private static GUIStyle _style;
        //private static GUIStyle GetDrawingStyle()
        //{
        //    if (null == _style)
        //    {
        //        _style = new GUIStyle();

        //        const int size = 18;
        //        const int weight = 1;

        //        ProgramaticStyle style = new ProgramaticStyle
        //        {
        //            Style = _style,
        //            Alignment = TextAnchor.MiddleCenter,
        //            NormalGraphics = new Rect(1, 1, new Fill())
        //        };

        //        ScrollbarHelper.ApplyButtonStyle(style, size, weight, TriangleDirection.Up);

        //        style.FixedWidth = size;
        //        style.FixedHeight = size;

        //        style.Validate();
        //    }

        //    return _style;
        //}
    }
}