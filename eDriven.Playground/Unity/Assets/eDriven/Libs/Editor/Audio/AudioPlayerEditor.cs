//#region License

///*
 
//Copyright (c) 2010-2014 Danko Kozar

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
 
//*/

//#endregion License

//using System.Reflection;
//using eDriven.Audio;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(AudioPlayer), true)]
//[CanEditMultipleObjects]
//public class AudioPlayerEditor : Editor
//{
//    //public bool SoundEnabled = true;
//    public SerializedProperty SoundEnabled;

//    //public float Volume = 1.0f;
//    public SerializedProperty Volume;

//    //public float Pitch = 1f;
//    public SerializedProperty Pitch;

//    //public float PitchRandomness;
//    public SerializedProperty PitchRandomness;

//    // ReSharper disable UnusedMember.Local
//    [Obfuscation(Exclude = true)]
//    void OnEnable()
//    {
//        // ReSharper restore UnusedMember.Local
//        // Setup the SerializedProperties
//        SoundEnabled = serializedObject.FindProperty("SoundEnabled");
//        Volume = serializedObject.FindProperty("Volume");
//        Pitch = serializedObject.FindProperty("Pitch");
//        PitchRandomness = serializedObject.FindProperty("PitchRandomness");
//    }

//    [Obfuscation(Exclude = true)]
//    public override void OnInspectorGUI()
//    {
//        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
//        serializedObject.Update();

//        SoundEnabled.boolValue = EditorGUILayout.Toggle("SoundEnabled", SoundEnabled.boolValue);

//        Volume.floatValue = EditorGUILayout.Slider("Volume", Volume.floatValue, 0, 1);
//        ProgressBar(Volume.floatValue, "Volume");

//        Pitch.floatValue = EditorGUILayout.Slider("Pitch", Pitch.floatValue, 0, 10);
        
//        PitchRandomness.floatValue = EditorGUILayout.Slider("PitchRandomness", PitchRandomness.floatValue, 0, 1);
        
//        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
//        serializedObject.ApplyModifiedProperties();
//    }

//    // Custom GUILayout progress bar.
//    static void ProgressBar (float value, string label) {
//        // Get a rect for the progress bar using the same margins as a textfield:
//        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
//        EditorGUI.ProgressBar (rect, value, label);
//        EditorGUILayout.Space ();
//    }
//}