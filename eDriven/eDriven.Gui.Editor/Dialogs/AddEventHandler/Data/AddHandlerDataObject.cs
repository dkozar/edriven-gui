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
using eDriven.Core.Events;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Reflection;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// The action data collected by the AddEventHandlerDialog
    /// </summary>
    internal class AddHandlerDataObject
    {
        /// <summary>
        /// Component adapter
        /// </summary>
        public ComponentAdapter Adapter;

        /// <summary>
        /// Event name if using the input mode
        /// </summary>
        public string EventName;

        /// <summary>
        /// Event attribute (collected from the actual class)
        /// </summary>
        public EventAttribute EventAttribute;

        /// <summary>
        /// The action
        /// </summary>
        public string Action = AddHandlerAction.MapExistingHandler;

        /// <summary>
        /// Script path
        /// </summary>
        public string ScriptPath;
        
        /// <summary>
        /// Script name
        /// </summary>
        public string ClassName;

        /// <summary>
        /// Method name
        /// </summary>
        public string MethodName;

        /// <summary>
        /// New or old attached script
        /// </summary>
        public Type AttachedScriptType;

        /// <summary>
        /// The instance ID of the game object
        /// </summary>
        public int TransformInstanceId;

        /// <summary>
        /// Snippet to create (could be the whole script, or a method)
        /// </summary>
        public string Snippet;

        /// <summary>
        /// The line number to open the snippet at
        /// </summary>
        public int SnippetLineNumber;

        public List<MethodInfo> Methods;

        public List<ScriptDesc> Scripts = new List<ScriptDesc>();

        /// <summary>
        /// Event phases
        /// </summary>
        public EventPhase EventPhases = EventPhase.Target | EventPhase.Bubbling;

        public bool OpenScript;
        
        public bool AddComponentInstantiatedHandler;
        public bool AddInitializeComponentHandler;
        public bool Cast;

        /// <summary>
        /// When adding the new event handler, with editing the existing script
        /// we are alowing to picking the script from disk and editing this script, even if it has already been attached to the same game object
        /// We won't throw an exception while scanning, we simply won't add the script while executing
        /// </summary>
        public bool ScriptAlreadyAttached;

        public string GetEventName()
        {
            if (null != EventAttribute)
            {
                return EventAttribute.Name;
            }
            return EventName;
        }

        public override string ToString()
        {
            return string.Format(@"EventAttribute: {0}, 
Action: {1}, 
Type: ""{2}"", 
AttachedScriptType: ""{3}"", 
MethodName: ""{4}"", 
Snippet: ""{5}"", 
EventPhases: ""{6}"", 
OpenScript: ""{7}""
EventName: ""{8}""", 
             null == EventAttribute ? "-" : EventAttribute.ToString(), 
             Action,
             ClassName ?? "-",
             AttachedScriptType,
             MethodName ?? "-",
             Snippet ?? "-",
             EventPhases, 
             OpenScript,
             EventName ?? "-");
        }

        public void Reset()
        {
            Debug.Log("Reset");
            EventName = null;
            EventAttribute = null;
            ScriptPath = null;
            ClassName = null;
            MethodName = null;
            Methods = null;
            Scripts.Clear();
        }
    }
}