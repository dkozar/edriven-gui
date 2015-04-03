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
using System.Reflection;
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(PanelAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class PanelEditor : GroupEditor
    {
        public SerializedProperty Title;
        public SerializedProperty Icon;
        public SerializedProperty ToolsGroupChildren;

        //private readonly PanelRenderer _toolsPanelRenderer = new PanelRenderer();
        //private SpecialGroupRenderer _toolsRenderer;

        private const string ToolsSettingsVisibleKey = "eDrivenToolsSettingsVisible";
        public static bool ToolsSettingsVisible
        {
            get { return EditorPrefs.GetBool(ToolsSettingsVisibleKey); }
            set
            {
                if (value == EditorPrefs.GetBool(ToolsSettingsVisibleKey))
                    return;

                EditorPrefs.SetBool(ToolsSettingsVisibleKey, value);
            }
        }

// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            Title = serializedObject.FindProperty("Title");
            Icon = serializedObject.FindProperty("Icon");
            ToolsGroupChildren = serializedObject.FindProperty("ToolGroupChildren");
            if (!ToolsGroupChildren.isArray)
            {
                throw new Exception("ToolGroupChildren is not an array");
            }

            //_toolsRenderer = new SpecialGroupRenderer(target, ToolGroupChildren)
            //                     {
            //                         RemoveHandler = ((PanelAdapter) target).ApplyContentGroup,
            //                         AddHandler = ((PanelAdapter) target).ApplyToolsGroup
            //                     };
            //Debug.Log(string.Format("* Loaded {0} tool group references", ToolGroupChildren.arraySize));
        }

        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Title.stringValue = EditorGUILayout.TextField("Title", Title.stringValue);
            Icon.objectReferenceValue = EditorGUILayout.ObjectField("Icon", Icon.objectReferenceValue, typeof(Texture), true);

            EditorGUILayout.Space();
        }

        //protected override void RenderExtendedOptions()
        //{
        //    base.RenderExtendedOptions();

        //    ToolsSettingsVisible = _toolsPanelRenderer.RenderStart(GuiContentCache.Instance.PanelToolsTitle, ToolsSettingsVisible);
        //    if (ToolsSettingsVisible)
        //    {
        //        _toolsRenderer.Render();
        //        _toolsPanelRenderer.RenderEnd();
        //    }
        //}
    }
}