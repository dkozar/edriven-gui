namespace eDriven.Gui.Components
{
    /// <summary>
    /// TODO: do it properly:
    /// Loading mask should push all of its visuals to the skin
    /// It should have an animator as a skin part
    /// </summary>
    public class LoadingMask : LoadingMaskBase<LoadingMaskAnimator>
    {
        public LoadingMask()
        {
        }

        public LoadingMask(DisplayListMember component)
            : base(component)
        {
        }

        public LoadingMask(DisplayListMember component, string message)
            : base(component, message)
        {
        }
    }
}