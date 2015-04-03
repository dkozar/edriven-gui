namespace eDriven.Gui.Components
{
    /// <summary>
    /// Spacer component
    /// Renders 'GUI.Spacer'
    /// </summary>
    public class Spacer : Component
    {
        public Spacer()
        {
            //Skin = SpacerSkin.Instance;
            FocusEnabled = false;
            MouseEnabled = false;
        }

        protected override void Measure()
        {
            // reset all, we don't need it since manipulating spacer size from the outside
            MeasuredWidth = 0;
            MeasuredHeight = 0;
            MeasuredMinWidth = 0;
            MeasuredMinHeight = 0;
        }

        #region RENDERING

        protected override void Render()
        {
            // spacer renders nothing
        }

        #endregion
    }
}