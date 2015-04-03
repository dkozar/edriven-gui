using System;
using eDriven.Core.Events;
using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Managers;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Button bar base
    /// </summary>
// ReSharper disable once UnusedMember.Global
    public class ButtonBarBase : ListBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ButtonBarBase()
        {
            /*tabChildren = false;
            tabEnabled = true;*/
            TabFocusEnabled = true;
            SetCurrentCaretIndex(0); 
        }

        /**
            *  
            *  If false, don't show the focusRing for the tab at caretIndex, see
            *  itemShowingCaret() below.
            * 
            *  If the caret index changes because of something other than a arrow
            *  or space keypress then we don't show the focus ring, i.e. we do not 
            *  set showsCaret=true for the item renderer at caretIndex.
            *     
            *  This flag is valid at commitProperties() time.  It's set to false
            *  if at least one selectedIndex change (see item_clickHandler()) occurred 
            *  because of a mouse click.
            */
        private bool _enableFocusHighlight = true;
    
        /**
            *  
            */    
        private bool _inCollectionChangeHandler;
    
        /**
            *  
            *  Used to distinguish item_clickHandler() calls initiated by the mouse, from calls
            *  initiated by pressing the space bar.
            */
        private bool _inKeyUpHandler;
    
        /**
            *  
            *  Index of item that is currently pressed by the
            *  spacebar.
            */
        private int? _pressedIndex;

        private bool _requireSelectionChanged;
        
        public override bool RequireSelection
        {
            get
            {
                return base.RequireSelection;
            }
            set
            {
                if (value == base.RequireSelection)
                    return;

                base.RequireSelection = value;
                _requireSelectionChanged = true;
                InvalidateProperties();
            }
        }

        public override IList DataProvider
        {
            get { return base.DataProvider; }
            set
            {
                if (base.DataProvider is ISelectableList)
                {
                    base.DataProvider.RemoveEventListener(FrameworkEvent.VALUE_COMMIT, DataProviderChangeHandler);
                    base.DataProvider.RemoveEventListener(Event.CHANGE, DataProviderChangeHandler);
                }

                if (value is ISelectableList)
                {
                    value.AddEventListener(FrameworkEvent.VALUE_COMMIT, DataProviderChangeHandler);
                    value.AddEventListener(Event.CHANGE, DataProviderChangeHandler);
                }

                base.DataProvider = value;

                if (value is ISelectableList)
                    SelectedIndex = ((ISelectableList)base.DataProvider).SelectedIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void DataProviderCollectionChangeHandler(Event e)
        {
            _inCollectionChangeHandler = true;
            base.DataProviderCollectionChangeHandler(e);
            _inCollectionChangeHandler = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newIndex"></param>
        /// <param name="add"></param>
        protected override void AdjustSelection(int newIndex, bool add)
        {
            // see comment in dataProvider_collectionChangeHandler
            if (_inCollectionChangeHandler && DataProvider is ISelectableList)
                return;

            base.AdjustSelection(newIndex, add);
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_requireSelectionChanged && null != DataGroup)
            {
                _requireSelectionChanged = false;
                for (var i = 0; i < DataGroup.NumberOfContentChildren; i++)
                {
                    var renderer = DataGroup.GetContentChildAt(i) as ButtonBarButton;
                    if (null != renderer)
                        renderer.AllowDeselection = !RequireSelection;
                }
            }
        
            _enableFocusHighlight = true;
        }

        /**
         *  
         *  Return the item renderer at the specified index, or null.
         */
        private IVisualElement GetItemRenderer(int index)
        {
            if (null == DataGroup || (index < 0) || (index >= DataGroup.NumberOfContentChildren))
                return null;

            return DataGroup.GetContentChildAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="showsCaret"></param>
        protected override void ItemShowingCaret(int index, bool showsCaret)
        {
            base.ItemShowingCaret(index, showsCaret);

            var renderer = GetItemRenderer(index);
            if (null != renderer)
            {
                var hasFocus = FocusManager.Instance.FocusedComponent == this;
                renderer.Depth = (showsCaret) ? 1 : 0;
                var itemRenderer = renderer as IItemRenderer;
                if (itemRenderer != null)   
                    itemRenderer.ShowsCaret = showsCaret && _enableFocusHighlight && hasFocus;
            }
        }

        /*/**
         *  
         *  Called when the focus is gained/lost by tabbing in or out.
         #1#
        override public function drawFocus(isFocused:Boolean):void
        {
            const renderer:IVisualElement = getItemRenderer(caretIndex);
            if (renderer)
            {
                renderer.depth = (isFocused) ? 1 : 0;
                if (renderer is IItemRenderer)             
                    IItemRenderer(renderer).showsCaret = isFocused;
            }
        }   */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="selected"></param>
        protected override void ItemSelected(int index, bool selected)
        {
            base.ItemSelected(index, selected);

            var renderer = GetItemRenderer(index) as IItemRenderer;
            if (null != renderer)
            {
                if (selected)
                    SetCurrentCaretIndex(index);  // causes itemShowingCaret() call
                renderer.Selected = selected;
            }
        
            if ((DataProvider is ISelectableList) && selected)
                ((ISelectableList)DataProvider).SelectedIndex = index;
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == DataGroup)
            {
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_ADD, DataGroupRendererAddHandler);
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_REMOVE, DataGroupRendererRemoveHandler);
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == DataGroup)
            {
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_ADD, DataGroupRendererAddHandler);
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_REMOVE, DataGroupRendererRemoveHandler);
            }
        }

        /**
         *  
         */
        private void DataProviderChangeHandler(Event e)
        {
            SelectedIndex = ((ISelectableList)DataProvider).SelectedIndex;
        }

        /**
         *  
         */
        private void DataGroupRendererAddHandler(Event e)
        {
            RendererExistenceEvent ree = (RendererExistenceEvent) e;

            IVisualElement renderer = ree.Renderer; 
            if (null != renderer)
            {
                renderer.AddEventListener(MouseEvent.CLICK, ItemClickHandler);
                var component = renderer as IFocusManagerComponent;
                if (component != null)
                    component.FocusEnabled = false;
                var button = renderer as ButtonBarButton;
                if (button != null)
                    button.AllowDeselection = !RequireSelection;
            }
        }

        /**
         *  
         */
        private void DataGroupRendererRemoveHandler(Event e)
        {
            RendererExistenceEvent ree = (RendererExistenceEvent) e;
            IVisualElement renderer = ree.Renderer;
            if (null != renderer)
                renderer.RemoveEventListener(MouseEvent.CLICK, ItemClickHandler);
        }

        /**
         *  
         *  Called synchronously when the space bar is pressed or the mouse is clicked. 
         */
        private void ItemClickHandler(Event e)
        {
            //MouseEvent me = (MouseEvent) e;
            var target = e.CurrentTarget as IItemRenderer;
            var newIndex = target != null ? 
                target.ItemIndex : 
                DataGroup.GetContentChildIndex((DisplayListMember) e.CurrentTarget);

            var oldSelectedIndex = SelectedIndex;
            if (newIndex == SelectedIndex)
            {
                if (!RequireSelection)
                    SetSelectedIndex(NO_SELECTION, true);
            }
            else
                SetSelectedIndex(newIndex, true);
        
            // Changing the selectedIndex typically causes a call to itemSelected() at 
            // commitProperties() time.   We'll update the caretIndex then.  If this 
            // method was -not- called as a consequence of a keypress, we will not show
            // the focus highlight at caretIndex.  See itemShowingCaret().
        
            if (_enableFocusHighlight && (SelectedIndex != oldSelectedIndex))
                _enableFocusHighlight = _inKeyUpHandler;
        }

        /**
         *  
         *  Increment or decrement the caretIndex.  Wrap if arrowKeysWrapFocus=true.
         */
        private void AdjustCaretIndex(int delta)
        {
            if (null == DataGroup || (CaretIndex < 0))
                return;
        
            int oldCaretIndex = CaretIndex;
            int length = DataGroup.NumberOfContentChildren;
        
            if (ArrowKeysWrapFocus)
                SetCurrentCaretIndex((CaretIndex + delta + length) % length);
            else
                SetCurrentCaretIndex(Math.Min(length - 1, Math.Max(0, CaretIndex + delta)));

            if (oldCaretIndex != CaretIndex)
            {
                DispatchEvent(
                    new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE, false, false) {
                        OldIndex = oldCaretIndex, 
                        NewIndex = CaretIndex
                    }
                );
            }
        }

        protected override void KeyDownHandler(Event e)
        {
            if (e.Phase == EventPhase.Bubbling)
                return;
        
            if (!Enabled || null == DataGroup || e.DefaultPrevented)
                return;
        
            // Block input if space bar is being held down.
            if (null != _pressedIndex)
            {
                e.PreventDefault();
                return;
            }
        
            base.KeyDownHandler(e);

            KeyboardEvent ke = (KeyboardEvent) e;
            switch (ke.KeyCode)
            {
                case KeyCode.UpArrow:
                case KeyCode.LeftArrow:
                {
                    AdjustCaretIndex(-1);
                    e.PreventDefault();
                    break;
                }
                case KeyCode.DownArrow:
                case KeyCode.RightArrow:
                {
                    AdjustCaretIndex(+1);
                    e.PreventDefault();
                    break;
                }            
                case KeyCode.Space:
                {
                    IItemRenderer renderer = GetItemRenderer(CaretIndex) as IItemRenderer;
                    if (null != renderer && ((!renderer.Selected && RequireSelection) || !RequireSelection))
                    {
                        renderer.DispatchEvent(e);
                        _pressedIndex = CaretIndex;
                    }
                    break;
                }            
            }
        }

        protected override void KeyUpHandler(Event e)
        {
            if (e.Phase == EventPhase.Bubbling)
                return;
        
            if (!Enabled)
                return;
        
            base.KeyUpHandler(e);
        
            KeyboardEvent ke = (KeyboardEvent) e;
            switch (ke.KeyCode)
            {
                case KeyCode.Space:
                {
                    _inKeyUpHandler = true;
                
                    // Need to check pressedIndex for NaN for the case when key up
                    // happens on an already selected renderer and under the condition
                    // that requireSelection=true.
                    if (null != _pressedIndex)
                    {
                        // Dispatch key up to the previously pressed item in case focus was lost
                        // through other interaction (e.g. mouse clicks, etc...)
                        IItemRenderer renderer = GetItemRenderer((int) _pressedIndex) as IItemRenderer;
                        if (null != renderer && ((!renderer.Selected && RequireSelection) || !RequireSelection))
                        {
                            renderer.DispatchEvent(e);
                            _pressedIndex = null;
                        }
                    }
                    _inKeyUpHandler = false;
                    break;
                }            
            }
        }
    }
}
