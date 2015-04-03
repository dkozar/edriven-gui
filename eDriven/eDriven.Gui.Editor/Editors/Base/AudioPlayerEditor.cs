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
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor
{
    [CustomEditor(typeof(AudioPlayer), true)]
    [CanEditMultipleObjects]
    [Obfuscation(Exclude = true)]
    public class AudioPlayerEditor : UnityEditor.Editor
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private readonly PanelRenderer _mainPanelRenderer = new PanelRenderer();
        
        #region Expanded

        private bool _expanded = true;
        public virtual bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        #endregion

        //public bool SoundEnabled = true;
        public SerializedProperty SoundEnabled;

        //public float Volume = 1.0f;
        public SerializedProperty Volume;

        //public float Pitch = 1f;
        public SerializedProperty Pitch;

        //public float PitchRandomness;
        public SerializedProperty PitchRandomness;

        // ReSharper disable UnusedMember.Local
        [Obfuscation(Exclude = true)]
        void OnEnable()
        {
            // ReSharper restore UnusedMember.Local
            // Setup the SerializedProperties
            SoundEnabled = serializedObject.FindProperty("SoundEnabled");
            Volume = serializedObject.FindProperty("Volume");
            Pitch = serializedObject.FindProperty("Pitch");
            PitchRandomness = serializedObject.FindProperty("PitchRandomness");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //DrawHeader();

            InspectorContentWrapper.Start();

            /*EditorSettings.ComponentDescriptorMainExpanded =*/

            Expanded = _mainPanelRenderer.RenderStart(GuiContentCache.Instance.AudioPlayerEditor, Expanded);

            if (Expanded)
            {
                SoundEnabled.boolValue = EditorGUILayout.Toggle("SoundEnabled", SoundEnabled.boolValue);

                Volume.floatValue = EditorGUILayout.Slider("Volume", Volume.floatValue, 0, 1);
                ProgressBar(Volume.floatValue, "Volume");

                Pitch.floatValue = EditorGUILayout.Slider("Pitch", Pitch.floatValue, 0, 10);

                PitchRandomness.floatValue = EditorGUILayout.Slider("PitchRandomness", PitchRandomness.floatValue, 0, 1);

                /**
                 * The order of applying the propertis MUST be as follows:
                 * 1. Applying modified properties by inspector (serializedObject.ApplyModifiedProperties())
                 * 2. Applying changes made in play mode
                 * This is because serializedObject.ApplyModifiedProperties() re-applies the properties when back from play mode
                 * */
                serializedObject.ApplyModifiedProperties();

                _mainPanelRenderer.RenderEnd();
            }

            InspectorContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
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
}