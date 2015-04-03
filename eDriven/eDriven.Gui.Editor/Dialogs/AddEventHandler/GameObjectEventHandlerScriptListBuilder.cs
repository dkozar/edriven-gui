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
using Event=eDriven.Core.Events.Event;


namespace eDriven.Gui.Editor.Dialogs
{
    internal class GameObjectEventHandlerScriptListBuilder
    {
        //public GameObject GameObject;

        private readonly List<ScriptDesc> _scripts = new List<ScriptDesc>();

        public List<Type> ComponentTypes;

        public List<ScriptDesc> Run()
        {
            //MonoBehaviour[] components = GameObject.GetComponents<MonoBehaviour>();
            foreach (Type type in ComponentTypes)
            {
                if (type.IsSubclassOf(typeof(ComponentAdapter)))
                    continue; // ignore component descriptors

                //Debug.Log("    -> " + type);

                var componentName = type.Name;
                ScriptDesc scriptDesc = new ScriptDesc(type.Name);
                var methods = Reflection.EditorReflector.GetMethodsBySignature(
                    type, 
                    typeof(void),
                    typeof(Event) // NOTE: This is eDriven.Core.Events.Event or else it won't work!!!!!
                );

                //Debug.Log("   methods -> " + methods.Count);

                methods.ForEach(delegate(MethodInfo methodInfo)
                {
                    scriptDesc.Add(new EventHandlerDesc(methodInfo.Name, methodInfo.GetType()) { ScriptName = componentName });
                });

                scriptDesc.EventHandlers.Sort(EventHandlerComparison);

                _scripts.Add(scriptDesc);
            }

            //_scripts.Sort(EventHandlerComparison);

            return _scripts;
        }

//        private static readonly Comparison<string> EventTypeComparison = delegate(string et1, string et2)
//                                                                             {
//                                                                                 return et1.CompareTo(et2);
//                                                                             };

        private static readonly Comparison<EventHandlerDesc> EventHandlerComparison = delegate(EventHandlerDesc eh1, EventHandlerDesc eh2)
        {
            return eh1.Name.CompareTo(eh2.Name);
        };
    }
}