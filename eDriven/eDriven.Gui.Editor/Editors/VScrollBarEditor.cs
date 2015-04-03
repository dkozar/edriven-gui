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
    [CustomEditor(typeof(VScrollBarAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class VScrollBarEditor : ComponentEditor
    {
        public SerializedProperty Minimum;
        public SerializedProperty Maximum;
        public SerializedProperty Value;
        public SerializedProperty PageSize;
        /*public SerializedProperty PageScrollSize;
        public SerializedProperty LineScrollSize;*/

        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            Minimum = serializedObject.FindProperty("Minimum");
            Maximum = serializedObject.FindProperty("Maximum");
            Value = serializedObject.FindProperty("Value");
            PageSize = serializedObject.FindProperty("PageSize");
            /*PageScrollSize = serializedObject.FindProperty("PageScrollSize");
            LineScrollSize = serializedObject.FindProperty("LineScrollSize");*/

            Hide("padding"); // hide Padding panel
        }

        /// <summary>
        /// Rendering controls at the end of the main panel
        /// </summary>
        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            //Direction.enumValueIndex = (int)(Direction)EditorGUILayout.EnumPopup(
            //                                                                "Direction",
            //                                                                (Direction)Enum.GetValues(typeof(Direction)).GetValue(Direction.enumValueIndex)
            //                                                            );

            Minimum.floatValue = EditorGUILayout.FloatField("Minimum", Minimum.floatValue);
            Maximum.floatValue = EditorGUILayout.FloatField("Maximum", Maximum.floatValue);
            Value.floatValue = EditorGUILayout.FloatField("Value", Value.floatValue);
            PageSize.floatValue = EditorGUILayout.FloatField("PageSize", PageSize.floatValue);
            /*PageScrollSize.floatValue = EditorGUILayout.FloatField("PageScrollSize", PageScrollSize.floatValue);
            LineScrollSize.floatValue = EditorGUILayout.FloatField("LineScrollSize", LineScrollSize.floatValue);*/
        
            EditorGUILayout.Space();
        }
    }
}