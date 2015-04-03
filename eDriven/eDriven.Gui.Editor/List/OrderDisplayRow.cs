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

using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.List
{
    internal class OrderDisplayRow : DraggableItem
// This class just has the capability of being dragged in GUI - it could be any type of generic data class
    {
        private readonly string _name;
    
        public Color? Color;
        public bool IsContainer;

        public override object Data { get; set;}

        public OrderDisplayRow(string name, object data, Rect position)
            : base(position)
        {
            _name = name;
            Data = data;

            _adapter = (ComponentAdapter) Data;
        }

        private Color _oldColor;

        private bool _isRemoved;
        private bool _isSelected;

        private readonly ComponentAdapter _adapter;

        public ComponentAdapter Adapter
        {
            get { return _adapter; }
        }

        public override ItemAction Render()
        {
            //Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 100.0f);
            Rect drawRect = Bounds;

            GUILayout.BeginArea(drawRect, StyleCache.Instance.ListRow); //GUI.skin.GetStyle("Box"));
            GUILayout.BeginHorizontal();

            GUILayout.Label(GuiContentCache.Instance.DragHandle, StyleCache.Instance.DragHandle, GUILayout.ExpandWidth(false));

            if (null != Color)
            {
                _oldColor = GUI.color;
                GUI.backgroundColor = (Color) Color;
            }
            GUILayout.Label(_name, IsContainer ? StyleCache.Instance.ContainerHandle : StyleCache.Instance.ComponentHandle, GUILayout.ExpandWidth(false));

            if (null != Color)
                GUI.backgroundColor = _oldColor;

            GUILayout.FlexibleSpace();

            if (EditorSettings.ShowDebugOptionsInInspector)
            {
                if (EditorSettings.ShowUniqueIdInInspector)
                {
                    if (null != _adapter && null != _adapter.transform)
                    { 
                        EditorGUILayout.HelpBox("T:" + _adapter.transform.GetInstanceID(), MessageType.None);
                        EditorGUILayout.HelpBox("A:" + _adapter.GetInstanceID(), MessageType.None);
                    }
                }

                if (EditorSettings.ShowGuidInInspector)
                {
                    //EditorGUILayout.HelpBox(_adapter.Guid, MessageType.None);
                }
            }

            if (GUILayout.Button("Select", StyleCache.Instance.BreadcrumbFirst, GUILayout.ExpandWidth(false)))
            {
                //Debug.Log("Selecting " + Index);
                _isSelected = true;
                //Selection.activeGameObject = ((ComponentAdapter)Data).gameObject;
            }

            if (GUILayout.Button(new GUIContent(string.Empty, TextureCache.Instance.Remove), StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20)))
            {
                if (EditorApplication.isPlaying || Event.current.control)
                {
                    _isRemoved = true;
                    //return new ItemAction(ItemAction.REMOVE, _adapter);
                }

                else if (EditorUtility.DisplayDialog("Remove child?", string.Format(@"Are you sure you want to remove child component:

{0}", Data), "OK", "Cancel"))
                {
                    _isRemoved = true;
                    //return new ItemAction(ItemAction.REMOVE, _adapter);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (_isRemoved)
            {
                _isRemoved = false;
                return new ItemAction(ItemAction.REMOVE, _adapter);
            }
            if (_isSelected)
            {
                _isSelected = false;
                Selection.activeGameObject = ((ComponentAdapter)Data).gameObject;
            }

            Drag(drawRect);

            return null;
        }
    }
}