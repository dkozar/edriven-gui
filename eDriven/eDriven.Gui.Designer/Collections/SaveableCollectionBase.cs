#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using System.Collections.Generic;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Designer.Collections
{
    [Saveable]
    [Serializable]
    public class SaveableCollectionBase<T>
    {
        private ComponentAdapter _componentAdapter;
        /// <summary>
        /// Component adapter to which the changes will apply
        /// </summary>
        public ComponentAdapter ComponentAdapter
        {
            protected get
            {
                return _componentAdapter;
            }
            set
            {
                _componentAdapter = value;
            }
        }

        [Saveable]
        [SerializeField]
        private T[] _items = { };

        /// <summary>
        /// Items
        /// </summary>
        public T[] Items
        {
            get
            {
                return _items;   
            }
        }

        /// <summary>
        /// Adds an item
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(T item)
        {
            var list = new List<T>(_items) { item };
            _items = list.ToArray();
        }

        /// <summary>
        /// Removes an item
        /// </summary>
        /// <param name="item"></param>
        public virtual void Remove(T item)
        {
            int found = -1;
            for (int i = 0; i < _items.Length; i++)
            {
                if (Items[i].Equals(item))
                {
                    found = i;
                    break;
                }
            }

            if (found > -1)
            {
                var list = new List<T>(_items);
                list.Remove(item);
                _items = list.ToArray();
            }
        }

        /// <summary>
        /// Reorders item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="index"></param>
        public virtual void Reorder(T item, int index)
        {
            var list = new List<T>(_items);
            int oldIndex = list.IndexOf(item);

            if (oldIndex < index)
            {
                index = Math.Max(index - 1, 0);
            }

            list.Remove(item);
            list.Insert(index, item);
            
            _items = list.ToArray();
        }

        /// <summary>
        /// Resets the collection
        /// </summary>
        public virtual void Reset()
        {
            _items = new T[] { };
        }

        public virtual object Clone()
        {
            SaveableCollectionBase<T> desc = new SaveableCollectionBase<T> { ComponentAdapter = _componentAdapter }; // the same adapter reference
            return desc;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(SaveableCollectionBase<T> other)
        {
            if (_items.Length != other._items.Length)
                return false;

            int len = _items.Length;
            for (int i = 0; i < len; i++)
            {
                if (!_items[i].Equals(other._items[i]))
                    return false;
            }

            return true;
        }

#pragma warning disable 659
        public override bool Equals(object obj)
#pragma warning restore 659
        {
            return Equals((SaveableCollectionBase<T>)obj);
        }

        /// <summary>
        /// int indexer
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public T this[int i]
        {
            get
            {
                return _items[i];
            }
            set
            {
                _items[i] = value;
            }
        }

        public int Count
        {
            get
            {
                return _items.Length;
            }
        }

        public void Insert(int index, T guid)
        {
            List<T> list = new List<T>(_items);
            list.Insert(index, guid);
            _items = list.ToArray();
        }

        public void Clear()
        {
            _items = new T[]{};
        }

        public int FindIndex(Predicate<T> func)
        {
            return (new List<T>(Items)).FindIndex(func);
        }
    }
}