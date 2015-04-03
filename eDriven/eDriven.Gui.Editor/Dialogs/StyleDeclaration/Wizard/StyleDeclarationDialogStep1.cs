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
using eDriven.Gui.Editor.Styles;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class StyleDeclarationDialogStep1 : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleDeclarationDialogStep1 _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationDialogStep1()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationDialogStep1 Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationDialogStep1 instance"));
#endif
                    _instance = new StyleDeclarationDialogStep1();
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
            var moduleCount = StyleModuleManager.Instance.AllModules.Count;
            _buttons = new List<GUIContent>();
            /* Display buttons only if more than one pack */
            if (moduleCount > 1)
            {
                foreach (var module in StyleModuleManager.Instance.AllModules)
                {
                    _buttons.Add(new GUIContent(module.Icon, module.Description));
                }
            }

            //Debug.Log("Initializing with: " + StyleDeclarationDialog.Instance.Data.Type);
            var cmpType = StyleDeclarationDialog.Instance.Data.Type;
            if (!string.IsNullOrEmpty(cmpType)) {
                _searchText = cmpType;
            }

            BuildList();
        }
        
        /// <summary>
        /// Toggle buttons displayed in the top-right
        /// </summary>
        private List<GUIContent> _buttons;
        
        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        private Vector2 _scrollPosition;

        private int _selectedIndex = -1;
        private int _newIndex = -1;

        private GUIContent[] _contents = {};

        private bool _changed;
      
        private const string InputTextFocusId = "eDrivenComponentTypeInputField";

        private List<TypeDescriptor> _typeDescriptorList;

        /// <summary>
        /// Processes the input data and builds the items list
        /// </summary>
        private void BuildList()
        {
            List<string> list = new List<string>();
            /*for (int i = 0; i < _state.Count; i++)
            {
                if (_state[i])
                    list.Add(StyleModuleManager.Instance.AllModules[i].Id);
            }*/

            foreach (var typePack in StyleModuleManager.Instance.ModulesVisibleInStyleDialog)
            {
                list.Add(typePack.Id);
            }

            _typeDescriptorList = StyleModuleManager.Instance.Search(_searchText, list);

            List<GUIContent> contents = new List<GUIContent>();
            foreach (var typeDescriptor in _typeDescriptorList)
            {
                contents.Add(typeDescriptor.ToGUIContent());
            }

            _contents = contents.ToArray();

            if (null != _selectedType)
            {
                for (int i = 0; i < _typeDescriptorList.Count; i++)
                {
                    if (_typeDescriptorList[i].Name == _selectedType)
                    {
                        _selectedIndex = i;
                        break;
                    }
                }
                _selectedType = null;
            }
        }
        
        private string _searchText = string.Empty;
        private string _newSearchText = string.Empty;

        //private string _typeToFind;

        public void Render()
        {
            if (StyleDeclarationDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.StyleableComponentList, MessageType.Info, true);
            }

            /**
             * Toggle buttons (mode switch)
             * */
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            /**
             * Search box
             * */
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false), GUILayout.Width(16));
            GUILayout.FlexibleSpace();

            GUILayout.Label(GuiContentCache.Instance.Search, StyleCache.Instance.Label, GUILayout.ExpandWidth(false));

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();

            GUI.SetNextControlName(InputTextFocusId);
            _newSearchText = GUILayout.TextField(_searchText, GUILayout.MinWidth(150), GUILayout.MaxWidth(400)/*, GUILayout.ExpandWidth(true)*//*, GUILayout.Height(30), */);
            GUI.FocusControl(InputTextFocusId);

            if (_newSearchText != _searchText)
            {
                _searchText = _newSearchText;
                _changed = true;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            if (null != _selectedType)
            {
                _searchText = _selectedType;
                _changed = true;
            }

            /**
             * Reset button
             * */
            if (!string.IsNullOrEmpty(_searchText))
            {
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Width(16));
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

            if (_buttons.Count > 0)
            {
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(false), GUILayout.Width(16));
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < _buttons.Count; i++)
                {
                    var oldEnabled = StyleModuleManager.Instance.AllModules[i].Enabled;
                    bool newEnabled = GUILayout.Toggle(oldEnabled, _buttons[i], StyleCache.Instance.ImageOnlyButton);
                    if (newEnabled != oldEnabled)
                    {
                        StyleModuleManager.Instance.AllModules[i].Enabled = newEnabled;
                        _changed = true;
                    }
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            if (_changed)
            {
                _changed = false;
                BuildList();
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = ColorMixer.FromHex(0x335fd8).ToColor();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));

            if (_contents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoStyleableClassesFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                GUILayout.Space(1);
                    
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

                _newIndex = GUILayout.SelectionGrid(_selectedIndex, _contents, 1,
                                                    StyleCache.Instance.GridButton,
                                                    GUILayout.ExpandWidth(true));

                EditorGUILayout.EndScrollView();

                GUILayout.Space(2);
            }

            EditorGUILayout.EndVertical();  // panel chrome

            GUILayout.Space(1);

            if (_newIndex != _selectedIndex) // proposed = from outside
            {
                _selectedIndex = _newIndex;

                var selectedType = _typeDescriptorList[_selectedIndex].Type;

                bool typeChanged = selectedType.FullName != StyleDeclarationDialog.Instance.Data.Type;

                StyleDeclarationDialog.Instance.Data.Type = selectedType.FullName;

                var module = StyleModuleManager.Instance.GetOwnerModule(selectedType);
                StyleDeclarationDialog.Instance.Data.ModuleId = module.Id;
                StyleDeclarationDialog.Instance.Data.AllowSubjectOmmision = module.AllowSubjectOmmision;

                /* If type changed and multiple clients not supported, clear the current collection */
                if (typeChanged && !module.AllowMultipleClients)
                    StyleDeclarationDialog.Instance.Data.StyleProperties.Clear();

                //StyleDeclarationDialogStep2.Instance.Descriptors = module.GetStyleDescriptors(selectedType);

                /*var mediaQueries = MediaQueryManager.Instance.Queries;

                var descriptors = new Dictionary<string, MediaQueryDescriptor>();
                foreach (var mediaQueryId in mediaQueries.Keys)
                {
                    descriptors[mediaQueryId] = new MediaQueryDescriptor(mediaQueryId, TextureCache.Instance.MediaQuery);
                }
                StyleDeclarationDialogStep3.Instance.Descriptors = descriptors;*/

                GotoSignal.Emit(1);
            }
        }

        public bool IsValid
        {
            get
            {
                return _selectedIndex > -1;
            }
        }

        public void Reset()
        {
            _selectedIndex = -1;
            _searchText = string.Empty;
            _selectedType = null;

            foreach (var module in StyleModuleManager.Instance.AllModules)
            {
                module.Enabled = true;
            }

            BuildList();
        }

        private string _selectedType;

        /// <summary>
        /// Selected type supplied
        /// </summary>
        /// <param name="type"></param>
        public void Select(string type)
        {
            _selectedType = type;
        }
    }
}