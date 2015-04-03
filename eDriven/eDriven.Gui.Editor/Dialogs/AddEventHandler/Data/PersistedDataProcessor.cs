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
using eDriven.Core.Events;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Display;
using eDriven.Gui.Editor.Persistence;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// Actual processor of the persisted data
    /// </summary>
    internal class PersistedDataProcessor
    {
        public const string CursorMarker = "|*|";

#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PersistedDataProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PersistedDataProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PersistedDataProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating AddHandlerPersistedDataProcessor instance"));
#endif
                    _instance = new PersistedDataProcessor();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {

        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void Process(AddEventHandlerPersistedData persistedData)
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            //Debug.Log("Process");
            int instanceId = persistedData.TransformInstanceId;
            Transform transform = EditorUtility.InstanceIDToObject(instanceId) as Transform;
            if (null == transform)
            {
                Debug.LogError("Cannot find object with InstanceID=" + instanceId);
            }
            else
            {
                ComponentAdapter adapter = GuiLookup.GetAdapter(transform);
                if (null == adapter)
                {
                    Debug.LogError("Cannot find adapter on transform having InstanceID=" + instanceId);
                }
                else
                {
                    //Debug.Log("Applying persistedData.Action: " + persistedData.Action);
                    Component cmp;

                    switch (persistedData.Action) // MapExistingHandler, AttachExistingScriptAndMapHandler, CreateNewScriptAndHandler
                    {
                        //case AddHandlerAction.MapExistingHandler:
                        default:
                            
                            CreateMapping(persistedData, adapter);
                            EventDisplay.Instance.Refresh();
                            cmp = adapter.gameObject.GetComponent(persistedData.ClassName);
                            break;
                        
                        case AddHandlerAction.AttachExistingScriptAndMapHandler:

                            /**
                             * In the case of the new handler creation, we are allowing the already attached script to be "attached" again
                             * It is not really attached if it already exists - we just don't throw an exception
                             * So, if the ScriptAlreadyAttached flag is true, we are referencing the script using gameObject.GetComponent,
                             * else we are using IO.Util
                             * */
                            cmp = persistedData.ScriptAlreadyAttached ?
                                adapter.gameObject.GetComponent(persistedData.ClassName) :
                                IO.Util.AddHandlerScript(adapter, persistedData);

                            if (null == cmp)
                            {
                                Debug.LogError("Couldn't add event handler script");
                                return;
                            }
                            CreateMapping(persistedData, adapter);
                            //EventDisplay.Instance.Refresh(); 
                            break;
                        
                        case AddHandlerAction.CreateNewHandlerInExistingScript:

                            /**
                             * In the case of the new handler creation, we are allowing the already attached script to be "attached" again
                             * It is not really attached if it already exists - we just don't throw an exception
                             * So, if the ScriptAlreadyAttached flag is true, we are referencing the script using gameObject.GetComponent,
                             * else we are using IO.Util
                             * */
                            cmp = persistedData.ScriptAlreadyAttached ?
                                adapter.gameObject.GetComponent(persistedData.ClassName) : 
                                IO.Util.AddHandlerScript(adapter, persistedData);

                            if (null == cmp)
                            {
                                Debug.LogError("Couldn't add event handler script");
                                return;
                            }
                            CreateMapping(persistedData, adapter);
                            //EventDisplay.Instance.Refresh(); 
                            break;

                        case AddHandlerAction.CreateNewScriptAndHandler:
                            
                            cmp = IO.Util.AddHandlerScript(adapter, persistedData);
                            if (null == cmp)
                            {
                                Debug.LogError("Couldn't add event handler script");
                                return;
                            }
                            CreateMapping(persistedData, adapter);
                            break;
                    }

                    if (null == cmp) // MapExistingHandler
                    {
                        throw new Exception("Couldn't add event the script");
                    }

                    MonoBehaviour mb = cmp as MonoBehaviour;
                    if (null == mb)
                    {
                        throw new Exception("Component is not a MonoBehaviour");
                    }

                    if (persistedData.OpenScript)
                    {
                        // open script
                        var script = MonoScript.FromMonoBehaviour(mb);
                        //Debug.Log("persistedData.SnippetLineNumber: " + persistedData.SnippetLineNumber);
                        AssetDatabase.OpenAsset(script, persistedData.SnippetLineNumber);
                    }

                    /**
                     * 3. Re-scan the hierarchy
                     * */
                    HierarchyViewDecorator.Instance.ReScan(/*adapter.GetInstanceID()*/);
                }
            }
        }

        private static void CreateMapping(AddEventHandlerPersistedData persistedData, ComponentAdapter adapter)
        {
            EventMapping mapping = new EventMapping
            {
                EventType = persistedData.EventName,
                ScriptName = persistedData.ClassName,
                MethodName = persistedData.MethodName,
                Phase = (EventPhase) persistedData.EventPhases
            };
            adapter.EventMap.Add(mapping);
            //EventDisplay.Instance.Refresh(); // TODO: Crashes the app
        }
    }
}