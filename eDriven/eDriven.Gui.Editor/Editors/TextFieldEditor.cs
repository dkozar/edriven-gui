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

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(TextFieldAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class TextFieldEditor : ComponentEditor
    {
        public SerializedProperty Text;
        public SerializedProperty PasswordMode;
        public SerializedProperty PasswordCharMask;
        public SerializedProperty Optimized;

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            Text = serializedObject.FindProperty("Text");
            PasswordMode = serializedObject.FindProperty("PasswordMode");
            PasswordCharMask = serializedObject.FindProperty("PasswordCharMask");
            Optimized = serializedObject.FindProperty("Optimized");
        }

        [Obfuscation(Exclude = true)]
        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Text.stringValue = EditorGUILayout.TextField("Text", Text.stringValue);
            PasswordMode.boolValue = EditorGUILayout.Toggle("PasswordMode", PasswordMode.boolValue);

            var mask = PasswordCharMask.stringValue;
            if (mask.Length == 0)
            {
                mask = "*";
            }
            else if (mask.Length > 1)
            {
                mask = mask.Substring(0, 1);
            }
            PasswordCharMask.stringValue = EditorGUILayout.TextField("PasswordCharMask", mask);
            Optimized.boolValue = EditorGUILayout.Toggle("Optimized", Optimized.boolValue);
        
            EditorGUILayout.Space();
        }
    }
}