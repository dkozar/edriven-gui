using System;
using eDriven.Core.Events;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class DropDownListBase : List
    {
        /// <summary>using eDriven.Gui.Components;

        /// 
        /// </summary>
        public DropDownListBase()
        {
            AllowMultipleSelection = false;

            DropDownController = new DropDownController();
        }

        //--------------------------------------------------------------------------
        //
        //  Skin parts
        //
        //--------------------------------------------------------------------------    

        //----------------------------------
        //  dropDown
        //----------------------------------

        /// <summary>
        /// 
        /// </summary>
        [SkinPart(Required=false)]

        public DisplayListMember DropDown;
    
        //----------------------------------
        //  openButton
        //----------------------------------

        /// <summary>
        /// 
        /// </summary>
        [SkinPart(Required=true)]

        public Button OpenButton;

        //--------------------------------------------------------------------------
        //
        //  Variables
        //
        //--------------------------------------------------------------------------
    
        private bool _labelChanged;
        // Stores the user selected index until the dropDown closes
    
// ReSharper disable once InconsistentNaming
        internal static int PAGE_SIZE = 5;

        public override bool AllowMultipleSelection
        {
            get { return base.AllowMultipleSelection; }
// ReSharper disable once ValueParameterNotUsed
            set
            {
                return;
            }
        }

        public override IList DataProvider
        {
            get
            {
                return base.DataProvider;
            }
            set
            {
                if (base.DataProvider == value)
                    return;
            
                base.DataProvider = value;
                _labelChanged = true;
                InvalidateProperties();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DragEnabled
        {
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DragMoveEnabled
        {
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool DropEnabled
        {
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string LabelField {
            set
            {
                if (base.LabelField == value)
                    return;

                base.LabelField = value;
                _labelChanged = true;
                InvalidateProperties();
            }
        }

        /**
         *  
         */
        private DropDownController _dropDownController;
        
        /// <summary>
        /// 
        /// </summary>
        internal DropDownController DropDownController
        {
            get
            {
                return _dropDownController;
            }
            set
            {
                /*if (_dropDownController == value)
                    return;*/
            
                _dropDownController = value;

                _dropDownController.AddEventListener(DropDownEvent.OPEN, DropDownControllerOpenHandler);
                _dropDownController.AddEventListener(DropDownEvent.CLOSE, DropDownControllerCloseHandler);

                if (null != OpenButton)
                    _dropDownController.OpenButton = OpenButton;
                if (null != DropDown)
                    _dropDownController.DropDown = DropDown;  
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return null != DropDownController && DropDownController.IsOpen;
            }
        }

        //----------------------------------
        //  userProposedSelectedIndex
        //----------------------------------

        /**
         *  
         */
        private int _userProposedSelectedIndex = NO_SELECTION;
    
        /**
         *  
         */
        internal virtual int UserProposedSelectedIndex
        {
            get { return _userProposedSelectedIndex; }
            set { _userProposedSelectedIndex = value; }
        }

        //--------------------------------------------------------------------------
        //
        //  Overridden methods
        //
        //--------------------------------------------------------------------------

        /// <summary>
        /// Commits properties
        /// </summary>
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

            if (null != DropDownController)
            {
                if (instance == OpenButton)
                {
                    DropDownController.OpenButton = OpenButton;
                }
                else if (instance == DropDown)
                {
                    DropDownController.DropDown = DropDown;
                }
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            if (null != DropDownController)
            {
                if (instance == OpenButton)
                    DropDownController.OpenButton = null;

                if (instance == DropDown)
                    DropDownController.DropDown = null;
            }

            base.PartRemoved(partName, instance);
        }

        public override string GetCurrentSkinState()
        {
            return !Enabled ? "disabled" : IsDropDownOpen ? "open" : "normal";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatchChangedEvents"></param>
        /// <returns></returns>
        protected override bool CommitSelection(bool dispatchChangedEvents)
        {
            var retVal = base.CommitSelection(dispatchChangedEvents);
            UpdateLabelDisplay();
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override bool IsItemIndexSelected(int index)
        {
            return UserProposedSelectedIndex == index;
        }

        //--------------------------------------------------------------------------
        //
        //  Methods
        //
        //--------------------------------------------------------------------------   

        /// <summary>
        /// Opens drop down
        /// </summary>
        public void OpenDropDown()
        {
            DropDownController.OpenDropDown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commit"></param>
        public void CloseDropDown(bool commit)
        {
            DropDownController.CloseDropDown(commit);
        }

        /**
         *  
         *  Called whenever we need to update the text passed to the labelDisplay skin part
         */
        // TODO (jszeto): Make this protected and make the name more generic (passing data to skin) 
        protected virtual void UpdateLabelDisplay(object displayItem = null)
        {
            // DropDownList and ComboBox will override this function
        }

        /**
         *  
         *  Called whenever we need to change the highlighted selection while the dropDown is open
         *  ComboBox overrides this behavior
         */
        internal virtual void ChangeHighlightedSelection(int newIndex, bool scrollToTop = false)
        {
            // Store the selection in userProposedSelectedIndex because we 
            // don't want to update selectedIndex until the dropdown closes
            ItemSelected(UserProposedSelectedIndex, false);
            UserProposedSelectedIndex = newIndex;
            ItemSelected(UserProposedSelectedIndex, true);

            float? topOffset = null;
            if (scrollToTop)
                topOffset = 0f;

            PositionIndexInView(UserProposedSelectedIndex, topOffset);

            IndexChangeEvent e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE) {OldIndex = CaretIndex};
            SetCurrentCaretIndex(UserProposedSelectedIndex);
            e.NewIndex = CaretIndex;
            DispatchEvent(e);
        }

        /**
         *   
         */ 
        internal void PositionIndexInView(int index, float? topOffset = null, 
                                                 float? bottomOffset = null, 
                                                 float? leftOffset = null,
                                                 float? rightOffset = null)
        {
            if (null == Layout)
                return;

            var spDelta = DataGroup.Layout.GetScrollPositionDeltaToElementHelper(index, topOffset, bottomOffset,
                                                                       leftOffset, rightOffset);
        
            if (null != spDelta)
            {
                DataGroup.HorizontalScrollPosition += spDelta.X;
                DataGroup.VerticalScrollPosition += spDelta.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void DataProviderCollectionChangeHandler(Event e)
        {
            base.DataProviderCollectionChangeHandler(e);

            if (e is CollectionEvent)
            {
                _labelChanged = true;
                InvalidateProperties();         
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void ItemMouseDownHandler(Event e)
        {
            base.ItemMouseDownHandler(e);
            UserProposedSelectedIndex = SelectedIndex;
            CloseDropDown(true);
        }

        protected override void KeyDownHandler(Event e)
        {
            if (!Enabled)
                return;

            KeyboardEvent ke = (KeyboardEvent) e;

            if (!DropDownController.ProcessKeyDown(e))
            {
                var navigationKey = ke.KeyCode;
                var navigationUnit = NavigationUnitUtil.GetNavigationUnit(navigationKey);
                           
                /*if (findKey(ke.charCode))
                {
                    event.preventDefault();
                    return;
                }*/
            
                if (!NavigationUnitUtil.IsNavigationUnit(navigationKey))
                    return;

                var proposedNewIndex = NO_SELECTION;
                int currentIndex;
                        
                if (IsDropDownOpen)
                {   
                    // Normalize the proposed index for getNavigationDestinationIndex
                    currentIndex = UserProposedSelectedIndex < NO_SELECTION ? NO_SELECTION : UserProposedSelectedIndex;
                    proposedNewIndex = Layout.GetNavigationDestinationIndex(currentIndex, navigationUnit, ArrowKeysWrapFocus);
                
                    if (proposedNewIndex != NO_SELECTION)
                    {
                        ChangeHighlightedSelection(proposedNewIndex);
                        e.PreventDefault();
                    }
                }
                else if (null != DataProvider)
                {
                    var maxIndex = DataProvider.Length - 1;
                
                    // Normalize the proposed index for getNavigationDestinationIndex
                    currentIndex = CaretIndex < NO_SELECTION ? NO_SELECTION : CaretIndex;

                    switch (navigationUnit)
                    {
                        case NavigationUnit.Up:
                        {
                            if (ArrowKeysWrapFocus && 
                                (currentIndex == 0 || 
                                 currentIndex == NO_SELECTION || 
                                 currentIndex == CUSTOM_SELECTED_ITEM))
                                proposedNewIndex = maxIndex;
                            else
                                proposedNewIndex = currentIndex - 1;  
                            e.PreventDefault();
                            break;
                        }                      
        
                        case NavigationUnit.Down:
                        {
                            if (ArrowKeysWrapFocus && 
                                (currentIndex == maxIndex || 
                                 currentIndex == NO_SELECTION || 
                                 currentIndex == CUSTOM_SELECTED_ITEM))
                                proposedNewIndex = 0;
                            else
                                proposedNewIndex = currentIndex + 1;  
                            e.PreventDefault();
                            break;
                        }
                        
                        case NavigationUnit.PageUp:
                        {
                            proposedNewIndex = currentIndex == NO_SELECTION ? 
                                NO_SELECTION : Math.Max(currentIndex - PAGE_SIZE, 0);
                            e.PreventDefault();
                            break;
                        }
                        
                        case NavigationUnit.PageDown:
                        {    
                            proposedNewIndex = currentIndex == NO_SELECTION ?
                                               PAGE_SIZE : (currentIndex + PAGE_SIZE);
                            e.PreventDefault();
                            break;
                        }
                       
                        case NavigationUnit.Home:
                        {
                            proposedNewIndex = 0;
                            e.PreventDefault();
                            break;
                        }

                        case NavigationUnit.End:
                        {
                            proposedNewIndex = maxIndex;  
                            e.PreventDefault();
                            break;
                        }  
                       
                    }
                
                    proposedNewIndex = Math.Min(proposedNewIndex, maxIndex);
                
                    if (proposedNewIndex >= 0)
                        SetSelectedIndex(proposedNewIndex, true);
                }
            }
            else
            {
                e.PreventDefault();
            }
        }

        public override void FocusOutHandler(Event e)
        {
            FocusEvent fe = (FocusEvent) e;
            if (IsOurFocus((DisplayObject)e.Target))
                DropDownController.ProcessFocusOut(e);

            base.FocusOutHandler(e);
        }

        internal virtual void DropDownControllerOpenHandler(Event e)
        {
            AddEventListener(FrameworkEvent.UPDATE_COMPLETE, open_updateCompleteHandler);
            UserProposedSelectedIndex = SelectedIndex;
            InvalidateSkinState();
        }

        /**
         *  
         */
        internal virtual void open_updateCompleteHandler(Event e)
        {
            RemoveEventListener(FrameworkEvent.UPDATE_COMPLETE, open_updateCompleteHandler);
            PositionIndexInView(SelectedIndex, 0);
        
            DispatchEvent(new DropDownEvent(DropDownEvent.OPEN));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void DropDownControllerCloseHandler(Event e)
        {
            AddEventListener(FrameworkEvent.UPDATE_COMPLETE, close_updateCompleteHandler);
            InvalidateSkinState();
        
            if (!e.DefaultPrevented)
            {
                // Even if the dropDown was programmatically closed, assume the selection 
                // changed as a result of a previous user interaction
                SetSelectedIndex(UserProposedSelectedIndex, true);  
            }
            else
            {
                ChangeHighlightedSelection(SelectedIndex);
            }
        }

        /**
         *  
         */
        private void close_updateCompleteHandler(Event e)
        {
            RemoveEventListener(FrameworkEvent.UPDATE_COMPLETE, close_updateCompleteHandler);
        
            DispatchEvent(new DropDownEvent(DropDownEvent.CLOSE));
        }
    }
}