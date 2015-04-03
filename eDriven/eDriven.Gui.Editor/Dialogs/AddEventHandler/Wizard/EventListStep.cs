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
using eDriven.Core.Signals;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Reflection;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Reflection;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class EventListStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EventListStep _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EventListStep()
        {
            // Constructor is protected
        }

        //public static int[] Ints
        //{
        //    get { return _intEvents; }
        //}

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventListStep Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EventListStep instance"));
#endif
                    _instance = new EventListStep();
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
            _inputMode = EditorSettings.AddEventHandlerInputMode;
        }

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        private Vector2 _scrollPosition;

        private int _selectedIndex = -1;
        private int _newIndex = -1;

        private GUIContent[] _contents = {};

        private readonly List<ScriptDesc> _scripts = new List<ScriptDesc>();
        internal List<ScriptDesc> Scripts
        {
            get { return _scripts; }
        }

        private readonly Dictionary<string, bool> _previousGroupState = new Dictionary<string, bool>();

        internal void Add(EventHandlerDesc desc)
        {
            var script = Scripts.Find(delegate(ScriptDesc groupDesc) { return groupDesc.Name == desc.ScriptName; });
            if (null == script)
            {
                script = new ScriptDesc(desc.ScriptName);
                Scripts.Add(script);
            }
            script.Add(desc);
        }

        internal void Clear()
        {
            foreach (ScriptDesc desc in _scripts)
            {
                _previousGroupState[desc.Name] = desc.Expanded;
                desc.Clear();
            }

            _scripts.Clear();
        }

        private static string[] _strEvents = new string[0];
        private static int[] _intEvents = new int[0];

        private bool _selected;
        private bool _changed;

        private bool _targetMode = true;
        private bool _bubblingMode;

        private bool _inputMode;
        
        private bool _allowBubbling;

        private const string InputTextFocusId = "eDrivenEventInputField";

        private readonly Dictionary<string, EventAttribute> _actualEventDict = new Dictionary<string, EventAttribute>();
        private List<string> _actualEventList;

        internal void Process()
        {
            //Debug.Log("Process!");

            _selectedIndex = -1;

            var adapter = AddEventHandlerDialog.Instance.Adapter;

            // allow th ebubbling button only for container
            _allowBubbling = adapter is GroupAdapter;
            //_allowBubbling = adapter.ComponentType.IsSubclassOf(typeof(Container)); // this way we enable all the containers (even the programmable ones) to be listened to

            //Debug.Log("_allowBubbling: " + _allowBubbling);

            if (!_allowBubbling) {
                _targetMode = true;
                _bubblingMode = false;
            }

            _actualEventDict.Clear();

            if (_targetMode)
            {
                var dict = EditorReflector.GetEvents(adapter);
                foreach (KeyValuePair<string, EventAttribute> pair in dict)
                {
                    if (!_actualEventDict.ContainsKey(pair.Key))
                        _actualEventDict.Add(pair.Key, pair.Value);
                }
            }

            if (_allowBubbling && _bubblingMode)
            {
                var dict = EditorReflector.GetEventsBubblingFromChildren(adapter);
                foreach (KeyValuePair<string, EventAttribute> pair in dict)
                {
                    if (!_actualEventDict.ContainsKey(pair.Key))
                        _actualEventDict.Add(pair.Key, pair.Value);
                }
            }

            _defaultEvent = EditorReflector.GetDefaultEventName(adapter);

            _actualEventList = new List<string>();
            foreach (string key in _actualEventDict.Keys)
            {
                _actualEventList.Add(key);
            }
            
            //Debug.Log("events: " + events.Count);

            List<string> events2 = new List<string>();

            foreach (string s in _actualEventList)
            {
                if (!string.IsNullOrEmpty(_searchText) && !s.ToUpper().Contains(_searchText.ToUpper()))
                    continue;
                events2.Add(string.Format("{0}", s));
            }

            _actualEventList = events2;
            _actualEventList.Sort();

            _strEvents = _actualEventList.ToArray();

            List<int> ints = new List<int>();
            for (int i = 0; i < _strEvents.Length; i++)
            {
                ints.Add(i);
            }
            _intEvents = ints.ToArray();

            List<GUIContent> contentList = new List<GUIContent>();

            foreach (string s in _strEvents)
            {
                contentList.Add(new GUIContent(" " + s, TextureCache.Instance.Event));
            }

            _contents = contentList.ToArray();

            // select a default event
            if (!string.IsNullOrEmpty(_defaultEvent))
            {
                _selectedIndex = _actualEventList.IndexOf(_defaultEvent);

                if (-1 == _selectedIndex)
                    return;

                _inputText = _defaultEvent;
                //Debug.Log("Setting out defaultEvent: " + _defaultEvent);
                //Debug.Log("_actualEventDict[_defaultEvent]: " + _actualEventDict[_defaultEvent]);
                //AddEventHandlerDialog.Instance.Data.EventAttribute = _actualEventDict[_defaultEvent];
                //AddEventHandlerDialog.Instance.Data.EventName = _defaultEvent;
                EventAttribute attr = _actualEventDict[_actualEventList[_selectedIndex]];
                if (null != attr)
                {
                    AddEventHandlerDialog.Instance.Data.EventAttribute = _actualEventDict[attr.Name];
                    //AddEventHandlerDialog.Instance.Data.EventName = null;
                }
                //Debug.Log("EventAttribute: " + AddEventHandlerDialog.Instance.Data.EventAttribute);
            }
        }

        //private static readonly Comparison<ScriptDesc> NameComparison = delegate(ScriptDesc c1, ScriptDesc c2)
        //                                                                    {
        //                                                                        return c1.Name.CompareTo(c2.Name);
        //                                                                    };

        private string _searchText = string.Empty;
        private string _newSearchText = string.Empty;

        private bool _oldEnabled;
        private string _inputText = string.Empty;
        private string _newInputText = string.Empty;
        private string _defaultEvent;

        public void Render()
        {
            //GUILayout.Space(-5);

            if (AddEventHandlerDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.EventList, MessageType.Info, true);
            }

            /**
             * Toggle buttons (mode switch)
             * */
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            _oldEnabled = GUI.enabled;
            GUI.enabled = _allowBubbling && !_inputMode;
            
            _selected = GUILayout.Toggle(_targetMode, new GUIContent("Target", _targetMode ? TextureCache.Instance.EventPhaseTargetOn : TextureCache.Instance.EventPhaseTarget), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _targetMode)
            {
                _targetMode = _selected;
                _changed = true;
            }

            _selected = GUILayout.Toggle(_bubblingMode, new GUIContent("Bubbling", _bubblingMode ? TextureCache.Instance.EventPhaseBubblingOn : TextureCache.Instance.EventPhaseBubbling), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _bubblingMode)
            {
                _bubblingMode = _selected;
                _changed = true;
            }
            GUI.enabled = _oldEnabled;

            /**
             * Search box
             * */
            _oldEnabled = GUI.enabled;
            GUI.enabled = !_inputMode;

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            GUILayout.Label(GuiContentCache.Instance.Search, StyleCache.Instance.Label);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (!_inputMode)
                GUI.SetNextControlName(InputTextFocusId);
            _newSearchText = GUILayout.TextField(_searchText, GUILayout.Width(200)/*, GUILayout.Height(30), */);
            if (!_inputMode)
                GUI.FocusControl(InputTextFocusId);  

            if (_newSearchText != _searchText)
            {
                _searchText = _newSearchText;
                _changed = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            /**
             * Reset button
             * */
            if (!string.IsNullOrEmpty(_searchText))
            {
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(TextureCache.Instance.Cancel, StyleCache.Instance.SmallOpenButton, GUILayout.ExpandWidth(false)))
                {
                    _searchText = string.Empty;
                    _changed = true;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            GUI.enabled = _oldEnabled;

            GUILayout.FlexibleSpace();

            /**
             * Input mode button
             * */
            _selected = GUILayout.Toggle(_inputMode, new GUIContent("Input mode", _inputMode ? TextureCache.Instance.InputModeOn : TextureCache.Instance.InputMode), StyleCache.Instance.Toggle, GUILayout.Height(30), GUILayout.ExpandWidth(false));
            if (_selected != _inputMode)
            {
                _inputMode = _selected;
                _changed = true;

                if (_selectedIndex > -1 && _selectedIndex < _strEvents.Length)
                    _inputText = _strEvents[_selectedIndex - 1];

                EditorSettings.AddEventHandlerInputMode = _inputMode;
            }

            if (_changed)
            {
                _changed = false;
                Process();
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = ColorMixer.FromHex(0x335fd8).ToColor();

            GUI.backgroundColor = Color.white;

//            EditorGUILayout.BeginHorizontal();
//
//            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));

            if (_inputMode)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                GUILayout.FlexibleSpace();

                GUILayout.Label("Event name: ", EditorStyles.largeLabel);

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
                GUILayout.FlexibleSpace();

                GUI.SetNextControlName(InputTextFocusId);
                _newInputText = GUILayout.TextField(_inputText, GUILayout.Width(200)/*, GUILayout.Height(30), */);
                GUI.FocusControl(InputTextFocusId);    

                if (_newInputText != _inputText)
                {
                    _inputText = _newInputText;
                    _changed = true;
                    AddEventHandlerDialog.Instance.Data.EventAttribute = null;
                    AddEventHandlerDialog.Instance.Data.EventName = _inputText;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();

                if (string.IsNullOrEmpty(_inputText))
                {
                    EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
                    GUILayout.FlexibleSpace();

                    GUILayout.Label(GuiContentCache.Instance.Error, GUILayout.ExpandWidth(false));

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (_contents.Length == 0)
                {
                    GUILayout.Label(GuiContentCache.Instance.NoEventsFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                }
                else
                {
                    GUILayout.Space(1);
                    
                    _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

                    _newIndex = GUILayout.SelectionGrid(_selectedIndex, _contents, 1,
                                                        StyleCache.Instance.GridButton,
                                                        GUILayout.ExpandWidth(true));

                    if (_newIndex != _selectedIndex)
                    {
                        _selectedIndex = _newIndex;
                        EventAttribute attr = _actualEventDict[_actualEventList[_selectedIndex]];
                        if (null != attr)
                        {
                            AddEventHandlerDialog.Instance.Data.EventAttribute = _actualEventDict[attr.Name];
                            AddEventHandlerDialog.Instance.Data.EventName = null;
                        }

                        GotoSignal.Emit(1);
                    }

                    EditorGUILayout.EndScrollView();

                    GUILayout.Space(2);
                }
            }

            EditorGUILayout.EndVertical();  // panel chrome

            GUILayout.Space(1);
        }

        public bool IsValid
        {
            get { return _inputMode ?
                _inputText.Length > 0 :
                _selectedIndex > -1; }
        }

        public void Reset()
        {
            _selectedIndex = -1;
        }
    }
}