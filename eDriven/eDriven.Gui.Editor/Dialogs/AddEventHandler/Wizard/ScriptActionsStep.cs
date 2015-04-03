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

using eDriven.Core.Signals;
using eDriven.Gui.Editor.IO;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class ScriptActionsStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static ScriptActionsStep _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private ScriptActionsStep()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ScriptActionsStep Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating ScriptOptionsStep instance"));
#endif
                    _instance = new ScriptActionsStep();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public void Initialize()
        {
            _hasAttachedHandlers =
                EditorReflector.ContainsEventHandlers(AddEventHandlerDialog.Instance.Adapter.gameObject);

            if (EditorSettings.ScriptExtension == ScriptExtensions.JAVASCRIPT)
                _selectedIndex = 0;
            else if (EditorSettings.ScriptExtension == ScriptExtensions.CSHARP)
                _selectedIndex = 1;
            else if (EditorSettings.ScriptExtension == ScriptExtensions.BOO)
                _selectedIndex = 2;
        }

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }
        
        internal void Process()
        {
            
        }

        private int _selectedIndex;
        private bool _oldSelected;
        private bool _selected;
        //private bool _changed;

        private bool _isChosen;
        private bool _hasAttachedHandlers;
        
        public void Render()
        {
            //GUILayout.Space(-5);

            if (AddEventHandlerDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.ScriptActions, MessageType.Info, true);
            }

            // mode switch
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            _oldSelected = _selectedIndex == 0;
            _selected = GUILayout.Toggle(_oldSelected, GuiContentCache.Instance.ScriptJavascript, StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _oldSelected && _selected)
            {
                _selectedIndex = 0;
                //_changed = true;
                EditorSettings.ScriptExtension = ScriptExtensions.JAVASCRIPT;
            }

            _oldSelected = _selectedIndex == 1;
            _selected = GUILayout.Toggle(_oldSelected, GuiContentCache.Instance.ScriptCSharp, StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _oldSelected && _selected)
            {
                _selectedIndex = 1;
                //_changed = true;
                EditorSettings.ScriptExtension = ScriptExtensions.CSHARP;
            }

            _oldSelected = _selectedIndex == 2;
            _selected = GUILayout.Toggle(_oldSelected, GuiContentCache.Instance.ScriptBoo, StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _oldSelected && _selected)
            {
                _selectedIndex = 2;
                //_changed = true;
                EditorSettings.ScriptExtension = ScriptExtensions.BOO;
            }
            
            EditorGUILayout.EndHorizontal();

            RenderButton1();

            RenderButton2();

            RenderButton3();

            RenderButton4();

            GUILayout.Space(1);
        }

        private void RenderButton1()
        {
            GUI.enabled = _hasAttachedHandlers;

            string text = " Map the attached event handler";
            if (!_hasAttachedHandlers)
            {
                text += " (no handlers found)";
            }

            //if (GUILayout.Button(GuiContentCache.Instance.EventHandlerAddButton, StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            if (GUILayout.Button(new GUIContent(text, TextureCache.Instance.EventHandlerAddButton), StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            {
                _isChosen = true;
                AddEventHandlerDialog.Instance.Data.Action = AddHandlerAction.MapExistingHandler;

                var eventHandlerScriptRetriever = new EventHandlerScriptRetriever
                {
                    Data = AddEventHandlerDialog.Instance.Data,
                    GameObject = AddEventHandlerDialog.Instance.Adapter.gameObject
                };
                var scripts = eventHandlerScriptRetriever.Process();
                AddEventHandlerDialog.Instance.Data.Scripts = scripts;
                EventHandlerListStep.Instance.Scripts = scripts;
                GotoSignal.Emit(1);
            }

            GUI.enabled = true;
        }

        private void RenderButton2()
        {
            string text = " Add the existing script + map event handler";
            if (Application.isPlaying)
                text += " (requires Play mode STOP)";

            if (GUILayout.Button(new GUIContent(text, TextureCache.Instance.EventHandlerScriptAddButton), StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            {
                var scanCommand = new AttachExistingScriptFileScan
                                      {
                                          Adapter = AddEventHandlerDialog.Instance.Adapter,
                                          Data = AddEventHandlerDialog.Instance.Data
                                      };
                _isChosen = scanCommand.Run();
                if (_isChosen)
                {
                    var eventHandlerScriptRetriever = new EventHandlerScriptRetriever
                                                          {
                                                              Data = AddEventHandlerDialog.Instance.Data,
                                                              //GameObject = AddEventHandlerDialog.Instance.Adapter.gameObject
                                                          };
                    var scripts = eventHandlerScriptRetriever.Process();

                    var data = AddEventHandlerDialog.Instance.Data;
                    data.Scripts = scripts;
                    data.ScriptAlreadyAttached = null != data.Adapter.gameObject.GetComponent(data.ClassName); // perhaps we don't need to stop the play mode

                    EventHandlerListStep.Instance.Scripts = scripts;

                    GotoSignal.Emit(1);
                }
            }
        }

        private void RenderButton3()
        {
            string text = " Create new event handler in the existing script";
            if (Application.isPlaying)
                text += " (requires Play mode STOP)";

            //GUI.enabled = false;
            if (GUILayout.Button(new GUIContent(text, TextureCache.Instance.EventHandlerCreateButton), StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            {
                var scanCommand = new AttachExistingScriptFileScan
                {
                    Adapter = AddEventHandlerDialog.Instance.Adapter,
                    Data = AddEventHandlerDialog.Instance.Data,
                    CreatingNewHandler = true
                };
                _isChosen = scanCommand.Run();
                if (_isChosen)
                {
                    var eventHandlerScriptRetriever = new EventHandlerScriptRetriever
                    {
                        Data = AddEventHandlerDialog.Instance.Data,
                        //GameObject = AddEventHandlerDialog.Instance.Adapter.gameObject
                    };

                    var scripts = eventHandlerScriptRetriever.Process();

                    //Debug.Log("AddEventHandlerDialog.Instance.Data: " + AddEventHandlerDialog.Instance.Data);
                    // important:
                    //AddEventHandlerDialog.Instance.Data.Action = AddHandlerAction.CreateNewHandlerInExistingScript;
                    AddEventHandlerDialog.Instance.Data.Scripts = scripts;
                    EventHandlerListStep.Instance.Scripts = scripts;

                    GotoSignal.Emit(2);
                }
            }
            //GUI.enabled = true;
        }

        private void RenderButton4()
        {
            string text = " Create new script + event handler";
            if (Application.isPlaying)
                text += " (requires Play mode STOP)";

            if (GUILayout.Button(new GUIContent(text, TextureCache.Instance.EventHandlerScriptCreateButton), StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            {
                //AddEventHandlerDialog.Instance.Data.Action = AddHandlerAction.CreateNewScriptAndHandler;
                var scanCommand = new CreateNewHandlerScriptFileScan
                {
                    DefaultClassName = "EventHandlers",
                    Adapter = AddEventHandlerDialog.Instance.Adapter,
                    Data = AddEventHandlerDialog.Instance.Data
                };
                _isChosen = scanCommand.Run();

                if (_isChosen)
                {
                    //Debug.Log("CreateNewHandlerScriptFileScan Is chosen: " + AddEventHandlerDialog.Instance.Data);

                    // skip the event handler list, go to the last step directly
                    //Debug.Log("GotoSignal -> Last!");
                    GotoSignal.Emit(2);
                }
            }
        }

        public bool IsValid
        {
            get { return _isChosen; }
        }

        public void Reset()
        {
            _isChosen = false;
        }
    }
}