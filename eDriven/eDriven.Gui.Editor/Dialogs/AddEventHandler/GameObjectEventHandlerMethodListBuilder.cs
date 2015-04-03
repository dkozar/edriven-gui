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
using System.Reflection;
using eDriven.Gui.Designer.Adapters;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class GameObjectEventHandlerMethodListBuilder
    {
        public GameObject GameObject;

        private readonly List<MethodInfo> _methods = new List<MethodInfo>();

        public List<MethodInfo> Run()
        {
            MonoBehaviour[] components = GameObject.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour component in components)
            {
                if (component is ComponentAdapter)
                    continue; // ignore component descriptors

                Debug.Log("    -> " + component.GetType());
                _methods.AddRange(Reflection.EditorReflector.GetMethodsBySignature(component.GetType(), typeof(void), typeof(Event))); // NOTE: This should be eDriven.Core.Events.Event or else it won't work!!!!!
            }

            _methods.Sort(EventHandlerComparison);

            return _methods;
        }

        private static readonly Comparison<string> EventTypeComparison = delegate(string et1, string et2)
                                                                             {
                                                                                 return et1.CompareTo(et2);
                                                                             };

        private static readonly Comparison<MethodInfo> EventHandlerComparison = delegate(MethodInfo eh1, MethodInfo eh2)
                                                                                    {
                                                                                        string handler1 = string.Format("{0}.{1}", eh1.ReflectedType, eh1.Name);
                                                                                        string handler2 = string.Format("{0}.{1}", eh2.ReflectedType, eh2.Name);

                                                                                        return handler1.CompareTo(handler2);
                                                                                    };
    }
}