using eDriven.Gui.Data;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Components
{
    #region Event metadata

    ///<summary>
    /// Base container class for data items
    ///</summary>
    [Event(Name = RendererExistenceEvent.RENDERER_ADD, Type = typeof(RendererExistenceEvent), Bubbles = false)]
    [Event(Name = RendererExistenceEvent.RENDERER_REMOVE, Type = typeof(RendererExistenceEvent), Bubbles = false)]
    
    #endregion

    // ReSharper disable UnusedMember.Global
    public class SkinnableDataContainer : SkinnableContainerBase, IItemRendererOwner
// ReSharper restore UnusedMember.Global
    {
        #region Skin parts

// ReSharper disable FieldCanBeMadeReadOnly.Global
        // ReSharper disable MemberCanBePrivate.Global

        ///<summary>
        /// Data group
        ///</summary>
        [SkinPart(Required=false)]
        public DataGroup DataGroup;

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        #endregion

        /**
         *  
         *  Several properties are proxied to dataGroup.  However, when dataGroup
         *  is not around, we need to store values set on SkinnableDataContainer.  This object 
         *  stores those values.  If dataGroup is around, the values are stored 
         *  on the dataGroup directly.  However, we need to know what values 
         *  have been set by the developer on the SkinnableDataContainer (versus set on 
         *  the dataGroup or defaults of the dataGroup) as those are values 
         *  we want to carry around if the dataGroup changes (via a new skin). 
         *  In order to store this info effeciently, dataGroupProperties becomes 
         *  a uint to store a series of BitFlags.  These bits represent whether a 
         *  property has been explicitly set on this SkinnableDataContainer.  When the 
         *  dataGroup is not around, dataGroupProperties is a typeless 
         *  object to store these proxied properties.  When dataGroup is around,
         *  dataGroupProperties stores booleans as to whether these properties 
         *  have been explicitly set or not.
         */
        private DataGroupProperties _dataGroupProperties = new DataGroupProperties();

        // ReSharper disable UnusedMember.Global

        ///<summary>
        /// Auto layout
        ///</summary>
        public virtual bool AutoLayout
        {
            get
            {
                if (null != DataGroup)
                    return DataGroup.AutoLayout;
                
                // want the default to be true
                bool? autoLayout = _dataGroupProperties.AutoLayout;
                return autoLayout ?? true;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.AutoLayout = value;
                    _dataGroupProperties.AutoLayoutSet = true;
                }
                else
                    _dataGroupProperties.AutoLayout = value;
            }
        }

        ///<summary>
        /// Data provider
        ///</summary>
        public virtual IList DataProvider
        {
            get
            {
                return (null != DataGroup) ? 
                    DataGroup.DataProvider :
                    _dataGroupProperties.DataProvider;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.DataProvider = value;
                    _dataGroupProperties.DataProviderSet = true;
                }
                else
                    _dataGroupProperties.DataProvider = value;
            }
        }

        ///<summary>
        /// Item renderer
        ///</summary>
        public virtual IFactory ItemRenderer
        {
            get
            {
                return (null != DataGroup) ?
                    DataGroup.ItemRenderer :
                    _dataGroupProperties.ItemRenderer;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.ItemRenderer = value;
                    _dataGroupProperties.ItemRendererSet = true;
                }
                else
                    _dataGroupProperties.ItemRenderer = value;
            }
        }

        ///<summary>
        /// Item renderer function
        ///</summary>
        public virtual ItemRendererFunction ItemRendererFunction
        {
            get
            {
                return (null != DataGroup) ?
                    DataGroup.ItemRendererFunction :
                    _dataGroupProperties.ItemRendererFunction;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.ItemRendererFunction = value;
                    _dataGroupProperties.ItemRendererFunctionSet = true;
                }
                else
                    _dataGroupProperties.ItemRendererFunction = value;
            }
        }

        ///<summary>
        /// Layout
        ///</summary>
        public virtual LayoutBase Layout
        {
            get
            {
                if (null != DataGroup)
                    return DataGroup.Layout;
                return _dataGroupProperties.Layout;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.Layout = value;
                    _dataGroupProperties.LayoutSet = true;
                }
                else
                    _dataGroupProperties.Layout = value;
            }
        }

        ///<summary>
        /// Typical item
        ///</summary>
        public virtual object TypicalItem
        {
            get
            {
                if (null != DataGroup)
                    return DataGroup.TypicalItem;
                return _dataGroupProperties.TypicalItem;
            }
            set
            {
                if (null != DataGroup)
                {
                    DataGroup.TypicalItem = value;
                    _dataGroupProperties.TypicalItemSet = true;
                }
                else
                    _dataGroupProperties.TypicalItem = value;
            }
        }

        // ReSharper restore UnusedMember.Global

        #region Implementation of IItemRendererOwner

        ///<summary>
        /// Returns the String for display in an item renderer
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        public virtual string ItemToLabel(object item)
        {
            if (null != item)
                return item.ToString();
            return string.Empty;
        }

        ///<summary>
        /// Updates the renderer for reuse
        ///</summary>
        ///<param name="renderer"></param>
        ///<param name="itemIndex"></param>
        ///<param name="data"></param>
        public virtual void UpdateRenderer(IVisualElement renderer, int itemIndex, object data)
        {
            // set the owner
            renderer.Owner = this;
            
            // Set the index
            if (renderer is IItemRenderer)
                ((IItemRenderer)renderer).ItemIndex = itemIndex;

            // set the label to the toString() of the data 
            if (renderer is IItemRenderer)
                ((IItemRenderer)renderer).Text = ItemToLabel(data);
            
            // always set the data last
            if ((renderer is IDataRenderer) && (renderer != data))
                ((IDataRenderer)renderer).Data = data;
        }

        #endregion

        protected override void PartAdded(string partName, object instance)
        {
            base.PartAdded(partName, instance);

            if (instance == DataGroup)
            {
                // copy proxied values from dataGroupProperties (if set) to dataGroup
                
                var newDataGroupProperties = new DataGroupProperties();
                
                if (null != _dataGroupProperties.Layout)
                {
                    DataGroup.Layout = _dataGroupProperties.Layout;
                    newDataGroupProperties.LayoutSet = true;
                }
                
                if (null != _dataGroupProperties.AutoLayout)
                {
                    DataGroup.AutoLayout = (bool) _dataGroupProperties.AutoLayout;
                    newDataGroupProperties.AutoLayoutSet = true;
                }
                
                if (null != _dataGroupProperties.DataProvider)
                {
                    //Debug.Log("_dataGroupProperties.DataProvider: " + _dataGroupProperties.DataProvider);
                    DataGroup.DataProvider = _dataGroupProperties.DataProvider;
                    newDataGroupProperties.DataProviderSet = true;
                }
                
                if (null != _dataGroupProperties.ItemRenderer)
                {
                    DataGroup.ItemRenderer = _dataGroupProperties.ItemRenderer;
                    newDataGroupProperties.ItemRendererSet = true;
                }
                
                if (null != _dataGroupProperties.ItemRendererFunction)
                {
                    DataGroup.ItemRendererFunction = _dataGroupProperties.ItemRendererFunction;
                    newDataGroupProperties.ItemRendererFunctionSet = true;
                }
                           
                if (null != _dataGroupProperties.TypicalItem)
                {
                    DataGroup.TypicalItem = _dataGroupProperties.TypicalItem;
                    newDataGroupProperties.TypicalItemSet = true;
                }

                _dataGroupProperties = newDataGroupProperties;
                
                // Register our instance as the dataGroup's item renderer update delegate.
                DataGroup.RendererUpdateDelegate = this;
                
                // The only reason we have these listeners is to re-dispatch events.  
                // We only add as necessary.
                
                if (HasEventListener(RendererExistenceEvent.RENDERER_ADD))
                {
                    DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_ADD, DispatchEvent);
                }
                
                if (HasEventListener(RendererExistenceEvent.RENDERER_REMOVE))
                {
                    DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_REMOVE, DispatchEvent);
                }
            }
        }

        protected override void PartRemoved(string partName, object instance)
        {
            base.PartRemoved(partName, instance);

            if (instance == DataGroup)
            {
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_ADD, DispatchEvent);
                DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_REMOVE, DispatchEvent);
                
                // copy proxied values from DataGroup (if explicitly set) to DataGroupProperties

                var newDataGroupProperties = new DataGroupProperties();

                if (_dataGroupProperties.LayoutSet)
                    newDataGroupProperties.Layout = DataGroup.Layout;

                if (_dataGroupProperties.AutoLayoutSet)
                    newDataGroupProperties.AutoLayout = DataGroup.AutoLayout;

                if (_dataGroupProperties.DataProviderSet)
                    newDataGroupProperties.DataProvider = DataGroup.DataProvider;

                if (_dataGroupProperties.ItemRendererSet)
                    newDataGroupProperties.ItemRenderer = DataGroup.ItemRenderer;

                if (_dataGroupProperties.ItemRendererFunctionSet)
                    newDataGroupProperties.ItemRendererFunction = DataGroup.ItemRendererFunction;

                if (_dataGroupProperties.TypicalItemSet)
                    newDataGroupProperties.TypicalItem = DataGroup.TypicalItem;
                
                _dataGroupProperties = newDataGroupProperties;
                
                DataGroup.DataProvider = null;
                DataGroup.Layout = null;
                DataGroup.RendererUpdateDelegate = null;
            }
        }

        #region Listeners

        public override void AddEventListener(string eventType, Core.Events.EventHandler handler, Core.Events.EventPhase phases, int priority)
        {
            base.AddEventListener(eventType, handler, phases, priority);

            if (eventType == RendererExistenceEvent.RENDERER_ADD && null != DataGroup)
            {
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_ADD, DispatchEvent);
            }

            if (eventType == RendererExistenceEvent.RENDERER_REMOVE && null != DataGroup)
            {
                DataGroup.AddEventListener(RendererExistenceEvent.RENDERER_REMOVE, DispatchEvent);
            }
        }

        public override void RemoveEventListener(string eventType, Core.Events.EventHandler handler, Core.Events.EventPhase phases)
        {
            base.RemoveEventListener(eventType, handler, phases);

            // if no one's listening to us for this event any more, let's 
            // remove our underlying event listener from the dataGroup.
            if (eventType == RendererExistenceEvent.RENDERER_ADD && null != DataGroup)
            {
                if (!HasEventListener(RendererExistenceEvent.RENDERER_ADD))
                {
                    DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_ADD, DispatchEvent);
                }
            }

            if (eventType == RendererExistenceEvent.RENDERER_REMOVE && null != DataGroup)
            {
                if (!HasEventListener(RendererExistenceEvent.RENDERER_REMOVE))
                {
                    DataGroup.RemoveEventListener(RendererExistenceEvent.RENDERER_REMOVE, DispatchEvent);
                }
            }
        }

        #endregion

    }
}