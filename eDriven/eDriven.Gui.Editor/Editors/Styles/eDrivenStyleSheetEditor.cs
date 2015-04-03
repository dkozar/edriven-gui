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

using System.Reflection;
using eDriven.Gui.Editor.Dialogs.Commands;
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Styles;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(eDrivenStyleSheet), true)]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
// ReSharper disable ClassNeverInstantiated.Global
    internal class eDrivenStyleSheetEditor : eDrivenEditorBase
// ReSharper restore ClassNeverInstantiated.Global
    {
// ReSharper disable MemberCanBePrivate.Global
        public SerializedProperty Name;
        public SerializedProperty Locked;
        public SerializedProperty StyleSheet;
// ReSharper restore MemberCanBePrivate.Global

        protected override void Initialize()
        {
            //MainWindow.SerializedObject = serializedObject; // commented out 20131018

            Name = serializedObject.FindProperty("Name");
            Locked = serializedObject.FindProperty("Locked");
            StyleSheet = serializedObject.FindProperty("StyleSheet");
        
            EditorPanelTitle = new GUIContent(" " + Name.stringValue, TextureCache.Instance.StyleSheet);

            /**
             * NOTE:
             * It's too early to initialize the styling overlay connector here!
             * This is the line that was commented:
             * "var connector = StylingOverlayConnector.Instance;"
             * When having this here, this initialized the framework before any other component,
             * and it seems to be wrong, because the framework object was somehow destroyed and re-initialized by other components
             * So: don't call the StylingOverlayConnector instance from here!!! (20131209)
             * */

#pragma warning disable 168
            //StylingOverlayConnector.Instance;
#pragma warning restore 168
        }

        public static bool EditorLocked;

        //private int count;
        /// <summary>
        /// Renders the main content
        /// That is the content within th emain (properties) panel
        /// No RenderStart and RenderEnd calls needed on panel renderer
        /// </summary>
        protected override void RenderMainOptions()
        {
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar, GUILayout.Height(30));

            #region New button

            if (EditorLocked)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GuiContentCache.Instance.StyleDeclarationCreate, StyleCache.Instance.Button,
                GUILayout.ExpandWidth(false), GUILayout.Height(30)))
            {
                //SerializedProperty declarations = GetDeclarations();
                //CreateNewDeclaration(declarations);
                AddStyleDeclarationCommand.Execute(serializedObject);
            }

            GUI.enabled = true;

            #endregion

            #region Lock button

            Locked.boolValue = GUILayout.Toggle(Locked.boolValue, GuiContentCache.Instance.StyleSheetLocked,
                StyleCache.Instance.Button, GUILayout.ExpandWidth(false), GUILayout.Height(30));

            EditorLocked = Locked.boolValue;

            #endregion

            GUILayout.FlexibleSpace();

            #region Expand/collapse

            EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true)); // GUILayout.Height(30));
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(GuiContentCache.Instance.Expand, StyleCache.Instance.ImageOnlyNoFrameButton,
                GUILayout.ExpandWidth(false)/*, GUILayout.Height(30)*/))
                ExpandAll();

            if (GUILayout.Button(GuiContentCache.Instance.Collapse, StyleCache.Instance.ImageOnlyNoFrameButton,
                GUILayout.ExpandWidth(false)/*, GUILayout.Height(30)*/))
                CollapseAll();

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            #endregion


            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.PropertyField(StyleSheet);

            GUILayout.Space(1);

            //if (GUI.changed)
            //{
            //    Debug.Log(string.Format("Stylesheet [{0}] -> Changed", Name.stringValue));
            //}
        }

        /// <summary>
        /// Renders extended options<br/>
        /// You could group options within other panels using panel renderer's RenderStart and RenderEnd calls
        /// </summary>
        protected override void RenderExtendedOptions() { }

        protected override void Apply() { }

        override public bool Expanded
        {
            get { return GetExpanded().boolValue; }
            set
            {
                if (value == GetExpanded().boolValue)
                    return;
                GetExpanded().boolValue = value;
                serializedObject.ApplyModifiedProperties();
            }
        }

        #region Expand / collapse

        private void CollapseAll()
        {
            SerializedProperty declarations = GetDeclarations();
            for (int i = 0; i < declarations.arraySize; i++)
            {
                SerializedProperty decl = declarations.GetArrayElementAtIndex(i);
                if (decl.isExpanded)
                {
                    decl.isExpanded = false;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void ExpandAll()
        {
            SerializedProperty declarations = GetDeclarations();
            for (int i = 0; i < declarations.arraySize; i++)
            {
                SerializedProperty decl = declarations.GetArrayElementAtIndex(i);
                if (!decl.isExpanded)
                {
                    decl.isExpanded = true;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Helper

        private SerializedProperty GetExpanded()
        {
            return serializedObject.FindProperty("Expanded");
        }
        
        private SerializedProperty GetDeclarations()
        {
            return serializedObject.FindProperty("StyleSheet").FindPropertyRelative("Declarations");
        }

        #endregion
    }
}