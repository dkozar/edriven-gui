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
using System.Text.RegularExpressions;
using eDriven.Core.Reflection;
using eDriven.Gui.Components;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Editor.Util;
using eDriven.Gui.Reflection;
using UnityEditor;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal sealed partial class ReferenceWindow : EDrivenEditorWindowBase
    {
        //[ExecuteInEditMode]

        //private SystemManager _systemManager;

        [MenuItem("Window/eDriven.Gui/Reference")]
// ReSharper disable UnusedMember.Local
        static void Init()
// ReSharper restore UnusedMember.Local
        {
            /**
             * Instantiate window
             * */
            var window = GetWindow(typeof(ReferenceWindow), false, "Reference");
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(100, 100);
            window.Show();

            IconSetter.SetIcon(window);
        }

        private static readonly PanelRenderer PanelRenderer = new PanelRenderer
        {
            Collapsible = false,
            Tools = new List<ToolDescriptor>(new[]
            {
                new ToolDescriptor("help", TextureCache.Instance.Help)
            })
        };

        [Obfuscation(Exclude = true)]
        // ReSharper disable UnusedMember.Local
        void OnEnable()
            // ReSharper restore UnusedMember.Local
        {
            IconSetter.SetIcon(this);
        }

        private Vector2 _scrollPosition;
        
        private GUIContent[] _contents = { };

        private const string SearchTextFocusId = "eDrivenComponentReferenceWindowSearchField";

        public static bool ShowHelp;
        private int _selectedIndex;
        private int _newIndex;
        private readonly List<Type> _classes = new List<Type>();

        private string _searchText = string.Empty;
        private string _newSearchText = string.Empty;

        private bool _filterChanged;
        private bool _selectionChanged;
        
        private Type _type;

        private string _componentDescription = string.Empty;
        private bool _state;

        private bool _showComponents;
        private bool _showSkinnableComponents;
        private bool _showSkins;
        private bool _initialized;

        internal void RefreshComponentList()
        {
            _showComponents = EditorSettings.ReferenceShowComponents;
            _showSkinnableComponents = EditorSettings.ReferenceShowSkinnableComponents;
            _showSkins = EditorSettings.ReferenceShowSkins;

            _selectedIndex = -1;
            _selectionChanged = false;

            _classes.Clear();

            var allClasses = GuiReflector.GetAllLoadedTypes();

            foreach (var type in allClasses)
            {
                if (typeof(Component).IsAssignableFrom(type))
                {
                    if (!string.IsNullOrEmpty(_searchText) && !PassesSearchFilter(type.FullName, _searchText))
                        /*!type.FullName.ToUpper().Contains(_searchText.ToUpper())*/
                        continue;
                    _classes.Add(type);
                }
            }

            _classes.Sort(TypeSort);
            //Debug.Log("_classes: " + _classes.Count);

            List<GUIContent> contentList = new List<GUIContent>();

            foreach (var @class in _classes)
            {
                var isSkinnableComponent = typeof(SkinnableComponent).IsAssignableFrom(@class);
                var isSkin = typeof(Skin).IsAssignableFrom(@class);
                var isSimpleComponent = !isSkinnableComponent && !isSkin;
                var texture = GuiComponentEvaluator.EvaluateComponentRowIcon(@class);

                if (_showComponents && isSimpleComponent ||
                    _showSkinnableComponents && isSkinnableComponent ||
                    _showSkins && isSkin)
                {
                    contentList.Add(new GUIContent(" " + @class.FullName, texture));    
                }
            }

            _contents = contentList.ToArray();
            //Debug.Log("_contents: " + _contents.Length);
        }

        

        private bool PassesSearchFilter(string fullName, string searchText)
        {
            if (fullName.ToUpper().Contains(searchText.ToUpper()))
                return true;

            return StringUtil.Capitals(fullName).Contains(StringUtil.Capitals(searchText));
        }

        /// <summary>
        /// Renders the controls allowed for instantiation in relation to selected parent
        /// </summary>
        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
// ReSharper disable once InconsistentNaming
        void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            EditorWindowContentWrapper.Start();

            if (null == PanelRenderer.ChromeStyle)
                PanelRenderer.ChromeStyle = StyleCache.Instance.PanelChromeSquared;

            PanelRenderer.RenderStart(GuiContentCache.Instance.ReferencePanelTitle, true);

            PanelContentWrapper.Start();

            ShowHelp = PanelRenderer.ClickedTools.Count > 0 && PanelRenderer.ClickedTools.Contains("help");

            if (ShowHelp)
            {
                EditorGUILayout.HelpBox(Help.ReferenceWindow, MessageType.Info, true);
            }

            switch (EditorSettings.ReferenceTabIndex)
            {
                case 0:
                    RenderSearch();
                    break;
                case 1:
                    RenderDescription();
                    break;
            }

            PanelContentWrapper.End();

            PanelRenderer.RenderEnd();

            GUILayout.Space(2);
            
            EditorWindowContentWrapper.End();
        }

        private void RenderSearch()
        {
            if (!_initialized)
            {
                RefreshComponentList();
                RefreshDescription();
                _initialized = true;
            }

            _filterChanged = false;

            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));

            /**
             * Search box
             * */

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
            GUILayout.FlexibleSpace();

            GUILayout.Label(GuiContentCache.Instance.Search, StyleCache.Instance.Label);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();


            GUI.SetNextControlName(SearchTextFocusId);
            _newSearchText = GUILayout.TextField(_searchText, GUILayout.Width(200), GUILayout.MinWidth(50), GUILayout.MaxWidth(300)/*, GUILayout.Height(30), */);
            GUI.FocusControl(SearchTextFocusId);

            if (_newSearchText != _searchText)
            {
                _searchText = _newSearchText;
                _filterChanged = true;
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
                    _filterChanged = true;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
            }

            GUILayout.FlexibleSpace(); // horizontal

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            _state = GUILayout.Toggle(_showComponents,
                        new GUIContent(string.Empty, _showComponents ? TextureCache.Instance.ComponentOn : TextureCache.Instance.ComponentOff, _printEvents ? "Showing events" : "Hiding events"),
                                              StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _showComponents)
            {
                _filterChanged = true;
                _showComponents = _state;
            }

            _state = GUILayout.Toggle(_showSkinnableComponents,
                                      new GUIContent(string.Empty, _showSkinnableComponents ? TextureCache.Instance.SkinnableComponentOn : TextureCache.Instance.SkinnableComponentOff, _printStyles ? "Showing styles" : "Hiding styles"),
                                      StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _showSkinnableComponents)
            {
                _filterChanged = true;
                _showSkinnableComponents = _state;
            }

            _state = GUILayout.Toggle(_showSkins,
                                      new GUIContent(string.Empty, _showSkins ? TextureCache.Instance.SkinOn : TextureCache.Instance.SkinOff, _printSkins ? "Showing skins" : "Hiding skins"),
                                      StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _showSkins)
            {
                _filterChanged = true;
                _showSkins = _state;
            }

            if (_filterChanged)
            {
                EditorSettings.ReferenceShowComponents = _showComponents;
                EditorSettings.ReferenceShowSkinnableComponents = _showSkinnableComponents;
                EditorSettings.ReferenceShowSkins = _showSkins;
                RefreshComponentList();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(1);

            if (_filterChanged)
            {
                RefreshComponentList();
                _filterChanged = false;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            if (_contents.Length == 0)
            {
                GUILayout.Label(GuiContentCache.Instance.NoComponentsFound, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            else
            {
                _newIndex = GUILayout.SelectionGrid(_selectedIndex, _contents, 1,
                                                     StyleCache.Instance.GridButton,
                                                     GUILayout.ExpandWidth(true));

                if (_selectedIndex != _newIndex)
                {
                    _selectedIndex = _newIndex;
                    _type = _classes[_selectedIndex];
                    EditorSettings.ReferenceSelectedType = _type.FullName;
                    _selectionChanged = true;
                    //Debug.Log("Chosen: " + _type);
                    //AddEventHandlerDialog.Instance.Repaint();
                }
            }

            EditorGUILayout.EndScrollView();

            if (_selectionChanged)
            {
                EditorSettings.ReferenceTabIndex = 1;
                _selectionChanged = false;
                if (null != _type)
                {
                    _printEvents = EditorSettings.ReferencePrintEvents;
                    _printStyles = EditorSettings.ReferencePrintStyles;
                    _printSkins = EditorSettings.ReferencePrintSkins;
                    _printSkinParts = EditorSettings.ReferencePrintSkinParts;
                    _printSignals = EditorSettings.ReferencePrintSignals;
                    RefreshDescription();
                }
            }
        }
        
        // ReSharper disable UnusedMember.Local
        void Update()
        // ReSharper restore UnusedMember.Local
        {
            // fixes the choppines when dragging things in edit mode
            if (!Application.isPlaying)
                Repaint();
        }
    }
}
