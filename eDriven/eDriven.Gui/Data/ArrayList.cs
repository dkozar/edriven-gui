using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;
using eDriven.Gui.Util;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Data
{
// ReSharper disable ClassNeverInstantiated.Global
    ///<summary>
    ///</summary>
    [Event(Name = CollectionEvent.COLLECTION_CHANGE, Type = typeof(CollectionEvent))]
    [Event(Name = PropertyChangeEvent.PROPERTY_CHANGE, Type = typeof(PropertyChangeEvent))]

    public class ArrayList : EventDispatcher, IList, IPropertyChangeNotifier
// ReSharper restore ClassNeverInstantiated.Global
    {
        ///<summary>
        /// Constructor
        ///</summary>
        ///<param name="source"></param>
// ReSharper disable UnusedMember.Global
        public ArrayList(List<object> source = null)
// ReSharper restore UnusedMember.Global
        {
            DisableEvents();
            Source = source;
            EnableEvents();
        }

        /**
         *   
         *  Indicates if events should be dispatched.
         *  calls to enableEvents() and disableEvents() effect the value when == 0
         *  events should be dispatched. 
         */
        private int _dispatchEvents;

        private List<object> _source;
// ReSharper disable MemberCanBePrivate.Global
        ///<summary>
        ///</summary>
        public List<object> Source
// ReSharper restore MemberCanBePrivate.Global
        {
            get { return _source; }
            set { 
                int i;
                int len;
                if (null != _source && _source.Count > 0)
                {
                    len = _source.Count;
                    for (i = 0; i < len; i++)
                    {
                        StopTrackUpdates(_source[i]);
                    }
                }
                _source = value ?? new List<object>();
                len = _source.Count;
                for (i = 0; i < len; i++)
                {
                    StartTrackUpdates(_source[i]);
                }
                
                if (_dispatchEvents == 0)
                {
                    CollectionEvent ce = new CollectionEvent(CollectionEvent.COLLECTION_CHANGE, null)
                                            {Kind = CollectionEventKind.RESET};
                    DispatchEvent(ce);
                }
            }
        }

        private string _uid;
        ///<summary>
        ///</summary>
// ReSharper disable UnusedMember.Global
// ReSharper disable ConvertToAutoProperty
        public string Uid
// ReSharper restore ConvertToAutoProperty
// ReSharper restore UnusedMember.Global
        {
            get { return _uid; }
            set { _uid = value; }
        }

        /// <summary>
        /// The number of items in this collection. 0 means no items while -1 means the length is unknown. 
        /// </summary>
        public virtual int Length
        {
            get
            {
                if (null != Source)
                    return Source.Count;
                return 0;
            }
        }

        /**
         *  Get the item at the specified index.
         * 
         *  Param:  index the index in the list from which to retrieve the item
         *  Param:  prefetch int indicating both the direction and amount of items
         *          to fetch during the request should the item not be local.
         *  Returns: the item at that index, null if there is none
         *  @throws ItemPendingError if the data for that index needs to be 
         *                           loaded from a remote location
         *  @throws RangeError if the index &lt; 0 or index &gt;= length
         */
        ///<summary>
        ///</summary>
        ///<param name="index"></param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
// ReSharper disable UnusedMember.Global
        public virtual object GetItemAt(int index)
// ReSharper restore UnusedMember.Global
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException("Range error");
            }
                
            return Source[index];
        }

        /**
         *  Place the item at the specified index.  
         *  If an item was already at that index the new item will replace it and it 
         *  will be returned.
         *
         *  Param:  item the new value for the index
         *  Param:  index the index at which to place the item
         *  Returns: the item that was replaced, null if none
         *  @throws RangeError if index is less than 0 or greater than or equal to length
         */
        public virtual object SetItemAt(object item, int index)
        {
            if (index < 0 || index >= Length) 
            {
                throw new IndexOutOfRangeException("Range error");
            }
            
            object oldItem = Source[index];
            Source[index] = item;
            StopTrackUpdates(oldItem);
            StartTrackUpdates(item);
            
            //dispatch the appropriate events 
            if (_dispatchEvents == 0)
            {
                var hasCollectionListener = HasEventListener(CollectionEvent.COLLECTION_CHANGE);
                var hasPropertyListener = HasEventListener(PropertyChangeEvent.PROPERTY_CHANGE);
                PropertyChangeEvent updateInfo = null; 
                
                if (hasCollectionListener || hasPropertyListener)
                {
                    updateInfo = new PropertyChangeEvent(PropertyChangeEvent.PROPERTY_CHANGE)
                                     {
                                         Kind = PropertyChangeEventKind.UPDATE,
                                         OldValue = oldItem,
                                         NewValue = item,
                                         Property = index.ToString()
                                     };
                }
                
                if (hasCollectionListener)
                {
                    CollectionEvent ce = new CollectionEvent(CollectionEvent.COLLECTION_CHANGE)
                                             {
                                                 Kind = CollectionEventKind.REPLACE,
                                                 Location = index
                                             };
                    ce.Items.Add(updateInfo);
                    DispatchEvent(ce);
                }
                
                if (hasPropertyListener)
                {
                    DispatchEvent(updateInfo);
                }
            }
            return oldItem;    
        }

        /**
         *  Add the specified item to the end of the list.
         *  Equivalent to addItemAt(item, length);
         * 
         *  Param: item the item to add
         */
        public virtual void AddItem(object item)
        {
            AddItemAt(item, Length);
        }

        /// <summary>
        /// Adds the item at the specified index
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public virtual void AddItemAt(object item, int index)
        {
            if (index < 0 || index > Length) 
            {
                throw new IndexOutOfRangeException("Range error");
            }
                
            Source.Insert(index, item);

            StartTrackUpdates(item);
            InternalDispatchEvent(CollectionEventKind.ADD, item, index);
        }

        ///<summary>
        ///</summary>
        ///<param name="addList"></param>
// ReSharper disable UnusedMember.Global
        public virtual void AddAll(IList addList)
// ReSharper restore UnusedMember.Global
        {
            AddAllAt(addList, Length);
        }

// ReSharper disable MemberCanBePrivate.Global
        ///<summary>
        ///</summary>
        ///<param name="addList"></param>
        ///<param name="index"></param>
        public virtual void AddAllAt(IList addList, int index)
// ReSharper restore MemberCanBePrivate.Global
        {
            var length = addList.Length;
            for (var i = 0; i < length; i++)
            {
                AddItemAt(addList.GetItemAt(i), i+index);
            }
        }
        
        public virtual int GetItemIndex(object item)
        {
            //return ArrayUtil.getItemIndex(item, source);
            return Source.IndexOf(item);
        }

        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
// ReSharper disable UnusedMember.Global
        public virtual bool RemoveItem(object item)
// ReSharper restore UnusedMember.Global
        {
            var index = GetItemIndex(item);
            var result = index >= 0;
            if (result)
                RemoveItemAt(index);

            return result;
        }

        public virtual object RemoveItemAt(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new Exception("Out of bounds");
            }

            //object removed = Source.splice(index, 1)[0];
            object removed = Source[index];
            Source.RemoveAt(index);
            StopTrackUpdates(removed);
            InternalDispatchEvent(CollectionEventKind.REMOVE, removed, index);
            //Debug.Log("Removed item at " + index + ", left: " + Length);
            return removed;
        }

        public virtual void RemoveAll()
        {
            if (Length > 0)
            {
                int len = Length;
                for (var i = 0; i < len; i++)
                {
                    StopTrackUpdates(Source[i]);
                }

                Source.Clear();
                InternalDispatchEvent(CollectionEventKind.RESET);
            }    
        }
     
        /// <summary>
        /// Notify the view that an item has been updated
        /// </summary>
        /// <param name="item"></param>
        /// <param name="property"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public void ItemUpdated(object item, object property = null, object oldValue = null, object newValue = null)
        {
            PropertyChangeEvent pce = new PropertyChangeEvent(PropertyChangeEvent.PROPERTY_CHANGE)
                                            {
                                                Kind = PropertyChangeEventKind.UPDATE,
                                                Source = item,
                                                Property = property.ToString(),
                                                OldValue = oldValue,
                                                NewValue = newValue
                                            };

            ItemUpdateHandler(pce);        
        }  

        /**
         *  Return an Array that is populated in the same order as the IList
         *  implementation.  
         * 
         *  @throws ItemPendingError if the data is not yet completely loaded
         *  from a remote location
         */ 
        ///<summary>
        ///</summary>
        ///<returns></returns>
// ReSharper disable MemberCanBePrivate.Global
         public virtual List<object> ToList()
// ReSharper restore MemberCanBePrivate.Global
         {
            return ListUtil<object>.Clone(Source);
         }

         public virtual object[] ToArray()
         {
            return ToList().ToArray();
         }

        //--------------------------------------------------------------------------
        //
        // Internal Methods
        // 
        //--------------------------------------------------------------------------

// ReSharper disable UnusedMember.Local
        private void EnableEvents()
// ReSharper restore UnusedMember.Local
        {
            _dispatchEvents++;
            if (_dispatchEvents > 0)
                _dispatchEvents = 0;
        }

// ReSharper disable UnusedMember.Local
        private void DisableEvents()
// ReSharper restore UnusedMember.Local
        {
            _dispatchEvents--;
        }

        /// <summary>
        /// Dispatches a collection event with the specified information.
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="item"></param>
        /// <param name="location"></param>
        private void InternalDispatchEvent(CollectionEventKind kind, object item = null, int location= -1)
        {
            //Debug.Log(string.Format("InternalDispatchEvent: {0}, {1}, {2}", kind, item, location));
            if (_dispatchEvents == 0)
            {
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
                    var objEvent = new PropertyChangeEvent(PropertyChangeEvent.PROPERTY_CHANGE) {Property = location.ToString()};
                    if (kind == CollectionEventKind.ADD)
                        objEvent.NewValue = item;
                    else
                        objEvent.OldValue = item;
                    DispatchEvent(objEvent);
                }
            }
        }
        
// ReSharper disable MemberCanBePrivate.Global
        protected void ItemUpdateHandler(Event e)
// ReSharper restore MemberCanBePrivate.Global
        {
            PropertyChangeEvent pce = (PropertyChangeEvent)e;
            InternalDispatchEvent(CollectionEventKind.UPDATE, e);
            // need to dispatch object event now
            if (_dispatchEvents == 0 && HasEventListener(PropertyChangeEvent.PROPERTY_CHANGE))
            {
                var objEvent = (PropertyChangeEvent)pce.Clone();
                var index = GetItemIndex(e.Target);
                objEvent.Property = index + "." + pce.Property;
                DispatchEvent(objEvent);
            }
        }

        protected virtual void StartTrackUpdates(object item)
        {
            if (/*null != item &&*/item is IEventDispatcher)
            {
                ((IEventDispatcher)item).AddEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ItemUpdateHandler);
            }
        }

        protected virtual void StopTrackUpdates(object item)
        {
            if (/*null != item && */item is IEventDispatcher)
            {
                ((IEventDispatcher)item).RemoveEventListener(PropertyChangeEvent.PROPERTY_CHANGE, ItemUpdateHandler);
            }
        }
    }
}
