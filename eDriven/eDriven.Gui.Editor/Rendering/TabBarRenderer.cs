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
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    public class TabBarRenderer
    {
        private List<TabDescriptor> _tabs = new List<TabDescriptor>();
        public List<TabDescriptor> Tabs
        {
            get
            {
                return _tabs;
            }
            set
            {
                _tabs = value;
            }
        }

        private int _tabIndex;
        public virtual int TabIndex
        {
            get { return _tabIndex; }
            set
            {
                if (value >= _tabs.Count)
                    throw new Exception("Selected tab index out of bounds");
                
                _tabIndex = value;
            }
            
        }

        private bool _clicked;

        public GUIStyle TabStyle;

        public static GUIStyle DefaultTabStyle;
        public static void ResetStyles()
        {
            DefaultTabStyle = null;
        }

        public float ButtonHeight = 30;
        public float Spacing;
        
        private bool _changed;
        public bool Changed
        {
            get { return _changed; }
        }

        private GUIStyle _styleToRender;

        public void Render()
        {
            _changed = false;

            if (null == DefaultTabStyle)
                DefaultTabStyle = StyleCache.Instance.TabButton; // default style

            _styleToRender = TabStyle ?? DefaultTabStyle;

            EditorGUILayout.BeginHorizontal();

            for (int index = 0; index < _tabs.Count; index++)
            {
                _clicked = GUILayout.Toggle(
                    _tabIndex == index, 
                    _tabIndex == index ? _tabs[index].Selected : _tabs[index].Normal,
                    _styleToRender,
                    GUILayout.Height(ButtonHeight), 
                    GUILayout.ExpandWidth(false)
                );

                GUILayout.Space(Spacing); // note: this renders one additional gap

                if (_clicked && _tabIndex != index) {
                    _tabIndex = index;
                    _changed = true;
                    OnChange(_tabIndex);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        protected virtual void OnChange(int index)
        {
            
        }
    }

    public class TabDescriptor
    {
        public TabDescriptor()
        {
        }

        public TabDescriptor(GUIContent normal, GUIContent selected)
        {
            Normal = normal;
            Selected = selected;
        }

        public GUIContent Normal;
        public GUIContent Selected;
    }
}