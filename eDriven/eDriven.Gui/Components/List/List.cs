using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Containers;
using eDriven.Gui.Data;
using eDriven.Gui.DragDrop;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.States;
using eDriven.Gui.Styles;
using UnityEngine;
using Event=eDriven.Core.Events.Event;
using Object=System.Object;

namespace eDriven.Gui.Components
{
    ///<summary>
    ///</summary>
    [Style(Name = "skinClass", Default = typeof(ListSkin))]
    
    public class List : ListBase, IFocusManagerComponent
    {
        ///<summary>
        /// Constructor
        ///</summary>
        public List()
        {
            //UseVirtualLayout = true;
            //FocusEnabled = true; // doesn't help -> TODO
            FocusEnabled = true;
            HighlightOnFocus = true;
            ProcessKeys = true;
        }

        /**
         *  
         *  The point where the mouse down event was received.
         *  Used to track whether a drag operation should be initiated when the user
         *  drags further than a certain threshold. 
         */
        private Point _mouseDownPoint;

        /**
         *  
         *  The index of the element the mouse down event was received for. Used to
         *  track which is the "focus item" for a drag and drop operation.
         */
        private int _mouseDownIndex = -1;
        
        /**
         *  
         *  When dragging is enabled with multiple selection, the selection is not
         *  comitted immediately on mouse down, but we wait to see whether the user
         *  intended to start a drag gesture instead. In that case we postpone
         *  comitting the selection until mouse up.
         */
        private bool _pendingSelectionOnMouseUp;

        /**
         *  
         */
        private bool _pendingSelectionShiftKey;

        /**
         *  
         */
        private bool _pendingSelectionCtrlKey;

        #region Skin parts

        ///<summary>
        ///</summary>
        [SkinPart(Required=false)]

// ReSharper disable UnusedMember.Global
        public IFactory DropIndicator;
// ReSharper restore UnusedMember.Global

        ///<summary>
        ///</summary>
        [SkinPart(Required = false)]

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnassignedField.Global
        public Scroller Scroller;
// ReSharper restore UnassignedField.Global
// ReSharper restore MemberCanBePrivate.Global

        #endregion

        #region Overriden properties

        /// <summary>
        /// A flag that indicates whether this List's focusable item renderers can take keyboard focus. 
        /// </summary>
        override public bool HasFocusableChildren
        {
            set
            {
                base.HasFocusableChildren = value;

                if (null != Scroller)
                    Scroller.HasFocusableChildren = value;
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
                // Uconditionally clear the selection, see SDK-21645
                if (!IsEmpty(_proposedSelectedIndices) || !IsEmpty(SelectedIndices))
                {
                    _proposedSelectedIndices.Clear();
                    _multipleSelectionChanged = true;
                    InvalidateProperties();
                }
                base.DataProvider = value;
            }
        }

        #endregion

        #region Properties

        /**
         *  
         */
        private bool _allowMultipleSelection;
        
// ReSharper disable MemberCanBePrivate.Global
        ///<summary>
        ///</summary>
        public virtual bool AllowMultipleSelection
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                return _allowMultipleSelection;
            }
// ReSharper disable UnusedMember.Global
            set
// ReSharper restore UnusedMember.Global
            {
                if (value == _allowMultipleSelection)
                    return;     
                
                _allowMultipleSelection = value; 
            }
        }
        
        /**
         *  
         *  Storage for the dragEnabled property.
         */
        private bool _dragEnabled;
        
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591
        public virtual bool DragEnabled
#pragma warning restore 1591
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                return _dragEnabled;
            }
// ReSharper disable UnusedMember.Global
            set
// ReSharper restore UnusedMember.Global
            {
                if (value == _dragEnabled)
                    return;
                _dragEnabled = value;
                
                if (_dragEnabled)
                {
                    AddEventListener(DragEvent.DRAG_START, DragStartHandler, EventPriority.DEFAULT_HANDLER);
                    AddEventListener(DragEvent.DRAG_COMPLETE, DragCompleteHandler, EventPriority.DEFAULT_HANDLER);
                }
                else
                {
                    RemoveEventListener(DragEvent.DRAG_START, DragStartHandler);
                    RemoveEventListener(DragEvent.DRAG_COMPLETE, DragCompleteHandler);
                }
            }
        }
        
        /**
         *  
         *  Storage for the dragMoveEnabled property.
         */
        private bool _dragMoveEnabled;
             
// ReSharper disable UnusedMember.Global
        ///<summary>
        ///</summary>
// ReSharper disable ConvertToAutoProperty
        public virtual bool DragMoveEnabled
// ReSharper restore ConvertToAutoProperty
// ReSharper restore UnusedMember.Global
        {
            get { return _dragMoveEnabled; }
            set { _dragMoveEnabled = value; }
        }

        /**
         *  
         *  Storage for the <code>dropEnabled</code> property.
         */
        private bool _dropEnabled;

#pragma warning disable 1591
// ReSharper disable UnusedMember.Global
        public virtual bool DropEnabled
// ReSharper restore UnusedMember.Global
#pragma warning restore 1591
        {
            get
            {
                return _dropEnabled;
            }
            set
            {
                if (value == _dropEnabled)
                    return;
                _dropEnabled = value;
                
                if (_dropEnabled)
                {
                    AddEventListener(DragEvent.DRAG_ENTER, DragEnterHandler, EventPriority.DEFAULT_HANDLER);
                    AddEventListener(DragEvent.DRAG_EXIT, DragExitHandler, EventPriority.DEFAULT_HANDLER);
                    AddEventListener(DragEvent.DRAG_OVER, DragOverHandler, EventPriority.DEFAULT_HANDLER);
                    AddEventListener(DragEvent.DRAG_DROP, DragDropHandler, EventPriority.DEFAULT_HANDLER);
                }
                else
                {
                    RemoveEventListener(DragEvent.DRAG_ENTER, DragEnterHandler);
                    RemoveEventListener(DragEvent.DRAG_EXIT, DragExitHandler);
                    RemoveEventListener(DragEvent.DRAG_OVER, DragOverHandler);
                    RemoveEventListener(DragEvent.DRAG_DROP, DragDropHandler);
                }
            }
        }

        #endregion

        #region Selected indices

        /**
         *  
         *  Internal storage for the SelectedIndices property.
         */
        private List<int> _selectedIndices;
        
        /**
         *  
         */
        private List<int> _proposedSelectedIndices = new List<int>(); 
        
        /**
         *  
         */
        private bool _multipleSelectionChanged;
        
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 1591
        public List<int> SelectedIndices
#pragma warning restore 1591
// ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                return _selectedIndices;
            }
// ReSharper disable UnusedMember.Global
            set
// ReSharper restore UnusedMember.Global
            {
                SetSelectedIndices(value, false);
            }
        }
        
        /**
         *  
         *  Used internally to specify whether the SelectedIndices changed programmatically or due to 
         *  user interaction. 
         * 
         *  Param: dispatchChangeEvent if true, the component will dispatch a "change" event if the
         *  value has changed. Otherwise, it will dispatch a "valueCommit" event. 
         */
// ReSharper disable MemberCanBePrivate.Global
        internal void SetSelectedIndices(List<int> value, bool dispatchChangeEvent/* = false*/)
// ReSharper restore MemberCanBePrivate.Global
        {
            if (_proposedSelectedIndices == value || 
                (null != value && value.Count == 1 &&
                 null != SelectedIndices && SelectedIndices.Count == 1 &&    
                 value[0] == SelectedIndices[0]))
                return; 
            
            if (dispatchChangeEvent)
                DispatchChangeAfterSelection = true;

            _proposedSelectedIndices = value;
            _multipleSelectionChanged = true;  
            InvalidateProperties();
        }

        #endregion

        #region Selected items

        ///<summary>
        ///</summary>
// ReSharper disable UnusedMember.Global
        public List<object> SelectedItems
// ReSharper restore UnusedMember.Global
        {
            get
            {
                List<object> result = null;
            
                if (null != SelectedIndices)
                {
                    result = new List<object>();
                    
                    var count = SelectedIndices.Count;
                    
                    for (int i = 0; i < count; i++)
                        result[i] = DataProvider.GetItemAt(SelectedIndices[i]);  
                }
                
                return result;
            }
            set
            {
                List<int> indices = null;
                
                if (null != value)
                {
                    indices = new List<int>();
                    
                    var count = value.Count;
                    
                    for (var i = 0; i < count; i++)
                    {
                        var index = DataProvider.GetItemIndex(value[i]);
                        if (index != -1)
                        { 
                            //indices.splice(0, 0, index);
                            indices.Insert(0, index);
                        }
                        // If an invalid item is in the SelectedItems vector,
                        // we set SelectedItems to an empty vector, which 
                        // essentially clears selection. 
                        if (index == -1)
                        {
                            indices = new List<int>();
                            break;  
                        }
                    }
                }
                
                _proposedSelectedIndices = indices;
                _multipleSelectionChanged = true;
                InvalidateProperties(); 
            }
        }
        
        #endregion

        #region Overriden methods

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_multipleSelectionChanged)
            {
                // _multipleSelectionChanged flag is cleared in commitSelection();
                // this is so, because commitSelection() could be called from
                // super.commitProperties() as well and in that case we don't
                // want to commitSelection() twice, as that will actually wrongly 
                // clear the selection.
                CommitSelection(true);
            }
        }

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == DataGroup)
            {
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_ADD, DataGroupRendererAddHandler);
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_REMOVE, DataGroupRendererRemoveHandler);
            }
            else if (instance == Scroller)
            {
                Scroller.HasFocusableChildren = HasFocusableChildren;
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            if (instance == DataGroup)
            {
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_ADD, DataGroupRendererAddHandler);
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_REMOVE, DataGroupRendererRemoveHandler);
            }

            base.PartRemoved(partName, instance);
        }

        protected override void ItemAdded(int index)
        {
            base.ItemAdded(index);

            AdjustSelection(index, true); 
        }

        protected override void ItemRemoved(int index)
        {
            base.ItemRemoved(index);

            AdjustSelection(index, false);
        }

        /**
         *  
         *  Used to filter _proposedSelectedIndices.
         */
// ReSharper disable UnusedMember.Local
        private bool IsValidIndex(int item/*, int index, List<int> v*/)
// ReSharper restore UnusedMember.Local
        {
            return (null != DataProvider) && (item >= 0) && (item < DataProvider.Length); 
        }

        protected override bool CommitSelection(bool dispatchChangedEvents/* = true*/)
        {
            // Clear the flag so that we don't commit the selection again.
            _multipleSelectionChanged = false;

            var oldSelectedIndex = SelectedIndex;
            var oldCaretIndex = CaretIndex;  
            
            _proposedSelectedIndices = _proposedSelectedIndices.FindAll(delegate (int index)
                                                                            {
                                                                                return (null != DataProvider) && (index >= 0) && (index < DataProvider.Length); 
                                                                            });
                   
            // Ensure that multiple selection is allowed and that proposed 
            // selected indices honors it. For example, in the single 
            // selection case, proposedSelectedIndices should only be a 
            // vector of 1 entry. If its not, we pare it down and select the 
            // first item.  
            if (!AllowMultipleSelection && !IsEmpty(_proposedSelectedIndices))
            {
                var temp = new List<int> {_proposedSelectedIndices[0]};
                _proposedSelectedIndices = temp;  
            }
            // Keep ProposedSelectedIndex in-sync with multiple selection properties. 
            if (!IsEmpty(_proposedSelectedIndices))
               ProposedSelectedIndex = GetFirstItemValue(_proposedSelectedIndices); 
            
            // Let ListBase handle the validating and commiting of the single-selection
            // properties.  
            var retVal = base.CommitSelection(false); 
            
            // If super.commitSelection returns a value of false, 
            // the selection was cancelled, so return false and exit. 
            if (!retVal)
                return false; 
            
            // Now keep _proposedSelectedIndices in-sync with single selection 
            // properties now that the single selection properties have been 
            // comitted.  
            if (SelectedIndex > NO_SELECTION)
            {
                if (null != _proposedSelectedIndices && _proposedSelectedIndices.IndexOf(SelectedIndex) == -1)
                    _proposedSelectedIndices.Add(SelectedIndex);
            }
            
            // Validate and commit the multiple selection related properties. 
            CommitMultipleSelection(); 
            
            // Set the caretIndex based on the current selection 
            SetCurrentCaretIndex(SelectedIndex);
            
            // And dispatch change and caretChange events so that all of 
            // the bindings update correctly. 
            if (dispatchChangedEvents && null != retVal)
            {
                IndexChangeEvent e; 
                
                if (DispatchChangeAfterSelection)
                {
                    e = new IndexChangeEvent(IndexChangeEvent.CHANGE)
                            {
                                OldIndex = oldSelectedIndex,
                                NewIndex = SelectedIndex
                            };
                    DispatchEvent(e);
                    DispatchChangeAfterSelection = false;
                }
                else
                {
                    DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
                }
                
                e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE)
                        {
                            OldIndex = oldCaretIndex,
                            NewIndex = CaretIndex
                        };
                DispatchEvent(e);    
            }
            
            return retVal; 
        }

        /**
         *  
         *  Given a new selection interval, figure out which
         *  items are newly added/removed from the selection interval and update
         *  selection properties and view accordingly. 
         */
// ReSharper disable MemberCanBePrivate.Global
        protected void CommitMultipleSelection()
// ReSharper restore MemberCanBePrivate.Global
        {
            var removedItems = new List<int>();
            var addedItems = new List<int>();
            int i;
            int count;
            
            if (!IsEmpty(_selectedIndices) && !IsEmpty(_proposedSelectedIndices))
            {
                // Changing selection, determine which items were added to the 
                // selection interval 
                count = _proposedSelectedIndices.Count;
                for (i = 0; i < count; i++)
                {
                    if (_selectedIndices.IndexOf(_proposedSelectedIndices[i]) < 0)
                        addedItems.Add(_proposedSelectedIndices[i]);
                }
                // Then determine which items were removed from the selection 
                // interval 
                count = _selectedIndices.Count; 
                for (i = 0; i < count; i++)
                {
                    if (_proposedSelectedIndices.IndexOf(_selectedIndices[i]) < 0)
                        removedItems.Add(_selectedIndices[i]);
                }
            }
            else if (!IsEmpty(_selectedIndices))
            {
                // Going to a null selection, remove all
                removedItems = _selectedIndices;
            }
            else if (!IsEmpty(_proposedSelectedIndices))
            {
                // Going from a null selection, add all
                addedItems = _proposedSelectedIndices;
            }
             
            // De-select the old items that were selected 
            if (removedItems.Count > 0)
            {
                count = removedItems.Count;
                for (i = 0; i < count; i++)
                {
                    ItemSelected(removedItems[i], false);
                }
            }
            
            // Select the new items in the new selection interval 
            if (!IsEmpty(_proposedSelectedIndices))
            {
                count = _proposedSelectedIndices.Count;
                for (i = 0; i < count; i++)
                {
                    ItemSelected(_proposedSelectedIndices[i], true);
                }
            }
            
            // Commit the selected indices and put _proposedSelectedIndices
            // back to its default value.  
            _selectedIndices = _proposedSelectedIndices;
            _proposedSelectedIndices = new List<int>();
        }

        protected override void ItemSelected(int index, bool selected)
        {
            base.ItemSelected(index, selected);

            var renderer = null != DataGroup ? DataGroup.GetContentChildAt(index) : null;
        
            if (renderer is IItemRenderer)
            {
                ((IItemRenderer)renderer).Selected = selected;
            }
        }

        protected override void ItemShowingCaret(int index, bool showsCaret)
        {
            base.ItemShowingCaret(index, showsCaret);

            var renderer = null != DataGroup ? DataGroup.GetContentChildAt(index) : null;
        
            if (renderer is IItemRenderer)
            {
                ((IItemRenderer)renderer).ShowsCaret = showsCaret;
            }
        }

        protected override bool IsItemIndexSelected(int index)
        {
            if (AllowMultipleSelection && (SelectedIndices != null))
                return SelectedIndices.IndexOf(index) != -1;

            return index == SelectedIndex;
        }

        #endregion
        
        #region Methods

        /**
         *  
         *  Returns the index of the last selected item. In single 
         *  selection, this is just selectedIndex. In multiple 
         *  selection, this is the index of the first selected item.  
         */
        private int GetLastSelectedIndex()
        {
            return null != SelectedIndices && SelectedIndices.Count > 0 ? SelectedIndices[SelectedIndices.Count - 1] : 0;
        }

        /**
         *  
         *  Given a Vector, returns the value of the first item, 
         *  or -1 if there are no items in the Vector; 
         */
        private static int GetFirstItemValue(IList<int> v)
        {
            if (null != v && v.Count > 0)
                return v[0];

            return -1;
        }

        /**
         *  
         *  Returns true if v is null or an empty Vector.
         */
        private static bool IsEmpty(ICollection<int> v)
        {
            return v == null || v.Count == 0;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected List<int> CalculateSelectedIndices(int index, bool shiftKey, bool ctrlKey)
// ReSharper restore MemberCanBePrivate.Global
        {
            int i; 
            List<int> interval = new List<int>();  
            
            if (!shiftKey)
            {
                if (ctrlKey)
                {
                    if (!IsEmpty(SelectedIndices))
                    {
                        // Quick check to see if SelectedIndices had only one selected item
                        // and that item was de-selected
                        if (SelectedIndices.Count == 1 && (SelectedIndices[0] == index))
                        {
                            // We need to respect requireSelection 
                            if (!RequireSelection)
                                return interval;
                            
                            interval.Insert(0, SelectedIndices[0]); 
                            return interval;
                        }
                        // Go through and see if the index passed in was in the 
                        // selection model. If so, leave it out when constructing
                        // the new interval so it is de-selected. 
                        bool found = false; 
                        for (i = 0; i < _selectedIndices.Count; i++)
                        {
                            if (_selectedIndices[i] == index)
                                found = true; 
                            else if (_selectedIndices[i] != index)
                                interval.Insert(0, _selectedIndices[i]);
                        }
                        if (!found)
                        {
                            // Nothing from the selection model was de-selected. 
                            // Instead, the Ctrl key was held down and we're doing a  
                            // new add. 
                            interval.Insert(0, index);   
                        }
                        return interval;
                    }
                    // Ctrl+click with no previously selected items 
                    
                    interval.Insert(0, index); 
                    return interval;
                }
                
                // A single item was newly selected, add that to the selection interval.  
                interval.Insert(0, index); 
                return interval;
            }
            
            // A contiguous selection action has occurred. Figure out which new 
            // indices to add to the selection interval and return that. 
            var start = (!IsEmpty(SelectedIndices)) ? SelectedIndices[SelectedIndices.Count - 1] : 0; 
            var end = index; 
            if (start < end)
            {
                for (i = start; i <= end; i++)
                {
                    interval.Insert(0, i); 
                }
            }
            else 
            {
                for (i = start; i >= end; i--)
                {
                    interval.Insert(0, i); 
                }
            }
            return interval;
        }

        #endregion

        #region Drag methods

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global
        protected void DragStartHandler(Event e)
// ReSharper restore MemberCanBeMadeStatic.Global
// ReSharper restore MemberCanBePrivate.Global
        {
            DragEvent de = (DragEvent) e;
            
            //if (event.isDefaultPrevented())
            //    return;
            
            //var dragSource:DragSource = new DragSource();
            //AddDragData(dragSource);
            //DragManager.doDrag(this, 
            //                   dragSource, 
            //                   event, 
            //                   createDragIndicator(), 
            //                   0 /*xOffset*/, 
            //                   0 /*yOffset*/, 
            //                   0.5 /*imageAlpha*/, 
            //                   dragMoveEnabled);
        }

        /**
         *  
         *  Used to sort the selected indices during drag and drop operations.
         */
        private static int CompareValues(int a, int b)
        {
            return a - b;
        } 

// ReSharper disable MemberCanBePrivate.Global
        protected void DragCompleteHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            DragEvent de = (DragEvent) e;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public DisplayObject CreateDragIndicator()
        {
            DisplayObject dragIndicator = null;
            return dragIndicator;
        }

        ///<summary>
        ///</summary>
        ///<param name="dragSource"></param>
        public void AddDragData(DragSource dragSource)
        {
            
        }

        /**
         *  
         */
        private List<object> CopySelectedItemsForDragDrop()
        {
            // Copy the vector so that we don't modify the original
             // since SelectedIndices returns a reference.
            var draggedIndices = new List<int>(SelectedIndices);
            var result = new List<Object>(draggedIndices.Count);

            // Sort in the order of the data source
            draggedIndices.Sort(CompareValues);
            
            // Copy the items
            var count = draggedIndices.Count;
            for (var i = 0; i < count; i++) {
                result[i] = DataProvider.GetItemAt(draggedIndices[i]);
            }
            return result;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected virtual void ItemMouseDownHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            MouseEvent me = (MouseEvent) e;

            // Handle the fixup of selection
            int newIndex;
            if (e.CurrentTarget is IItemRenderer)
                newIndex = ((IItemRenderer)e.CurrentTarget).ItemIndex;
            else
                newIndex = DataGroup.GetContentChildIndex(e.CurrentTarget as DisplayListMember/*as IVisualElement*/);

            if (!AllowMultipleSelection)
            {
                // Single selection case, set the selectedIndex 
                IItemRenderer currentRenderer;
                if (CaretIndex >= 0)
                {
                    currentRenderer = DataGroup.GetContentChildAt(CaretIndex) as IItemRenderer;
                    if (null != currentRenderer)
                        currentRenderer.ShowsCaret = false;
                }
                
                // Check to see if we're deselecting the currently selected item 
                if (me.Control && SelectedIndex == newIndex)
                {
                    _pendingSelectionOnMouseUp = true;
                    _pendingSelectionCtrlKey = true;
                    _pendingSelectionShiftKey = me.Shift;
                }
                else
                    SetSelectedIndex(newIndex, true);
            }
            else 
            {
                // Don't commit the selection immediately, but wait to see if the user
                // is actually dragging. If they don't drag, then commit the selection
                if (IsItemIndexSelected(newIndex))
                {
                    _pendingSelectionOnMouseUp = true;
                    _pendingSelectionShiftKey = me.Shift;
                    _pendingSelectionCtrlKey = me.Control;
                }
                else
                {
                    SetSelectedIndices(CalculateSelectedIndices(newIndex, me.Shift, me.Control), true);
                }
            }
            
            // If selection is pending on mouse up then we have just moused down on
            // an item, part of an already commited selection.
            // However if we moused down on an item that's not currently selected,
            // we must commit the selection before trying to start dragging since
            // listeners may prevent the item from being selected.
            if (!_pendingSelectionOnMouseUp)
                ValidateProperties();

            DisplayListMember dlm = (DisplayListMember) e.Target;

            _mouseDownPoint = dlm.LocalToGlobal(me.LocalPosition);
            _mouseDownIndex = newIndex;

            var listenForDrag = (DragEnabled && null != SelectedIndices && SelectedIndices.IndexOf(newIndex) != -1);
            // Handle any drag gestures that may have been started
            if (listenForDrag)
            {
                // Listen for MOUSE_MOVE on the sandboxRoot.
                // The user may have cliked on the item renderer close
                // to the edge of the list, and we still want to start a drag
                // operation if they move out of the list.
                SystemManager.Instance.MouseMoveSignal.Connect(MouseMoveHandler);
            }

            if (_pendingSelectionOnMouseUp || listenForDrag)
            {
                SystemManager.Instance.MouseUpSignal.Connect(MouseUpHandler);
            }
        }

        private void MouseMoveHandler(object[] parameters)
        {
            var position = parameters[1];

            if (null == _mouseDownPoint || !DragEnabled)
                return;
            
            //var pt:Point = new Point(event.localX, event.localY);
            //pt = DisplayObject(event.target).localToGlobal(pt);
            
            //const DRAG_THRESHOLD:int = 5;
            
            //if (Math.abs(_mouseDownPoint.x - pt.x) > DRAG_THRESHOLD ||
            //    Math.abs(_mouseDownPoint.y - pt.y) > DRAG_THRESHOLD)
            //{
            //    var dragEvent:DragEvent = new DragEvent(DragEvent.DRAG_START);
            //    dragEvent.dragInitiator = this;
                
            //    var localMouseDownPoint:Point = this.globalToLocal(_mouseDownPoint);
                
            //    dragEvent.localX = localMouseDownPoint.x;
            //    dragEvent.localY = localMouseDownPoint.y;
            //    dragEvent.buttonDown = true;
                
            //    // We're starting a drag operation, remove the handlers
            //    // that are monitoring the mouse move, we don't need them anymore:
            //    dispatchEvent(dragEvent);

            //    // Finally, remove the mouse handlers
            //    RemoveMouseHandlersForDragStart();
            //}
        }

        private static void RemoveMouseHandlersForDragStart()
        {
            //// If dragging failed, but we had a pending selection, commit it here
            //if (_pendingSelectionOnMouseUp && !DragManager.isDragging)
            //{
            //    if (AllowMultipleSelection)
            //    {
            //        SetSelectedIndices(CalculateSelectedIndices(_mouseDownIndex, _pendingSelectionShiftKey, _pendingSelectionCtrlKey), true);
            //    }
            //    else
            //    {
            //        // Must be deselecting the current selected item.
            //        setSelectedIndex(NO_SELECTION, true);
            //    }
            //}

            //// Always clean up the flag, even if currently dragging.
            //_pendingSelectionOnMouseUp = false;
            
            //_mouseDownPoint = null;
            //_mouseDownIndex = -1;
            
            //systemManager.getSandboxRoot().removeEventListener(MouseEvent.MOUSE_MOVE, MouseMoveHandler, false);
            //systemManager.getSandboxRoot().removeEventListener(MouseEvent.MOUSE_UP, MouseUpHandler, false);
            //systemManager.getSandboxRoot().removeEventListener(SandboxMouseEvent.MOUSE_UP_SOMEWHERE, MouseUpHandler, false);
        }

        private static void MouseUpHandler(object[] parameters)
        {
            RemoveMouseHandlersForDragStart();
        }

        #endregion

        #region Drop methods

        /*private DropLocation CalculateDropLocation(Event e)
        {
            DragEvent de = (DragEvent) e;

            // Verify data format
            if (!Enabled || !de.DragSource.HasFormat("itemsByIndex"))
                return null;
            
            // Calculate the drop location
            return Layout.CalculateDropLocation(e);
        }*/

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public DisplayObject CreateDropIndicator()
        {
            DisplayObject dropIndicatorInstance = null;
            return dropIndicatorInstance;
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public DisplayObject DestroyDropIndicator()
        {
            DisplayObject dropIndicatorInstance = null; // TEMP
            //DisplayObject dropIndicatorInstance = Layout.dropIndicator;
            //if (null == dropIndicatorInstance)
            //    return null;
            
            //// Release the reference from the layout
            //layout.dropIndicator = null;
            
            //// Release it if it's a dynamic skin part
            //var count:int = numDynamicParts("dropIndicator");
            //for (var i:int = 0; i < count; i++)
            //{
            //    if (dropIndicatorInstance == getDynamicPartAt("dropIndicator", i))
            //    {
            //        // This was a dynamic part, remove it now:
            //        removeDynamicPartInstance("dropIndicator", dropIndicatorInstance);
            //        break;
            //    }
            //}
            return dropIndicatorInstance;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected void DragEnterHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            DragEvent de = (DragEvent) e;
            //if (e.DefaultPrevented)
            //    return;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected void DragOverHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            DragEvent de = (DragEvent) e;

            //if (e.DefaultPrevented)
            //    return;
        }

// ReSharper disable MemberCanBePrivate.Global
        protected void DragExitHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            if (e.DefaultPrevented)
                return;
            
            //// Hide if previously showing
            //layout.hideDropIndicator();
            
            //// Hide focus
            //drawFocus(false);
            //drawFocusAnyway = false;
            
            //// Destroy the dropIndicator instance
            //DestroyDropIndicator();
        }

// ReSharper disable MemberCanBePrivate.Global
        protected void DragDropHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            DragEvent de = (DragEvent) e;

            //if (event.isDefaultPrevented())
            //    return;
            
            //// Hide the drop indicator
            //layout.hideDropIndicator();
            //DestroyDropIndicator();
            
            //// Hide focus
            //drawFocus(false);
            //drawFocusAnyway = false;
            
            //// Get the dropLocation
            //var dropLocation:DropLocation = CalculateDropLocation(event);
            //if (!dropLocation)
            //    return;
            
            //// Find the dropIndex
            //var dropIndex:int = dropLocation.dropIndex;
            
            //// Make sure the manager has the appropriate action
            //DragManager.showFeedback(event.ctrlKey ? DragManager.COPY : DragManager.MOVE);
            
            //var dragSource:DragSource = event.dragSource;
            //var items:Vector.<Object> = dragSource.dataForFormat("itemsByIndex") as Vector.<Object>;

            //var caretIndex:int = -1;
            //if (dragSource.hasFormat("caretIndex"))
            //    caretIndex = event.dragSource.dataForFormat("caretIndex") as int;
            
            //// Clear the selection first to avoid extra work while adding and removing items.
            //// We will set a new selection further below in the method.
            //var indices:Vector.<int> = SelectedIndices; 
            //SetSelectedIndices(new Vector.<int>(), false);
            //validateProperties(); // To commit the selection
            
            //// If we are reordering the list, remove the items now,
            //// adjusting the dropIndex in the mean time.
            //// If the items are drag moved to this list from a different list,
            //// the drag initiator will remove the items when it receives the
            //// DragEvent.DRAG_COMPLETE event.
            //if (dragMoveEnabled &&
            //    event.action == DragManager.MOVE &&
            //    event.dragInitiator == this)
            //{
            //    // Remove the previously selected items
            //    indices.sort(compareValues);
            //    for (var i:int = indices.length - 1; i >= 0; i--)
            //    {
            //        if (indices[i] < dropIndex)
            //            dropIndex--;
            //        dataProvider.removeItemAt(indices[i]);
            //    }
            //}
            
            //// Drop the items at the dropIndex
            //var newSelection:Vector.<int> = new Vector.<int>();

            //// Update the selection with the index of the caret item
            //if (caretIndex != -1)
            //    newSelection.push(dropIndex + caretIndex);

            //var copyItems:Boolean = (event.action == DragManager.COPY);
            //for (i = 0; i < items.length; i++)
            //{
            //    // Get the item, clone if needed
            //    var item:Object = items[i];
            //    if (copyItems)
            //        item = CopyItemWithUid(item);

            //    // Copy the data
            //    dataProvider.addItemAt(items[i], dropIndex + i);

            //    // Update the selection
            //    if (i != caretIndex)
            //        newSelection.push(dropIndex + i);
            //}

            //// Set the selection
            //SetSelectedIndices(newSelection, false);

            //// Scroll the caret index in view
            //if (caretIndex != -1)
            //{
            //    // Sometimes we may need to scroll several times as for virtual layouts
            //    // this is not guaranteed to bring in the element in view the first try
            //    // as some items in between may not be loaded yet and their size is only
            //    // estimated.
            //    var delta:Point;
            //    var loopCount:int = 0;
            //    while (loopCount++ < 10)
            //    {
            //        validateNow();
            //        delta = layout.getScrollPositionDeltaToElement(dropIndex + caretIndex);
            //        if (!delta || (delta.x == 0 && delta.y == 0))
            //            break;
            //        layout.horizontalScrollPosition += delta.x;
            //        layout.verticalScrollPosition += delta.y;
            //    }
            //}
        }

        protected object CopyItemWithUid(object item)
        {
            return null;
        }

        #endregion

        #region Event handlers

        /**
         *  
         *  Called when an item has been added to this component.
         */
// ReSharper disable MemberCanBeMadeStatic.Local
        private void DataGroupRendererAddHandler(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
        {
            RendererExistenceEvent ree = (RendererExistenceEvent) e;
            //var index = ree.Index;
            var renderer = ree.Renderer;
            
            if (null == renderer)
                return;

            // creates stack overflow if renderer is not just a Component (for instnce HGroup):
            renderer.AddEventListener(MouseEvent.MOUSE_DOWN, ItemMouseDownHandler);
        }
        
        /**
         *  
         *  Called when an item has been removed from this component.
         */
// ReSharper disable MemberCanBeMadeStatic.Local
        private void DataGroupRendererRemoveHandler(Event e)
// ReSharper restore MemberCanBeMadeStatic.Local
        {
            RendererExistenceEvent ree = (RendererExistenceEvent) e;
            //var index = ree.Index;
            var renderer = ree.Renderer;
            
            if (null == renderer)
                return;

            // creates stack overflow if renderer is not just a Component (for instnce HGroup):
            renderer.RemoveEventListener(MouseEvent.MOUSE_DOWN, ItemMouseDownHandler);
        }

// ReSharper disable MemberCanBePrivate.Global
        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        private void EnsureIndexIsVisible(int index) // TEMP private
// ReSharper restore MemberCanBePrivate.Global
        {
            if (null == Layout)
                return;

            var spDelta = DataGroup.Layout.GetScrollPositionDeltaToElement(index);

            //Debug.Log("spDelta: " + spDelta);

            if (null != spDelta)
            {
                DataGroup.HorizontalScrollPosition += spDelta.X;
                DataGroup.VerticalScrollPosition += spDelta.Y;
            }
        }

        override protected void AdjustSelection(int index, bool add/*=false*/)
        {
            int i; 
            int curr; 
            var newInterval = new List<int>(); 
            IndexChangeEvent e; 
            
            if (SelectedIndex == NO_SELECTION || DoingWholesaleChanges)
            {
                // The case where one item has been newly added and it needs to be 
                // selected and careted because requireSelection is true. 
                if (null != DataProvider && DataProvider.Length == 1 && RequireSelection)
                {
                    newInterval.Add(0);
                    _selectedIndices = newInterval;   
                    SelectedIndex = 0; 
                    ItemShowingCaret(0, true); 
                    // If the selection properties have been adjusted to account for items that
                    // have been added or removed, send out a "valueCommit" event and 
                    // "caretChange" event so any bindings to them are updated correctly.
                     
                    DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
                    
                    e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE) {OldIndex = -1, NewIndex = CaretIndex};
                    DispatchEvent(e); 
                }
                return; 
            }
            
            // Ensure multiple and single selection are in-sync before adjusting  
            // selection. Sometimes if selection has been changed before adding/removing
            // an item, we may not have handled selection via invalidation, so in those 
            // cases, force a call to commitSelection() to validate and commit the selection. 
            if ((null == SelectedIndices && SelectedIndex > NO_SELECTION) ||
                (SelectedIndex > NO_SELECTION && null != SelectedIndices && SelectedIndices.IndexOf(SelectedIndex) == -1))
            {
                CommitSelection(true); 
            }
            
            // Handle the add or remove and adjust selection accordingly. 
            if (add)
            {
                if (null != SelectedIndices)
                {
                    for (i = 0; i < SelectedIndices.Count; i++)
                    {
                        curr = SelectedIndices[i];

                        // Adding an item above one of the selected items,
                        // bump the selected item up. 
                        if (curr >= index)
                            newInterval.Add(curr + 1);
                        else
                            newInterval.Add(curr);
                    }
                }
            }
            else
            {
                // Quick check to see if we're removing the only selected item
                // in which case we need to honor requireSelection. 
                if (null != SelectedIndices && !IsEmpty(SelectedIndices) && SelectedIndices.Count == 1 
                    && index == SelectedIndex && RequireSelection)
                {
                    //Removing the last item 
                    if (DataProvider.Length == 0)
                    {
                        newInterval = new List<int>(); 
                    }
                    else if (index == 0)
                    {
                        // We can't just set selectedIndex to 0 directly
                        // since the previous value was 0 and the new value is
                        // 0, so the setter will return early.
                        ProposedSelectedIndex = 0; 
                        InvalidateProperties();
                        return;
                    }    
                    else
                    {
                        newInterval.Add(0);  
                    }
                }
                else if (null != SelectedIndices)
                {    
                    for (i = 0; i < SelectedIndices.Count; i++)
                    {
                        curr = SelectedIndices[i]; 
                        // Removing an item above one of the selected items,
                        // bump the selected item down. 
                        if (curr > index)
                            newInterval.Add(curr - 1); 
                        else if (curr < index) 
                            newInterval.Add(curr);
                    }
                }
            }
            
            if (CaretIndex == SelectedIndex)
            {
                var oldIndex = CaretIndex; 
                CaretIndex = GetFirstItemValue(newInterval);
                e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE) {OldIndex = oldIndex, NewIndex = CaretIndex};
                DispatchEvent(e); 
            }
            else 
            {
                 ItemShowingCaret(CaretIndex, false); 
                CaretIndexAdjusted = true; 
                InvalidateProperties(); 
            }
            
            var oldIndices = SelectedIndices;  
            _selectedIndices = newInterval;
            SelectedIndex = GetFirstItemValue(newInterval);
            if (_selectedIndices != oldIndices)
            {
                SelectedIndexAdjusted = true; 
                InvalidateProperties(); 
            }
        }

        internal int FindStringLoop(string str, int startIndex, int stopIndex)
        {
            // Try to find the item based on the start and stop indices. 
            for (var i = startIndex; i != stopIndex; i++)
            {
                var itmStr = ItemToLabel(DataProvider.GetItemAt(startIndex));

                itmStr = itmStr.Substring(0, str.Length);
                if (str == itmStr || str.ToUpper() == itmStr.ToUpper())
                {
                   return startIndex;
                }
            }
            return -1;
        }

        #endregion

        protected override void KeyDownHandler(Event e)
        {
            base.KeyDownHandler(e);

            //Debug.Log("Key down: " + e);
            KeyboardEvent ke = (KeyboardEvent) e;

            if (null == DataProvider || null == Layout || e.DefaultPrevented)
                return;

            // In lue of a formal item editor architecture (pending), we will
            // defer all keyboard events to the target if the target happens to 
            // be an editable input control.
            //if (isEditableTarget(event.target))
            //    return;
            
            // 1. Was the space bar hit? 
            // Hitting the space bar means the current caret item, 
            // that is the item currently in focus, is being 
            // selected. 
            if (ke.KeyCode == KeyCode.Space)
            {
                SetSelectedIndex(CaretIndex, true); 
                e.PreventDefault();
                return; 
            }

            // 2. Or was an alphanumeric key hit? 
            // Hitting an alphanumeric key causes List's
            // findKey method to run to find a matching 
            // item in the dataProvider whose first char 
            // matches the keystroke. 
            //if (findKey(event.charCode))
            //{
            //    event.preventDefault();
            //    return;
            //}
                
            // 3. Was a navigation key hit (like an arrow key,
            // or Shift+arrow key)?  
            // Delegate to the layout to interpret the navigation
            // key and adjust the selection and caret item based
            // on the combination of keystrokes encountered.      
            AdjustSelectionAndCaretUponNavigation(e); 
        }

// ReSharper disable MemberCanBePrivate.Global
        protected void AdjustSelectionAndCaretUponNavigation(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            //Debug.Log("AdjustSelectionAndCaretUponNavigation");

            KeyboardEvent ke = (KeyboardEvent) e;

            // Some unrecognized key stroke was entered, return. 
            if (!NavigationUnitUtil.IsNavigationUnit(ke.KeyCode))
                return;

            NavigationUnit navigationUnit = NavigationUnitUtil.GetNavigationUnit(ke.KeyCode);
            //Debug.Log("navigationUnit:" + navigationUnit);
            //Debug.Log("CaretIndex:" + CaretIndex);
                
            var proposedNewIndex = Layout.GetNavigationDestinationIndex(CaretIndex, navigationUnit, ArrowKeysWrapFocus);

            //Debug.Log("proposedNewIndex:" + proposedNewIndex);
            
            // Note that the KeyboardEvent is canceled even if the current selected or in focus index
            // doesn't change because we don't want another component to start handling these
            // events when the index reaches a limit.
            if (proposedNewIndex == -1)
                return;
                
            e.PreventDefault(); 
            
            // Contiguous multi-selection action. Create the new selection
            // interval.   
            if (_allowMultipleSelection && ke.Shift && null != SelectedIndices)
            {
                var startIndex = GetLastSelectedIndex(); 
                var newInterval = new List<int>();  
                int i; 
                if (startIndex <= proposedNewIndex)
                {
                    for (i = startIndex; i <= proposedNewIndex; i++)
                    {
                        newInterval.Insert(0, i); 
                    }
                }
                else 
                {
                    for (i = startIndex; i >= proposedNewIndex; i--)
                    {
                        newInterval.Insert(0, i); 
                    }
                }
                SetSelectedIndices(newInterval, true);
                EnsureIndexIsVisible(proposedNewIndex); 
            }
            // Entering the caret state with the Ctrl key down 
            else if (ke.Control)
            {
                var oldCaretIndex = CaretIndex;
                SetCurrentCaretIndex(proposedNewIndex);
                var ice = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE)
                              {
                                  OldIndex = oldCaretIndex,
                                  NewIndex = CaretIndex
                              };
                DispatchEvent(ice);    
                EnsureIndexIsVisible(proposedNewIndex); 
            }
            // Its just a new selection action, select the new index.
            else
            {
                SetSelectedIndex(proposedNewIndex, true);
                EnsureIndexIsVisible(proposedNewIndex);
            }
        }
    }
}
