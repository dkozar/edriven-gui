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

namespace eDriven.Gui.Editor.Dialogs
{
    internal class ScriptDesc
    {
        public string Name;

        public bool Expanded = true;

        private readonly List<EventHandlerDesc> _eventHandlers = new List<EventHandlerDesc>();
        internal List<EventHandlerDesc> EventHandlers
        {
            get { return _eventHandlers; }
        }

        private GUIContent[] _contents;

        public GUIContent[] Contents
        {
            get { return _contents; }
        }

        public ScriptDesc(string name)
        {
            Name = name;
        }

        internal void Add(EventHandlerDesc desc)
        {
            _eventHandlers.Add(desc);
        }

        public void Clear()
        {
            _eventHandlers.Clear();
            _contents = new GUIContent[] { };
        }

        public void Process()
        {
            _eventHandlers.Sort(NameComparison);
            List<GUIContent> contents = new List<GUIContent>();
            foreach (EventHandlerDesc desc in _eventHandlers)
            {
                contents.Add(desc.GetContent());
            }
            _contents = contents.ToArray();
        }

        public override string ToString()
        {
            return string.Format(@"Type ""{0}"" [{1} descriptors]", Name, _eventHandlers.Count);
        }

        private static readonly Comparison<EventHandlerDesc> NameComparison = delegate(EventHandlerDesc c1, EventHandlerDesc c2)
                                                                           {
                                                                               return c1.Name.CompareTo(c2.Name);
                                                                           };
    }
}