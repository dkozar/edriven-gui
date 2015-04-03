using System.Collections.Generic;
using eDriven.Core.Util;
using eDriven.Gui.Components;

namespace eDriven.Gui.Util
{
    internal sealed class PriorityQueue
    {
        private readonly Dictionary<int, PriorityBin> _priorityBins = new Dictionary<int, PriorityBin>();

        private readonly ObjectPool<PriorityBin> _pool = new ObjectPool<PriorityBin>(1000);

        /**
         *  
         *  The smallest occupied index in arrayOfDictionaries.
         */
        private int _minPriority;

        /**
         *  
         *  The largest occupied index in arrayOfDictionaries.
         */
        private int _maxPriority = -1;

        /**
         *  
         *  Used to keep track of change deltas.
         */
        private int Generation; // TODO

        private PriorityBin _bin;

        private InvalidationManagerClient _obj;

        internal void AddObject(InvalidationManagerClient obj, int priority)
        {
            //Debug.Log(string.Format("AddObject [{0}; {1}]", obj, priority));

            if (_maxPriority < _minPriority)
            {
                _minPriority = priority;
                _maxPriority = priority;
            }
            else
            {
                if (priority < _minPriority)
                    _minPriority = priority;
                if (priority > _maxPriority)
                    _maxPriority = priority;
            }
            
            _bin = _priorityBins.ContainsKey(priority) ? _priorityBins[priority] : null;

            if (null == _bin)
            {
                // If no hash exists for the specified priority, create one.
                //_bin = new PriorityBin(); // TODO: Use ObjectPool of PriorityBins (but not very important)
                _bin = _pool.Get(); // here
                
                _bin.Length = 0; // 20120229
                _bin.Items.Clear();
                //_bin.Reset(); // testing 20120421

                _priorityBins[priority] = _bin;
                _bin.Items.Add(obj);
                _bin.Length++;
            }
            else
            {
                // If we don't already hold the obj in the specified hash, add it
                // and update our item count.
                if (!_bin.Items.Contains(obj))
                {
                    _bin.Items.Add(obj);
                    _bin.Length++;
                }
            }

            Generation++;
        }

        internal InvalidationManagerClient RemoveLargest()
        {
            _obj = null;

            if (_minPriority <= _maxPriority)
            {
                PriorityBin bin = _priorityBins.ContainsKey(_maxPriority) ? _priorityBins[_maxPriority] : null;

                while (null == bin || 0 == bin.Length)
                {
                    _maxPriority--;
                    if (_maxPriority < _minPriority)
                        return null;
                    bin = _priorityBins.ContainsKey(_maxPriority) ? _priorityBins[_maxPriority] : null;
                }

                // Remove the item with largest priority from our priority queue.
                // Must use a for loop here since we're removing a specific item
                // from a 'Dictionary' (no means of directly indexing).
                //foreach (object obj in bin.Items)
                //{
                //    _obj = obj as IInvalidationManagerClient;
                //    RemoveChild(_obj, _maxPriority);
                //    break;
                //}
                if (bin.Items.Count > 0)
                {
                    _obj = bin.Items[0]/* as IInvalidationManagerClient*/;
                    RemoveChild(_obj, _maxPriority);
                }

                bin = _priorityBins.ContainsKey(_maxPriority) ? _priorityBins[_maxPriority] : null;
                while (null == bin || 0 == bin.Length)
                {
                    _maxPriority--;
                    if (_maxPriority < _minPriority)
                        break;
                    bin = _priorityBins.ContainsKey(_maxPriority) ? _priorityBins[_maxPriority] : null;
                }
            }

            return _obj;
        }

        internal InvalidationManagerClient RemoveSmallest()
        {
            _obj = null;

            if (_minPriority <= _maxPriority)
            {
                PriorityBin bin = _priorityBins.ContainsKey(_minPriority) ? _priorityBins[_minPriority] : null;

                while (null == bin || 0 == bin.Length)
                {
                    _minPriority++;
                    if (_minPriority > _maxPriority)
                        return null;

                    bin = _priorityBins.ContainsKey(_minPriority) ? _priorityBins[_minPriority] : null;
                }

                // Remove the item with largest priority from our priority queue.
                // Must use a for loop here since we're removing a specific item
                // from a 'Dictionary' (no means of directly indexing).
                //foreach (object obj in bin.Items)
                //{
                //    _obj = obj as IInvalidationManagerClient;
                //    RemoveChild(_obj, _minPriority);
                //    break;
                //}
                if (bin.Items.Count > 0)
                {
                    _obj = bin.Items[0]/* as IInvalidationManagerClient*/;
                    RemoveChild(_obj, _minPriority);
                }

                bin = _priorityBins.ContainsKey(_minPriority) ? _priorityBins[_minPriority] : null;
                while (null == bin || 0 == bin.Length)
                {
                    _minPriority++;
                    if (_minPriority > _maxPriority)
                        break;

                    bin = _priorityBins.ContainsKey(_minPriority) ? _priorityBins[_minPriority] : null;
                }
            }

            return _obj;
        }

        #region Removing items with greater NestLevel

        public InvalidationManagerClient RemoveLargestChild(InvalidationManagerClient client)
        {
            var max = _maxPriority;
            var min = client.NestLevel; // _minPriority;

            while (min <= max)
            {
                PriorityBin bin = null;

                if (_priorityBins.ContainsKey(max) && _priorityBins[max].Length > 0)
                {
                    bin = _priorityBins[max];
                }
                
                if (null != bin && bin.Length > 0)
                {
                    if (max == client.NestLevel)
                    {
                        // If the current level we're searching matches that of our
                        // client, no need to search the entire list, just check to see
                        // if the client exists in the queue (it would be the only item
                        // at that nestLevel).
                        if (bin.Items.Contains(client))
                        {
                            RemoveChild(client, max);
                            return client;
                        }
                    }
                    else
                    {
                        foreach (InvalidationManagerClient obj in bin.Items)
                        {
                            if (/*obj is Component && */Contains(client, obj))
                            {
                                RemoveChild(obj/* as IInvalidationManagerClient*/, max);
                                return obj;
                            }
                        }
                    }

                    max--;
                }

                else
                {
                    if (max == _maxPriority)
                        _maxPriority--;
                    max--;
                    if (max < min)
                        break;
                }
            }

            return null;
        }

        public InvalidationManagerClient RemoveSmallestChild(InvalidationManagerClient client)
        {
            var min = client.NestLevel;

            while (min <= _maxPriority)
            {
                //var bin = _priorityBins[min];

                if (_priorityBins.ContainsKey(min) && _priorityBins[min].Length > 0)
                {
                    PriorityBin bin = _priorityBins[min];
                //}

                //if (null != bin && bin.Length > 0)
                //{
                    if (min == client.NestLevel)
                    {
                        // If the current level we're searching matches that of our
                        // client, no need to search the entire list, just check to see
                        // if the client exists in the queue (it would be the only item
                        // at that nestLevel).
                        if (bin.Items.Contains(client))
                        {
                            RemoveChild(client, min);
                            return client;
                        }
                    }
                    else
                    {
                        foreach (InvalidationManagerClient obj in bin.Items)
                        {
                            if (/*obj is DisplayListMember && */Contains(/*(IChildList)*/client, /*(DisplayListMember)*/obj))
                            {
                                RemoveChild(obj/* as IInvalidationManagerClient*/, min);
                                return obj;
                            }
                        }
                    }

                    min++;
                }

                else
                {
                    if (min == _minPriority)
                        _minPriority++;
                    min++;
                    if (min > _maxPriority)
                        break;
                }
            }

            return null;
        }

        private static bool Contains(IInvalidationManagerClient parent, DisplayListMember child)
        {
            var doc = parent as DisplayObjectContainer;
            if (null != doc)
            {
                // include me in the search!
                return doc.Contains(child, true); // BUG BUG - this was a bug - it was parent.Children.Contains(), which doesn't go deep, only the direct children!!!
            }

            return parent == child;
        }

        #endregion

// ReSharper disable UnusedMethodReturnValue.Local
        private InvalidationManagerClient RemoveChild(InvalidationManagerClient client, int level)
// ReSharper restore UnusedMethodReturnValue.Local
        {
            //var priority = (level >= -1) ? level : client.NestLevel;
            var priority = (level >= 0) ? level : client.NestLevel; // 20120229

            _bin = _priorityBins.ContainsKey(priority) ? _priorityBins[priority] : null;

            if (null != _bin && _bin.Items.Contains(client))
            {   
                _bin.Items.Remove(client);
                _bin.Length--;

                if (0 == _bin.Length){
                    _pool.Put(_bin);
                    _priorityBins.Remove(priority);
                }

                Generation++;

                return client;
            }
            
            return null;
        }

        internal void RemoveAll()
        {
            foreach (PriorityBin bin in _priorityBins.Values)
            {
                _pool.Put(bin);
            }

            _priorityBins.Clear();

            _minPriority = 0;
            _maxPriority = -1;
            Generation += 1;
        }

        internal bool IsEmpty()
        {
            //Debug.Log(string.Format("IsEmpty {0} > {1}: {2}", _minPriority, _maxPriority, _minPriority > _maxPriority));
            return _minPriority > _maxPriority;
        }
    }

    /**
     *  Represents one priority bucket of entries.
     *  
     */
    internal class PriorityBin
    {
        internal int Length;
        internal List<InvalidationManagerClient> Items = new List<InvalidationManagerClient>();

        //public void Reset() // testing 20120421
        //{
        //    Length = 0;
        //    Items.Clear();
        //}
    }
}
