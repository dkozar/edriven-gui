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
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class GroupDesc
    {
        public string Name;

        public bool Expanded = true;

        private readonly List<ComponentTypeDesc> _components = new List<ComponentTypeDesc>();

        internal List<ComponentTypeDesc> Components
        {
            get { return _components; }
        }

        private GUIContent[] _contents;

        public GUIContent[] Contents
        {
            get { return _contents; }
        }

        public GroupDesc(string groupName)
        {
            Name = groupName;
        }

        internal void Add(ComponentTypeDesc desc)
        {
            _components.Add(desc);
        }

        public void Clear()
        {
            _components.Clear();
            _contents = new GUIContent[] { };
        }

        public void Process()
        {
            _components.Sort(NameComparison);
            List<GUIContent> contents = new List<GUIContent>();
            foreach (ComponentTypeDesc desc in _components)
            {
                contents.Add(desc.GetContent());
            }
            _contents = contents.ToArray();
        }

        public override string ToString()
        {
            return string.Format(@"Type ""{0}"" [{1} descriptors]", Name, _components.Count);
        }

        private static readonly Comparison<ComponentTypeDesc> NameComparison = delegate(ComponentTypeDesc c1, ComponentTypeDesc c2)
                                                                                   {
                                                                                       return c1.Name.CompareTo(c2.Name);
                                                                                   };
    }
}