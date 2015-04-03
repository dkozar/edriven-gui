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
using UnityEditor;

namespace eDriven.Gui.Editor
{
    public abstract class ComponentEditorBase : eDrivenEditorBase
    {
        protected override void Apply()
        {
            if (null != target)
            {
                //Debug.Log("Apply: " + this);
                var t = (IComponentAdapter)target;
                //Debug.Log("t: " + t);
                //Debug.Log("t.Component: " + t.Component);
                if (null != t.Component)
                    t.Apply(t.Component);
            }

            Undirty();
        }

        protected virtual bool IsValid()
        {
            return true;
        }
        
        /// <summary>
        /// Expanded key
        /// The key used to save this panel's state with editor settings
        /// </summary>
        public string EditorPanelKey = "eDrivenComponentDescriptorMainExpanded";

        override public bool Expanded
        {
            get { return EditorPrefs.GetBool(EditorPanelKey); }
            set
            {
                if (value == EditorPrefs.GetBool(EditorPanelKey))
                    return;

                EditorPrefs.SetBool(EditorPanelKey, value);
            }
        }
    }
}