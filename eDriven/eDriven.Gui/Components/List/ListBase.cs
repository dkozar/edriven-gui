using eDriven.Gui.Data;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Components
{
	///<summary>
	/// Base class for all components that support selection
	///</summary>
	/// 
	[Event(Name = IndexChangeEvent.CHANGING, Type = typeof(IndexChangeEvent), Bubbles = false)]
	[Event(Name = IndexChangeEvent.CHANGE, Type = typeof(IndexChangeEvent), Bubbles = false)]
	[Event(Name = IndexChangeEvent.CARET_CHANGE, Type = typeof(IndexChangeEvent), Bubbles = false)]

// ReSharper disable UnusedMember.Global
	public class ListBase : SkinnableDataContainer
// ReSharper restore UnusedMember.Global
	{
		// ReSharper disable UnusedMember.Global
		// ReSharper disable InconsistentNaming
		
		///<summary>
		///</summary>
		public const int NO_SELECTION = -1;
		
		/**
		 *  
		 *  Static constant representing no proposed selection.
		 */
		internal const int NO_PROPOSED_SELECTION = -2;
		
		/**
		 *  
		 *  Static constant representing no item in focus. 
		 */
		private const int NO_CARET = -1;
		
		/**
		 *  
		 */
		internal const int CUSTOM_SELECTED_ITEM = -3;

		// ReSharper restore InconsistentNaming
		// ReSharper restore UnusedMember.Global

		/**
		 *  
		 */
		internal bool AllowCustomSelectedItem;
		
		/**
		 *  
		 */
		internal bool DispatchChangeAfterSelection;

		/**
		 *  
		 */
		internal bool DoingWholesaleChanges;

		/**
		 *  
		 */
		private bool _dataProviderChanged;

		/// <summary>
		/// Data provider
		/// </summary>
		public override IList DataProvider
		{
			get
			{
				return base.DataProvider;
			}
			set
			{
				if (null != DataProvider)
					DataProvider.RemoveEventListener(CollectionEvent.COLLECTION_CHANGE, DataProviderCollectionChangeHandler);

				_dataProviderChanged = true;
				DoingWholesaleChanges = true;

				// ensure that our listener is added before the dataGroup which adds a listener during
				// the base class setter if the dataGroup already exists.  If the dataGroup isn't
				// created yet, then we still be first.
				if (null != value)
					value.AddEventListener(CollectionEvent.COLLECTION_CHANGE, DataProviderCollectionChangeHandler);

				base.DataProvider = value;
				InvalidateProperties();
			}
		}

		///<summary>
		///</summary>
		public bool ArrowKeysWrapFocus;

		//----------------------------------
		//  caretIndex
		//----------------------------------
		
		/**
		 *  
		 */
		private int _caretIndex = NO_CARET; 

		/**
		 *  Item that is currently in focus. 
		 *
		 *  Default: -1
		 */
		///<summary>
		///</summary>
		public int CaretIndex
		{
			get
			{
				return _caretIndex;
			}
			internal set
			{
				_caretIndex = value;
			}
		}

		private bool _labelFieldOrFunctionChanged;

		private string _labelField = "Label";
		///<summary>
		///</summary>
		public virtual string LabelField
		{
			get { 
				return _labelField;
			}
			set
			{
				if (value == _labelField)
					return;

				_labelField = value;
				_labelFieldOrFunctionChanged = true;
				InvalidateProperties();
			}
		}

		private LabelFunction _labelFunction;
		///<summary>
		///</summary>
		public virtual LabelFunction LabelFunction
		{
			get { 
				return _labelFunction;
			}
			set
			{
				if (value == _labelFunction)
					return;

				_labelFunction = value;
				_labelFieldOrFunctionChanged = true;
				InvalidateProperties();
			}
		}

		private bool _requireSelectionChanged;
		private bool _requireSelection;
		///<summary>
		///</summary>
		public virtual bool RequireSelection
		{
			get { 
				return _requireSelection;
			}
			set
			{
				if (value == _requireSelection)
					return;

				_requireSelection = value;

				// We only need to update if the value is changing 
				// from false to true
				if (value)
				{
					_requireSelectionChanged = true;
					InvalidateProperties();
				}
			}
		}

		#region SelectedIndex

/**
		 *  
		 *  The proposed selected index. This is a temporary variable that is
		 *  used until the selected index is committed.
		 */
		protected int ProposedSelectedIndex = NO_PROPOSED_SELECTION;
		
		/** 
		 *  
		 *  Flag that is set when the selectedIndex has been adjusted due to
		 *  items being added or removed. When this flag is true, the value
		 *  of the selectedIndex has changed, but the actual selected item
		 *  is the same. This flag is cleared in commitProperties().
		 */
		protected bool SelectedIndexAdjusted;
		
		/** 
		 *  
		 *  Flag that is set when the caretIndex has been adjusted due to
		 *  items being added or removed. This flag is cleared in 
		 *  commitProperties().
		 */
		protected bool CaretIndexAdjusted;
		
		/**
		 *  
		 *  Internal storage for the selectedIndex property.
		 */
		private int _selectedIndex = NO_SELECTION;

		/**
		 *  The 0-based index of the selected item, or -1 if no item is selected.
		 *  Setting the <code>selectedIndex</code> property deselects the currently selected
		 *  item and selects the data item at the specified index.
		 *
		 *  <p>The value is always between -1 and (<code>dataProvider.length</code> - 1). 
		 *  If items at a lower index than <code>selectedIndex</code> are 
		 *  removed from the component, the selected index is adjusted downward
		 *  accordingly.</p>
		 *
		 *  <p>If the selected item is removed, the selected index is set to:</p>
		 *
		 *  <ul>
		 *    <li>-1 if <code>requireSelection</code> = <code>false</code> 
		 *     or there are no remaining items.</li>
		 *    <li>0 if <code>requireSelection</code> = <code>true</code> 
		 *     and there is at least one item.</li>
		 *  </ul>
		 *
		 *  <p>When the user changes the <code>selectedIndex</code> property by interacting with the control,
		 *  the control dispatches the <code>change</code> and <code>changing</code> events. 
		 *  When you change the value of the <code>selectedIndex</code> property programmatically,
		 *  it dispatches the <code>valueCommit</code> event.</p>
		 */
		public virtual int SelectedIndex
		{
			get
			{
				if (ProposedSelectedIndex != NO_PROPOSED_SELECTION)
					return ProposedSelectedIndex;
					
				return _selectedIndex;
			}
			set
			{
				SetSelectedIndex(value, false);
			}
		}
	  
		/**
		 *  
		 *  Used internally to specify whether the selectedIndex changed programmatically or due to 
		 *  user interaction. 
		 * 
		 *  Param: dispatchChangeEvent if true, the component will dispatch a "change" event if the
		 *  value has changed. Otherwise, it will dispatch a "valueCommit" event. 
		 */
// ReSharper disable MemberCanBePrivate.Global
		internal void SetSelectedIndex(int value, bool dispatchChangeEvent/* = false*/)
// ReSharper restore MemberCanBePrivate.Global
		{
			if (value == _selectedIndex)
				return;
			
			if (dispatchChangeEvent)
				DispatchChangeAfterSelection = true;

			ProposedSelectedIndex = value;
			InvalidateProperties();
		}

		#endregion

		#region SelectedItem

		/**
		 *  
		 */
		protected object PendingSelectedItem;

		/**
		 *  
		 */
		private object _selectedItem;

		/**
		 *  The item that is currently selected. 
		 *  Setting this property deselects the currently selected 
		 *  item and selects the newly specified item.
		 *
		 *  <p>Setting <code>selectedItem</code> to an item that is not 
		 *  in this component results in no selection, 
		 *  and <code>selectedItem</code> being set to <code>undefined</code>.</p>
		 * 
		 *  <p>If the selected item is removed, the selected item is set to:</p>
		 *
		 *  <ul>
		 *    <li><code>undefined</code> if <code>requireSelection</code> = <code>false</code> 
		 *      or there are no remaining items.</li>
		 *    <li>The first item if <code>requireSelection</code> = <code>true</code> 
		 *      and there is at least one item.</li>
		 *  </ul>
		 *
		 *  <p>When the user changes the <code>selectedItem</code> property by interacting with the control,
		 *  the control dispatches the <code>change</code> and <code>changing</code> events. 
		 *  When you change the value of the <code>selectedItem</code> property programmatically,
		 *  it dispatches the <code>valueCommit</code> event.</p>
		 */
		///<summary>
		///</summary>
		public object SelectedItem
		{
			get
			{
				if (null != PendingSelectedItem)
					return PendingSelectedItem;

				if (AllowCustomSelectedItem && _selectedIndex == CUSTOM_SELECTED_ITEM)
					return _selectedItem;
				
				if (_selectedIndex == NO_SELECTION || DataProvider == null)
				   return null;

				return DataProvider.Length > _selectedIndex ? DataProvider.GetItemAt(_selectedIndex) : null;
			}
			set
			{
				SetSelectedItem(value, false);
			}
		}

		/**
		 *  
		 *  Used internally to specify whether the selectedItem changed programmatically or due to 
		 *  user interaction. 
		 * 
		 *  Param: dispatchChangeEvent if true, the component will dispatch a "change" event if the
		 *  value has changed. Otherwise, it will dispatch a "valueCommit" event. 
		 */
// ReSharper disable MemberCanBePrivate.Global
		internal void SetSelectedItem(object value, bool dispatchChangeEvent/* = false*/)
// ReSharper restore MemberCanBePrivate.Global
		{
			if (_selectedItem == value)
				return;
			
			if (dispatchChangeEvent)
				DispatchChangeAfterSelection = true;
			
			PendingSelectedItem = value;
			InvalidateProperties();
		}
		
		#endregion

		protected override void CommitProperties()
		{
			IndexChangeEvent e; 
			bool changedSelection = false;
			
			base.CommitProperties();
			
			if (_dataProviderChanged)
			{
				_dataProviderChanged = false;
				DoingWholesaleChanges = false;
			
				if (_selectedIndex >= 0 && null != DataProvider && _selectedIndex < DataProvider.Length)
				   ItemSelected(_selectedIndex, true);
				else if (_requireSelection)
				   ProposedSelectedIndex = 0;
				else
				   SetSelectedIndex(-1, false);
			}
				
			if (_requireSelectionChanged)
			{
				_requireSelectionChanged = false;
				
				if (_requireSelection &&
						SelectedIndex == NO_SELECTION &&
						null != DataProvider &&
						DataProvider.Length > 0)
				{
					// Set the proposed selected index here to make sure
					// CommitSelection() is called below.
					ProposedSelectedIndex = 0;
				}
			}
			
			if (null != PendingSelectedItem)
			{
				if (null != DataProvider)
					ProposedSelectedIndex = DataProvider.GetItemIndex(PendingSelectedItem);
				
				if (AllowCustomSelectedItem && ProposedSelectedIndex == -1)
				{
					ProposedSelectedIndex = CUSTOM_SELECTED_ITEM;
					_selectedItem = PendingSelectedItem;
				}
				  
				PendingSelectedItem = null;
			}
			
			if (ProposedSelectedIndex != NO_PROPOSED_SELECTION)
				changedSelection = CommitSelection(true);
			
			// If the selectedIndex has been adjusted to account for items that
			// have been added or removed, send out a "change" event 
			// so any bindings to selectedIndex are updated correctly.
			if (SelectedIndexAdjusted)
			{
				SelectedIndexAdjusted = false;
				if (!changedSelection)
				{
					DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
				}
			}
			
			if (CaretIndexAdjusted)
			{
				CaretIndexAdjusted = false;
				if (!changedSelection)
				{
					// Put the new caretIndex renderer into the
					// caret state and dispatch an "caretChange" 
					// event to update any bindings. Additionally, update 
					// the backing variable. 
					ItemShowingCaret(_selectedIndex, true); 
					_caretIndex = _selectedIndex; 
					
					e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE); 
					e.OldIndex = _caretIndex; 
					e.NewIndex = _caretIndex;
					DispatchEvent(e);  
				}
			}
			
			if (_labelFieldOrFunctionChanged)
			{
				_labelFieldOrFunctionChanged = false; 

				// Cycle through all instantiated renderers to push the correct text 
				// in to the renderer by setting its label property
				if (null != DataGroup)
				{
					// if virtual layout, only loop through the indices in view
					// otherwise, loop through all of the item renderers
					/*if (null != Layout && Layout.UseVirtualLayout)
					{
						foreach (int itemIndex in DataGroup.GetItemIndicesInView())
						{
							UpdateRendererLabelProperty(itemIndex);
						}
					}
					else
					{*/
						var n = DataGroup.NumberOfContentChildren;
						for (var itemIndex = 0; itemIndex < n; itemIndex++)
						{
							UpdateRendererLabelProperty(itemIndex);
						}
					/*}*/
				}
			}
		}

		/**
		 *  
		 */
		private void UpdateRendererLabelProperty(int itemIndex)
		{
			// grab the renderer at that index and re-compute it's label property
			var renderer = DataGroup.GetContentChildAt(itemIndex) as IItemRenderer; 
			if (null != renderer)
				renderer.Text = ItemToLabel(renderer.Data); 
		}

		/*protected override void PartAdded(string partName, object instance)
		{
			base.PartAdded(partName, instance);

			if (instance == DataGroup)
			{
				// Not your typical delegation, see 'set useVirtualLayout'
				if (_useVirtualLayout && null != DataGroup.Layout)
					DataGroup.Layout.UseVirtualLayout = true;
			}
		}*/

		public override void UpdateRenderer(IVisualElement renderer, int itemIndex, object data)
		{
			//var transitions;
		 
			// First clean up any old, stale properties like selected and caret   
			if (renderer is IItemRenderer)
			{
				//IItemRenderer(renderer).selected = false;
				((IItemRenderer)renderer).ShowsCaret = false;
			}    
			
			// Set any new properties on the renderer now that it's going to 
			// come back into use. 
			ItemSelected(itemIndex, IsItemIndexSelected(itemIndex));
		/*
			if (IsItemIndexSelected(itemIndex))
				ItemSelected(itemIndex, true);
		*/

			if (IsItemIndexShowingCaret(itemIndex))
				ItemShowingCaret(itemIndex, true);
			
			// Now run through and initialize the renderer correctly.  We 
			// call super.updateRenderer() last because super.updateRenderer()
			// sets the data on the item renderer, and that should be done last.
			base.UpdateRenderer(renderer, itemIndex, data); 
		}

		/**
		 *  Given a data item, return the correct text a renderer
		 *  should display while taking the <code>labelField</code> 
		 *  and <code>labelFunction</code> properties into account. 
		 */
		override public string ItemToLabel(object item)
		{
			return LabelUtil.ItemToLabel(item, LabelField, LabelFunction);
		}

		/**
		 *  Called when an item is selected or deselected. 
		 *  Subclasses must override this method to display the selection.
		 *
		 *  Param: index The item index that was selected.
		 *
		 *  Param: selected <code>true</code> if the item is selected, 
		 *  and <code>false</code> if it is deselected.
		 */
		protected virtual void ItemSelected(int index, bool selected)
		{
			// Subclasses must override this method to display the selection.
		}

		/**
		 *  Called when an item is in its caret state or not. 
		 *  Subclasses must override this method to display the caret. 
		 */
		protected virtual void ItemShowingCaret(int index, bool showsCaret)
		{
			// Subclasses must override this method to display the caret.
		}

		/**
		 *  Returns <code>true</code> if the item at the index is selected.
		 * 
		 *  Param: index The index of the item whose selection status is being checked
		 *
		 *  Returns: <code>true</code> if the item at that index is selected, 
		 *  and <code>false</code> otherwise.
		 */
		protected virtual bool IsItemIndexSelected(int index)
		{        
			return index == _selectedIndex;
		}

		/**
		 *  Returns true if the item at the index is the caret item, which is
		 *  essentially the item in focus. 
		 * 
		 *  Param: index The index of the item whose caret status is being checked
		 *
		 *  Returns: true if the item at that index is the caret item, false otherwise.
		 */
		protected virtual bool IsItemIndexShowingCaret(int index)
		{        
			return index == _caretIndex;
		}

		/**
		 *  
		 *  Set current caret index. This function takes the item that was
		 *  previously the caret item and sets its caret property to false,
		 *  then takes the current proposed caret item and sets its 
		 *  caret property to true as well as updating the backing variable. 
		 * 
		 */

		/// <summary>
		/// Sets current caret index
		/// </summary>
		/// <param name="value"></param>
		protected virtual void SetCurrentCaretIndex(int value)
		{
			ItemShowingCaret(_caretIndex, false); 
			
			_caretIndex = value;

			if (CUSTOM_SELECTED_ITEM != _caretIndex)
				ItemShowingCaret(_caretIndex, true);
		}

		/**
		 *  
		 *  The selection validation and commitment workhorse method. 
		 *  Called to commit the pending selected index. This method dispatches
		 *  the "changing" event, and if the event is not cancelled,
		 *  commits the selection change and then dispatches the "change"
		 *  event.
		 * 
		 *  Returns true if the selection was committed, or false if the selection
		 *  was cancelled.
		 */
		protected virtual bool CommitSelection(bool dispatchChangedEvents/* = true*/)
		{
			// Step 1: make sure the proposed selected index is in range.
			var maxIndex = null != DataProvider ? DataProvider.Length - 1 : -1;
			var oldSelectedIndex = _selectedIndex;
			var oldCaretIndex = _caretIndex;
			IndexChangeEvent e;
			
			if (!AllowCustomSelectedItem || ProposedSelectedIndex != CUSTOM_SELECTED_ITEM)
			{
				if (ProposedSelectedIndex < NO_SELECTION)
					ProposedSelectedIndex = NO_SELECTION;
				if (ProposedSelectedIndex > maxIndex)
					ProposedSelectedIndex = maxIndex;
				if (_requireSelection && ProposedSelectedIndex == NO_SELECTION && 
					null != DataProvider && DataProvider.Length > 0)
				{
					ProposedSelectedIndex = NO_PROPOSED_SELECTION;
					return false;
				}
			}
			
			// Step 2: dispatch the "changing" event. If preventDefault() is called
			// on this event, the selection change will be cancelled.        
			if (DispatchChangeAfterSelection)
			{
				e = new IndexChangeEvent(IndexChangeEvent.CHANGING, false, true)
						{
							OldIndex = _selectedIndex,
							NewIndex = ProposedSelectedIndex
						};
				//if (!DispatchEvent(e))
				if (e.DefaultPrevented)
				{
					// The event was cancelled. Cancel the selection change and return.
					ProposedSelectedIndex = NO_PROPOSED_SELECTION;
					return false;
				}
			}
			
			// Step 3: commit the selection change and caret change
			_selectedIndex = ProposedSelectedIndex;
			ProposedSelectedIndex = NO_PROPOSED_SELECTION;
			
			if (oldSelectedIndex != NO_SELECTION)
				ItemSelected(oldSelectedIndex, false);
			if (_selectedIndex != NO_SELECTION && _selectedIndex != CUSTOM_SELECTED_ITEM)
				ItemSelected(_selectedIndex, true);
			SetCurrentCaretIndex(_selectedIndex); 

			// Step 4: dispatch the "change" event and "caretChange" 
			// events based on the dispatchChangeEvents parameter. Overrides may  
			// chose to dispatch the change/caretChange events 
			// themselves, in which case we wouldn't want to dispatch the event 
			// here. 
			if (dispatchChangedEvents)
			{
				// Dispatch the change event
				if (DispatchChangeAfterSelection)
				{
					e = new IndexChangeEvent(IndexChangeEvent.CHANGE)
							{
								OldIndex = oldSelectedIndex,
								NewIndex = _selectedIndex
							};
					DispatchEvent(e);
					DispatchChangeAfterSelection = false;
				}
				else
				{
					DispatchEvent(new FrameworkEvent(FrameworkEvent.VALUE_COMMIT));
				}
				
				//Dispatch the caretChange event 
				e = new IndexChangeEvent(IndexChangeEvent.CARET_CHANGE)
						{
							OldIndex = oldCaretIndex,
							NewIndex = _caretIndex
						};
				DispatchEvent(e);
			}
			
			return true;
		 }

		/**
		 *  Adjusts the selected index to account for items being added to or 
		 *  removed from this component. This method adjusts the selected index
		 *  value and dispatches a <code>change</code> event. 
		 *  It does not dispatch a <code>changing</code> event 
		 *  or allow the cancellation of the selection. 
		 *  It also does not call the <code>itemSelected()</code> method, 
		 *  since the same item is selected; 
		 *  the only thing that has changed is the index of the item.
		 * 
		 *  <p>A <code>change</code> event is dispatched in the next call to 
		 *  the <code>commitProperties()</code> method.</p>
		 *
		 *  <p>The <code>changing</code> event is not sent when
		 *  the <code>selectedIndex</code> is adjusted.</p>
		 *
		 *  Param: newIndex The new index.
		 *   
		 *  Param: add <code>true</code> if an item was added to the component, 
		 *  and <code>false</code> if an item was removed.
		 */
		protected virtual void AdjustSelection(int newIndex, bool add/*=false*/)
		{
			if (ProposedSelectedIndex != NO_PROPOSED_SELECTION)
				ProposedSelectedIndex = newIndex;
			else
				_selectedIndex = newIndex;
			SelectedIndexAdjusted = true;
			InvalidateProperties();
		}

		/**
		 *  Called when an item has been added to this component. Selection
		 *  and caret related properties are adjusted accordingly. 
		 * 
		 *  Param: index The index of the item being added. 
		 */
		protected virtual void ItemAdded(int index)
		{
			// if doing wholesale changes, we'll handle this more effeciently in commitProperties() with dataProviderChanged == true
			if (DoingWholesaleChanges)
				return;
			
			if (_selectedIndex == NO_SELECTION)
			{
				// If there's no selection, there's nothing to adjust unless 
				// we requireSelection and need to select what was added
				if (_requireSelection)
					AdjustSelection(index, false);
			}
			else if (index <= _selectedIndex)
			{
				// If an item is added before the selected item, bump up our
				// selected index backing variable.
				AdjustSelection(_selectedIndex + 1, false);
			}
		}

		/**
		 *  Called when an item has been removed from this component.
		 *  Selection and caret related properties are adjusted 
		 *  accordingly. 
		 * 
		 *  Param: index The index of the item being removed. 
		 */
		protected virtual void ItemRemoved(int index)
		{
			if (_selectedIndex == NO_SELECTION || DoingWholesaleChanges)
				return;
			
			// If the selected item is being removed, clear the selection (or
			// reset to the first item if requireSelection is true)
			if (index == _selectedIndex)
			{
				if (_requireSelection && null != DataProvider && DataProvider.Length > 0)
				{       
					if (index == 0)
					{
						//We can't just set selectedIndex to 0 directly
						//since the previous value was 0 and the new value is
						//0, so the setter will return early.  
						ProposedSelectedIndex = 0;
						InvalidateProperties();
					}
					else 
						SetSelectedIndex(0, false);
				}
				else
					AdjustSelection(-1, false);
			}
			else if (index < _selectedIndex)
			{
				// An item below the selected index has been removed, bump
				// the selected index backing variable.
				AdjustSelection(_selectedIndex - 1, false);
			}
		}

		#region Event handlers

		/**
		 *  
		 *  Called when contents within the dataProvider changes.  
		 *
		 *  Param: event The collection change event
		 */
		protected virtual void DataProviderCollectionChangeHandler(Event e)
		{
			if (e is CollectionEvent)
			{
				var ce = (CollectionEvent) e;

				if (ce.Kind == CollectionEventKind.ADD)
				{
					ItemAdded(ce.Location);
				}
				else if (ce.Kind == CollectionEventKind.REMOVE)
				{
					ItemRemoved(ce.Location);
				}
				else if (ce.Kind == CollectionEventKind.RESET)
				{
					// Data provider is being reset, clear out the selection
					if (DataProvider.Length == 0)
					{
						SetSelectedIndex(NO_SELECTION, false);
						SetCurrentCaretIndex(NO_CARET);
					}
					else
					{
						_dataProviderChanged = true; 
						InvalidateProperties(); 
					}
				}
				else if (ce.Kind == CollectionEventKind.REPLACE ||
					ce.Kind == CollectionEventKind.MOVE ||
					ce.Kind == CollectionEventKind.REFRESH)
				{
					//These cases are handled by the DataGroup skinpart  
				}
			}
				
		}

		#endregion

	}
}