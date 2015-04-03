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
using eDriven.Core.Signals;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class EventHandlerCreationStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EventHandlerCreationStep _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EventHandlerCreationStep()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventHandlerCreationStep Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EventHandlerCreationStep instance"));
#endif
                    _instance = new EventHandlerCreationStep();
                }

                return _instance;
            }
        }

        #endregion

        private AddHandlerDataObject _data;

        private string _action;

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public void Initialize()
        {
            _data = AddEventHandlerDialog.Instance.Data;
            if (null == _data)
                return;

            //Debug.Log("_data: " + _data);

            _action = _data.Action;

            _eventName = _data.GetEventName();
            _className = _data.ClassName;

            if (_data.Action == AddHandlerAction.CreateNewScriptAndHandler)
            {
                var attr = _eventName;
                if (string.IsNullOrEmpty(_eventName))
                    throw new Exception("Event name not defined");

                if (attr.Length > 0)
                {
                    string firstChar = attr[0].ToString();
                    attr = attr.Remove(0, 1);
                    firstChar = firstChar.ToUpper();
                    attr = attr.Insert(0, firstChar);
                }
                _methodName = string.Format("On{0}", attr);
            }
            else
            {
                _methodName = _data.MethodName ?? string.Empty;
            }
            
            //Debug.Log("_methodName: " + _methodName);
            _isEditingScript = _data.Action == AddHandlerAction.CreateNewScriptAndHandler || _data.Action == AddHandlerAction.CreateNewHandlerInExistingScript;
            //_openScript = _data.Action != AddHandlerAction.MapExistingHandler;

            _capturePhase = EditorSettings.CreationSettingsCapturePhase;
            _targetPhase = EditorSettings.CreationSettingsTargetPhase;
            _bubblingPhase = EditorSettings.CreationSettingsBubblingPhase;
            _cast = EditorSettings.CreationSettingsCast;
            _addComponentInstantiatedHandler = EditorSettings.CreationSettingsAddComponentInstantiatedHandler;
            _addInitializeComponentHandler = EditorSettings.CreationSettingsAddInitializeComponentHandler;
            _openScript = EditorSettings.CreationSettingsOpenScript;

            // init error msg
            _errorMsg = GetErrorMessage();
        }

        #region Static

        private const string InputTextFocusId = "eDrivenEventInputField";

        #endregion

        #region Properties

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        public bool IsValid
        {
            get { return IsValidMethodName(_methodName) && 
                         !(_isEditingScript && IsExistingMethodName(_methodName)) 
                         && HasEventPhasesDefined; }
        }

        #endregion

        #region Members

        private string _eventName = string.Empty;
        private string _className = string.Empty;
        private string _methodName = string.Empty;
        private bool _isEditingScript;
        
        private bool _capturePhase;
        private bool _newCapturePhase;

        private bool _targetPhase;
        private bool _newTargetPhase;

        private bool _bubblingPhase;
        private bool _newBubblingPhase;

        private bool _cast;
        private bool _newCast;

        private bool _addComponentInstantiatedHandler;
        private bool _newAddComponentInstantiatedHandler;

        private bool _addInitializeComponentHandler;
        private bool _newAddInitializeComponentHandler;

        private bool _openScript;
        private bool _newOpenScript;
        
        private Vector2 _scrollPosition;

        private string _errorMsg;

        #endregion

        public void Render()
        {
            //GUILayout.Space(-5);

            if (AddEventHandlerDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.EventHandlerCreationSettings, MessageType.Info, true);
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandWidth(true));

            // script name
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent(string.Format(@" Event: ""{0}""", _eventName), TextureCache.Instance.Event), StyleCache.Instance.Label);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // script name
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent(" Script: " + _className, TextureCache.Instance.EventHandlerScript), StyleCache.Instance.Label);
            GUILayout.EndHorizontal();

            GUILayout.Space(4);

            // handler name
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (_isEditingScript)
            {
                GUILayout.Label(new GUIContent(" Event handler: ", TextureCache.Instance.EventHandler), StyleCache.Instance.Label, GUILayout.ExpandWidth(false));
                
                GUI.SetNextControlName(InputTextFocusId);
                string newMethodName = GUILayout.TextField(_methodName, GUILayout.Width(240));
                if (_methodName != newMethodName)
                {
                    _methodName = newMethodName;
                    _errorMsg = GetErrorMessage();
                }
                GUI.FocusControl(InputTextFocusId);  
            }
            else
            {
                GUILayout.Label(new GUIContent(string.Format(" Event handler: {0}", _methodName), TextureCache.Instance.EventHandler), StyleCache.Instance.Label, GUILayout.ExpandWidth(false));
            }
            GUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(_errorMsg))
            {
                EditorGUILayout.HelpBox(_errorMsg, MessageType.Error, true);
            }

            GUILayout.Space(4);

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(new GUIContent(" Event phases: ", TextureCache.Instance.Event), StyleCache.Instance.Label);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            _newCapturePhase = GUILayout.Toggle(_capturePhase, new GUIContent(" Capture phase", _capturePhase ? TextureCache.Instance.EventPhaseCaptureOn : TextureCache.Instance.EventPhaseCapture), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(true));
            if (_capturePhase != _newCapturePhase)
            {
                _capturePhase = _newCapturePhase;
                EditorSettings.CreationSettingsCapturePhase = _capturePhase;
            }

            _newTargetPhase = GUILayout.Toggle(_targetPhase, new GUIContent(" Target phase", _targetPhase ? TextureCache.Instance.EventPhaseTargetOn : TextureCache.Instance.EventPhaseTarget), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(true));
            if (_targetPhase != _newTargetPhase)
            {
                _targetPhase = _newTargetPhase;
                EditorSettings.CreationSettingsTargetPhase = _targetPhase;
            } 
            
            _newBubblingPhase = GUILayout.Toggle(_bubblingPhase, new GUIContent(" Bubbling phase", _bubblingPhase ? TextureCache.Instance.EventPhaseBubblingOn : TextureCache.Instance.EventPhaseBubbling), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(true));
            if (_bubblingPhase != _newBubblingPhase)
            {
                _bubblingPhase = _newBubblingPhase;
                EditorSettings.CreationSettingsBubblingPhase = _bubblingPhase;
            }
            
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            if (!HasEventPhasesDefined)
            {
                EditorGUILayout.HelpBox("Cannot create event handler having none of the event phases supplied.", MessageType.Error, true);
            }

            GUILayout.Space(4);

            if (_isEditingScript)
            {
                _newCast = GUILayout.Toggle(_cast, "Cast to original event type", GUILayout.ExpandWidth(false));
                if (_cast != _newCast)
                {
                    _cast = _newCast;
                    EditorSettings.CreationSettingsCast = _cast;
                }

                if (_action == AddHandlerAction.CreateNewScriptAndHandler)
                {
                    _newAddComponentInstantiatedHandler = GUILayout.Toggle(_addComponentInstantiatedHandler, @"Add the ""ComponentInstantiated"" handler", GUILayout.ExpandWidth(false));
                    if (_addComponentInstantiatedHandler != _newAddComponentInstantiatedHandler)
                    {
                        _addComponentInstantiatedHandler = _newAddComponentInstantiatedHandler;
                        EditorSettings.CreationSettingsAddComponentInstantiatedHandler = _addComponentInstantiatedHandler;
                    }

                    _newAddInitializeComponentHandler = GUILayout.Toggle(_addInitializeComponentHandler, @"Add the ""InitializeComponent"" handler", GUILayout.ExpandWidth(false));
                    if (_addInitializeComponentHandler != _newAddInitializeComponentHandler)
                    {
                        _addInitializeComponentHandler = _newAddInitializeComponentHandler;
                        EditorSettings.CreationSettingsAddInitializeComponentHandler = _addInitializeComponentHandler;
                    }
                }
            }

            _newOpenScript = GUILayout.Toggle(_openScript, @"Open script", GUILayout.ExpandWidth(false));
            if (_openScript != _newOpenScript)
            {
                _openScript = _newOpenScript;
                EditorSettings.CreationSettingsOpenScript = _openScript;
            }

            if (EditorApplication.isPlaying)
            {
                var text = @"The application is currently running.
Play mode will be stopped before "; // TODO: fix texts

                if (_isEditingScript)
                    text += "creating a script.";
                else if (_openScript)
                    text += "opening a script (alowing you to edit it).";
                else
                    text += "adding a script.";

                EditorGUILayout.HelpBox(text, MessageType.Info, true);
            }

            GUILayout.FlexibleSpace();

            //if (GUILayout.Button("Bla", StyleCache.Instance.BigButton, GUILayout.ExpandHeight(true)))
            //{
            //    Debug.Log("First option");
            //}

            GUILayout.EndScrollView();

            GUILayout.Space(1);
        }

        private string GetErrorMessage()
        {
            if (string.IsNullOrEmpty(_methodName))
            {
                return "Method name must be defined.";
            }
            
            if (!IsValidMethodName(_methodName))
            {
                return "Invalid method name.";
            }
            
            if (_isEditingScript && IsExistingMethodName(_methodName))
            {
                return "Method name already exists. Please choose another!";
            }

            return null;
        }

        private static bool IsValidMethodName(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
                return false;

            return System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(methodName);
        }

        private static bool IsExistingMethodName(string methodName)
        {
            // grab the _data
            var data = AddEventHandlerDialog.Instance.Data;
            //Debug.Log("_data.Scripts: " + data.Scripts.Count);
            var script = data.Scripts.Find(delegate(ScriptDesc scriptDesc)
                                               {
                                                   return scriptDesc.Name == data.ClassName;
                                               });

            //Debug.Log("script: " + script);
            if (null == script)
                return false;

            return script.EventHandlers.Exists(delegate(EventHandlerDesc eventHandlerDesc)
                                                   {
                                                       return eventHandlerDesc.Name == methodName;
                                                   });
        }

        private bool HasEventPhasesDefined
        {
            get { return _capturePhase || _targetPhase || _bubblingPhase; }
        }

        public void Reset()
        {
            //_isChosen = false;
        }

        /// <summary>
        /// Processes the last wizard step
        /// </summary>
        internal void Process()
        {
            // grab the _data
            var data = AddEventHandlerDialog.Instance.Data;
            
            // method name
            data.MethodName = _methodName;

            // should we add the casting line, and do we have enough information to cast?
            data.Cast = _cast && 
                        null != data.EventAttribute && 
                        data.EventAttribute.Type.IsSubclassOf(typeof (Core.Events.Event));

            // add creation handlers?
            data.AddComponentInstantiatedHandler = _addComponentInstantiatedHandler;
            data.AddInitializeComponentHandler = _addInitializeComponentHandler;

            // 1. set the OpenScript flag
            data.OpenScript = _openScript;

            // 2. create snippet
            switch (data.Action)
            {
                case AddHandlerAction.CreateNewScriptAndHandler:
                    SnippetCreator.CreateSnippet(data);
                    break;
                case AddHandlerAction.CreateNewHandlerInExistingScript:
                    SnippetCreator.InsertHandler(data);
                    break;
            }

            // copy event phases
            data.EventPhases = 0;
            if (_capturePhase)
                data.EventPhases = data.EventPhases | EventPhase.Capture;
            if (_targetPhase)
                data.EventPhases = data.EventPhases | EventPhase.Target;
            if (_bubblingPhase)
                data.EventPhases = data.EventPhases | EventPhase.Bubbling;

            // save the transform instance ID - it will be looked upon the play mode stop
            data.TransformInstanceId = AddEventHandlerDialog.Instance.Adapter.transform.GetInstanceID();

            // convert to persisted object 
            AddEventHandlerPersistedData persistedData = AddEventHandlerPersistedData.FromDataObject(data);

            switch (data.Action)
            {
                    //case AddHandlerAction.MapExistingHandler:
                    //case AddHandlerAction.AttachExistingScriptAndMapHandler:
                default:
                    AddEventHandlerPersistedData.PostCompileProcessingMode = false;
                    break;
                case AddHandlerAction.CreateNewHandlerInExistingScript:
                case AddHandlerAction.CreateNewScriptAndHandler:
                    /**
                     * In the case of editing scripts, the application will be recompiled (automatically)
                     * so we will do the rest after the recompiling process is finished (not after the play mode is stopped)
                     * */
                    AddEventHandlerPersistedData.PostCompileProcessingMode = true;
                    break;
            }

            PersistedDataProcessingLogic.Process(persistedData);
        }
    }
}