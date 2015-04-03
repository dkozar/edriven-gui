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
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Display
{
    internal abstract class DisplayBase
    {
        public static Object Target;

        public Rect Bounds;

        //public static int Padding = 16;

        internal ComponentAdapter Adapter;
        internal GroupAdapter GroupAdapter;

        /// <summary>
        /// Since all of the tree displays use the selection change, I decided to implement it in superclass
        /// </summary>
        internal void ProcessSelectionChange()
        {
            //Debug.Log("ProcessSelectionChange: " + Selection.activeObject);
            if (null == Selection.activeGameObject)
                return;

            GameObject go = Selection.activeGameObject;

            /*if (null == go)
                throw new Exception("Couldn't get the selection");*/

            Adapter = go.GetComponent(typeof(ComponentAdapter)) as ComponentAdapter;
            if (null == Adapter)
            {
                /**
                 * Not a GUI component
                 * S hould do cleanup and handle selection (deselect basically)
                 * */
                GroupAdapter = null;
                //HandleSelectionChange();
                return;
            }
            Target = Adapter;
            GroupAdapter = Target as GroupAdapter;
            
            HandleSelectionChange();
        }

        /// <summary>
        /// Since all of the tree displays use the selection change, I decided to implement it in superclass
        /// </summary>
        internal void ProcessHierarchyChange()
        {
            HandleHierarchyChange();
        }

        public abstract void Initialize();

        protected virtual void HandleSelectionChange()
        {
            
        }

        protected virtual void HandleHierarchyChange()
        {
            
        }

        internal abstract void OnLostFocus();

        public abstract void Render();

        public abstract void Update();

        protected bool CheckSelection(bool mustBeContainer, bool renderMessage = true)
        {
            if (null == Selection.activeTransform)
            {
                if (renderMessage)
                    GUILayout.Label(GuiContentCache.Instance.NoSelectionContent, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return false;
            }

            Adapter = GuiLookup.GetAdapter(Selection.activeTransform);
            GroupAdapter = Adapter as GroupAdapter;

            if (null == Adapter)
            {
                if (renderMessage)
                    GUILayout.Label(GuiContentCache.Instance.NotEDrivenComponentContent, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return false;
            }

            if (mustBeContainer && null == GroupAdapter)
            {
                if (renderMessage)
                    GUILayout.Label(GuiContentCache.Instance.NotAContainerContent, StyleCache.Instance.CenteredLabel, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                return false;
            }

            return true;
        }
    }
}