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
using System.Reflection;
using System.Text;
using eDriven.Core.Events;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Designer.Collections;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component=UnityEngine.Component;
using Event=eDriven.Core.Events.Event;
using EventHandler=eDriven.Core.Events.EventHandler;

namespace eDriven.Gui.Designer
{
    [Serializable]
    public class EventMappingDescriptor : SaveableCollectionBase<EventMapping>, ICloneable // where T:ICloneable
    {
        private static readonly Type[] TypeOfEvent = new[] { typeof(Event) };

        private const BindingFlags AllInstanceMethodsBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private static readonly ParameterModifier[] Modifiers = new[] { new ParameterModifier() };

        [Saveable]
        [SerializeField]
        private bool _enabled = true;
        /// <summary>
        /// Enabled
        /// </summary>
        public bool Enabled {
            get
            {
                return _enabled;
            }
            set {
                if (value == _enabled)
                    return;

                _enabled = value;
                if (DesignerState.IsPlaying) // if playing, add/remove handlers
                {
                    ProcessListeners(ComponentAdapter, null, _enabled);
                }
            }
        }

        /// <summary>
        /// Processes multiple event listeners
        /// </summary>
        /// <param name="adapter">Component descriptor referencing event handlers</param>
        /// <param name="component">Component dispatching events</param>
        /// <param name="enabled">Enabled state</param>
        public static void ProcessListeners(ComponentAdapter adapter, Components.Component component, bool enabled)
        {
            if (null == adapter)
                return;

            foreach (EventMapping mapping in adapter.EventMap.Items)
            {
                ProcessListener(adapter, component, mapping, enabled && mapping.Enabled); // use (enabled && mapping.Enabled) here!
            }
        }

        /// <summary>
        /// Processes a single event listener
        /// </summary>
        /// <param name="adapter">Component descriptor referencing event handlers</param>
        /// <param name="component">Component dispatching events</param>
        /// <param name="mapping">Event mapping</param>
        /// <param name="enabled">Enabled state</param>
        public static void ProcessListener(ComponentAdapter adapter, Components.Component component, EventMapping mapping, bool enabled)
        {
            if (null == component && (null == adapter || null == adapter.Component)) // not instantiated (edit mode) and no component supplied
                return; // "null == adapter" check added 20130218 because having problems with prefabs

            //Debug.Log("mapping.ScriptName: " + mapping.ScriptName);
            //Debug.Log("mapping: " + mapping);
            Component script = adapter.GetComponent(mapping.ScriptName);

            if (null == script)
            {
                Debug.LogWarning("Script " + mapping.ScriptName + " not found on " + adapter.gameObject, adapter.gameObject);
                return;
            }

            Type type = script.GetType();
            //Debug.Log("script: " + type);
            //Debug.Log("mapping.MethodName: " + mapping.MethodName);
            MethodInfo methodInfo = type.GetMethod(mapping.MethodName, AllInstanceMethodsBindingFlags, null, TypeOfEvent, Modifiers); //type.GetMethod(mapping.MethodName, TypeOfEvent);
            if (null == methodInfo)
            {
                Debug.LogWarning(string.Format("Method [{0}] not found in script [{1}]", mapping.MethodName, mapping.ScriptName), adapter);
                return;
            }

            //Debug.Log("method: " + methodInfo);
            EventHandler handler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), script, methodInfo);
            //Debug.Log("handler: " + handler);

            var cmp = component ?? adapter.Component;

            if (enabled)
            {
                //Debug.Log("Wiring [" + mapping.EventType + "] event to [" + handler + "] handler on " + componentDescriptor);
                cmp.AddEventListener(mapping.EventType, handler, mapping.Phase);
            }
            else
            {
                cmp.RemoveEventListener(mapping.EventType, handler, mapping.Phase);
            }
        }

        /// <summary>
        /// Processes a single event listener phases
        /// </summary>
        /// <param name="adapter">Component descriptor referencing event handlers</param>
        /// <param name="component">Component dispatching events</param>
        /// <param name="mapping">Event mapping</param>
        /// <param name="enabled">Enabled state</param>
        public static void ProcessPhases(ComponentAdapter adapter, Components.Component component, EventMapping mapping, bool enabled)
        {
            if (null == component && null == adapter.Component)  // not instantiated (edit mode) and no component supplied
                return;

            Component script = adapter.GetComponent(mapping.ScriptName);

            if (null == script)
            {
                Debug.LogWarning("Component " + mapping.ScriptName + " not found on " + adapter.gameObject);
                return;
            }

            Type type = script.GetType();
            MethodInfo methodInfo = type.GetMethod(mapping.MethodName, AllInstanceMethodsBindingFlags, null, TypeOfEvent, Modifiers); //type.GetMethod(mapping.MethodName, TypeOfEvent);
            EventHandler handler = (EventHandler)Delegate.CreateDelegate(typeof(EventHandler), script, methodInfo);

            var cmp = component ?? adapter.Component;

            /**
             * 1. Remove all phases
             * */
            cmp.RemoveEventListener(mapping.EventType, handler, EventPhase.Capture | EventPhase.Target | EventPhase.Bubbling);

            /**
             * 2. If enabled, subscribe again
             * */
            if (enabled)
            {
                cmp.AddEventListener(mapping.EventType, handler, mapping.Phase);
            }
        }

        public override void Add(EventMapping mapping)
        {
            base.Add(mapping);
            ProcessListener(ComponentAdapter, null, mapping, _enabled);
        }

        public override void Remove(EventMapping mapping)
        {
            base.Remove(mapping);
            ProcessListener(ComponentAdapter, null, mapping, false);
        }

        public override void Reorder(EventMapping mapping, int index)
        {
            base.Reorder(mapping, index);

            /**
             * Re-process listeners to get them to the right order
             * */
            ProcessListeners(ComponentAdapter, null, false);
            ProcessListeners(ComponentAdapter, null, _enabled);
        }

        /// <summary>
        /// Resets the collection
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ProcessListeners(ComponentAdapter, null, false);
        }

        public override object Clone()
        {
            //Debug.Log("base.Clone(): " + base.Clone());
            EventMappingDescriptor desc = new EventMappingDescriptor
                                              {
                                                  ComponentAdapter = ComponentAdapter,
                                                  Enabled = Enabled
                                              }; //base.Clone();

            foreach (var mapping in Items)
            {
                //Debug.Log("mapping: " + mapping);
                desc.Add((EventMapping) mapping.Clone());
            }

            return desc;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        override public bool Equals(object other)
        {
            EventMappingDescriptor desc = (EventMappingDescriptor)other;
            if (Enabled != desc.Enabled)
                return false;

            return base.Equals(other);
        }

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return (_enabled.GetHashCode() * 397) ^ (_mappings != null ? _mappings.GetHashCode() : 0);
        //    }
        //}

        #region To string

        private StringBuilder _sb;

        public override string ToString()
        {
            if (null == _sb)
                _sb = new StringBuilder();

            _sb.AppendLine(string.Format("EventMappingDescriptor ({0} items):", Items.Length));
            foreach (EventMapping item in Items)
            {
                _sb.AppendLine("  -> " + item);
            }

            return _sb.ToString();
        }

        #endregion
    }
}