using System;
using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Events;
using eDriven.Gui.Reflection;

namespace eDriven.Gui.Data
{
    /// <summary>
    /// Array list subclass with filtering, sorting and refresh
    /// </summary>
    [Event(Name = FILTER_FUNCTION_CHANGED, Type = typeof(Event))]
    [Event(Name = SORT_CHANGED, Type = typeof(Event))]

    public class ArrayCollection : ArrayList, ICollectionView
    {
        public ArrayCollection(List<object> source) : base(source)
        {
        }

        // ReSharper disable InconsistentNaming
        /// <summary>
        /// Filter function changed constant
        /// </summary>
        public const string FILTER_FUNCTION_CHANGED = "filterFunctionChanged";

        /// <summary>
        /// Sort function changed constant
        /// </summary>
        public const string SORT_CHANGED = "sortChanged";
        // ReSharper restore InconsistentNaming

        #region Implementation of ICollectionView

        private FilterFunction _filterFunction;
        /// <summary>
        /// Filter function
        /// </summary>
        public FilterFunction FilterFunction
        {
            get
            {
                return _filterFunction;
            }
            set
            {
                _filterFunction = value;
                DispatchEvent(new Event(FILTER_FUNCTION_CHANGED));
            }
        }

        private Sort _sort;
        /// <summary>
        /// Sort
        /// </summary>
        public Sort Sort
        {
            get
            {
                return _sort;
            }
            set
            {
                _sort = value;
                DispatchEvent(new Event(SORT_CHANGED));
            }
        }

        public bool Contains(object item)
        {
            return GetItemIndex(item) != -1;
        }

        #endregion

        /// <summary>
        /// Refresh
        /// </summary>
        /// <returns></returns>
        public bool Refresh()
        {
            return InternalRefresh(true);
        }

        private bool InternalRefresh(bool dispatch)
        {
            if (null != _sort || null != _filterFunction)
            {
                try
                {
                    PopulateLocalIndex();
                }
                catch(Exception ex)
                {
                    /*pending.addResponder(new ItemResponder(
                        function(data:Object, token:Object = null):void
                        {
                            internalRefresh(dispatch);
                        },
                        function(info:Object, token:Object = null):void
                        {
                            //no-op
                        }));*/
                    return false;
                }

                if (null != _filterFunction)
                {
                    var tmp = new List<object>();
                    var len = _localIndex.Count;
                    for (int i = 0; i < len; i++)
                    {
                        var item = _localIndex[i];
                        if (FilterFunction(item))
                        {
                            tmp.Add(item);
                        }
                    }
                    _localIndex = tmp;
                }
                if (null != _sort)
                {
                    _sort.DoSort(_localIndex);
                    dispatch = true;
                }
            }
            else //if (_localIndex)
            {
                _localIndex = null;
            }

            //revision++;
            //pendingUpdates = null;
            if (dispatch)
            {
                var refreshEvent = new CollectionEvent(CollectionEvent.COLLECTION_CHANGE)
                {
                    Kind = CollectionEventKind.REFRESH
                };
                DispatchEvent(refreshEvent);
            }
            return true;
        }

        List<object> _localIndex;
            
        private void PopulateLocalIndex()
        {
            if (null != Source)
            {
                _localIndex = new List<object>(Source);
            }
            else
            {
                _localIndex = new System.Collections.Generic.List<object>();
            }
        }

        #region IList

        public override int Length
        {
            get
            {
                if (null != _localIndex)
                {
                    return _localIndex.Count;
                }
                return base.Length;
            }
        }

        public override void AddItem(object item)
        {
            AddItemAt(item, Length);
        }

        public override void AddItemAt(object item, int index)
        {
            if (index < 0 || index > Length)
            {
                throw new IndexOutOfRangeException("Range error");
            }

            var listIndex = index;
            //if we're sorted addItemAt is meaningless, just add to the end
            if (null != _localIndex && null != _sort)
            {
                listIndex = base.Length;
            }
            else if (null != _localIndex && null != _filterFunction)
            {
                // if end of filtered list, put at end of source list
                if (listIndex == _localIndex.Count)
                    listIndex = base.Length;
                // if somewhere in filtered list, find it and insert before it
                // or at beginning
                else 
                    listIndex = base.GetItemIndex(_localIndex[index]);
            }
            base.AddItemAt(item, listIndex);
        }

        public override object GetItemAt(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException("Range error");
            }

            if (null != _localIndex)
            {
                return _localIndex[index];
            }
            return base.GetItemAt(index);
        }

        public override int GetItemIndex(object item)
        {
            int i;
        
            if (null != _localIndex && null != _sort)
            {
                /*int startIndex = _sort.FindItem(_localIndex, item, Sort.FIRST_INDEX_MODE);
                if (startIndex == -1)
                    return -1;

                var endIndex:int = _sort.findItem(localIndex, item, Sort.LAST_INDEX_MODE);
                for (i = startIndex; i <= endIndex; i++)
                {
                    if (localIndex[i] == item)
                        return i;
                }*/ // TODO

                return -1;
            }
            if (null != _localIndex && null != _filterFunction)
            {
                var len = _localIndex.Count;
                for (i = 0; i < len; i++)
                {
                    if (_localIndex[i] == item)
                        return i;
                }

                return -1;
            }

            // fallback
            return base.GetItemIndex(item);
        }

        public override void RemoveAll()
        {
            var len = Length;
            if (len > 0)
            {
                if (null != _localIndex)
                {
                    for (var i = len - 1; i >= 0; i--)
                    {
                        RemoveItemAt(i);
                    }
                }
                else
                {
                    base.RemoveAll();
                }
            }
        }

        public override object RemoveItemAt(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException("Range error");
            }

            var listIndex = index;
            if (null != _localIndex)
            {
                var oldItem = _localIndex[index];
                listIndex = base.GetItemIndex(oldItem);
            }
            return base.RemoveItemAt(listIndex);
        }

        public override object SetItemAt(object item, int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException("Range error");
            }

            var listIndex = index;
            if (null != _localIndex)
            {
                if (index > _localIndex.Count)
                {
                    listIndex = base.Length;
                }
                else
                {
                    var oldItem = _localIndex[index];
                    listIndex = base.GetItemIndex(oldItem);
                }
            }
            return base.SetItemAt(item, listIndex);
        }

        public override object[] ToArray()
        {
            return null != _localIndex ? _localIndex.ToArray() : base.ToArray();
        }

        #endregion

    }
}
