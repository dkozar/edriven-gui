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

using System.Collections.Generic;
using eDriven.Audio;
using eDriven.Core.Signals;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Mappers;
using eDriven.Gui.Styles;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Persistence
{
    /// <summary>
    /// A class handling the drawing into the hierarchy view
    /// </summary>
    internal class HierarchyViewDecorator
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static HierarchyViewDecorator _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private HierarchyViewDecorator()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static HierarchyViewDecorator Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Instantiating HierarchyViewDecorator instance"));
                    }
#endif
                    _instance = new HierarchyViewDecorator();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        #region Members

        /// <summary>
        /// The list of game object IDs containing the component adapter
        /// </summary>
        private readonly List<int> _componentAdapterIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing the component adapter
        /// </summary>
        private readonly List<int> _guiIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing the component adapter
        /// </summary>
        private readonly List<int> _guiInspectorIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing style mappers
        /// </summary>
        private readonly List<int> _styleSheetIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing font mappers
        /// </summary>
        private readonly List<int> _fontMapperIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing audio player mappers
        /// </summary>
        private readonly List<int> _audioPlayerMapperIds = new List<int>();
        
        /// <summary>
        /// The list of game object IDs containing event handler scripts
        /// </summary>
        private readonly List<int> _eventHandlerScriptIds = new List<int>();

        /// <summary>
        /// The list of game object IDs containing event handler scripts
        /// </summary>
        private readonly List<int> _eventHandlerScriptsWithHandlersIds = new List<int>();

        private float _x;
        private float _y;

        #endregion

        /// <summary>
        /// Play mode stopped
        /// </summary>
        public Signal ButtonClickedSignal = new Signal();
        
        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("=== HierarchyViewDecorator INITIALIZE ===");
            }
#endif
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        //private bool _isProcessed;

        private readonly List<int> _scannedItems = new List<int>();

        /// <summary>
        /// hierarchyWindowItemOnGUI callback
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="selectionrect"></param>
        private void OnHierarchyWindowItemOnGUI(int instanceId, Rect selectionrect)
        {
            if (!_scannedItems.Contains(instanceId))
            {
                ProcessGuis(instanceId);
                ProcessGuiInspectors(instanceId);
                ProcessComponentAdapters(instanceId);
                ProcessStyleSheets(instanceId);
                ProcessFontMappers(instanceId);
                ProcessAudioPlayerMappers(instanceId);
                ProcessEventHandlerScripts(instanceId);
                ProcessEventHandlers(instanceId);
                //_isProcessed = true;
                _scannedItems.Add(instanceId);
            }

            Render(instanceId, selectionrect);
        }

        ~HierarchyViewDecorator()  // destructor
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
        }

        #region Rendering

        private void Render(int instanceid, Rect selectionrect)
        {
            _x = selectionrect.x + selectionrect.width - 16 - 3; // 3px = padding right
            _y = selectionrect.y;

            if (_guiInspectorIds.Contains(instanceid))
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.BulletGuiInspector, StyleCache.Instance.Dot);
                _x -= 16;
            }
            else if (_guiIds.Contains(instanceid)) // Gui and GuiInspector are exclusive
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.BulletGui, StyleCache.Instance.Dot);
                _x -= 16;
            }
            if (_componentAdapterIds.Contains(instanceid))
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.SymbolComponentAdapter, StyleCache.Instance.Dot);
                _x -= 16;
            }
            if (_styleSheetIds.Contains(instanceid))
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.BulletStyleSheet, StyleCache.Instance.Dot);
                _x -= 16;
            }
            if (_fontMapperIds.Contains(instanceid))
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.BulletFontMapper, StyleCache.Instance.Dot);
                _x -= 16;
            }
            if (_audioPlayerMapperIds.Contains(instanceid))
            {
                GUI.Label(new Rect(_x, _y, 16, 16), TextureCache.Instance.BulletAudioPlayerMapper, StyleCache.Instance.Dot);
                _x -= 16;
            }
            if (_eventHandlerScriptIds.Contains(instanceid) || _eventHandlerScriptsWithHandlersIds.Contains(instanceid))
            {
                if (GUI.Button(new Rect(_x, _y, 16, 16),
                    _eventHandlerScriptIds.Contains(instanceid) ? TextureCache.Instance.ScriptEventHandlerUsed : TextureCache.Instance.EventHandlerScript, 
                    StyleCache.Instance.Dot))
                {
                    Selection.activeInstanceID = instanceid; // select it, because the button hovers the clickable area
                    ButtonClickedSignal.Emit(instanceid, "event");
                }
                _x -= 16;
            }
        }

        #endregion

        #region Helper

        private void ProcessGuis(int instanceid)
        {
            if (!_guiIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
                Gui gui = obj.GetComponent<Gui>();
                if (null != gui)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven Gui: {0} [{1}]", gui, instanceid));
                    }
#endif
                    _guiIds.Add(instanceid);
                }
            }
        }

        private void ProcessGuiInspectors(int instanceid)
        {
            if (!_guiInspectorIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
                GuiInspector gui = obj.GetComponent<GuiInspector>();
                if (null != gui)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven Gui: {0} [{1}]", gui, instanceid));
                    }
#endif
                    _guiInspectorIds.Add(instanceid);
                }
            }
        }

        private void ProcessComponentAdapters(int instanceid)
        {
            if (!_componentAdapterIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
                ComponentAdapter adapter = obj.GetComponent<ComponentAdapter>();
                if (null != adapter)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven component adapter: {0} [{1}]", adapter, instanceid));
                    }
#endif
                    _componentAdapterIds.Add(instanceid);
                }
            }
        }

        private void ProcessStyleSheets(int instanceid)
        {
            if (!_styleSheetIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);

                /*StyleMapperBase mapper = obj.GetComponent<StyleMapperBase>();
                if (null != mapper)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven style mapper: {0} [{1}]", mapper, instanceid));
                    }
#endif
                    _styleSheetIds.Add(instanceid);
                }*/

                eDrivenStyleSheet stylesheet = obj.GetComponent<eDrivenStyleSheet>();
                if (null != stylesheet)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven stylesheet: {0} [{1}]", stylesheet, instanceid));
                    }
#endif
                    _styleSheetIds.Add(instanceid);
                } 
            }
        }

        private void ProcessFontMappers(int instanceid)
        {
            if (!_fontMapperIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
                FontMapper mapper = obj.GetComponent<FontMapper>();
                if (null != mapper)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven font mapper: {0} [{1}]", mapper, instanceid));
                    }
#endif
                    _fontMapperIds.Add(instanceid);
                }
            }
        }

        private void ProcessAudioPlayerMappers(int instanceid)
        {
            if (!_audioPlayerMapperIds.Contains(instanceid))
            {
                GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
                AudioPlayerMapper mapper = obj.GetComponent<AudioPlayerMapper>();
                if (null != mapper)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Found eDriven font mapper: {0} [{1}]", mapper, instanceid));
                    }
#endif
                    _audioPlayerMapperIds.Add(instanceid);
                }
            }
        }

        private void ProcessEventHandlerScripts(int instanceid)
        {
            /**
             * 1. If there is no component adapter available, this is not what we're looking for
             * */
            if (!_componentAdapterIds.Contains(instanceid))
                return;

            GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
            bool contains = EditorReflector.ContainsEventHandlerScripts(obj);

            /**
             * 2. Check for event handler scripts via reflection
             * */
            if (contains && !_eventHandlerScriptsWithHandlersIds.Contains(instanceid))
            {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Adding eDriven event handler: {0} [{1}]", obj, instanceid));
                    }
#endif
                _eventHandlerScriptsWithHandlersIds.Add(instanceid);
            }
            else if (!contains && _eventHandlerScriptsWithHandlersIds.Contains(instanceid))
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format("Removing eDriven event handler: {0} [{1}]", obj, instanceid));
                }
#endif
                _eventHandlerScriptsWithHandlersIds.Remove(instanceid);
            }
        }

        private void ProcessEventHandlers(int instanceid)
        {
            /**
             * 1. If there is no component adapter available, this is not what we're looking for
             * */
            if (!_componentAdapterIds.Contains(instanceid))
                return;

            GameObject obj = (GameObject)EditorUtility.InstanceIDToObject(instanceid);
            bool contains = EditorReflector.ContainsEventHandlers(obj);

            /**
             * 2. Check for event handler scripts via reflection
             * */
            if (contains && !_eventHandlerScriptIds.Contains(instanceid))
            {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Adding eDriven event handler: {0} [{1}]", obj, instanceid));
                    }
#endif
                    _eventHandlerScriptIds.Add(instanceid);
            }
            else if (!contains && _eventHandlerScriptIds.Contains(instanceid))
            {
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format("Removing eDriven event handler: {0} [{1}]", obj, instanceid));
                }
#endif
                _eventHandlerScriptIds.Remove(instanceid);
            }
        }

        public void ReScan()
        {
            //Debug.Log("ReScan");
            _scannedItems.Clear();
        }

        public void ReScan(int instanceId)
        {
            //Debug.Log("ReScan: " + instanceId);
            _scannedItems.Remove(instanceId);
        }

        #endregion

    }
}