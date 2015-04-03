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
using eDriven.Core.Events;
using eDriven.Gui.Designer;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;
using Event=UnityEngine.Event;

namespace eDriven.Gui.Editor.List
{
    internal class EventDisplayRow : DraggableItem
// This class just has the capability of being dragged in GUI - it could be any type of generic data class
    {
        public Color? Color;
        public bool IsContainer;
        
        private bool _capturePhase;
        private bool _targetPhase;
        private bool _bubblingPhase;

        public override object Data { get; set;}

        public EventDisplayRow(object data, Rect position)
            : base(position)
        {
            Data = data;
            _mapping = (EventMapping) Data;

            _capturePhase = (_mapping.Phase & EventPhase.Capture) > 0;
            _targetPhase = (_mapping.Phase & EventPhase.Target) > 0;
            _bubblingPhase = (_mapping.Phase & EventPhase.Bubbling) > 0;
        }

        private Color _oldColor;

        private readonly EventMapping _mapping;
        //public EventMapping Mapping
        //{
        //    get { return _mapping; }
        //}

        private bool _opening;
        public bool Opening
        {
            get { return _opening; }
            set { _opening = value; }
        }

        private static float _openingScriptStartTime;
        private static bool _oldEnabled;

        private bool _phase;
        private bool _phasesChanged;

        public override ItemAction Render()
        {
            //Rect drawRect = new Rect(Position.x, Position.y, 100.0f, 100.0f);
            Rect drawRect = Bounds;

            GUILayout.BeginArea(drawRect, StyleCache.Instance.ListRow); //GUI.skin.GetStyle("Box"));
            GUILayout.BeginHorizontal();

            GUILayout.Label(GuiContentCache.Instance.DragHandle, StyleCache.Instance.DragHandle, GUILayout.ExpandWidth(false));
            
            if (null != Color)
            {
                _oldColor = GUI.color;
                GUI.backgroundColor = (Color) Color;
            }
            //GUILayout.Label(_name, IsContainer ? StyleCache.Instance.ContainerHandleStyle : StyleCache.Instance.ComponentHandleStyle, GUILayout.ExpandWidth(false));

            if (null != Color)
                GUI.backgroundColor = _oldColor;

            if (_opening)
            {
                ProgressBar(Mathf.PingPong((Time.realtimeSinceStartup - _openingScriptStartTime), 1), "Opening in editor...");
            }
            else
            {
                GUILayout.Label(_mapping.EventType, StyleCache.Instance.DragHandle, GUILayout.MaxWidth(120)); //(position.width - 60)/2));

                // enable/disable
                bool enabled = GUILayout.Toggle(
                    _mapping.Enabled,
                    new GUIContent(_mapping.Enabled ? TextureCache.Instance.EventFlowEnabled : TextureCache.Instance.EventFlowDisabled),
                    StyleCache.Instance.ImageOnlyButtonWide
                    );
                
                if (enabled != _mapping.Enabled) {
                    _mapping.Enabled = enabled;
                    return new ItemAction(ItemAction.ENABLE, _mapping);
                }

                if (EditorSettings.ShowEventPhases)
                {
                    _phasesChanged = false;

                    _phase = GUILayout.Toggle(_capturePhase,
                        new GUIContent(string.Empty, _capturePhase ? TextureCache.Instance.EventPhaseCaptureOn : TextureCache.Instance.EventPhaseCapture, _capturePhase ? "Capture phase is ON" : "Capture phase is OFF"),
                                              StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
                    if (_phase != _capturePhase)
                    {
                        _phasesChanged = true;
                        _capturePhase = _phase;
                    }

                    _phase = GUILayout.Toggle(_targetPhase,
                                              new GUIContent(string.Empty, _targetPhase ? TextureCache.Instance.EventPhaseTargetOn : TextureCache.Instance.EventPhaseTarget, _targetPhase ? "Target phase is ON" : "Target phase is OFF"),
                                              StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
                    if (_phase != _targetPhase)
                    {
                        _phasesChanged = true;
                        _targetPhase = _phase;
                    }

                    _phase = GUILayout.Toggle(_bubblingPhase,
                                              new GUIContent(string.Empty, _bubblingPhase ? TextureCache.Instance.EventPhaseBubblingOn : TextureCache.Instance.EventPhaseBubbling, _bubblingPhase ? "Bubbling phase is ON" : "Bubbling phase is OFF"),
                                              StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20));
                    if (_phase != _bubblingPhase)
                    {
                        _phasesChanged = true;
                        _bubblingPhase = _phase;
                    }

                    if (_phasesChanged)
                    {
                        _mapping.Phase = new EventPhase();
                        if (_capturePhase)
                            _mapping.Phase |= EventPhase.Capture;
                        if (_targetPhase)
                            _mapping.Phase |= EventPhase.Target;
                        if (_bubblingPhase)
                            _mapping.Phase |= EventPhase.Bubbling;
                        
                        return new ItemAction(ItemAction.PHASES_CHANGED, _mapping);
                    }
                }

                GUILayout.Label(string.Format("{0}.{1}", _mapping.ScriptName, _mapping.MethodName), StyleCache.Instance.DragHandle, GUILayout.ExpandWidth(true));
            }

            GUILayout.FlexibleSpace();

            // open script button
            _oldEnabled = GUI.enabled;
            if (_opening)
                GUI.enabled = false;

            if (GUILayout.Button(new GUIContent(string.Empty, TextureCache.Instance.EventHandlerScript), StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20)))
            {
                MonoBehaviour component = Selection.activeGameObject.GetComponent(_mapping.ScriptName) as MonoBehaviour;

                if (null == component)
                {
                    string msg = string.Format(@"Cannot open the script ""{0}"".

Note: The script has probably been removed from the project, but without previously removing the event handler.", _mapping.ScriptName);
                    EditorUtility.DisplayDialog("Error", msg, "OK");
                    return null;
                }

                _opening = true;
                _openingScriptStartTime = Time.realtimeSinceStartup;
                
                var script = MonoScript.FromMonoBehaviour(component);
                AssetDatabase.OpenAsset(script);
            }
            if (_opening)
                GUI.enabled = _oldEnabled;

            if (GUILayout.Button(new GUIContent(string.Empty, TextureCache.Instance.Remove), StyleCache.Instance.ImageOnlyButton, GUILayout.Width(20)))
            {
                if (EditorApplication.isPlaying || Event.current.control)
                {
                    return new ItemAction(ItemAction.REMOVE, _mapping);
                }
                
                if (EditorUtility.DisplayDialog("Remove event handler?", string.Format(@"Are you sure you want to remove event handler:

{0}", Data), "OK", "Cancel"))
                {
                    return new ItemAction(ItemAction.REMOVE, _mapping);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            Drag(drawRect);

            return null;
        }

        // Custom GUILayout progress bar.
        static void ProgressBar(float value, string label)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(300, 20, "TextField", GUILayout.ExpandWidth(true));
            rect.y -= 1;
            EditorGUI.ProgressBar(rect, value, label);
            //EditorGUILayout.Space();
        }
    }
}