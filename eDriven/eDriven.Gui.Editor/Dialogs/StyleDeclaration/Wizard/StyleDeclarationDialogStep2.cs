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
using eDriven.Core.Signals;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Styles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.Serialization;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class StyleDeclarationDialogStep2 : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleDeclarationDialogStep2 _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationDialogStep2()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationDialogStep2 Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationDialogStep2 instance"));
#endif
                    _instance = new StyleDeclarationDialogStep2();
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
            var selectedType = StyleDeclarationDialog.Instance.Data.Type;
            if (!string.IsNullOrEmpty(selectedType))
            {
                var type = GlobalTypeDictionary.Instance[selectedType];
                var module = StyleModuleManager.Instance.GetOwnerModule(type);
                Descriptors = module.GetStyleDescriptors(type);
            }

            BuildLists();
        }

        private readonly Signal _gotoSignal = new Signal();
        public Signal GotoSignal
        {
            get { return _gotoSignal; }
        }

        private Vector2 _leftScrollPosition;
        private Vector2 _rightScrollPosition;

        private int _leftNewIndex = -1;
        private int _rightNewIndex = -1;

        private GUIContent[] _leftContents = {};
        private GUIContent[] _rightContents = {};

        private static string[] _strings = new string[0];

        private bool _changed;

        private const string InputTextFocusId = "eDrivenStylePropertyInputField";

        private Dictionary<string, MemberDescriptor> _descriptors = new Dictionary<string, MemberDescriptor>();
        public Dictionary<string, MemberDescriptor> Descriptors
        {
            get { return _descriptors; }
            set { _descriptors = value; }
        }

        private readonly List<StyleProperty> _styleProperties = new List<StyleProperty>();

        private List<string> _leftPropertyNameList = new List<string>();

        private readonly List<string> _rightPropertyNameList = new List<string>();

        private void BuildLists()
        {
            _leftNewIndex = -1;
            
            _leftPropertyNameList.Clear();
            _rightPropertyNameList.Clear();

            _styleProperties.Clear();
            foreach (var descriptor in _descriptors.Values)
            {
                _styleProperties.Add(descriptor.ToStyleProperty());
            }

            foreach (StyleProperty property in _styleProperties)
            {
                var match = StyleDeclarationDialog.Instance.Data.StyleProperties.Find(delegate(StyleProperty stylePropertyDto)
                                                                                                     {
                                                                                                        return stylePropertyDto.Name == property.Name;
                                                                                                     });

                if (!_descriptors.ContainsKey(property.Name))
                    continue; // skip

                if (!_leftPropertyNameList.Contains(property.Name) && null == match) // if not already selected
                    _leftPropertyNameList.Add(property.Name);
            }

            foreach (StyleProperty stylePropertyDto in StyleDeclarationDialog.Instance.Data.StyleProperties)
            {
                if (!_rightPropertyNameList.Contains(stylePropertyDto.Name))
                    _rightPropertyNameList.Add(stylePropertyDto.Name);
            }

            List<string> classes = new List<string>();

            foreach (string s in _leftPropertyNameList)
            {
                if (!string.IsNullOrEmpty(_searchText) && !s.ToUpper().Contains(_searchText.ToUpper()))
                    continue;
                classes.Add(string.Format("{0}", s));
            }

            _leftPropertyNameList = classes;
            _leftPropertyNameList.Sort();

            _rightPropertyNameList.Sort();

            _strings = _leftPropertyNameList.ToArray();

            var ints = new List<int>();
            for (var i = 0; i < _strings.Length; i++)
            {
                ints.Add(i);
            }

            /**
             * 1. Left list
             * */
            List<GUIContent> contentList = new List<GUIContent>();
            foreach (var style in _strings)
            {
                if (_descriptors.ContainsKey(style)) // we might skip some
                    contentList.Add(_descriptors[style].ToGUIContent()); //pack.Module.GetStyleIcon(type, style)));
            }
            _leftContents = contentList.ToArray();

            /**
             * 2. Right list
             * */
            contentList.Clear();
            foreach (var prop in StyleDeclarationDialog.Instance.Data.StyleProperties)
            {
                if (_descriptors.ContainsKey(prop.Name)) // we might skip some
                    contentList.Add(_descriptors[prop.Name].ToGUIContent());
            }
            _rightContents = contentList.ToArray();
        }

        private string _searchText = string.Empty;
        private string _newSearchText = string.Empty;

        private bool _oldEnabled;

        private int _halfWidth;

        public void Render()
        {
            _changed = false;

            _halfWidth = (int) Math.Floor(Screen.width*0.5f - 10);

            if (StyleDeclarationDialog.Instance.ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.ComponentStyleList, MessageType.Info, true);
            }

            /**
             * Toggle buttons (mode switch)
             * */
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            /**
             * Search box
             * */
            _oldEnabled = GUI.enabled;
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            GUILayout.Label(GuiContentCache.Instance.Search, StyleCache.Instance.Label);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            GUI.SetNextControlName(InputTextFocusId);
            _newSearchText = GUILayout.TextField(_searchText, GUILayout.MinWidth(150), GUILayout.MaxWidth(400)/*, GUILayout.Height(30), */);
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
            
            if (_changed)
            {
                _changed = false;
                BuildLists();
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = ColorMixer.FromHex(0x335fd8).ToColor();

            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginVertical(StyleCache.Instance.ScrollviewBackground, GUILayout.ExpandWidth(true));
            
            GUILayout.Space(1);

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            #region Left

            if (_leftContents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoStylePropertiesFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                _leftScrollPosition = EditorGUILayout.BeginScrollView(_leftScrollPosition,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinWidth(_halfWidth));

                _leftNewIndex = -1;
                for (int i = 0; i < _leftContents.Length; i++)
                {
                    var content = _leftContents[i];
                    if (GUILayout.Button(content, StyleCache.Instance.GridButton, GUILayout.ExpandWidth(true)))
                        _leftNewIndex = i;
                }
                
                if (-1 != _leftNewIndex)
                {
                    string propertyName = _leftPropertyNameList[_leftNewIndex];
                    if (null != propertyName)
                    {
                        var attr = GetStyleProperty(propertyName);

                        var match = StyleDeclarationDialog.Instance.Data.StyleProperties.Find(delegate(StyleProperty prop)
                        {
                            return prop.Name == attr.Name;
                        });

                        if (null == match)
                            StyleDeclarationDialog.Instance.Data.StyleProperties.Add(attr);
                    }
                    BuildLists();
                }
                EditorGUILayout.EndScrollView();
            }
                    
            #endregion

            #region Right

            if (_rightContents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoStylePropertiesSelected, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                StyleProperty toRemove = null;

                _rightScrollPosition = EditorGUILayout.BeginScrollView(_rightScrollPosition,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MinWidth(_halfWidth));

                _rightNewIndex = -1;
                for (int i = 0; i < _rightContents.Length; i++)
                {
                    var content = _rightContents[i];
                    if (GUILayout.Button(content, StyleCache.Instance.GridButton, GUILayout.ExpandWidth(true)))
                        _rightNewIndex = i;
                }

                if (-1 != _rightNewIndex)
                {
                    string propertyName = _rightPropertyNameList[_rightNewIndex];
                    if (null != propertyName)
                    {
                        toRemove = GetStyleProperty(propertyName);
                    }

                    if (null != toRemove)
                    {
                        var match =
                            StyleDeclarationDialog.Instance.Data.StyleProperties.Find(
                                delegate(StyleProperty styleProperty)
                                {
                                    return styleProperty.Name == toRemove.Name;
                                });

                        if (null != match)
                        {
                            StyleDeclarationDialog.Instance.Data.StyleProperties.Remove(match);
                        }
                    }
                    else
                    {
                        StyleDeclarationDialog.Instance.Data.StyleProperties.RemoveAt(_rightNewIndex);
                    }
                    BuildLists();
                }
                EditorGUILayout.EndScrollView();
            }

            #endregion

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(2);

            EditorGUILayout.EndVertical();  // panel chrome

            GUILayout.Space(1);
        }

        private StyleProperty GetStyleProperty(string attributeName)
        {
            var property = _styleProperties.Find(delegate(StyleProperty a)
                                {
                                    return a.Name == attributeName;
                                });

            return property;
        }

        public bool IsValid
        {
            get { return StyleDeclarationDialog.Instance.Data.StyleProperties.Count > 0; }
        }

        public void Reset()
        {
            _searchText = string.Empty;
        }
    }
}