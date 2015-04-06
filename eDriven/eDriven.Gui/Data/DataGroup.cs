using System;
using System.Collections.Generic;
using eDriven.Core.Geom;
using eDriven.Gui.Components;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Data
{
	///<summary>
	/// Base container class for data items
	///</summary>
// ReSharper disable UnusedMember.Global
	public class DataGroup : GroupBase, IItemRendererOwner
// ReSharper restore UnusedMember.Global
	{
		///<summary>
		/// Container
		///</summary>
		public DataGroup()
		{
			_rendererUpdateDelegate = this;
			MouseEnabled = true;
		}

		/**
		 *  
		 *  flag to indicate whether a child in the item renderer has a non-zero layer, requiring child re-ordering.
		 */
		private int _layeringFlags;
		
		// ReSharper disable InconsistentNaming
		private const int LAYERING_ENABLED = 0x1;
		private const int LAYERING_DIRTY = 0x2;
		// ReSharper restore InconsistentNaming

		//----------------------------------
		//  typicalItem
		//----------------------------------
		
		private object _typicalItem;
// ReSharper disable UnaccessedField.Local
		private object _explicitTypicalItem;
// ReSharper restore UnaccessedField.Local
		private bool _typicalItemChanged;
		private ILayoutElement _typicalLayoutElement;

		///<summary>
		///</summary>
// ReSharper disable UnusedMember.Global
		public object TypicalItem
// ReSharper restore UnusedMember.Global
		{
			get { return _typicalItem; }
			set { 
				if (value == _typicalItem)
					return;
				_typicalItem = _explicitTypicalItem = value;
				_typicalItemChanged = true;
				InvalidateProperties();
			}
		}

		private void SetTypicalLayoutElement(ILayoutElement element)
		{
			_typicalLayoutElement = element;
			if (null != Layout)
				Layout.TypicalLayoutElement = element;
		}

		private void InitializeTypicalItem()
		{
			if (null == _typicalItem)
			{
				SetTypicalLayoutElement(null);
				return;
			}
			
			IVisualElement renderer = CreateRendererForItem(_typicalItem, false);
			DisplayListMember obj = renderer as DisplayListMember;
			if (null == obj)
			{
				SetTypicalLayoutElement(null);
				return;
			}

			base.AddChild(obj);
			SetUpItemRenderer(renderer, 0, _typicalItem);
			if (obj is IInvalidating)
				((IInvalidating)obj).ValidateNow();
			SetTypicalLayoutElement((ILayoutElement) renderer); // casted!
			base.RemoveChild(obj);
		} 

		/// <summary>
		/// Create and validate a new item renderer (IR) for dataProvider[index]
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
// ReSharper disable UnusedMember.Global
		internal IVisualElement CreateItemRendererFor(int index)
// ReSharper restore UnusedMember.Global
		{
			if ((index < 0) || (null == DataProvider) || (index >= DataProvider.Length))
				return null;

			object item = DataProvider.GetItemAt(index);
			IVisualElement renderer = CreateRendererForItem(item, true);

			base.AddChild((DisplayListMember)renderer); 
			SetUpItemRenderer(renderer, index, item);

			var invalidating = renderer as IInvalidating;
			if (invalidating != null)
				invalidating.ValidateNow();

			base.RemoveChild((DisplayListMember)renderer);
			
			return renderer;
		}
		
		/**
		 *  
		 *  Called before measure/updateDisplayList(), if useVirtualLayout=true, to guarantee
		 *  that the typicalLayoutElement has been defined.  If it hasn't, typicalItem is 
		 *  initialized to dataProvider[0] and layout.typicalLayoutElement is set.
		 */
		private void EnsureTypicalLayoutElement()
		{
			if (null != Layout.TypicalLayoutElement)
				return;
			
			IList list = DataProvider;
			if (null != list && list.Length > 0)
			{
				_typicalItem = list.GetItemAt(0);
				InitializeTypicalItem();
			}
		}

		//----------------------------------
		//  layout
		//----------------------------------
		
		private bool _useVirtualLayoutChanged;

		public override LayoutBase Layout
		{
			get
			{
				return base.Layout;
			}
			set
			{
				LayoutBase oldLayout = Layout;
				if (value == oldLayout)
					return; 
				
				if (null != oldLayout)
				{
					oldLayout.TypicalLayoutElement = null;
					oldLayout.RemoveEventListener("useVirtualLayoutChanged", LayoutUseVirtualLayoutChangedHandler);
				}

				// Changing the layout may implicitly change layout.useVirtualLayout
				//if (null != oldLayout && null != value && (oldLayout.UseVirtualLayout != value.UseVirtualLayout))
				//    ChangeUseVirtualLayout();
				base.Layout = value;  
  
				if (null != value)
				{
					// If typicalLayoutElement was specified for this DataGroup, then use
					// it, otherwise use the layout's typicalLayoutElement, if any.
					if (null != _typicalLayoutElement)
						value.TypicalLayoutElement = _typicalLayoutElement;
					else
						_typicalLayoutElement = value.TypicalLayoutElement;
					value.AddEventListener("useVirtualLayoutChanged", LayoutUseVirtualLayoutChangedHandler);
				}
			}
		}

		/**
		 *  
		 *  If layout.useVirtualLayout changes, recreate the ItemRenderers.  This can happen
		 *  if the layout's useVirtualLayout property is changed directly, or if the DataGroup's
		 *  layout is changed. 
		 */    
		private void ChangeUseVirtualLayout()
		{
			RemoveDataProviderListener();
			RemoveAllItemRenderers();
			_useVirtualLayoutChanged = true;
			InvalidateProperties();
		}

		private void LayoutUseVirtualLayoutChangedHandler(Event e)
		{
			ChangeUseVirtualLayout();
		}

		//----------------------------------
		//  ItemRenderer
		//----------------------------------
		
		/**
		 *  
		 *  Storage for the ItemRenderer property.
		 */
		private IFactory _itemRenderer;
		
		private bool _itemRendererChanged;

// ReSharper disable MemberCanBePrivate.Global
		///<summary>
		///</summary>
		public IFactory ItemRenderer
// ReSharper restore MemberCanBePrivate.Global
		{
			get
			{
				return _itemRenderer;
			}
// ReSharper disable UnusedMember.Global
			set
// ReSharper restore UnusedMember.Global
			{
				_itemRenderer = value;
			
				RemoveDataProviderListener();
				RemoveAllItemRenderers();
				InvalidateProperties();
				
				_itemRendererChanged = true;
				_typicalItemChanged = true;
			}
		}
		
		//----------------------------------
		//  ItemRendererFunction
		//----------------------------------

		/**
		 *  
		 *  Storage for the ItemRendererFunction property.
		 */
		private ItemRendererFunction _itemRendererFunction;

		///<summary>
		/// Returns the item renderer factory depending of the input (supplied item)
		///</summary>
// ReSharper disable MemberCanBePrivate.Global
		public ItemRendererFunction ItemRendererFunction
// ReSharper restore MemberCanBePrivate.Global
		{
			get
			{
				return _itemRendererFunction;
			}
// ReSharper disable UnusedMember.Global
			set
// ReSharper restore UnusedMember.Global
			{
				_itemRendererFunction = value;
			
				RemoveDataProviderListener();
				RemoveAllItemRenderers();
				InvalidateProperties();
				
				_itemRendererChanged = true;
				_typicalItemChanged = true;
			}
		}

		/**
		 *  
		 *  Storage for the rendererUpdateDelegate property.
		 */
		private IItemRendererOwner _rendererUpdateDelegate;

		///<summary>
		///</summary>
// ReSharper disable UnusedMember.Global
		public IItemRendererOwner RendererUpdateDelegate
// ReSharper restore UnusedMember.Global
		{
			get { return _rendererUpdateDelegate; }
			set { _rendererUpdateDelegate = value; }
		}

        private IList _dataProvider;
		private bool _dataProviderChanged;

		///<summary>
		/// The data provider for this DataGroup
		///</summary>
// ReSharper disable MemberCanBePrivate.Global
		public IList DataProvider
// ReSharper restore MemberCanBePrivate.Global
		{
			get
			{
				return _dataProvider;
			}
// ReSharper disable UnusedMember.Global
			set
// ReSharper restore UnusedMember.Global
			{
				if (_dataProvider == value)
					return;

				RemoveDataProviderListener();
				_dataProvider = value;  // listener will be added by commitProperties()
				_dataProviderChanged = true;
				
				InvalidateProperties();
				DispatchEvent(new Event("dataProviderChanged"));    
			}
		}

		/**
		 *  
		 *  Used below for sorting the _virtualRendererIndices Vector.
		 */
// ReSharper disable UnusedMember.Local
		private static int SortDecreasing(int x, int y)
// ReSharper restore UnusedMember.Local
		{
			return y - x;
		}

		/**
		 *  
		 *  Apply ItemRemoved() to the renderer and dataProvider item at index.
		 */
		private void RemoveRendererAt(int index)
		{
			IVisualElement renderer = _indexToRenderer[index];
			object item;
			
			if (renderer is IDataRenderer && (ItemRenderer != null || ItemRendererFunction != null))
				item = ((IDataRenderer)renderer).Data;
			else
				item = renderer;
			ItemRemoved(item, index);
		}

		/**
		 *  
		 *  Remove all of the item renderers, clear the _indexToRenderer table, clear
		 *  any cached virtual layout data, and clear the typical layout element.  Note that
		 *  this method does not depend on the dataProvider itself, see RemoveRendererAt().
		 */ 
		private void RemoveAllItemRenderers()
		{
			if (_indexToRenderer.Count == 0)
				return;

			for (int index = _indexToRenderer.Count - 1; index >= 0; index--)
				RemoveRendererAt(index);

			_indexToRenderer.Clear();  // should be redundant

			if (null != Layout)
			{
				Layout.ClearVirtualLayoutCache();
				Layout.TypicalLayoutElement = null;
			}
		}
		
		/**
		 *  Return the indices of the item renderers visible within this DataGroup.
		 * 
		 *  <p>If clipAndEnableScrolling=true, return the indices of the visible=true 
		 *  ItemRenderers that overlap this DataGroup's scrollRect, i.e. the ItemRenders 
		 *  that are at least partially visible relative to this DataGroup.  If 
		 *  clipAndEnableScrolling=false, return a list of integers from 
		 *  0 to dataProvider.length - 1.  Note that if this DataGroup's owner is a 
		 *  Scroller, then clipAndEnableScrolling has been set to true.</p>
		 * 
		 *  <p>The corresponding item renderer for each returned index can be 
		 *  retrieved with getElementAt(), even if the layout is virtual</p>
		 * 
		 *  <p>The order of the items in the returned Vector is not guaranteed.</p>
		 * 
		 *  <p>Note that the VerticalLayout and HorizontalLayout classes provide bindable
		 *  firstIndexInView and lastIndexInView properties which convey the same information
		 *  as this method.</p>
		 * 
		 *  Returns: The indices of the visible item renderers.
		 */
		///<summary>
		///</summary>
		///<returns></returns>
// ReSharper disable UnusedMember.Global
		public List<int> GetItemIndicesInView()
// ReSharper restore UnusedMember.Global
		{
			if (null != Layout/* && Layout.UseVirtualLayout*/)
				return (null != _virtualRendererIndices) ? new List<int>(_virtualRendererIndices) : new List<int>(0);
			
			if (null == DataProvider)
				return new List<int>();
			
			Rectangle scrollR = ScrollRect;
			int dataProviderLength = DataProvider.Length;
			
			if (null != scrollR)
			{
				List<int> visibleIndices = new List<int>();
				Rectangle eltR = new Rectangle();
				//const perspectiveProjection:PerspectiveProjection = transform.perspectiveProjection;            
				
				for (int index = 0; index < dataProviderLength; index++)
				{
					//IVisualElement elt = GetContentChildAt(index); // element!
					InvalidationManagerClient elt = (InvalidationManagerClient) GetContentChildAt(index); // element!
					if (null == elt || !elt.Visible)
						continue;
					
					eltR.X = LayoutUtil.GetLayoutBoundsX(elt); //elt.getLayoutBoundsX();
					eltR.Y = LayoutUtil.GetLayoutBoundsX(elt); //elt.getLayoutBoundsY();
					eltR.Width = LayoutUtil.GetLayoutBoundsWidth(elt); //elt.getLayoutBoundsWidth();
					eltR.Height = LayoutUtil.GetLayoutBoundsHeight(elt); //elt.getLayoutBoundsHeight();
					
					if (scrollR.Intersects(eltR))
						visibleIndices.Add(index);
				}

				return visibleIndices;
			}
			
			List<int> allIndices = new List<int>(dataProviderLength);
			for (var index = 0; index < dataProviderLength; index++)
				allIndices[index] = index;
			return allIndices;
		}

		//--------------------------------------------------------------------------
		//
		//  Item -> Renderer mapping
		//
		//--------------------------------------------------------------------------
		
        /// <summary>
        /// Creates the item renderer for the item, if needed.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="failRte"></param>
        /// <returns></returns>
		private IVisualElement CreateRendererForItem(object item, bool failRte/*:Boolean=true*/)
		{
			//Debug.Log("CreateRendererForItem: " + item);
			IVisualElement myItemRenderer = null;
			
			// Rules for lookup:
			// 1. if ItemRendererFunction is defined, call it to get the renderer factory and instantiate it
			// 2. if ItemRenderer is defined, instantiate one
			// 3. if item is an IVisualElement and a DisplayObject, use it directly
			
			// 1. if ItemRendererFunction is defined, call it to get the renderer factory and instantiate it    
			if (ItemRendererFunction != null)
			{
				IFactory rendererFactory = ItemRendererFunction(item);
				
				// if the function returned a factory, use that.
				// otherwise, if it returned null, try using the item directly
				if (null != rendererFactory)
					myItemRenderer = (IVisualElement) rendererFactory.NewInstance();
				else if (item is IVisualElement && item is DisplayObject)
					myItemRenderer = (IVisualElement)item;
			}
			
			// 2. if ItemRenderer is defined, instantiate one
			if (null == myItemRenderer && null != ItemRenderer) {
				//Debug.Log("ItemRenderer: " + ItemRenderer);
				myItemRenderer = (IVisualElement)ItemRenderer.NewInstance();
			}
			
			// 3. if item is an IVisualElement and a DisplayObject, use it directly
			if (null == myItemRenderer && item is IVisualElement && item is DisplayObject)
				myItemRenderer = (IVisualElement)item;
			
			// Couldn't find item renderer.  Throw an RTE.
			if (null == myItemRenderer && failRte)
			{
				string err;
				if (item is IVisualElement || item is DisplayObject)
					err = "Cannot display visual element";
				else
					err = "Unable to create renderer";
				throw new Exception(err);
			}
			
			return myItemRenderer;
		}

		/** 
		 *  
		 *  If layout.useVirtualLayout=false, then ensure that there's one item 
		 *  renderer for every dataProvider item.   This method is only intended to be
		 *  called by commitProperties().
		 * 
		 *  Reuse as many of the IItemRenderer renderers in indexToRenders as possible.
		 *  Note that if ItemRendererFunction was specified, we can reuse any of them. 
		 */
		private void CreateItemRenderers()
		{
			//Debug.Log("CreateItemRenderers...");

			if (null == DataProvider)
			{
				RemoveAllItemRenderers();
				return;
			}

			//Debug.Log("CreateItemRenderers: " + DataProvider.Length);

			/*if (null != Layout && Layout.UseVirtualLayout)
			{
				// The item renderers will be created lazily, at updateDisplayList() time
				InvalidateSize();
				InvalidateDisplayList();
				return;
			}*/
			
			// Can't reuse renderers if ...
			if ((ItemRenderer == null) || (ItemRendererFunction != null))  
				RemoveAllItemRenderers();   // _indexToRenderer.length = 0

			int dataProviderLength = DataProvider.Length; 

			// Remove the renderers we're not going to need
			for(int index = _indexToRenderer.Count - 1; index >= dataProviderLength; index--)
				RemoveRendererAt(index);
			
			// Reset the existing renderers
			for(int index = 0; index < _indexToRenderer.Count; index++)
			{
				object item = DataProvider.GetItemAt(index);
				IVisualElement renderer = _indexToRenderer[index];
				if (null != renderer)
				{
					SetUpItemRenderer(renderer, index, item);
				}
				else // can't reuse this renderer
				{
					RemoveRendererAt(index); 
					ItemAdded(item, index);
				}
			}
			
			// Create new renderers
			for (int index = _indexToRenderer.Count; index < dataProviderLength; index++)
				ItemAdded(DataProvider.GetItemAt(index), index);        
		}

		protected override void CommitProperties()
		{
			// If the ItemRenderer, ItemRendererFunction, or useVirtualLayout properties changed,
			// then recreate the item renderers from scratch.  If just the dataProvider changed,
			// and layout.useVirtualLayout=false, then reuse as many item renderers as possible,
			// remove the extra ones, or create more as needed.

			if (_itemRendererChanged || _useVirtualLayoutChanged || _dataProviderChanged)
			{
				_itemRendererChanged = false;
				_useVirtualLayoutChanged = false;
				
				// item renderers and the dataProvider listener have already been removed
				CreateItemRenderers();
				AddDataProviderListener();

				// Don't reset the scroll positions until the new ItemRenderers have been
				// created, see bug https://bugs.adobe.com/jira/browse/SDK-23175
				if (_dataProviderChanged)
				{
					//Debug.Log("_dataProviderChanged");
					_dataProviderChanged = false;
					VerticalScrollPosition = HorizontalScrollPosition = 0;
				}
				
				//maskChanged = true;
			}
			
			// Need to create item renderers before calling super.commitProperties()
			// GroupBase's commitProperties reattaches the mask
			base.CommitProperties();
			
			if ((_layeringFlags & LAYERING_DIRTY) > 0)
			{
				//if (null != Layout && Layout.UseVirtualLayout)
				//    InvalidateDisplayList();
				//else
					ManageDisplayObjectLayers();
			}
			
			if (_typicalItemChanged)
			{
				_typicalItemChanged = false;
				InitializeTypicalItem();
			}
		}

		/**
		 *  
		 *  True if we are updating a renderer currently. 
		 *  We keep track of this so we can ignore any dataProvider collectionChange
		 *  UPDATE events while we are updating a renderer just in case we try to update 
		 *  the rendererInfo of the same renderer twice.  This can happen if setting the 
		 *  data in an item renderer causes the data to mutate and issue a propertyChange
		 *  event, which causes an collectionChange.UPDATE event in the dataProvider.  This 
		 *  can happen for components which are being treated as data because the first time 
		 *  they get set on the renderer, they get added to the display list, which may 
		 *  cause a propertyChange event (if there's a child with an ID in it, that causes 
		 *  a propertyChange event) or the data to morph in some way.
		 */
		private bool _renderersBeingUpdated;

		/**
		 *   
		 *  Sets the renderer's data, owner and label properties. 
		 *  It does this by calling rendererUpdateDelegate.UpdateRenderer().
		 *  By default, rendererUpdateDelegate points to ourselves, but if 
		 *  the "true owner" of the item renderer is a List, then the 
		 *  rendererUpdateDelegate points to that object.  The 
		 *  rendererUpdateDelegate.UpdateRenderer() call is in charge of 
		 *  setting all the properties on the renderer, like owner, itemIndex, 
		 *  data, selected, etc...  Note that data should be the last property 
		 *  set in this lifecycle.
		 */
		private void SetUpItemRenderer(IVisualElement renderer, int itemIndex, object data)
		{
			if (null == renderer)
				return;
			
			// keep track of whether we are actively updating an renderers 
			// so we can ignore any collectionChange.UPDATE events
			_renderersBeingUpdated = true;
			
			// Defer to the rendererUpdateDelegate
			// to update the renderer. By default, the delegate is DataGroup
			_rendererUpdateDelegate.UpdateRenderer(renderer, itemIndex, data);
			
			// technically if this got called "recursively", this _renderersBeingUpdated flag
			// would be prematurely set to false, but in most cases, this check should be 
			// good enough.
			_renderersBeingUpdated = false;
		}

		#region Implementation of IItemRendererOwner
		
		/**
		 *  @inheritDoc
		 * 
		 *  Given a data item, return the toString() representation 
		 *  of the data item for an item renderer to display. Null 
		 *  data items return the empty string. 
		 */
		public string ItemToLabel(object item)
		{
			if (null != item)
				return item.ToString();
			return " ";
		}

		/// <summary>
		/// Updates the renderer
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="itemIndex"></param>
		/// <param name="data"></param>
		public void UpdateRenderer(IVisualElement renderer, int itemIndex, object data)
		{
			// set the owner
			renderer.Owner = this;
			
			// Set the index
			if (renderer is IItemRenderer)
				((IItemRenderer)renderer).ItemIndex = itemIndex;
			
			// set the label to the toString() of the data 
			if (renderer is IItemRenderer)
				((IItemRenderer)renderer).Text = ItemToLabel(data);

			//Debug.Log("item to label: " + ItemToLabel(data));
			
			// always set the data last
			if ((renderer is IDataRenderer) && (renderer != data))
				((IDataRenderer)renderer).Data = data;
		}

		#endregion

		/**
		 *  
		 *  If the renderer at the specified index has a non-zero depth then: append it to
		 *  layers.topLayerItems if depth > 0, or to layers.bottomLayerItems if depth < 0.
		 *  Otherwise, if depth is zero (the default) then change the renderer's childIndex 
		 *  to insertIndex and increment insertIndex.  The possibly incremented insertIndex is returned.
		 *   
		 *  See ManageDisplayObjectLayers().
		 */
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedParameter.Local
		private int SortItemAt(int index, object layers, int insertIndex)
// ReSharper restore UnusedParameter.Local
// ReSharper restore UnusedMember.Local
		{
			//IVisualElement renderer = GetContentChildAt(index);
			//float layer = renderer.Depth;

			//if (layer != 0)
			//{
			//    if (layer > 0)
			//    {
			//        if (layers.topLayerItems == null)
			//            layers.topLayerItems = new List<IVisualElement>();
			//        layers.topLayerItems.push(renderer);
			//    }
			//    else
			//    {
			//        if (layers.bottomLayerItems == null)
			//            layers.bottomLayerItems = new List<IVisualElement>();
			//        layers.bottomLayerItems.push(renderer);
			//    }

			//    return insertIndex;
			//}

			//base.SetChildIndex(renderer as DisplayObject, insertIndex);
			return insertIndex + 1;
		}

		/**
		 *  
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
		private void ManageDisplayObjectLayers()
// ReSharper restore MemberCanBeMadeStatic.Local
		{      
			//_layeringFlags &= ~LAYERING_DIRTY;
			
			//var insertIndex:uint = 0;
			//const layers:Object = {topLayerItems:null,  bottomLayerItems:null};
			
			//if (layout && layout.useVirtualLayout)
			//{
			//    for each (var index:int in _virtualRendererIndices)
			//    insertIndex = SortItemAt(index, layers, insertIndex);
			//}
			//else
			//{
			//    for (index = 0; index < numElements; index++) 
			//        insertIndex = SortItemAt(index, layers, insertIndex);            
			//}
			
			//// itemRenderers should be both DisplayObjects and IVisualElements
			//const topLayerItems:Vector.<IVisualElement> = layers.topLayerItems;
			//const bottomLayerItems:Vector.<IVisualElement> = layers.bottomLayerItems; 
			
			//var myItemRenderer:IVisualElement;
			//var keepLayeringEnabled:Boolean = false;
			//var len:int = numElements;
			//var i:int;
			
			//if (topLayerItems != null)
			//{
			//    keepLayeringEnabled = true;
			//    GroupBase.sortOnLayer(topLayerItems);
			//    len = topLayerItems.length;
			//    for (i=0;i<len;i++)
			//    {
			//        myItemRenderer = topLayerItems[i];
			//        super.setChildIndex(myItemRenderer as DisplayObject, insertIndex++);
			//    }
			//}
			
			//if (bottomLayerItems != null)
			//{
			//    keepLayeringEnabled = true;
			//    insertIndex=0;
				
			//    GroupBase.sortOnLayer(bottomLayerItems);
			//    len = bottomLayerItems.length;
				
			//    for (i=0;i<len;i++)
			//    {
			//        myItemRenderer = bottomLayerItems[i];
			//        super.setChildIndex(myItemRenderer as DisplayObject, insertIndex++);
			//    }
			//}
			
			//if (keepLayeringEnabled == false)
			//{
			//    _layeringFlags &= ~LAYERING_ENABLED;
			//} 
		}

		//--------------------------------------------------------------------------
		//
		//  Layout item iteration
		//
		//  Iterators used by Layout objects. For visual items, the layout item
		//  is the item itself. For data items, the layout item is the item renderer
		//  instance that is associated with the item.
		//--------------------------------------------------------------------------
		
		///<summary>
		///</summary>
		public override int NumberOfContentChildren
		{
			get 
			{ 
				 return null != DataProvider ? DataProvider.Length : 0;
			}
		}

		/** 
		 *  
		 *  Maps from renderer index (same as dataProvider index) to the item renderer itself.
		 */
		private readonly List<IVisualElement> _indexToRenderer = new List<IVisualElement>(); 

		/**
		 *  
		 *  The set of layout element indices requested with getVirtualElementAt() 
		 *  during updateDisplayList(), and the set of "old" indices that were requested
		 *  in the previous pass.  These vectors are used by FinishVirtualLayout()
		 *  to distinguish IRs that can be recycled or discarded.   The _virtualRendererIndices
		 *  vector is used in various places to iterate over all of the virtual renderers.
		 */
		private readonly List<int> _virtualRendererIndices/* = null*/;
#pragma warning disable 169
		private List<int> _oldVirtualRendererIndices/* = null*/;
#pragma warning restore 169
		
		/**
		 *   
		 *  During a virtual layout, _virtualLayoutUnderway is true.  This flag is used 
		 *  to defeat calls to invalidateSize(), which occur when IRs are lazily validated.   
		 *  See invalidateSize() and updateDisplayList().
		 */
		private bool _virtualLayoutUnderway;
		
		/**
		 *  
		 *  _freeRenderers - IRs that were created by getLayoutElementAt() but
		 *  are no longer in view.   They'll be reused by getLayoutElementAt().
		 *  The list is updated by FinishVirtualLayout().  
		 */
		private readonly List<IItemRenderer> _freeRenderers = new List<IItemRenderer>();

		/**
		 *  
		 *  True if it's OK to recycle virtual item renderers.   More about
		 *  the conditions under which recyling can work at getVirtualElementAt().
		 */
		private bool IsRecyclingOk
		{
			get
			{
				return (ItemRendererFunction == null) && (ItemRenderer != null);   
			}
		}

		/**
		 *  
		 *  Called before super.updateDisplayList() if layout.useVirtualLayout=true.
		 *  Copies _virtualRendererIndices to oldRendererIndices, clears _virtualRendererIndices
		 *  (which will be repopulated by subsequence getVirtualElementAt() calls), and
		 *  calls ensureTypicalElement().
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
//        private void StartVirtualLayout()
//// ReSharper restore MemberCanBeMadeStatic.Local
//        {
//            //// lazily create the _virtualRendererIndices vectors
			
//            //if (!_virtualRendererIndices)
//            //    _virtualRendererIndices = new Vector.<int>();
//            //if (!_oldVirtualRendererIndices)
//            //    _oldVirtualRendererIndices = new Vector.<int>();
			
//            //// Copy the contents _virtualRendererIndices to _oldVirtualRendererIndices
//            //// and then clear _virtualRendererIndices
			
//            //_oldVirtualRendererIndices.length = 0;
//            //for each (var index:int in _virtualRendererIndices)
//            //    _oldVirtualRendererIndices.push(index);
//            //_virtualRendererIndices.length = 0;
			
//            //// Ensure that layout.typicalLayoutElement is set
//            //EnsureTypicalLayoutElement();
//        }

		/**
		 *  
		 *  Called after super.updateDisplayList() finishes.
		 * 
		 *  Discard the ItemRenderers that aren't needed anymore, i.e. the ones
		 *  not explicitly requested with getVirtualElementAt() during the most
		 *  recent super.updateDisplayList().
		 * 
		 *  Discarded IRs may be added to the _freeRenderers list per the rules
		 *  defined in getVirtualElementAt().  If any visible renderer has a non-zero
		 *  depth we resort all of them with ManageDisplayObjectLayers(). 
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
//        private void FinishVirtualLayout()
//// ReSharper restore MemberCanBeMadeStatic.Local
//        {
//            //if (_oldVirtualRendererIndices.length == 0)
//            //    return;
			
//            //// Remove the old ItemRenderers that aren't new ItemRenderers and if 
//            //// recycling is possible, add them to the _freeRenderers list.
			
//            //const recyclingOK:Boolean = IsRecyclingOk();       
			
//            //for each (var vrIndex:int in _oldVirtualRendererIndices)
//            //{
//            //    // Skip renderers that are still in view.
//            //    if (_virtualRendererIndices.indexOf(vrIndex) != -1)
//            //        continue;
				
//            //    // Remove previously "in view" IR from the item=>IR table
//            //    var elt:IVisualElement = _indexToRenderer[vrIndex] as IVisualElement;
//            //    delete _indexToRenderer[vrIndex];
				
//            //    // Free or remove the IR.
//            //    var item:Object = (dataProvider.length > vrIndex) ? dataProvider.getItemAt(vrIndex) : null;
//            //    if (recyclingOK && (item != elt) && (elt is IDataRenderer))
//            //    {
//            //        // IDataRenderer(elt).data = null;  see https://bugs.adobe.com/jira/browse/SDK-20962
//            //        elt.includeInLayout = false;
//            //        elt.visible = false;
					
//            //        // Reset back to (0,0), otherwise when the element is reused
//            //        // it will be validated at its last layout size which causes
//            //        // problems with text reflow.
//            //        elt.setLayoutBoundsSize(0, 0, false);
					
//            //        _freeRenderers.push(elt);
//            //    }
//            //    else if (elt)
//            //    {
//            //        dispatchEvent(new RendererExistenceEvent(RendererExistenceEvent.RENDERER_REMOVE, false, false, elt, vrIndex, item));
//            //        super.removeChild(DisplayObject(elt));
//            //    }
//            //}
			
//            //// If there are any visible renderers whose depth property is non-zero
//            //// then use ManageDisplayObjectLayers to resort the children list.  Note:
//            //// we're assuming that the layout has set the bounds of any elements that
//            //// were allocated but aren't actually visible to 0x0.
			
//            //var depthSortRequired:Boolean = false;
//            //for each (vrIndex in _virtualRendererIndices)
//            //{
//            //    elt = _indexToRenderer[vrIndex] as IVisualElement;
//            //    if (!elt || !elt.visible || !elt.includeInLayout)
//            //        continue;
//            //    if ((elt.width == 0) || (elt.height == 0))
//            //        continue;
//            //    if (elt.depth != 0)
//            //    {
//            //        depthSortRequired = true;
//            //        break;
//            //    }
//            //}
			
//            //if (depthSortRequired)
//            //    ManageDisplayObjectLayers();
//        }

// ReSharper disable UnusedMember.Global
		internal void ClearFreeRenderers()
// ReSharper restore UnusedMember.Global
		{
			int freeRenderersLength = _freeRenderers.Count;
			for (var i = 0; i < freeRenderersLength; i++)
				_freeRenderers[i] = null;
			_freeRenderers.Clear();
		}

		/**
		 *  
		 *  During virtual layout getLayoutElementAt() eagerly validates lazily
		 *  created (or recycled) IRs.   We don't want changes to those IRs to
		 *  invalidate the size of this Component.
		 */
		override public void InvalidateSize()
		{
			if (!_virtualLayoutUnderway)
				base.InvalidateSize();
		}

		/**
		 *   
		 *  Make sure there's a valid typicalLayoutElement for virtual layout.
		 */
		//override protected void Measure()
		//{
		//    if (null != Layout && Layout.UseVirtualLayout)
		//        EnsureTypicalLayoutElement();

		//    base.Measure();
		//}

		///**
		// *  
		// *  Manages the state required by virtual layout. 
		// */
		//override protected void UpdateDisplayList(float width, float height)
		//{
		//    //drawBackground();

		//    if (null != Layout && Layout.UseVirtualLayout)
		//    {
		//        _virtualLayoutUnderway = true;
		//        StartVirtualLayout();
		//    }

		//    base.UpdateDisplayList(width, height);
			
		//    if (_virtualLayoutUnderway)
		//    {
		//        FinishVirtualLayout();
		//        _virtualLayoutUnderway = false;
		//    }
		//}

		///<summary>
		///</summary>
		///<param name="index"></param>
		///<returns></returns>
		public override DisplayListMember GetContentChildAt(int index)
		{
			if (index < 0 || null == DataProvider || index >= DataProvider.Length)
				return null;

		    if (0 == _indexToRenderer.Count)
		        return null;

			return (DisplayListMember) _indexToRenderer[index];
		}
		
        ///<summary>
		///</summary>
		///<param name="index"></param>
		///<param name="eltWidth"></param>
		///<param name="eltHeight"></param>
		///<returns></returns>
		override public IVisualElement GetVirtualElementAt(int index, float eltWidth/*=NaN*/, float eltHeight/*=NaN*/)
		{
			if ((index < 0) || (null == DataProvider) || (index >= DataProvider.Length))
				return null;
			
			IVisualElement elt = _indexToRenderer[index];
			
			if (_virtualLayoutUnderway)
			{
				if (_virtualRendererIndices.IndexOf(index) == -1)
					_virtualRendererIndices.Add(index);
				
				bool createdIR = false;
				bool recycledIR = false;
				object item = null;

				// If the IR for index already exists, it may not be associated with the 
				// right item if the dataProvider has changed
				if (null != elt)
					SetUpItemRenderer(elt, index, DataProvider.GetItemAt(index));
				else
				{
					item = DataProvider.GetItemAt(index);
					
					if (IsRecyclingOk && (_freeRenderers.Count > 0))
					{
						//elt = _freeRenderers.pop();
						elt = _freeRenderers[_freeRenderers.Count - 1];
						_freeRenderers.RemoveAt(_freeRenderers.Count-1);

						elt.Visible = true;
						elt.IncludeInLayout = true;
						recycledIR = true;
					}
					else 
					{
						elt = CreateRendererForItem(item, true);
						createdIR = true;
					}
					
					_indexToRenderer[index] = elt;
				}

				AddItemRendererToDisplayList((DisplayListMember)elt, -1);            
				
				if (createdIR || recycledIR) 
				{
					SetUpItemRenderer(elt, index, item);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
					if (null != eltWidth || null != eltHeight)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
					{
						// If we're going to set the width or height of this
						// layout element, first force it to initialize its
						// measuredWidth,Height.    
						if (elt is IInvalidating) 
							((IInvalidating)elt).ValidateNow();

						LayoutUtil.SetLayoutBoundsSize((InvalidationManagerClient) elt, eltWidth, eltHeight);
					}
					if (elt is IInvalidating)
						((IInvalidating)elt).ValidateNow();
				}
				if (createdIR)
				{
					var ree = new RendererExistenceEvent(RendererExistenceEvent.RENDERER_ADD, false, false)
								  {
									  Renderer = elt,
									  Index = index,
									  Data = item
								  };
					DispatchEvent(ree); //, elt, index, item));
				}
					
			}
			
			return elt;
		}

		///<summary>
		///</summary>
		///<param name="element"></param>
		///<returns></returns>
		override public int GetContentChildIndex(DisplayListMember element)
		{
			if ((null == DataProvider) || null == element)
				return -1;
			
			return _indexToRenderer.IndexOf(element);
		}

		///**
		// *  
		// */
		/////<summary>
		/////</summary>
		//override public void InvalidateLayering()
		//{
		//    _layeringFlags |= (LAYERING_ENABLED | LAYERING_DIRTY);
		//    InvalidateProperties();
		//}
		
		/**
		 *  
		 *  Set the itemIndex of the IItemRenderer at index to index. See ResetRenderersIndices.
		 */
		private void ResetRendererItemIndex(int index)
		{
			IItemRenderer renderer = _indexToRenderer[index] as IItemRenderer;
			if (null != renderer)
				renderer.ItemIndex = index;    
		}

		/**
		 *  
		 *  Recomputes every renderer's index.
		 *  This is useful when an item gets added that may change the renderer's 
		 *  index.  In turn, this index may cuase the renderer to change appearance, 
		 *  like when alternatingItemColors is used.
		 */
		private void ResetRenderersIndices()
		{
			if (_indexToRenderer.Count == 0)
				return;
			
			/*if (null != Layout && Layout.UseVirtualLayout)
			{
				foreach (int index in _virtualRendererIndices)
					ResetRendererItemIndex(index);
			}
			else
			{*/
				int indexToRendererLength = _indexToRenderer.Count;
				for (int index = 0; index < indexToRendererLength; index++)
					ResetRendererItemIndex(index);
			/*}*/
		}

		private void ItemAdded(object item, int index)
		{
			if (null != Layout)
				Layout.ElementAdded(index);
			
			//if (null != Layout && Layout.UseVirtualLayout)
			//{
			//    // Increment all of the indices in _virtualRendererIndices that are >= index.
				
			//    if (null != _virtualRendererIndices)
			//    {
			//        int virtualRendererIndicesLength = _virtualRendererIndices.Count;
			//        for (int i = 0; i < virtualRendererIndicesLength; i++)
			//        {
			//            int vrIndex = _virtualRendererIndices[i];
			//            if (vrIndex >= index)
			//                _virtualRendererIndices[i] = vrIndex + 1;
			//        }
					
			//        //_indexToRenderer.splice(index, 0, null); // shift items >= index to the right
			//        _indexToRenderer.Insert(index, null); // shift items >= index to the right
			//        // virtual ItemRenderer itself will be added lazily, by updateDisplayList()
			//    }
				
			//    InvalidateSize();
			//    InvalidateDisplayList();
			//    return;
			//}

			IVisualElement myItemRenderer = CreateRendererForItem(item, true);
			//_indexToRenderer.splice(index, 0, myItemRenderer);
			_indexToRenderer.Insert(index, myItemRenderer);

			AddItemRendererToDisplayList(myItemRenderer as DisplayListMember, index);
			SetUpItemRenderer(myItemRenderer, index, item);
			
			DispatchEvent(new RendererExistenceEvent(RendererExistenceEvent.RENDERER_ADD, false, false)
							  {
								  Renderer = myItemRenderer,
								  Index = index,
								  Data = item
							  });
			
			InvalidateSize();
			InvalidateDisplayList();
		}

		private void ItemRemoved(object item, int index)
		{
			if (null != Layout)
				Layout.ElementRemoved(index);
			
			// Decrement all of the indices in _virtualRendererIndices that are > index
			// Remove the one (at vrItemIndex) that equals index.

			if (null != _virtualRendererIndices && (_virtualRendererIndices.Count > 0))
			{
				int vrItemIndex = -1;  // location of index in _virtualRendererIndices 
				int virtualRendererIndicesLength = _virtualRendererIndices.Count;
				for (int i = 0; i < virtualRendererIndicesLength; i++)
				{
					int vrIndex = _virtualRendererIndices[i];
					if (vrIndex == index)
						vrItemIndex = i;
					else if (vrIndex > index)
						_virtualRendererIndices[i] = vrIndex - 1;
				}
				if (vrItemIndex != -1)
					//_virtualRendererIndices.splice(vrItemIndex, 1);
					_virtualRendererIndices.Insert(vrItemIndex, 1);
			}

			// Remove the old renderer at index from _indexToRenderer[], from the 
			// DataGroup, and clear its data property (if any).
			
			IVisualElement oldRenderer = _indexToRenderer[index];
			
			if (_indexToRenderer.Count > index)
				//_indexToRenderer.splice(index, 1);
				_indexToRenderer.RemoveAt(index);

			DispatchEvent(new RendererExistenceEvent(RendererExistenceEvent.RENDERER_REMOVE, false, false)
							  {
								  Renderer = oldRenderer,
								  Index = index,
								  Data = item
							  });

			//Debug.Log("oldRenderer: " + oldRenderer);
			//Debug.Log("Data: " + ((IDataRenderer)oldRenderer).Data);

			if (oldRenderer is IDataRenderer && oldRenderer != item)
				((IDataRenderer)oldRenderer).Data = null;

			DisplayObject child = oldRenderer as DisplayObject;
			if (null != child)
				base.RemoveChild((DisplayListMember) child);

			InvalidateSize();
			InvalidateDisplayList();
		}

		/**
		 *  
		 */ 
		private void AddItemRendererToDisplayList(DisplayListMember child, int index/* = -1*/)
		{ 
			object childParent = child.Parent;
// ReSharper disable ConvertToConstant.Local
			//int overlayCount = /*(null != _overlay) ? _overlay.numDisplayObjects :*/ 0;
// ReSharper restore ConvertToConstant.Local
			int childIndex = (index != -1) ? index : base.NumberOfChildren/* - overlayCount*/;

			if (childParent == this)
			{
				//Debug.Log("childParent: " + childParent);
				/*base.*/SetChildIndex(child, childIndex - 1);
				return;
			}

			if (childParent is DataGroup)
			{
				//Debug.Log("removing from datagroup: " + child);
				((DataGroup)childParent)._removeChild(child); // Not the Q-method: careful!
			}

			//if ((_layeringFlags & LAYERING_ENABLED) > 0 || 
			//    (child as IVisualElement).Depth != 0)
			//    InvalidateLayering();

			//Debug.Log("index: " + index + " base.NumberOfChildren: " + base.NumberOfChildren + " childIndex: " + childIndex + " _contentPane: " + (null != _contentPane ? _contentPane.QNumberOfChildren.ToString() : "-"));
			//Debug.Log(string.Format(@"AddItemRendererToDisplayList. Adding {0} at index {1}", child, index));

			base.AddChildAt(child, childIndex);
		}

		/**
		 *  
		 * 
		 *  This method allows access to the base class's implementation
		 *  of removeChild() (Component's version), which can be useful since components
		 *  can override removeChild() and thereby hide the native implementation.  For 
		 *  instance, we override removeChild() here to throw an RTE to discourage people
		 *  from using this method.  We need this method so we can remove children
		 *  that were previously attached to another DataGroup (see addItemToDisplayList).
		 */
// ReSharper disable InconsistentNaming
		private DisplayListMember _removeChild(DisplayListMember child)
// ReSharper restore InconsistentNaming
		{
			return base.RemoveChild(child);
		}

		/**
		 *  
		 */
		private void AddDataProviderListener()
		{
			if (null != _dataProvider)
				_dataProvider.AddEventListener(CollectionEvent.COLLECTION_CHANGE, DataProviderCollectionChangeHandler); // there's more to it!
		}

		/**
		 *  
		 */
		private void RemoveDataProviderListener()
		{
			if (null != _dataProvider)
				_dataProvider.RemoveEventListener(CollectionEvent.COLLECTION_CHANGE, DataProviderCollectionChangeHandler);
		}

		private void DataProviderCollectionChangeHandler(Event e)
		{
			CollectionEvent ce = (CollectionEvent) e;

			switch (ce.Kind)
			{
				case CollectionEventKind.ADD:
				{
					// items are added
					// figure out what items were added and where
					// for virtualization also figure out if items are now in view
					AdjustAfterAdd(ce.Items, ce.Location);
					break;
				}
					
				case CollectionEventKind.REPLACE:
				{
					// items are replaced
					AdjustAfterReplace(ce.Items, ce.Location);
					break;
				}
					
				case CollectionEventKind.REMOVE:
				{
					// items are added
					// figure out what items were removed
					// for virtualization also figure out what items are now in view
					AdjustAfterRemove(ce.Items, ce.Location);
					break;
				}
					
				case CollectionEventKind.MOVE:
				{
					// one item is moved
					AdjustAfterMove(ce.Items[0], ce.Location, ce.OldLocation);
					break;
				}
					
				case CollectionEventKind.REFRESH:
				{
					// from a filter or sort...let's just reset everything
					RemoveDataProviderListener();
					_dataProviderChanged = true;
					InvalidateProperties();
					break;
				}
					
				case CollectionEventKind.RESET:
				{
					// reset everything
					RemoveDataProviderListener();                
					_dataProviderChanged = true;
					InvalidateProperties();
					break;
				}
					
				case CollectionEventKind.UPDATE:
				{
					// if a renderer is currently being updated, let's 
					// just ignore any UPDATE events.
					if (_renderersBeingUpdated)
						break;
					
					//update the renderer's data and data-dependant
					//properties. 
					for (int i = 0; i < ce.Items.Count; i++)
					{
						PropertyChangeEvent pce = (PropertyChangeEvent) ce.Items[i]; 
						if (null != pce)
						{
							int index = DataProvider.GetItemIndex(pce.Source);
                            IVisualElement renderer = _indexToRenderer[index];
                            SetUpItemRenderer(renderer, index, pce.Source); 
						}
					}
					break;
				}
			}
		}

		/**
		 *  
		 */
		private void AdjustAfterAdd(IList<object> items, int location)
		{
			int length = items.Count;
			for (int i = 0; i < length; i++)
			{
				ItemAdded(items[i], location + i);
			}
			
			// the order might have changed, so we might need to redraw the other 
			// renderers that are order-dependent (for instance alternatingItemColor)
			ResetRenderersIndices();
		}
		
		/**
		 *  
		 */
		private void AdjustAfterRemove(IList<object> items, int location)
		{
			int length = items.Count;
			for (int i = length-1; i >= 0; i--)
			{
				ItemRemoved(items[i], location + i);
			}
			
			// the order might have changed, so we might need to redraw the other 
			// renderers that are order-dependent (for instance alternatingItemColor)
			ResetRenderersIndices();
		}
		
		/**
		 *  
		 */
		private void AdjustAfterMove(object item, int location, int oldLocation)
		{
			ItemRemoved(item, oldLocation);
			
			// if item is removed before the newly added item
			// then change index to account for this
			if (location > oldLocation)
				ItemAdded(item, location-1);
			else
				ItemAdded(item, location);
		}
		
		/**
		 *  
		 */
// ReSharper disable MemberCanBeMadeStatic.Local
// ReSharper disable SuggestBaseTypeForParameter
// ReSharper disable UnusedParameter.Local
		private void AdjustAfterReplace(List<object> items, int location)
// ReSharper restore UnusedParameter.Local
// ReSharper restore SuggestBaseTypeForParameter
// ReSharper restore MemberCanBeMadeStatic.Local
		{
			int length = items.Count;
			for (int i = length - 1; i >= 0; i--)
			{
				//ItemRemoved(((CollectionEvent)items[i]).oldValue, location + i); // TODO: what is being passed here?? oldValue (?) PropertyChangeEvent??              
			}
			
			//for (i = length-1; i >= 0; i--)
			//{
			//    ItemAdded(items[i].newValue, location);
			//}
		}
		
	}
}
