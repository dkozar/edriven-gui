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
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Rendering
{
    internal class SpecialGroupRenderer
    {
        [NonSerialized]
        private string _errorMessage;
        [NonSerialized]
        private int _removeAtIndex = -1;
        [NonSerialized]
        private int _insertAtIndex = -1;
        [NonSerialized]
        private bool _doRemove;

        private readonly Object _target;

        private readonly SerializedProperty _groupChildren;

        public delegate void ApplyAdapter(ComponentAdapter adapter);
        public delegate void ApplyAdapterList(System.Collections.Generic.List<ComponentAdapter> adapters);

        public ApplyAdapterList AddHandler;
        public ApplyAdapter RemoveHandler;

        public SpecialGroupRenderer(Object target, SerializedProperty toolsGroupChildren)
        {
            _target = target;
            _groupChildren = toolsGroupChildren;
        }

        Color _oldColor;
        
        public void Render()
        {
            bool changed = false;

            if (_groupChildren.arraySize == 0)
            {
                EditorGUILayout.HelpBox(@"Add slots and insert controls using drag & drop.
Note: controls must be direct children of this component.", MessageType.Info, true);
            }

            if (!string.IsNullOrEmpty(_errorMessage))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox(_errorMessage, MessageType.Error, true);
                if (GUILayout.Button(new GUIContent("Dismiss", TextureCache.Instance.Cancel), StyleCache.Instance.Button, GUILayout.Height(38)))
                {
                    _errorMessage = null;
                }
                GUILayout.EndHorizontal();
            }

            int count = _groupChildren.arraySize;

            for (int i = 0; i < count; i++)
            {
                #region 1 row

                ComponentAdapter oldAdapter = (ComponentAdapter)_groupChildren.GetArrayElementAtIndex(i).objectReferenceValue;

                EditorGUILayout.BeginHorizontal();

                _oldColor = GUI.color;
                if (null == oldAdapter)
                {
                    GUI.color = Color.green;
                }

                ComponentAdapter newAdapter = (ComponentAdapter)EditorGUILayout.ObjectField(string.Empty, oldAdapter, typeof(ComponentAdapter), true,
                                                                                            GUILayout.ExpandWidth(true));

                GUI.color = _oldColor;

                #endregion

                #region Button add

                if (GUILayout.Button(GuiContentCache.Instance.AddItem, StyleCache.Instance.ImageOnlyButton, GUILayout.ExpandWidth(false)))//, StyleCache.Instance.ControlButtonStyle))
                {
                    _insertAtIndex = i;
                    changed = true;
                }

                #endregion

                #region Button remove

                if (GUILayout.Button(GuiContentCache.Instance.RemoveItem, StyleCache.Instance.ImageOnlyButton, GUILayout.ExpandWidth(false)))
                {
                    _removeAtIndex = i;
                    _doRemove = true;
                    changed = true;
                }

                #endregion

                if (oldAdapter != newAdapter)
                {
                    if (null != newAdapter)
                    {
                        // only direct children of dialog component
                        if (newAdapter.transform.parent == ((ComponentAdapter)_target).transform)
                        {
                            bool contains = false;
                            var enumerator = _groupChildren.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                ComponentAdapter adapter = (ComponentAdapter)((SerializedProperty)enumerator.Current).objectReferenceValue;
                                if (adapter == newAdapter)
                                {
                                    contains = true;
                                    break;
                                }
                            }

                            if (contains)
                            {
                                _errorMessage = "Error: button already assigned at another position";
                                EditorApplication.Beep();
                                Debug.LogWarning(_errorMessage);
                            }
                            else
                            {
                                _groupChildren.GetArrayElementAtIndex(i).objectReferenceValue = newAdapter;
                            }
                        }
                        else
                        {
                            _errorMessage = "Error: only the direct children could be assigned as buttons";
                            EditorApplication.Beep();
                            Debug.LogWarning(_errorMessage);
                        }
                    }
                    changed = true;
                }

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);
            }

            #region Add field

            _oldColor = GUI.color;
            GUI.color = Color.green;
            
            /* ADD to the end */
            ComponentAdapter added = (ComponentAdapter)EditorGUILayout.ObjectField(string.Empty, null, typeof(ComponentAdapter), true, 
                                                                                   GUILayout.ExpandWidth(true));
            
            if (null != added)
            {
                _groupChildren.arraySize++; // YEAH!
                _groupChildren.GetArrayElementAtIndex(Math.Max(_groupChildren.arraySize - 1, 0)).objectReferenceValue = added;
                changed = true;
            }
            GUI.color = _oldColor;

            #endregion


            //if (GUILayout.Button(new GUIContent("Add tools element", TextureCache.Instance.Add), StyleCache.Instance.Button, GUILayout.Height(30)))//, StyleCache.Instance.ControlButtonStyle))
            //{
            //    _groupChildren.arraySize++; // YEAH!
            //    _groupChildren.GetArrayElementAtIndex(Math.Max(_groupChildren.arraySize - 1, 0)).objectReferenceValue = null;
            //}

            if (_doRemove)
            {
                _doRemove = false;
                if (EditorApplication.isPlaying)
                {
                    DoRemove();
                }
                else if (EditorUtility.DisplayDialog("Remove element?", string.Format(@"Are you sure you want to remove tool group element?"), "OK", "Cancel"))
                {
                    DoRemove();
                }
            }

            if (_insertAtIndex > -1)
            {
                _groupChildren.InsertArrayElementAtIndex(_insertAtIndex);
                _groupChildren.GetArrayElementAtIndex(_insertAtIndex).objectReferenceValue = null;
                _insertAtIndex = -1;
            }

            if (changed)
            {
                //Debug.Log("ToolsGroup group changed!");
                System.Collections.Generic.List<ComponentAdapter> adapters = new System.Collections.Generic.List<ComponentAdapter>();
                count = _groupChildren.arraySize;
                for (int i = 0; i < count; i++)
                {
                    ComponentAdapter adapter = (ComponentAdapter) _groupChildren.GetArrayElementAtIndex(i).objectReferenceValue;

                    if (null != adapter)
                        adapters.Add(adapter);
                }
                AddHandler(adapters);
            }
        }

        private void DoRemove()
        {
            //Debug.Log("DoRemove");
            var child = _groupChildren.GetArrayElementAtIndex(_removeAtIndex).objectReferenceValue;
            _groupChildren.GetArrayElementAtIndex(_removeAtIndex).objectReferenceValue = null;
            _groupChildren.DeleteArrayElementAtIndex(_removeAtIndex);
            _removeAtIndex = -1;

            RemoveHandler((ComponentAdapter)child);
        }
    }
}