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
using eDriven.Gui.Editor.Rendering;
using eDriven.Gui.Layout;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Display.Layout
{
    internal class HorizontalLayoutDisplay : LayoutDisplayBase
    {
        private SerializedProperty _horizontalAlign;
        private SerializedProperty _verticalAlign;
        private SerializedProperty _gap;
        private SerializedProperty _paddingLeft;
        private SerializedProperty _paddingRight;
        private SerializedProperty _paddingTop;
        private SerializedProperty _paddingBottom;
        private SerializedProperty _syncPadding;

        private int _prevPaddingLeft;
        private int _prevPaddingRight;
        private int _prevPaddingTop;
        private int _prevPaddingBottom;
        
        public override void Initialize(SerializedObject serializedObject)
        {
            _horizontalAlign = serializedObject.FindProperty("HorizontalAlign");
            _verticalAlign = serializedObject.FindProperty("VerticalAlign");
            _gap = serializedObject.FindProperty("Gap");
            _paddingLeft = serializedObject.FindProperty("PaddingLeft");
            _paddingRight = serializedObject.FindProperty("PaddingRight");
            _paddingTop = serializedObject.FindProperty("PaddingTop");
            _paddingBottom = serializedObject.FindProperty("PaddingBottom");
            _syncPadding = serializedObject.FindProperty("SyncPadding");
        }

        public override void Render()
        {
            /*EditorGUIUtility.labelWidth = Screen.width / 2;
            EditorGUIUtility.fieldWidth = Screen.width / 2;*/

            _horizontalAlign.enumValueIndex = (int)(HorizontalAlign)EditorGUILayout.EnumPopup(
                                                                        "Horizontal align",
                                                                        (HorizontalAlign)Enum.GetValues(typeof(HorizontalAlign)).GetValue(_horizontalAlign.enumValueIndex),
                                                                        GUILayout.ExpandWidth(false),
                                                                        GUILayout.Width(300)
                                                                    );

            _verticalAlign.enumValueIndex = (int)(VerticalAlign)EditorGUILayout.EnumPopup(
                                                                    "Vertical align",
                                                                    (VerticalAlign)Enum.GetValues(typeof(VerticalAlign)).GetValue(_verticalAlign.enumValueIndex),
                                                                    GUILayout.ExpandWidth(false),
                                                                    GUILayout.Width(300)
                                                                );

            _gap.intValue = EditorGUILayout.IntField("Gap", _gap.intValue, GUILayout.ExpandWidth(false));

            GUILayout.Space(15);
            /*_paddingLeft.intValue = EditorGUILayout.IntField("Padding left", _paddingLeft.intValue);
            _paddingRight.intValue = EditorGUILayout.IntField("Padding right", _paddingRight.intValue);
            _paddingTop.intValue = EditorGUILayout.IntField("Padding top", _paddingTop.intValue);
            _paddingBottom.intValue = EditorGUILayout.IntField("Padding bottom", _paddingBottom.intValue);*/

            _prevPaddingLeft = _paddingLeft.intValue;
            _prevPaddingRight = _paddingRight.intValue;
            _prevPaddingTop = _paddingTop.intValue;
            _prevPaddingBottom = _paddingBottom.intValue;

            EditorSettings.PaddingExpanded = EditorGUILayout.Foldout(EditorSettings.PaddingExpanded, "Padding");
            if (EditorSettings.PaddingExpanded)
            {
                SynchronizedBlock.Render(_syncPadding, _paddingLeft, _paddingRight, _paddingTop, _paddingBottom,
                    ref _prevPaddingLeft, ref _prevPaddingRight, ref _prevPaddingTop, ref _prevPaddingBottom);
            }
        }
    }
}