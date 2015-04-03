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
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(ButtonAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class ButtonEditor : ComponentEditor
    {
        public SerializedProperty Text;
        public SerializedProperty Icon;
        public SerializedProperty ToggleMode;
        public SerializedProperty Selected;

        //private string _icon;
        //private bool _iconExistsInResources;

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            Text = serializedObject.FindProperty("Text");
            Icon = serializedObject.FindProperty("Icon");
            ToggleMode = serializedObject.FindProperty("ToggleMode");
            Selected = serializedObject.FindProperty("Selected");
        }

        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Text.stringValue = EditorGUILayout.TextField("Text", Text.stringValue);

            //if (_icon != Icon.stringValue)
            //{
            //    _icon = Icon.stringValue;
            //    _iconExistsInResources = string.IsNullOrEmpty(_icon) || null != Resources.Load(_icon);
            //}

            //if (!_iconExistsInResources)
            //{
            //EditorGUILayout.BeginHorizontal();
            //}

            //Icon.stringValue = EditorGUILayout.TextField("Icon", Icon.stringValue);
            Icon.objectReferenceValue = EditorGUILayout.ObjectField("Icon", Icon.objectReferenceValue, typeof(Texture), true);

            //if (!_iconExistsInResources)
            //{
            //GUILayout.Label(TextureCache.Instance.Exclamation, GUILayout.ExpandWidth(false));
            //EditorGUILayout.EndHorizontal();
            //}

            ToggleMode.boolValue = IsHidden("toggle") || EditorGUILayout.Toggle("ToggleMode", ToggleMode.boolValue);

            if (ToggleMode.boolValue)
                Selected.boolValue = EditorGUILayout.Toggle("Selected", Selected.boolValue);

            //EditorGUILayout.Space();
            GUILayout.Space(2);
        }
    }
}