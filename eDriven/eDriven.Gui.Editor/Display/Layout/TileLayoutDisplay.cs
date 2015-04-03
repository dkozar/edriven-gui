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
    internal class TileLayoutDisplay : LayoutDisplayBase
    {
        private SerializedProperty _layoutDirection;
        private SerializedProperty _horizontalAlign;
        private SerializedProperty _verticalAlign;
        private SerializedProperty _horizontalGap;
        private SerializedProperty _verticalGap;
        private SerializedProperty _rowAlign;
        private SerializedProperty _columnAlign;
        private SerializedProperty _useRowHeight;
        private SerializedProperty _useColumnWidth;
        private SerializedProperty _rowHeight;
        private SerializedProperty _columnWidth;
        private SerializedProperty _requestedRowCount;
        private SerializedProperty _requestedColumnCount;

        private readonly InsetRenderer _insetRowHeightRenderer = new InsetRenderer { ExpandWidth = false };
        private readonly InsetRenderer _insetColumnWidthRenderer = new InsetRenderer { ExpandWidth = false };
        
        public override void Initialize(SerializedObject serializedObject)
        {
            _layoutDirection = serializedObject.FindProperty("TileOrientation");
            _horizontalAlign = serializedObject.FindProperty("HorizontalAlign");
            _verticalAlign = serializedObject.FindProperty("VerticalAlign");
            _horizontalGap = serializedObject.FindProperty("HorizontalGap");
            _verticalGap = serializedObject.FindProperty("VerticalGap");
            _rowAlign = serializedObject.FindProperty("RowAlign");
            _columnAlign = serializedObject.FindProperty("ColumnAlign");
            _useRowHeight = serializedObject.FindProperty("UseRowHeight");
            _useColumnWidth = serializedObject.FindProperty("UseColumnWidth");
            _rowHeight = serializedObject.FindProperty("RowHeight");
            _columnWidth = serializedObject.FindProperty("ColumnWidth");
            _requestedRowCount = serializedObject.FindProperty("RequestedRowCount");
            _requestedColumnCount = serializedObject.FindProperty("RequestedColumnCount");
        }

        public override void Render()
        {
            //GUILayout.Label("TileLayoutDisplay");
            //GUILayout.Space(3);

            _layoutDirection.enumValueIndex = (int)(TileOrientation)EditorGUILayout.EnumPopup(
                                                                        "Orientation",
                                                                        (TileOrientation)Enum.GetValues(typeof(TileOrientation)).GetValue(_layoutDirection.enumValueIndex),
                                                                        GUILayout.ExpandWidth(false),
                                                                        GUILayout.Width(300)
                                                                    );

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

            _horizontalGap.intValue = Math.Max(0, EditorGUILayout.IntField("Horizontal gap", _horizontalGap.intValue, GUILayout.ExpandWidth(false)));
            _verticalGap.intValue = Math.Max(0, EditorGUILayout.IntField("Vertical gap", _verticalGap.intValue, GUILayout.ExpandWidth(false)));

            _requestedRowCount.intValue = Math.Max(0, EditorGUILayout.IntField("Requested row count", _requestedRowCount.intValue, GUILayout.ExpandWidth(false)));
            _requestedColumnCount.intValue = Math.Max(0, EditorGUILayout.IntField("Requested column count", _requestedColumnCount.intValue, GUILayout.ExpandWidth(false)));

            _rowAlign.enumValueIndex = (int)(RowAlign)EditorGUILayout.EnumPopup(
                                                                        "Row align",
                                                                        (RowAlign)Enum.GetValues(typeof(RowAlign)).GetValue(_rowAlign.enumValueIndex),
                                                                        GUILayout.ExpandWidth(false),
                                                                        GUILayout.Width(300)
                                                                    );

            _columnAlign.enumValueIndex = (int)(RowAlign)EditorGUILayout.EnumPopup(
                                                                        "Column align",
                                                                        (ColumnAlign)Enum.GetValues(typeof(ColumnAlign)).GetValue(_columnAlign.enumValueIndex),
                                                                        GUILayout.ExpandWidth(false),
                                                                        GUILayout.Width(300)
                                                                    );

            _useRowHeight.boolValue = _insetRowHeightRenderer.RenderStart("Row height", _useRowHeight.boolValue);
            if (_useRowHeight.boolValue)
            {
                _rowHeight.intValue = Math.Max(0, EditorGUILayout.IntField(string.Empty, _rowHeight.intValue, GUILayout.ExpandWidth(false)));
                _insetRowHeightRenderer.RenderEnd();
            }

            _useColumnWidth.boolValue = _insetColumnWidthRenderer.RenderStart("Column width", _useColumnWidth.boolValue);
            if (_useColumnWidth.boolValue)
            {
                _columnWidth.intValue = Math.Max(0, EditorGUILayout.IntField(string.Empty, _columnWidth.intValue, GUILayout.ExpandWidth(false)));
                _insetColumnWidthRenderer.RenderEnd();
            }
        }
    }
}