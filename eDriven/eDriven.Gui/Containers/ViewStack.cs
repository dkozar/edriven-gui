using System;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Containers
{
    ///<summary>
    /// A container displaying only one child at a time
    ///</summary>
    public class ViewStack : Group, ISelectableList
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        ///<summary>
        ///</summary>
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        // ReSharper disable InconsistentNaming
        // ReSharper disable MemberCanBePrivate.Global
        #pragma warning disable 1591
        public static string NEXT = "next";
        public static string PREVIOUS = "previous";
        #pragma warning restore 1591
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore InconsistentNaming
        
        ///<summary>
        ///</summary>
        public ViewStack()
        {
            //ClipAndEnableScrolling = false; / a must
            AddEventListener(ChildExistenceChangedEvent.CHILD_ADD, ChildAddHandler);
            AddEventListener(ChildExistenceChangedEvent.CHILD_REMOVE, ChildRemoveHandler);
        }

        public override void Dispose()
        {
            base.Dispose();
            AddEventListener(ChildExistenceChangedEvent.CHILD_ADD, ChildAddHandler);
            AddEventListener(ChildExistenceChangedEvent.CHILD_REMOVE, ChildRemoveHandler);
        }

        private void ChildAddHandler(Event e)
        {
            ChildExistenceChangedEvent cece = (ChildExistenceChangedEvent) e;
            var child = cece.RelatedObject;
            var index = GetChildIndex(child);

            //Debug.Log("ChildAddHandler: " + child);

            if (child is InteractiveComponent)
            {
                var uiChild = (InteractiveComponent)child;
                // ViewStack creates all of its children initially invisible.
                // They are made as they become the selected child.
                uiChild.Visible = false;
                //Debug.Log("Set invisible: " + uiChild);
            }
            //if (child is INavigatorContent)
            //{
            //    child.addEventListener("labelChanged", navigatorChildChangedHandler);
            //    child.addEventListener("iconChanged", navigatorChildChangedHandler);
            //}

            // If we just created the first child and no selected index has
            // been proposed, then propose this child to be selected.
            if (NumberOfChildren == 1 && proposedSelectedIndex == -1)
            {
                SelectedIndex = 0;
            }
            else if (index <= _selectedIndex && NumberOfChildren > 1 && proposedSelectedIndex == -1)         
            {
                SelectedIndex++;
            }
        }

        private void ChildRemoveHandler(Event e)
        {
            ChildExistenceChangedEvent cece = (ChildExistenceChangedEvent) e;
            var child = cece.RelatedObject;
            var index = GetChildIndex(child);
            
            //if (child is INavigatorContent)
            //{
            //    child.removeEventListener("labelChanged", navigatorChildChangedHandler);
            //    child.removeEventListener("iconChanged", navigatorChildChangedHandler);
            //}

            // Handle the simple case.
            if (index > _selectedIndex)
                return;

            var currentSelectedIndex = _selectedIndex;

            // This matches one of the two conditions:
            // 1. a view before the current was deleted, or
            // 2. the current view was deleted and it was also
            //    at the end of the stack.
            // In both cases, we need to decrement selectedIndex.
            if (index < currentSelectedIndex ||
                currentSelectedIndex == (NumberOfChildren - 1))
            {
                // If the selectedIndex was already 0, it should go to -1.
                // -1 is a special value; in order to avoid runtime errors
                // in various methods, we need to skip the range checking in
                // commitSelectedIndex() and set _selectedIndex explicitly here.
                if (currentSelectedIndex == 0)
                {
                    SelectedIndex = -1;
                    _selectedIndex = -1;
                }
                else
                {
                    SelectedIndex--;
                }
            }
            else if (index == currentSelectedIndex)
            {
                // If we're deleting the currentSelectedIndex and there is another
                // child after it, it will become the new selected child so we
                // need to make sure it is instantiated.
                _needToInstantiateSelectedChild = true;
                InvalidateProperties();
            }
        }

        #region Properties

/**
         *  
         *  autoLayout is always true for ViewStack.
         */
        public override bool AutoLayout
        {
            get
            {
                return true;
            }
            set { }
        }

        private bool _resizeToContent;
        ///<summary>
        /// Resize viewstack to each child's content (on selected index change)
        ///</summary>
        public bool ResizeToContent
        {
            get
            {
                return _resizeToContent;
            }
            set
            {
                _resizeToContent = value;

                if (value)
                    InvalidateSize();
            }
        }

        //----------------------------------
        //  contentX
        //----------------------------------

        /**
         *  The x coordinate of the area of the ViewStack container
         *  in which content is displayed, in pixels.
         *  The default value is equal to the value of the
         *  <code>paddingLeft</code> style property,
         *  which has a default value of 0.
         *
         *  Override the <code>get()</code> method if you do not want
         *  your content to start layout at x = 0.
         */
        protected int ContentX
        {
            get
            {
                return (int) GetStyle("paddingLeft");
            }
        }

        //----------------------------------
        //  contentY
        //----------------------------------

        /**
         *  The y coordinate of the area of the ViewStack container
         *  in which content is displayed, in pixels.
         *  The default value is equal to the value of the
         *  <code>paddingTop</code> style property,
         *  which has a default value of 0.
         *
         *  Override the <code>get()</code> method if you do not want
         *  your content to start layout at y = 0.
         */
        protected int ContentY
        {
            get
            {
                return (int)GetStyle("paddingTop");
            }
        }

        

        #endregion

        #region Members

/**
         *  
         */
        private bool _needToInstantiateSelectedChild = false;

        #endregion

        protected override void CreationComplete()
        {
            base.CreationComplete();

            if (-1 == SelectedIndex && NumberOfChildren > 0)
                SelectedIndex = 0;
        }

        public override void SetFocus()
        {
            if (null != SelectedChild)
                ((InteractiveComponent)SelectedChild).SetFocus();
        }

        public DisplayListMember SelectedChild
        {
            get
            {
                if (-1 == SelectedIndex)
                    return null;

                ////Debug.Log("Retreiving SelectedChild: " + _selectedIndex);
                //if (SelectedIndex < NumberOfChildren)
                //    return Children[SelectedIndex];

                //throw new ListBaseException(ListBaseException.IndexOutOfRange);
                return GetChildAt(SelectedIndex);
            }
// ReSharper disable UnusedMember.Global
            set
// ReSharper restore UnusedMember.Global
            {
                var newIndex = GetChildIndex(value);

                if (newIndex >= 0 && newIndex < NumberOfChildren)
                    SelectedIndex = newIndex;
            }
        }

        //----------------------------------
        //  selectedIndex
        //----------------------------------

        /**
         *  
         *  Storage for the selectedIndex property.
         */
        private int _selectedIndex = -1;

        /**
         *  
         */
        private int proposedSelectedIndex = -1;

        /**
         *  
         */
        private int initialSelectedIndex = -1;

        /**
         *  
         *  Store the last selectedIndex
         */
        private int lastIndex = -1;

        /**
         *  
         *  Whether a change event has to be dispatched in commitProperties()
         */
        private bool dispatchChangeEventPending = false;

        /**
         *  The zero-based index of the currently visible child container.
         *  Child indexes are in the range 0, 1, 2, ..., n - 1,
         *  where <i>n</i> is the number of children.
         *  The default value is 0, corresponding to the first child.
         *  If there are no children, the value of this property is <code>-1</code>.
         * 
         *  <p><strong>Note:</strong> When you add a new child to a ViewStack 
         *  container, the <code>selectedIndex</code> property is automatically 
         *  adjusted, if necessary, so that the selected child remains selected.</p>
         */
        public int SelectedIndex
        {
            get
            {
                return proposedSelectedIndex == -1 ?
                   _selectedIndex :
                   proposedSelectedIndex;
            }
            set
            {
                // Bail if the index isn't changing.
                if (value == _selectedIndex)
                    return;

                // Propose the specified value as the new value for selectedIndex.
                // It gets applied later when measure() calls CommitSelectedIndex().
                // The proposed value can be "out of range", because the children
                // may not have been created yet, so the range check is handled
                // in CommitSelectedIndex(), not here. Other calls to this setter
                // can change the proposed index before it is committed. Also,
                // ChildAddHandler() proposes a value of 0 when it creates the first
                // child, if no value has yet been proposed.
                proposedSelectedIndex = value;
                InvalidateProperties();

                //// Set a flag which will cause the HistoryManager to save state
                //// the next time measure() is called.
                //if (historyManagementEnabled && _selectedIndex != -1 && !bInLoadState)
                //    bSaveState = true;

                DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
            }
        }

        /// <summary>
        /// Shows the previous view
        /// </summary>
        public void Previous()
        {
            //Debug.Log(string.Format("ViewStack.Previous | SelectedIndex: {0}, NumberOfChildren: {1}", SelectedIndex, NumberOfChildren));
            if (SelectedIndex > 0) {
                SelectedIndex--;
                DispatchEvent(new Event(PREVIOUS));
            }
        }

        /// <summary>
        /// Shows the next view
        /// </summary>
        public void Next()
        {
            //Debug.Log(string.Format("ViewStack.Next | SelectedIndex: {0}, NumberOfChildren: {1}", SelectedIndex, NumberOfChildren));
            if (SelectedIndex < NumberOfChildren - 1)
            {
                SelectedIndex++;
                DispatchEvent(new Event(NEXT));
            }
        }

        public override DisplayListMember AddChild(DisplayListMember child)
        {
            DisplayListMember c = base.AddChild(child);
            //c.Visible = false; // invisible by default
            //_childrenChanged = true;
            //InvalidateProperties();
            return c;
        }

        public override DisplayListMember AddChildAt(DisplayListMember child, int index)
        {
            DisplayListMember obj = base.AddChildAt(child, index);
            obj.Visible = false; // invisible by default
            //_childrenChanged = true;
            //InvalidateProperties();
            InternalDispatchEvent(CollectionEventKind.ADD, obj, index);
            return obj;
        }

        override public DisplayListMember RemoveChild(DisplayListMember child)
        {
            int index = GetChildIndex(child);
            DisplayListMember obj = RemoveChildAt(index);
            //_childrenChanged = true;
            //InvalidateProperties();
            InternalDispatchEvent(CollectionEventKind.REMOVE, obj, index);
            return obj;
        }

        public override void RemoveAllChildren()
        {
            base.RemoveAllChildren();
            InternalDispatchEvent(CollectionEventKind.RESET, null, -1);
        }

        private float? _vsMinWidth;
        private float? _vsMinHeight;
        private float? _vsPreferredWidth;
        private float? _vsPreferredHeight;

        protected override void CommitProperties()
        {
            if (proposedSelectedIndex != -1)
            {
                CommitSelectedIndex(proposedSelectedIndex);
                proposedSelectedIndex = -1;
            }

            if (_needToInstantiateSelectedChild)
            {
                //instantiateSelectedChild();
                _needToInstantiateSelectedChild = false;
                if (SelectedChild is IInvalidating)
                    ((IInvalidating)SelectedChild).InvalidateSize();

                //Debug.Log("Invalidating size...");
                InvalidateSize();
                InvalidateDisplayList();
                //InvalidateParentSizeAndDisplayList();
            }

            // Dispatch the change event only after the child has been
            // instantiated.
            if (dispatchChangeEventPending)
            {
                DispatchChangeEvent(lastIndex, _selectedIndex);
                dispatchChangeEventPending = false;
            }
        }

        protected override void Measure()
        {
            //Debug.Log(string.Format("Measure..."));

            base.Measure();
        
            // A ViewStack measures itself based only on its selectedChild.
            // The minimum, maximum, and preferred sizes are those of the
            // selected child, plus the borders and margins.
            float minWidth = 0;
            float minHeight = 0;
            float preferredWidth = 0;
            float preferredHeight = 0;

            // Only measure once.  Thereafter, we'll just use cached values.
            //
            // We need to copy the cached values into the measured fields
            // again to handle the case where scaleX or scaleY is not 1.0.
            // When the ViewStack is zoomed, code in Component.measureSizes
            // scales the measuredWidth/Height values every time that
            // measureSizes is called.  (bug 100749)
            if (null != _vsPreferredWidth && !_resizeToContent)
            {
                MeasuredMinWidth = (float) _vsMinWidth;
                MeasuredMinHeight = (float) _vsMinHeight;
                MeasuredWidth = (float) _vsPreferredWidth;
                MeasuredHeight = (float) _vsPreferredHeight;
                return;
            }

            if (NumberOfChildren > 0 && SelectedIndex != -1)
            {
                Component child = (Component) GetChildAt(SelectedIndex);

                minWidth = child.MinWidth;
                preferredWidth = child.GetExplicitOrMeasuredWidth();

                minHeight = child.MinHeight;
                preferredHeight = child.GetExplicitOrMeasuredHeight();
            }

            float wPadding = (int)GetStyle("paddingLeft") + (int)GetStyle("paddingRight");
            minWidth += wPadding;
            preferredWidth += wPadding;

            float hPadding = (int)GetStyle("paddingTop") + (int)GetStyle("paddingBottom");
            minHeight += hPadding;
            preferredHeight += hPadding;

            MeasuredMinWidth = minWidth;
            MeasuredMinHeight = minHeight;
            MeasuredWidth = preferredWidth;
            MeasuredHeight = preferredHeight;

            //Debug.Log(string.Format("MeasuredWidth: {0}, MeasuredHeight: {1}", MeasuredWidth, MeasuredHeight));

            // Don't remember sizes if we don't have any children
            if (NumberOfChildren == 0)
                return;

            _vsMinWidth = minWidth;
            _vsMinHeight = minHeight;
            _vsPreferredWidth = preferredWidth;
            _vsPreferredHeight = preferredHeight;
        }

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);

            //var nChildren = NumberOfChildren;
            var w = ContentWidth;
            var h = ContentHeight;
            var left = ContentX;
            var top = ContentY;

            // Stretch the selectedIndex to fill our size
            if (_selectedIndex != -1)
            {
                //Debug.Log("Handling the selected index");
                InvalidationManagerClient child = (InvalidationManagerClient) GetChildAt(_selectedIndex);

                var newWidth = w;
                var newHeight = h;

                if (null != child.PercentWidth)
                {
                    if (newWidth > child.MaxWidth)
                        newWidth = child.MaxWidth;
                }
                else
                {
                    if (newWidth > child.ExplicitWidth)
                        newWidth = (float) child.ExplicitWidth;
                }

                if (null != child.PercentHeight)
                {
                    if (newHeight > child.MaxHeight)
                        newHeight = child.MaxHeight;
                }
                else
                {
                    if (newHeight > child.ExplicitHeight)
                        newHeight = (float) child.ExplicitHeight;
                }

                // Don't send events for the size/move. The set visible below
                if (child.Width != newWidth || child.Height != newHeight)
                {
                    child.SetActualSize(newWidth, newHeight);
                    SetActualSize(newWidth+left, newHeight+top); //this!
                }
                if (child.X != left || child.Y != top)
                    child.Move(left, top);

                // Now that the child is properly sized and positioned it
                // can be shown.
                //Debug.Log("Set visible: " + child);
                child.Visible = true;
            }
        }

        #region Methods

        /**
         *  Commits the selected index. This function is called during the commit 
         *  properties phase when the selectedIndex (or selectedItem) property
         *  has changed.
         */
// ReSharper disable MemberCanBePrivate.Global
        protected void CommitSelectedIndex(int newIndex)
// ReSharper restore MemberCanBePrivate.Global
        {
            // The selectedIndex must be -1 if there are no children,
            // even if a selectedIndex has been proposed.
            if (NumberOfChildren == 0)
            {
                _selectedIndex = -1;
                return;
            }

            // If there are children, ensure that the new index is in bounds.
            if (newIndex < 0)
                newIndex = 0;
            else if (newIndex > NumberOfChildren - 1)
                newIndex = NumberOfChildren - 1;

            //// Stop all currently playing effects
            //if (lastIndex != -1 && lastIndex < NumberOfChildren)
            //    Component(GetChildAt(lastIndex)).endEffectsStarted();
            
            //if (_selectedIndex != -1)
            //    (selectedChild as Component).endEffectsStarted();

            // Remember the old index.
            lastIndex = _selectedIndex;

            // Bail if the index isn't changing.
            if (newIndex == lastIndex)
                return;

            // Commit the new index.
            _selectedIndex = newIndex;

            // Remember our initial selected index so we can
            // restore to our default state when the history
            // manager requests it.
            if (initialSelectedIndex == -1)
                initialSelectedIndex = _selectedIndex;

            // Only dispatch a change event if we're going to and from
            // a valid index
            if (lastIndex != -1 && newIndex != -1)
                dispatchChangeEventPending = true;

            //var listenForEffectEnd = false;

            if (lastIndex != -1 && lastIndex < NumberOfChildren)
            {
                var currentChild = (InvalidationManagerClient)GetChildAt(lastIndex);

                currentChild.SetVisible(false, true); // Hide the current child

                //if (currentChild.getStyle("hideEffect"))
                //{
                //    var hideEffect:Effect = EffectManager.lastEffectCreated; // This should be the hideEffect

                //    if (hideEffect)
                //    {
                //        hideEffect.addEventListener(EffectEvent.EFFECT_END, hideEffectEndHandler);
                //        listenForEffectEnd = true;
                //    }
                //}
            }

            // If we don't have to wait for a hide effect to finish
            //if (!listenForEffectEnd)
            //    hideEffectEndHandler(null);
            _needToInstantiateSelectedChild = true;
        }

        /**
         *  
         */
        private void DispatchChangeEvent(int oldIndex, int newIndex)
        {
            var e = new IndexChangeEvent(Event.CHANGE)
                        {
                            OldIndex = oldIndex,
                            NewIndex = newIndex,
                            RelatedObject = GetChildAt(newIndex)
                        };
            DispatchEvent(e);
        }

        private void InternalDispatchEvent(CollectionEventKind kind, object item/* = null*/, int location/* = -1*/)
        {
            // copied from ArrayList
            //Debug.Log(string.Format("InternalDispatchEvent: {0}, {1}, {2}", kind, item, location));
            if (HasEventListener(CollectionEvent.COLLECTION_CHANGE))
            {
                var ce = new CollectionEvent(CollectionEvent.COLLECTION_CHANGE) { Kind = kind };
                ce.Items.Add(item);
                ce.Location = location;
                DispatchEvent(ce);
            }

            // now dispatch a complementary PropertyChangeEvent
            if (HasEventListener(PropertyChangeEvent.PROPERTY_CHANGE) &&
               (kind == CollectionEventKind.ADD || kind == CollectionEventKind.REMOVE))
            {
                var objEvent = new PropertyChangeEvent(PropertyChangeEvent.PROPERTY_CHANGE) { Property = location.ToString() };
                if (kind == CollectionEventKind.ADD)
                    objEvent.NewValue = item;
                else
                    objEvent.OldValue = item;
                DispatchEvent(objEvent);
            }
        }

        #endregion


        #region Implementation of IList

        /// <summary>
        /// The number of items in this collection. 0 means no items while -1 means the length is unknown. 
        /// </summary>
        public int Length
        {
            get { return NumberOfChildren; }
        }

        /// <summary>
        /// Adds the specified item to the end of the list
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(object item)
        {
            AddChild(item as DisplayListMember);
        }

        /// <summary>
        /// Adds the item at the specified index
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public void AddItemAt(object item, int index)
        {
            AddChildAt(item as DisplayListMember, index);
        }

        ///<summary>
        /// Gets the item at the specified index
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        public object GetItemAt(int index)
        {
            return GetChildAt(index);
        }

        ///<summary>
        /// Returns the index of the item if it is in the list such that GetItemAt(index) == item
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public int GetItemIndex(object item)
        {
            return GetChildIndex(item as DisplayListMember);
        }

        ///<summary>
        /// Notifies the view that an item has been updated
        ///</summary>
        ///<param name="item">The item being updated</param>
        ///<param name="property">The property being updated</param>
        ///<param name="oldValue">Old value</param>
        ///<param name="newValue">New value</param>
        public void ItemUpdated(object item, object property, object oldValue, object newValue)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        /// Removes all items from the list
        ///</summary>
        public void RemoveAll()
        {
            // do nothing
        }

        ///<summary>
        /// Removes the item at the specified index and returns it.<br/>
        /// Any items that were after this index are now one index earlier.
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        public object RemoveItemAt(int index)
        {
            return RemoveChildAt(index);
        }

        ///<summary>
        /// Places the item at the specified index.<br/>
        /// If an item was already at that index the new item will replace it and it will be returned.
        ///</summary>
        ///<param name="item"></param>
        ///<param name="index"></param>
        ///<returns></returns>
        public object SetItemAt(object item, int index)
        {
            var result = RemoveChildAt(index);
            AddChildAt(item as DisplayListMember,index);
            return result;
        }

        ///<summary>
        /// Returns an array of items that is populated in the same order
        ///</summary>
        ///<returns></returns>
        public object[] ToArray()
        {
            var result = new System.Collections.Generic.List<DisplayListMember>();
            for (var i =0;i<NumberOfChildren;i++)
            {
                result.Add(GetChildAt(i));
            }
            return result.ToArray();
        }

        #endregion
    }
}