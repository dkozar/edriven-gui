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

using System.Collections.Generic;
using eDriven.Gui.Designer;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Rendering
{
    internal class Breadcrumbs
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// Currently selected hierarchy path
        /// </summary>
        private List<Transform> _path;
        internal void RefreshPath()
        {
            _path = GuiLookup.GetPath(Selection.activeTransform);
        }

        public bool Enabled;

        internal void Render()
        {
            if (null != _path)
            {
                GUILayout.BeginHorizontal();
                int count = 0;
                int total = _path.Count;
                foreach (Transform transform in _path)
                {
                    if (null != transform)
                    {
                        bool selected = count == total - 1;
                        bool newSelected = GUILayout.Toggle(selected, transform.gameObject.name, count == 0 ? StyleCache.Instance.BreadcrumbFirst : StyleCache.Instance.Breadcrumb);
                        if (Enabled && newSelected != selected)
                        {
                            Selection.activeGameObject = transform.gameObject;
                        }
                    }
                    count++;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //GUILayout.Space(2);
            }    
        }
    }
}