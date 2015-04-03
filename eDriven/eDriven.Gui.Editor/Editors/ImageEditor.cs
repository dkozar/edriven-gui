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
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(ImageAdapter))]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class ImageEditor : ComponentEditor
    {
        public SerializedProperty Texture;
        public SerializedProperty ScaleMode;
        public SerializedProperty AspectRatio;
        public SerializedProperty AlphaBlend;
        public SerializedProperty Mode;
        public SerializedProperty Scale9Left;
        public SerializedProperty Scale9Right;
        public SerializedProperty Scale9Top;
        public SerializedProperty Scale9Bottom;
        public SerializedProperty TilingAnchor;
    
        [Obfuscation(Exclude = true)]
// ReSharper disable UnusedMember.Local
        protected override void Initialize()
// ReSharper restore UnusedMember.Local
        {
            base.Initialize();

            //Hide(PADDING);

            Texture = serializedObject.FindProperty("Texture");
            ScaleMode = serializedObject.FindProperty("ScaleMode");
            AspectRatio = serializedObject.FindProperty("AspectRatio");
            AlphaBlend = serializedObject.FindProperty("AlphaBlend");
            //Tiling = serializedObject.FindProperty("Tiling");
            Mode = serializedObject.FindProperty("Mode");
            Scale9Left = serializedObject.FindProperty("Scale9Left");
            Scale9Right = serializedObject.FindProperty("Scale9Right");
            Scale9Top = serializedObject.FindProperty("Scale9Top");
            Scale9Bottom = serializedObject.FindProperty("Scale9Bottom");
            TilingAnchor = serializedObject.FindProperty("TilingAnchor");
        }

        [Obfuscation(Exclude = true)]
        protected override void RenderMainOptions()
        {
            base.RenderMainOptions();

            Texture.objectReferenceValue = EditorGUILayout.ObjectField("Texture", Texture.objectReferenceValue, typeof(Texture), true);

            var mode = (ImageMode)Enum.GetValues(typeof(ImageMode)).GetValue(Mode.enumValueIndex);
            Mode.enumValueIndex = (int)(ImageMode)EditorGUILayout.EnumPopup(
                "Mode",
                mode
            );

            if (ImageMode.Normal == mode)
            {
                ScaleMode.enumValueIndex = (int)(ScaleMode)EditorGUILayout.EnumPopup(
                    "Scale mode",
                    (ImageScaleMode)Enum.GetValues(typeof(ImageScaleMode)).GetValue(ScaleMode.enumValueIndex)
                );

                //GUILayout.BeginHorizontal();
                AspectRatio.floatValue = EditorGUILayout.FloatField("Aspect ratio", AspectRatio.floatValue);
                //if (GUILayout.Button("Reset")) // TODO
                //{
                //    AspectRatio.floatValue = 
                //}
                //GUILayout.EndHorizontal();
            }
            else if (ImageMode.Scale9 == mode)
            {
                Scale9Left.intValue = EditorGUILayout.IntField("Scale 9 left", Scale9Left.intValue);
                Scale9Right.intValue = EditorGUILayout.IntField("Scale 9 right", Scale9Right.intValue);
                Scale9Top.intValue = EditorGUILayout.IntField("Scale 9 top", Scale9Top.intValue);
                Scale9Bottom.intValue = EditorGUILayout.IntField("Scale 9 bottom", Scale9Bottom.intValue);
            }
            else if (ImageMode.Patch9 == mode)
            {
                // no settings :)
            }
            else
            {
                TilingAnchor.enumValueIndex = (int)(Anchor)EditorGUILayout.EnumPopup(
                    "Tiling anchor",
                    (Anchor)Enum.GetValues(typeof(Anchor)).GetValue(TilingAnchor.enumValueIndex)
                );
            }

            if (ImageMode.Normal == mode || ImageMode.Tiled == mode) { 
                AlphaBlend.boolValue = EditorGUILayout.Toggle("AlphaBlend", AlphaBlend.boolValue);
            }

            EditorGUILayout.Space();
        }
    }
}