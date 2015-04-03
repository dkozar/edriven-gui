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

    public class NumericStepperDecrementButtonSkin : Skin
    {
        public NumericStepperDecrementButtonSkin()
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
            _background.SetStyle("backgroundStyle", NumericStepperDecrementButtonStyle.Instance);
            AddChild(_background);
        }
    }
}