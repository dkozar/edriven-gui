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
using System.Reflection;
using eDriven.Core.Reflection;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Reflection;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [Obfuscation(Exclude = true)]
    internal partial class ReferenceWindow
    {
        private Vector2 _scrollPosition2;

        //private bool _printInheritance;
        private bool _printEvents;
        private bool _printMulticastDelegates;
        private bool _printStyles;
        private bool _printSkins;
        private bool _printSkinParts;
        private bool _printSkinStates;
        private bool _printSignals;

        private void RefreshDescription()
        {
            GUI.FocusControl(SearchTextFocusId); // focus out the text area

            //_printInheritance = EditorSettings.ReferencePrintInheritance;
            _printEvents = EditorSettings.ReferencePrintEvents;
            _printMulticastDelegates = EditorSettings.ReferencePrintMulticastDelegates;
            _printStyles = EditorSettings.ReferencePrintStyles;
            _printSkins = EditorSettings.ReferencePrintSkins;
            _printSkinParts = EditorSettings.ReferencePrintSkinParts;
            _printSkinStates = EditorSettings.ReferencePrintSkinStates;
            _printSignals = EditorSettings.ReferencePrintSignals;

            if (null != _type)
                _componentDescription = ComponentReference.Describe(_type, _printEvents, _printMulticastDelegates, _printStyles, _printSkins, _printSkinParts, _printSkinStates, _printSignals);
        }

        private void RenderDescription()
        {
            if (null == _type)
            {
                if (!string.IsNullOrEmpty(EditorSettings.ReferenceSelectedType))
                {
                    // get previously selected type

                    if (!GlobalTypeDictionary.Instance.ContainsKey(EditorSettings.ReferenceSelectedType)) {
                        Debug.LogError("Reference window: Unknown selected type: " + EditorSettings.ReferenceSelectedType);
                        EditorSettings.ReferenceSelectedType = null; // reset
                        return;
                    }

                    _type = GlobalTypeDictionary.Instance[EditorSettings.ReferenceSelectedType];
                    RefreshDescription();
                }
                else {
                    EditorSettings.ReferenceTabIndex = 0; // fallback to first tab
                    RefreshComponentList();
                }
            }

            _filterChanged = false;

            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(35));
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.Back, StyleCache.Instance.Button, GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                EditorSettings.ReferenceTabIndex = 0;
                _type = null;
                EditorSettings.ReferenceSelectedType = null;
                RefreshComponentList(); // just in case if application recompiled
            }
            
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(GuiContentCache.Instance.CopyToClipboard, StyleCache.Instance.Button, GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                GUI.FocusControl(SearchTextFocusId); // focus out the text area
                TextEditor te = new TextEditor { content = new GUIContent(_componentDescription) };
                te.SelectAll();
                te.Copy();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace(); // horizontal

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(false));
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            #region Events

            _state = GUILayout.Toggle(_printEvents,
                new GUIContent(string.Empty,
                    _printEvents ? TextureCache.Instance.EventOn : TextureCache.Instance.EventOff,
                    _printEvents ? "Showing events" : "Hiding events"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printEvents)
            {
                _filterChanged = true;
                _printEvents = _state;
            }

            #endregion

            #region Multicast delegates

            _state = GUILayout.Toggle(_printMulticastDelegates,
                new GUIContent(string.Empty,
                    _printMulticastDelegates ? TextureCache.Instance.MulticastDelegateOn : TextureCache.Instance.MulticastDelegateOff,
                    _printMulticastDelegates ? "Showing multicast delegates" : "Hiding multicast delegates"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printMulticastDelegates)
            {
                _filterChanged = true;
                _printMulticastDelegates = _state;
            }

            #endregion

            #region Styles

            _state = GUILayout.Toggle(_printStyles,
                new GUIContent(string.Empty,
                    _printStyles ? TextureCache.Instance.StyleOn : TextureCache.Instance.StyleOff,
                    _printStyles ? "Showing styles" : "Hiding styles"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printStyles)
            {
                _filterChanged = true;
                _printStyles = _state;
            }

            #endregion

            #region Skins

            _state = GUILayout.Toggle(_printSkins,
                new GUIContent(string.Empty, _printSkins ? TextureCache.Instance.SkinOn : TextureCache.Instance.SkinOff,
                    _printSkins ? "Showing skins" : "Hiding skins"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printSkins)
            {
                _filterChanged = true;
                _printSkins = _state;
            }

            #endregion

            #region Skin parts

            _state = GUILayout.Toggle(_printSkinParts,
                new GUIContent(string.Empty,
                    _printSkinParts ? TextureCache.Instance.SkinPartOn : TextureCache.Instance.SkinPartOff,
                    _printSkinParts ? "Showing skin parts" : "Hiding skin parts"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printSkinParts)
            {
                _filterChanged = true;
                _printSkinParts = _state;
            }

            #endregion

            #region Skin states

            _state = GUILayout.Toggle(_printSkinStates,
                new GUIContent(string.Empty,
                    _printSkinStates ? TextureCache.Instance.SkinStateOn : TextureCache.Instance.SkinStateOff,
                    _printSkinStates ? "Showing skin states" : "Hiding skin states"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printSkinStates)
            {
                _filterChanged = true;
                _printSkinStates = _state;
            }

            #endregion

            #region Signals

            _state = GUILayout.Toggle(_printSignals,
                new GUIContent(string.Empty,
                    _printSignals ? TextureCache.Instance.SignalOn : TextureCache.Instance.SignalOff,
                    _printSignals ? "Showing signals" : "Hiding signals"),
                StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
            if (_state != _printSignals)
            {
                _filterChanged = true;
                _printSignals = _state;
            }

            #endregion

            if (_filterChanged)
            {
                EditorSettings.ReferencePrintEvents = _printEvents;
                EditorSettings.ReferencePrintMulticastDelegates = _printMulticastDelegates;
                EditorSettings.ReferencePrintStyles = _printStyles;
                EditorSettings.ReferencePrintSkins = _printSkins;
                EditorSettings.ReferencePrintSkinParts = _printSkinParts;
                EditorSettings.ReferencePrintSkinStates = _printSkinStates;
                EditorSettings.ReferencePrintSignals = _printSignals;
                RefreshDescription();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            _scrollPosition2 = EditorGUILayout.BeginScrollView(_scrollPosition2, GUILayout.ExpandHeight(true));

            /* NOTE: It won't update with new text while focused!!!!! */
            EditorGUILayout.TextArea(_componentDescription, StyleCache.Instance.NormalLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            //GUILayout.Label(_componentDescription, StyleCache.Instance.NormalLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            
            EditorGUILayout.EndScrollView();
        }

        private int TypeSort(Type x, Type y)
        {
            return String.CompareOrdinal(x.FullName, y.FullName);
        }
    }
}
