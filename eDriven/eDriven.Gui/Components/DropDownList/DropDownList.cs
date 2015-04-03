using eDriven.Gui.Managers;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Drop down list
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(DropDownListSkin))]

    public class DropDownList : DropDownListBase
    {
        //----------------------------------
        //  labelDisplay
        //----------------------------------

        /// <summary>
        /// Label display
        /// </summary>
        [SkinPart(Required=false)]

        public TextBase LabelDisplay;

        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------

        private bool _labelChanged;
        private float? _labelDisplayExplicitWidth; 
        private float? _labelDisplayExplicitHeight; 
        private bool _sizeSetByTypicalItem;

        private string _prompt;
        
        /// <summary>
        /// The text displayed when no value selected
        /// </summary>
        public string Prompt
        {
            get { 
                return _prompt;
            }
            set
            {
                if (value == _prompt)
                    return;

                _prompt = value;
                _labelChanged = true;
                InvalidateProperties();
            }
        }

        public override object TypicalItem
        {
            set
            {
                base.TypicalItem = value;
                InvalidateSize();
            }
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_labelChanged)
            {
                _labelChanged = false;
                UpdateLabelDisplay();
            }
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == LabelDisplay)
            {
                _labelChanged = true;
                InvalidateProperties();
            }
        }

        protected override void Measure()
        {
            if (null != LabelDisplay)
            {
                // If typicalItem is set, then use it for measurement
                if (null != TypicalItem)
                {
                    // Save the labelDisplay's dimensions in case we clear out typicalItem
                    if (!_sizeSetByTypicalItem)
                    {
                        _labelDisplayExplicitWidth = LabelDisplay.ExplicitWidth;
                        _labelDisplayExplicitHeight = LabelDisplay.ExplicitHeight;
                        _sizeSetByTypicalItem = true;
                    }

                    LabelDisplay.ExplicitWidth = null;
                    LabelDisplay.ExplicitHeight = null;

                    // Swap in the typicalItem into the labelDisplay
                    UpdateLabelDisplay(TypicalItem);
                    InvalidationManager.Instance.ValidateClient(Skin, true);

                    // Force the labelDisplay to be sized to the measured size
                    LabelDisplay.Width = LabelDisplay.MeasuredWidth;
                    LabelDisplay.Height = LabelDisplay.MeasuredHeight;

                    // Set the labelDisplay back to selectedItem
                    UpdateLabelDisplay();
                }
                else if (_sizeSetByTypicalItem && null == TypicalItem)
                {
                    // Restore the labelDisplay to its original size
                    if (_labelDisplayExplicitWidth != null)
                        LabelDisplay.Width = (float)_labelDisplayExplicitWidth;

                    if (_labelDisplayExplicitHeight != null)
                        LabelDisplay.Height = (float)_labelDisplayExplicitHeight;

                    _sizeSetByTypicalItem = false;
                }
            }
            
            base.Measure();
        }

        protected override void UpdateLabelDisplay(object displayItem = null)
        {
            if (null == LabelDisplay)
                return;
            if (null == displayItem)
                displayItem = SelectedItem;
            LabelDisplay.Text = null != displayItem ? 
                LabelUtil.ItemToLabel(displayItem, LabelField, LabelFunction) : 
                _prompt;
        }
    }
}
