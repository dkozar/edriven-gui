namespace eDriven.Gui.Components
{
    /// <summary>
    /// Skinnable container base
    /// </summary>
    
    /**
     *  Normal State
     */
    [SkinStates("normal", "disabled")]
    
    public class SkinnableContainerBase : SkinnableComponent
    {
        private DisplayObject _defaultButton;
        public DisplayObject DefaultButton
        {
            get { 
                return _defaultButton;
            }
            set
            {
                if (value == _defaultButton)
                    return;

                _defaultButton = value;
            }
        }

        public override string GetCurrentSkinState()
        {
            return Enabled ? "normal" : "disabled";
        }
    }
}