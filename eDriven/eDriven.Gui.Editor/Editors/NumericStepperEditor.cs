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
using eDriven.Gui.Components;
using eDriven.Gui.Designer.Adapters;
using UnityEditor;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(NumericStepperAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class NumericStepperEditor : ComponentEditor
    {
        public SerializedProperty Value;
        public SerializedProperty Minimum;
        public SerializedProperty Maximum;
        public SerializedProperty Mode;
        public SerializedProperty Step;
    
        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            //Hide(PADDING);

            Value = serializedObject.FindProperty("Value");
            Minimum = serializedObject.FindProperty("Minimum");
            Maximum = serializedObject.FindProperty("Maximum");
            Mode = serializedObject.FindProperty("Mode");
            Step = serializedObject.FindProperty("Step");
        }

        [Obfuscation(Exclude = true)]
        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Value.floatValue = EditorGUILayout.FloatField("Value", Value.floatValue);
            Minimum.floatValue = EditorGUILayout.FloatField("Minimum", Minimum.floatValue);
            Maximum.floatValue = EditorGUILayout.FloatField("Maximum", Maximum.floatValue);
            Step.floatValue = EditorGUILayout.FloatField("Step", Step.floatValue);
            //Mode.enumValueIndex = 0;
            Mode.enumValueIndex = (int)(NumericStepperMode)EditorGUILayout.EnumPopup("Mode",
                (NumericStepperMode)Enum.GetValues(typeof(NumericStepperMode)).GetValue(Mode.enumValueIndex)
                );

            EditorGUILayout.Space();
        }
    }
}