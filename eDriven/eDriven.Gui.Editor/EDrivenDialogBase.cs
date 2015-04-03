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
    internal class EDrivenDialogBase : EditorWindow
    {
        /// <summary>
        /// Initialize the EditorState and its default listeners (processors)
        /// (in constructor ???)
        /// </summary>
        public EDrivenDialogBase()
        {
// ReSharper disable once UnusedVariable
            EditorState.Instance.ThemeChangeSignal.Connect(ThemeChangeSlot);
        }

        private void ThemeChangeSlot(object[] parameters)
        {
            //Debug.Log("Theme changed: " + this);
            Repaint();
        }

        /*void Update()
        {
            // fixes the choppines when dragging things in edit mode
            //if (!Application.isPlaying)
            Repaint();
        }*/
    }
}