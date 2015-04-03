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
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.MediaQueries;
using eDriven.Gui.Util;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal class StyleDeclarationDialogStep3 : IWizardStep
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static StyleDeclarationDialogStep3 _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StyleDeclarationDialogStep3()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StyleDeclarationDialogStep3 Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StyleDeclarationDialogStep3 instance"));
#endif
                    _instance = new StyleDeclarationDialogStep3();
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
            var mediaQueries = MediaQueryManager.Instance.Queries;

            var descriptors = new Dictionary<string, MediaQueryDescriptor>();
            foreach (var mediaQueryId in mediaQueries.Keys)
            {
                descriptors[mediaQueryId] = new MediaQueryDescriptor(mediaQueryId, TextureCache.Instance.MediaQuery);
            }
            Descriptors = descriptors;

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

        private const string InputTextFocusId = "eDrivenStyleMediaQueryInputField";

        private Dictionary<string, MediaQueryDescriptor> _descriptors = new Dictionary<string, MediaQueryDescriptor>();
        public Dictionary<string, MediaQueryDescriptor> Descriptors
        {
            get { return _descriptors; }
            set { _descriptors = value; }
        }

        private readonly List<MediaQuery> _mediaQueries = new List<MediaQuery>();

        private List<string> _leftPropertyNameList = new List<string>();

        private readonly List<string> _rightPropertyNameList = new List<string>();

        private void BuildLists()
        {
            _leftNewIndex = -1;

            _leftPropertyNameList.Clear();
            _rightPropertyNameList.Clear();

            _mediaQueries.Clear();
            foreach (var descriptor in _descriptors.Values)
            {
                //_mediaQueries.Add(descriptor.ToMediaQuery());
                _mediaQueries.Add(MediaQueryManager.Instance.Queries[descriptor.Name]);
            }

            foreach (MediaQuery mediaQuery in _mediaQueries)
            {
                var match = StyleDeclarationDialog.Instance.Data.MediaQueries.Find(delegate(MediaQuery query)
                {
                    return query.Name == mediaQuery.Name;
                });

                if (!_descriptors.ContainsKey(mediaQuery.Name))
                    continue; // skip

                if (!_leftPropertyNameList.Contains(mediaQuery.Name) && null == match) // if not already selected
                    _leftPropertyNameList.Add(mediaQuery.Name);
            }

            foreach (MediaQuery mediaQuery in StyleDeclarationDialog.Instance.Data.MediaQueries)
            {
                if (!_rightPropertyNameList.Contains(mediaQuery.Name))
                    _rightPropertyNameList.Add(mediaQuery.Name);
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
                    contentList.Add(_descriptors[style].ToGUIContent());
            }
            _leftContents = contentList.ToArray();

            /**
             * 2. Right list
             * */
            contentList.Clear();
            foreach (var prop in StyleDeclarationDialog.Instance.Data.MediaQueries)
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
                GUILayout.Label(GuiContentCache.Instance.NoMediaQueriesFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
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
                    string queryId = _leftPropertyNameList[_leftNewIndex];
                    if (null != queryId)
                    {
                        var attr = GetMediaQuery(queryId);

                        var match = StyleDeclarationDialog.Instance.Data.MediaQueries.Find(delegate(MediaQuery q)
                        {
                            return q.Name == attr.Name;
                        });

                        if (null == match)
                            StyleDeclarationDialog.Instance.Data.MediaQueries.Add(attr);
                    }
                    BuildLists();
                }
                EditorGUILayout.EndScrollView();
            }
                    
            #endregion

            #region Right

            if (_rightContents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoMediaQueriesSelected, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                MediaQuery toRemove = null;

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
                    string queryId = _rightPropertyNameList[_rightNewIndex];
                    if (null != queryId)
                    {
                        toRemove = GetMediaQuery(queryId);
                    }

                    if (null != toRemove)
                    {
                        var match = StyleDeclarationDialog.Instance.Data.MediaQueries.Find(
                                delegate(MediaQuery mediaQuery)
                                {
                                    return mediaQuery.Name == toRemove.Name;
                                });

                        if (null != match)
                        {
                            StyleDeclarationDialog.Instance.Data.MediaQueries.Remove(match);
                        }
                    }
                    else
                    {
                        StyleDeclarationDialog.Instance.Data.MediaQueries.RemoveAt(_rightNewIndex);
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

        private MediaQuery GetMediaQuery(string attributeId)
        {
            var property = _mediaQueries.Find(delegate(MediaQuery query)
            {
                return query.Name == attributeId;
            });

            return property;
        }

        public bool IsValid
        {
            get { return true; } // always true
        }

        public void Reset()
        {
            _searchText = string.Empty;
        }
    }
}