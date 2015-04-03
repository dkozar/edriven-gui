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

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class ChildGroup // : ICloneable
    {
        public string GroupName;

        private readonly List<ComponentAdapter> _adapters; // = new List<ComponentAdapter>();
        
        public List<ComponentAdapter> Adapters
        {
            get
            {
                return _adapters;
            }
        }

        public ChildGroup(List<ComponentAdapter> adapters)
        {
            _adapters = adapters;
        }

        //public void Add(ComponentAdapter adapter)
        //{
        //    if (_adapters.Contains(adapter))
        //        throw new Exception("Child group already contains adapter");
        //    _adapters.Add(adapter);
        //}

        public void Add(ComponentAdapter adapter)
        {
            if (_adapters.Contains(adapter))
                throw new Exception("Child group already contains adapter");
            _adapters.Add(adapter);
        }

        public void AddRange(List<ComponentAdapter> adapters)
        {
            foreach (ComponentAdapter adapter in adapters)
            {
                if (_adapters.Contains(adapter))
                    throw new Exception("Child group already contains adapter");
            }

            _adapters.AddRange(adapters);
        }

        public void Insert(ComponentAdapter adapter, int index)
        {
            if (_adapters.Contains(adapter))
                throw new Exception("Child group already contains adapter");
            _adapters.Insert(index, adapter);
        }

        public void Remove(ComponentAdapter adapter)
        {
            if (!_adapters.Contains(adapter))
                throw new Exception("Child group doesn't contains adapter");
            _adapters.Remove(adapter);
        }

        public void Clear()
        {
            _adapters.Clear();
        }

        //public object Clone()
        //{
        //    return new ChildGroup(ListUtil<ComponentAdapter>.Clone(_adapters));
        //}
    }
}