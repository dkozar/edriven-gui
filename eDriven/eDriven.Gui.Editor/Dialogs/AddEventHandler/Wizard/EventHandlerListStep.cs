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
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class EventHandlerListStep : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EventHandlerListStep _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EventHandlerListStep()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventHandlerListStep Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EventListStep instance"));
#endif
                    _instance = new EventHandlerListStep();
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

        }

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        public GUIContent StageContent;

        private int _selectedIndex = -1;
        private int _newIndex = -1;
        private Vector2 _scrollPosition;

        private string _searchText = string.Empty;
        private string _newSearchText = string.Empty;
        private bool _changed;

        private GUIContent[] _contents = {};

        public float Width;

        private List<ScriptDesc> _scripts = new List<ScriptDesc>();
        internal List<ScriptDesc> Scripts
        {
            get { return _scripts; }
            set {
                //Debug.Log("Scripts set: " + value.Count);
                _scripts = value;
                Process();
            }
        }

        private readonly Dictionary<string, bool> _previousGroupState = new Dictionary<string, bool>();

        internal void Clear()
        {
            foreach (ScriptDesc desc in _scripts)
            {
                _previousGroupState[desc.Name] = desc.Expanded;
                desc.Clear();
            }

            _scripts.Clear();
        }

        private static string[] _strHandlers = new string[0];

        private readonly List<EventHandlerDesc> _descriptors = new List<EventHandlerDesc>();

        //private List<string> _actualEventHandlerList;

        internal void Process()
        {
            _selectedIndex = -1;

            List<string> handlerList = new List<string>();

            _descriptors.Clear();

            foreach (ScriptDesc scriptDesc in Scripts)
            {
                foreach (EventHandlerDesc eventDesc in scriptDesc.EventHandlers)
                {
                    var label = string.Format(@"{0}.{1}", eventDesc.ScriptName, eventDesc.Name);
                    if (!string.IsNullOrEmpty(_searchText) && !label.ToUpper().Contains(_searchText.ToUpper()))
                        continue;

                    handlerList.Add(label);
                    _descriptors.Add(eventDesc);
                }
            }

            //handlerList.Sort();

            _strHandlers = handlerList.ToArray();

            List<GUIContent> contentList = new List<GUIContent>();

            foreach (string s in _strHandlers)
            {
                contentList.Add(new GUIContent(" " + s, TextureCache.Instance.EventHandler));
            }

            _contents = contentList.ToArray();
        }

        public void Render()
        {
            GUI.backgroundColor = ColorMixer.FromHex(0x335fd8).ToColor();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            GUILayout.FlexibleSpace();
            GUILayout.Label(GuiContentCache.Instance.Search, StyleCache.Instance.Label); //EditorStyles.largeLabel);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            _newSearchText = GUILayout.TextField(_searchText, GUILayout.Width(200)/*, GUILayout.Height(30), */);
            if (_newSearchText != _searchText)
            {
                _searchText = _newSearchText;
                _changed = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            if (!string.IsNullOrEmpty(_searchText))
            {
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(TextureCache.Instance.Cancel, StyleCache.Instance.SmallOpenButton, GUILayout.ExpandWidth(false)))
                {
                    _searchText = string.Empty;
                    _changed = true;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            //if (EditorSettings.ShowControls)
            //{
            EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));
            // panel content

            if (AddEventHandlerDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.EventHandlerList, MessageType.Info, true);
            }

            GUILayout.Space(1);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            if (_contents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoEventHandlersFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                _newIndex = GUILayout.SelectionGrid(_selectedIndex, _contents, 1,
                                                     StyleCache.Instance.GridButton,
                                                     GUILayout.ExpandWidth(true));

                if (_selectedIndex != _newIndex)
                {
                    _selectedIndex = _newIndex;
                    var descriptor = _descriptors[_selectedIndex];
                    AddEventHandlerDialog.Instance.Data.ClassName = descriptor.ScriptName;
                    AddEventHandlerDialog.Instance.Data.MethodName = descriptor.Name;
                    //Debug.Log("AddEventHandlerDialog.Instance.Data.MethodName set to: " + AddEventHandlerDialog.Instance.Data.MethodName);

                    GotoSignal.Emit(1);
                    //AddEventHandlerDialog.Instance.Repaint();
                }
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(2);

            EditorGUILayout.EndVertical();  // panel chrome

            GUILayout.Space(1);

            if (_changed)
            {
                _changed = false;
                Process();
            }
        }

        public bool IsValid
        {
            get { return _selectedIndex > -1; }
        }

        public void Reset()
        {
            _selectedIndex = -1;
            _scripts.Clear();
        }
    }
}