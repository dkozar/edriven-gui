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
using eDriven.Gui.Editor.Processing;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    /// <summary>
    /// Handles common tasks for each eDriven editor window
    /// For instance, each of the windows should react on OnSelectionChange
    /// This is because we want the selection change to work when any of the eDriven windows is open, not only the main window
    /// The EditorState object should be set appropriatelly and all the interested parties
    /// (including windows) should subscribe to EditorState object (its signals)
    /// </summary>
    internal class EDrivenEditorWindowBase : EditorWindow
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
// ReSharper restore UnassignedField.Global
#endif

        [Obfuscation(Exclude = true)]
// ReSharper disable once UnusedMember.Local
        void OnSelectionChange()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(this + ": OnSelectionChange: " + Selection.activeTransform);
            }
#endif
            EditorState.Instance.SelectedTransform = Selection.activeTransform;
        }

        ///// <summary>
        ///// A hack for avoiding "ghost" changes
        ///// </summary>
        //private static float? _lastTime;

        ///* Important: a flag has to be static!!! */
        //private static bool _hierarchyInvalidated;
        //private void InvalidateHierarchy()
        //{
        //    //Debug.Log(this + ": InvalidateHierarchy");
        //    _lastTime = Time.time;
        //    _hierarchyInvalidated = true;
        //}

        //public const float JitterLevel = 0.2f /*sec*/;
        
        /// <summary>
        /// Note: for this handler to fire, Main window has to be (always) visible!
        /// </summary>
        [Obfuscation(Exclude = true)]
// ReSharper disable once UnusedMember.Local
        void OnHierarchyChange()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log(this + ": OnHierarchyChange");
            }
#endif
            /*if (EditorApplication.isPlaying)
                InvalidateHierarchy();
            else*/
                EditorState.Instance.HierarchyChange();
        }

// ReSharper disable once UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void Update()
        {
            /*if (_hierarchyInvalidated)
            {
                /* A hack for avoiding "ghost" changes #1#
                if (null == _lastTime || (Time.time - _lastTime > JitterLevel))
                {
                    _lastTime = Time.time;
                    _hierarchyInvalidated = false;
                    EditorState.Instance.HierarchyChange();
                }
            }*/

            /*if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log(EditorState.Instance.Adapter);
                if (null != Selection.activeTransform)
                {
                    Destroy(Selection.activeTransform.gameObject);
                }
            }*/

            //_hierarchyChangedInThisFrame = false;
            UpdateHandler();
        }

        protected virtual void UpdateHandler()
        {
            
        }

        #region Constructor

        /// <summary>
        /// Initialize the EditorState and its default listeners (processors)
        /// (in constructor ???)
        /// </summary>
        public EDrivenEditorWindowBase()
        {
// ReSharper disable once UnusedVariable
            var subscriptionInitilizer = Bootstrap.Instance;
        }

        #endregion
    }
}