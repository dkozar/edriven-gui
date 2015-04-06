using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Combo box
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(ComboBoxSkin))]

    public class ComboBox : DropDownListBase
    {
        //----------------------------------
        //  labelDisplay
        //----------------------------------

        /// <summary>
        /// Text input
        /// </summary>
        [SkinPart(Required = false)]
        public TextField TextInput;

        public ComboBox()
        {
            AddEventListener(KeyboardEvent.KEY_DOWN, CaptureKeyDownHandler, EventPhase.Capture);
            AllowCustomSelectedItem = true;
        }

        //--------------------------------------------------------------------------
        //
        //  Static Variables
        //
        //--------------------------------------------------------------------------
        // ReSharper disable once InconsistentNaming
        public new const int CUSTOM_SELECTED_ITEM = ListBase.CUSTOM_SELECTED_ITEM;

        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------
    
        private bool _isTextInputInFocus;
    
        private int _actualProposedSelectedIndex = NO_SELECTION;  
    
        private bool _userTypedIntoText;

        public delegate List<int> ItemMatchingFunctionDelegate(ComboBox combo, string inputText);

        public delegate object LabelToItemFunctionDelegate(string inputText);

        //--------------------------------------------------------------------------
        //  itemMatchingFunction
        //--------------------------------------------------------------------------
    
        public ItemMatchingFunctionDelegate ItemMatchingFunction = null;

        private bool _labelToItemFunctionChanged;
        private LabelToItemFunctionDelegate _labelToItemFunction;
        /// <summary>
        /// 
        /// </summary>
        public LabelToItemFunctionDelegate LabelToItemFunction
        {
            get { 
                return _labelToItemFunction;
            }
            set
            {
                if (value == _labelToItemFunction)
                    return;

                _labelToItemFunction = value;
                _labelToItemFunctionChanged = true;
                InvalidateProperties();
            }
        }

        private bool _maxCharsChanged;
        private int _maxChars;
        /// <summary>
        /// 
        /// </summary>
        public int MaxChars
        {
            get { 
                return _maxChars;
            }
            set
            {
                if (value == _maxChars)
                    return;

                _maxChars = value;
                _maxCharsChanged = true;
                InvalidateProperties();
            }
        }

        public bool OpenOnInput;

        private bool _restrictChanged;
        private string _restrict;
        /// <summary>
        /// 
        /// </summary>
        public string Restrict
        {
            get { 
                return _restrict;
            }
            set
            {
                if (value == _restrict)
                    return;

                _restrict = value;
                _restrictChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int SelectedIndex {
            set
            {
                base.SelectedIndex = value;
                _actualProposedSelectedIndex = value;
            }
        }

        private bool _typicalItemChanged;
        public override object TypicalItem
        {
            set
            {
                if (value == TypicalItem)
                    return;

                base.TypicalItem = value;

                _typicalItemChanged = true;
                InvalidateProperties();
            }
        }

        internal override int UserProposedSelectedIndex
        {
            set
            {
                base.UserProposedSelectedIndex = value;
                _actualProposedSelectedIndex = value;
            }
        }

        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------
    
        /**
         *   
         */ 
        private void ProcessInputField()
        {
            if (null == DataProvider || DataProvider.Length <= 0)
                return;
        
            // If the textInput has been changed, then use the input string as the selectedItem
            _actualProposedSelectedIndex = CUSTOM_SELECTED_ITEM; 
                    
            if (TextInput.Text != string.Empty)
            {
                List<int> matchingItems;
// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (ItemMatchingFunction != null)
                    matchingItems = ItemMatchingFunction(this, TextInput.Text);
                else
                    matchingItems = FindMatchingItems(TextInput.Text);
            
                if (matchingItems.Count > 0)
                {
                    base.ChangeHighlightedSelection(matchingItems[0], true);
                
                    var typedLength = TextInput.Text.Length;
                    var item = (null != DataProvider) ? DataProvider.GetItemAt(matchingItems[0]) : null;
                    if (null != item)
                    {
                        // If we found a match, then replace the textInput text with the match and 
                        // select the non-typed characters
                        var itemString = ItemToLabel(item);
                        /*textInput.selectAll();
                        textInput.insertText(itemString);
                        textInput.selectRange(typedLength, itemString.length);*/
                    }
                }
                else
                {
                    base.ChangeHighlightedSelection(CUSTOM_SELECTED_ITEM);
                }
            }
            else
            {
                // If the input string is empty, then don't select anything
                base.ChangeHighlightedSelection(NO_SELECTION);  
            }
        }

        /**
         *   
         */ 
        // Returns an array of possible values
        private List<int> FindMatchingItems(string input)
        {
            // For now, just select the first match
            int startIndex;
            int stopIndex;
            int retVal;  
            var retVector = new List<int>();

            retVal = FindStringLoop(input, 0, DataProvider.Length); 
        
            if (retVal != -1)
                retVector.Add(retVal);
            return retVector;
        }

        /**
         *   
         */ 
        private object GetCustomSelectedItem()
        {
            // Grab the text from the textInput and process it through labelToItemFunction
            var input = TextInput.Text;
            if (input == string.Empty)
                return null;
            else if (_labelToItemFunction != null)
                return LabelToItemFunction(input);
            else
                return input;
        }

        /**
         *   
         *  Helper function to apply the textInput text to selectedItem
         */ 
        internal void ApplySelection()
        {
            if (_actualProposedSelectedIndex == CUSTOM_SELECTED_ITEM)
            {
                var itemFromInput = GetCustomSelectedItem();
                if (null != itemFromInput)
                    SetSelectedItem(itemFromInput, true);
                else
                    SetSelectedIndex(NO_SELECTION, true);
            }
            else
            {
                SetSelectedIndex(_actualProposedSelectedIndex, true);
            }
                
            //TextInput.selectRange(-1, -1);
        
            _userTypedIntoText = false;
        }

        protected override void CommitProperties()
        {
            // Keep track of whether selectedIndex was programmatically changed
            var selectedIndexChanged = ProposedSelectedIndex != NO_PROPOSED_SELECTION;
        
            // If selectedIndex was set to CUSTOM_SELECTED_ITEM, and no selectedItem was specified,
            // then don't change the selectedIndex
            if (ProposedSelectedIndex == CUSTOM_SELECTED_ITEM && 
                null == PendingSelectedItem)
            {
                ProposedSelectedIndex = NO_PROPOSED_SELECTION;
            }
                
            base.CommitProperties();
        
            if (null != TextInput)
            {
                if (_maxCharsChanged)
                {
                    TextInput.MaxChars = _maxChars;
                    _maxCharsChanged = false;
                }
            
                if (_restrictChanged)
                {
                    TextInput.Restrict = _restrict;
                    _restrictChanged = false;
                }
            
                if (_typicalItemChanged)
                {
                    if (TypicalItem != null)
                    {
                        var itemString = LabelUtil.ItemToLabel(TypicalItem, LabelField, LabelFunction);
                        //TextInput.widthInChars = itemString.Length;
                    }
                    /*else
                    {
                        // Just set it back to the default value
                        //textInput.widthInChars = 10; 
                    }*/
                
                    _typicalItemChanged = false;
                }
                
                // Clear the TextInput because we were programmatically set to NO_SELECTION
                // We call this after super.commitProperties because commitSelection might have
                // changed the value to NO_SELECTION
                if (selectedIndexChanged && SelectedIndex == NO_SELECTION)
                    TextInput.Text = string.Empty;
            }
        }

        protected override void UpdateLabelDisplay(object displayItem = null)
        {
            base.UpdateLabelDisplay(displayItem);

            if (null != TextInput)
            {
                if (null == displayItem)
                    displayItem = SelectedItem;
                if (null != displayItem)
                {
                    TextInput.Text = LabelUtil.ItemToLabel(displayItem, LabelField, LabelFunction);
                }
            }
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == TextInput)
            {
                UpdateLabelDisplay();
                TextInput.AddEventListener(TextFieldEvent.TEXT_CHANGE, textInput_changeHandler);
                TextInput.AddEventListener(FocusEvent.FOCUS_IN, TextInputFocusInHandler, EventPhase.Capture);
                TextInput.AddEventListener(FocusEvent.FOCUS_OUT, TextInputFocusOutHandler, EventPhase.Capture);
                TextInput.MaxChars = MaxChars;
                TextInput.Restrict = Restrict;
                TextInput.FocusEnabled = false;

                //TextInput.textDisplay.batchTextInput = false;
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == TextInput)
            {
                TextInput.RemoveEventListener(TextFieldEvent.TEXT_CHANGE, textInput_changeHandler);
                TextInput.RemoveEventListener(FocusEvent.FOCUS_IN, TextInputFocusInHandler, EventPhase.Capture);
                TextInput.RemoveEventListener(FocusEvent.FOCUS_OUT, TextInputFocusOutHandler, EventPhase.Capture);
            }
        }

        internal override void ChangeHighlightedSelection(int newIndex, bool scrollToTop = false)
        {
            base.ChangeHighlightedSelection(newIndex, scrollToTop);

            if (newIndex > 0)
            {
                var item = null != DataProvider ? DataProvider.GetItemAt(newIndex) : null;
                if (null != item)
                {
                    var itemString = ItemToLabel(item);
                    /*TextInput.selectAll();
                    TextInput.insertText(itemString);
                    TextInput.selectAll();*/
             
                    _userTypedIntoText = false;
                }
            }
        }

        /**
         *   
         */ 
        /*override mx_internal function findKey(eventCode:int):Boolean // TODO
        {
            return false;
        }*/

        protected override void KeyDownHandler(Event e)
        {
            if (!_isTextInputInFocus)
                KeyDownHandlerHelper(e);
        }

        /**
         *   
         */ 
        protected void CaptureKeyDownHandler(Event e)
        {        
            if (_isTextInputInFocus)
                KeyDownHandlerHelper(e);
        }

        /**
         *   
         */ 
        private void KeyDownHandlerHelper(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent)e;

            base.KeyDownHandler(e);
        
            if (ke.KeyCode == KeyCode.Return && !IsDropDownOpen) 
            {
                // commit the current text
                ApplySelection();
            }
            else if (ke.KeyCode == KeyCode.Escape)
            {
                // Restore the previous selectedItem
                TextInput.Text = ItemToLabel(SelectedItem);
                ChangeHighlightedSelection(SelectedIndex);
            }
        }

        public override void FocusOutHandler(Event e)
        {
            // always commit the selection if we focus out        
            if (!IsDropDownOpen)
            {
                if (TextInput.Text != ItemToLabel(SelectedItem))
                    ApplySelection();
            }

            base.FocusOutHandler(e);
        }

        internal override void DropDownControllerOpenHandler(Event e)
        {
            base.DropDownControllerOpenHandler(e);

            // If the user typed in text, start off by not showing any selection
            // If this does match, then processInputField will highlight the match
            UserProposedSelectedIndex = _userTypedIntoText ? NO_SELECTION : SelectedIndex;
        }

        protected override void DropDownControllerCloseHandler(Event e)
        {
            base.DropDownControllerCloseHandler(e);

            // Commit the textInput text as the selection
            if (!e.DefaultPrevented)
            {
                ApplySelection();
            }
        }

        //--------------------------------------------------------------------------
        //
        //  Event Handlers
        //
        //--------------------------------------------------------------------------
    
        /**
         *   
         */ 
        private void TextInputFocusInHandler(Event e)
        {
            _isTextInputInFocus = true;
        }
    
        /**
         *   
         */ 
        private void TextInputFocusOutHandler(Event e)
        {
            _isTextInputInFocus = false;
        }
    
        /**
         *   
         */ 
        protected void textInput_changeHandler(Event e)
        {  
            _userTypedIntoText = true;

            TextFieldEvent tfe = (TextFieldEvent) e;

            /*var operation:FlowOperation = tfe.operation;

            // Close the dropDown if we press delete or cut the selected text
            if (operation is DeleteTextOperation || operation is CutOperation)
            {
                super.changeHighlightedSelection(CUSTOM_SELECTED_ITEM);
            }
            else
            {
                if (openOnInput)
                {
                    if (!isDropDownOpen)
                    {
                        // Open the dropDown if it isn't already open
                        openDropDown();
                        addEventListener(DropDownEvent.OPEN, editingOpenHandler);
                        return;
                    }   
                }
            
                processInputField();
            }*/
        }
    
        /**
         *   
         */ 
        private void EditingOpenHandler(Event e)
        {
            RemoveEventListener(DropDownEvent.OPEN, EditingOpenHandler);
            ProcessInputField();
        }
    }
}
