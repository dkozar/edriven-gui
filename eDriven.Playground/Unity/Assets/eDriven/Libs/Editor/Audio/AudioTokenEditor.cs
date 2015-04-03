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
using eDriven.Audio;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioToken))]
[CanEditMultipleObjects]
public class AudioTokenEditor : Editor
{
    //public string Id;
    public SerializedProperty Id;

    //public bool SoundEnabled = true;
    public SerializedProperty SoundEnabled;

    //public AudioClip AudioClip;
    public SerializedProperty AudioClip;
    
    //public float Volume = 1.0f;
    public SerializedProperty Volume;

    //public bool Loop;
    public SerializedProperty Loop;

    //public float Pitch = 1f;
    public SerializedProperty Pitch;

    //public float PitchRandomness;
    public SerializedProperty PitchRandomness;

    //public float MinDistance;
    public SerializedProperty MinDistance;

    //public float MaxDistance = 1f;
    public SerializedProperty MaxDistance;

    //public AudioRolloffMode RolloffMode = AudioRolloffMode.Linear;
    public SerializedProperty RolloffMode;

    private AudioRolloffMode _rolloffMode = AudioRolloffMode.Linear;
    
// ReSharper disable UnusedMember.Local
    void OnEnable () {
// ReSharper restore UnusedMember.Local
        // Setup the SerializedProperties
        Id = serializedObject.FindProperty("Id");
        AudioClip = serializedObject.FindProperty("AudioClip");
        Volume = serializedObject.FindProperty("Volume");
        Loop = serializedObject.FindProperty("Loop");
        Pitch = serializedObject.FindProperty("Pitch");
        PitchRandomness = serializedObject.FindProperty("PitchRandomness");
        MinDistance = serializedObject.FindProperty("MinDistance");
        MaxDistance = serializedObject.FindProperty("MaxDistance");
        RolloffMode = serializedObject.FindProperty("RolloffMode");
    }

    [Obfuscation(Exclude = true)]
    public override void OnInspectorGUI()
    {
        // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
        serializedObject.Update();

        Id.stringValue = EditorGUILayout.TextField("Id", Id.stringValue);

        AudioClip.objectReferenceValue = EditorGUILayout.ObjectField("AudioClip", AudioClip.objectReferenceValue, typeof(AudioClip), true);

        Volume.floatValue = EditorGUILayout.Slider("Volume", Volume.floatValue, 0, 1);
        ProgressBar(Volume.floatValue, "Volume");

        Loop.boolValue = EditorGUILayout.Toggle("Loop", Loop.boolValue);

        Pitch.floatValue = EditorGUILayout.Slider("Pitch", Pitch.floatValue, 0, 1);
        PitchRandomness.floatValue = EditorGUILayout.Slider("PitchRandomness", PitchRandomness.floatValue, 0, 1);
        
        MinDistance.floatValue = EditorGUILayout.Slider("MinDistance", MinDistance.floatValue, 0, 1);
        
        MaxDistance.floatValue = EditorGUILayout.Slider("MaxDistance", MaxDistance.floatValue, 0, 1);
        
        // TODO: BUG: this never gets applied. Try to work it out.
        _rolloffMode = (AudioRolloffMode) EditorGUILayout.EnumPopup("RolloffMode", _rolloffMode);
        RolloffMode.enumValueIndex = (int) _rolloffMode;

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    // Custom GUILayout progress bar.
    static void ProgressBar(float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}