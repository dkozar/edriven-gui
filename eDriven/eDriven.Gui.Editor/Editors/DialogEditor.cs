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

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(DialogAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class DialogEditor : WindowEditor
    {
        public SerializedProperty Draggable;
        public SerializedProperty Resizable;
        public SerializedProperty CloseOnEsc;
        public SerializedProperty ControlBarGroupChildren;

        /*private const string ButtonGroupSettingsVisibleKey = "eDrivenButtonGroupSettingsVisible";
        public static bool ButtonGroupSettingsVisible
        {
            get { return EditorPrefs.GetBool(ButtonGroupSettingsVisibleKey); }
            set
            {
                if (value == EditorPrefs.GetBool(ButtonGroupSettingsVisibleKey))
                    return;

                EditorPrefs.SetBool(ButtonGroupSettingsVisibleKey, value);
            }
        }*/

        //private readonly PanelRenderer _buttonGroupPanelRenderer = new PanelRenderer();
        //private SpecialGroupRenderer _buttonGroupRenderer;

// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            Draggable = serializedObject.FindProperty("Draggable");
            Resizable = serializedObject.FindProperty("Resizable");
            CloseOnEsc = serializedObject.FindProperty("CloseOnEsc");
            ControlBarGroupChildren = serializedObject.FindProperty("ControlBarGroupChildren");
            if (!ControlBarGroupChildren.isArray)
            {
                throw new Exception("ControlBarGroupChildren is not an array");
            }

            //_buttonGroupRenderer = new SpecialGroupRenderer(target, ControlBarGroupChildren)
            //{
            //    RemoveHandler = ((DialogAdapter)target).ApplyContentGroup, // PanelAdapter actually
            //    AddHandler = ((DialogAdapter)target).ApplyButtonGroup
            //};
            //Debug.Log(string.Format("* Loaded {0} button group references", ControlBarGroupChildren.arraySize));
        }

        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Draggable.boolValue = EditorGUILayout.Toggle("Draggable", Draggable.boolValue);
            Resizable.boolValue = EditorGUILayout.Toggle("Resizable", Resizable.boolValue);
            CloseOnEsc.boolValue = EditorGUILayout.Toggle("CloseOnEsc", CloseOnEsc.boolValue);

            //EditorGUILayout.Space();
        }

//    protected override void RenderExtendedOptions()
//    {
//        base.RenderExtendedOptions();
//
//        ButtonGroupSettingsVisible = _buttonGroupPanelRenderer.RenderStart(GuiContentCache.Instance.DialogButtonGroupTitle, ButtonGroupSettingsVisible);
//
//        if (ButtonGroupSettingsVisible)
//        {
//            _buttonGroupRenderer.Render();
//            _buttonGroupPanelRenderer.RenderEnd();
//        }
//    }
    }
}