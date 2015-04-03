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

using System.Collections.Generic;
using eDriven.Core.Events;
using eDriven.Gui.Designer;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.List;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;
using UnityEditor;

namespace eDriven.Gui.Editor.Display
{
    /// <summary>
    /// The window displaying event handler mapping for the selected component
    /// </summary>
    internal class EventDisplay : DisplayBase
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EventDisplay _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EventDisplay()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventDisplay Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EventDisplay instance"));
#endif
                    _instance = new EventDisplay();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        public override void Initialize()
        {
            _list = new DraggableList { ScrollbarWidth = 15, ElementHeight = 31 };
            _list.RemovedSignal.Connect(RemovedSlot);
            _list.EnabledSignal.Connect(EnabledSlot);
            _list.PhaseChangedSignal.Connect(PhaseChangedSlot);
            _list.PositionChangedSignal.Connect(PositionChangedSlot);

            _data.AddGroup(new ChildGroup());
        }

        #region Static

        public static EventPhase DefaultEventPhase = EventPhase.Target | EventPhase.Bubbling;

        #endregion

        #region Members

        private static DraggableList _list;
        private readonly ChildGroupRenderingList _data = new ChildGroupRenderingList();

        private Rect _lastRect;
        private Rect _displayRect;

        private List<EventMapping> _eventMappingList = new List<EventMapping>();

        /// <summary>
        /// Set by the position changed slot
        /// Checked from the Update
        /// </summary>
        private object[] _positionChangedParms;

        /// <summary>
        /// Set by the removed slot
        /// Checked from the Update
        /// </summary>
        private object[] _removedParms;
        
        #endregion

        #region Native Unity handlers

// ReSharper disable UnusedMember.Local
        void OnEnable()
// ReSharper restore UnusedMember.Local
        {
            Initialize();
        }

        private void RemovedSlot(object[] parameters)
        {
            _removedParms = parameters;
        }

        private void EnabledSlot(object[] parameters)
        {
            DraggableItem item = (DraggableItem) parameters[0];
            EventMapping mapping = (EventMapping)item.Data;
            //Debug.Log(string.Format("EnabledSlot: {0}", mapping.Enabled));
            EventMappingDescriptor.ProcessListener(Adapter, null, mapping, mapping.Enabled);
        }

        private void PhaseChangedSlot(object[] parameters)
        {
            DraggableItem item = (DraggableItem)parameters[0];
            EventMapping mapping = (EventMapping)item.Data;
            //Debug.Log(string.Format("PhaseChangedSlot: {0}", mapping));
            EventMappingDescriptor.ProcessPhases(Adapter, null, mapping, mapping.Enabled);
        }

        private void PositionChangedSlot(object[] parameters)
        {
            _positionChangedParms = parameters;
        }

        Rect _rect;

        public bool ShowEventPhases
        {
            get
            {
                return EditorSettings.ShowEventPhases;
            }
            set
            {
                if (value == EditorSettings.ShowEventPhases)
                    return;

                EditorSettings.ShowEventPhases = value;
                // refresh grid
            }
        }

        public bool ShowAddHandlerPanel
        {
            get
            {
                return EditorSettings.ShowAddHandlerPanel;
            }
            set
            {
                if (value == EditorSettings.ShowAddHandlerPanel)
                    return;

                EditorSettings.ShowAddHandlerPanel = value;
                // refresh grid
            }
        }

        public override void Render()
        {
            EditorGUILayout.BeginHorizontal(StyleCache.Instance.Toolbar);

            if (null != Adapter && null != Selection.activeGameObject)
            {
                Adapter.EventMap.Enabled = GUILayout.Toggle(
                    Adapter.EventMap.Enabled,
                    new GUIContent(string.Empty, Adapter.EventMap.Enabled ? TextureCache.Instance.EventsEnabled : TextureCache.Instance.EventsDisabled),
                    StyleCache.Instance.BigButton, GUILayout.ExpandWidth(false), GUILayout.Height(30)
                );

                ShowEventPhases = GUILayout.Toggle(
                    ShowEventPhases, 
                    new GUIContent(string.Empty, ShowEventPhases ? TextureCache.Instance.EventPhasesOn: TextureCache.Instance.EventPhases),
                    StyleCache.Instance.BigButton, GUILayout.ExpandWidth(false), GUILayout.Height(30)
                );
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Toggle(
                    true,
                    new GUIContent(string.Empty, TextureCache.Instance.EventsEnabled),
                    StyleCache.Instance.BigButton, GUILayout.ExpandWidth(false), GUILayout.Height(30)
                );
                GUILayout.Toggle(
                    true,
                    new GUIContent(string.Empty, TextureCache.Instance.EventPhasesOn),
                    StyleCache.Instance.BigButton, GUILayout.ExpandWidth(false), GUILayout.Height(30)
                );
                GUI.enabled = true;
            }

            EditorSettings.MouseDoubleClickEnabled = GUILayout.Toggle(
                EditorSettings.MouseDoubleClickEnabled,
                new GUIContent(string.Empty, EditorSettings.MouseDoubleClickEnabled ? TextureCache.Instance.EventDoubleClickSelected : TextureCache.Instance.EventDoubleClick),
                StyleCache.Instance.BigButton, GUILayout.ExpandWidth(false), GUILayout.Height(30)
            );

            GUILayout.FlexibleSpace();

            if (null == Adapter)
                GUI.enabled = false;

            if (GUILayout.Button(GuiContentCache.Instance.AddEventHandlerButton, StyleCache.Instance.BigButton, GUILayout.Height(30)))
            {
                var dialog = AddEventHandlerDialog.Instance;
                dialog.Reset();
                dialog.Adapter = Adapter;
                dialog.ShowUtility();
            }

            if (GUILayout.Button(new GUIContent("Remove all handlers", TextureCache.Instance.RemoveAllHandlers), StyleCache.Instance.Button, GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Remove all?", "Are you sure you want to remove all event handlers?", "OK", "Cancel"))
                {
                    if (null != Adapter)
                    {
                        _eventMappingList.Clear();
                        DisplayData();
                        Adapter.EventMap.Reset();
                    }
                }
            }

            if (null == Adapter)
                GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            if (!CheckSelection(false))
                return;
        
            _rect = GUILayoutUtility.GetLastRect();
        
            if (_rect.width != 1 && _rect != _lastRect)
            {
                _lastRect = _rect;
                //_displayRect = new Rect(Padding, _lastRect.yMax + Padding, Bounds.width - Padding*2, Bounds.height - _lastRect.yMax);
                _displayRect = new Rect(_lastRect.x + 1, _lastRect.yMax + 1, _lastRect.width - 2, Bounds.height - _lastRect.yMax);
                _list.UpdateLayout(_displayRect.width, _displayRect.height);
            }
        
            GUILayout.BeginArea(_displayRect);
            _list.Render();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Checks the flags and executes the change received via the slots
        /// (the change that is not healthy to do inside the Render method)
        /// </summary>
        public override void Update()
        {
            if (null != _positionChangedParms)
            {
                DraggableItem item = (DraggableItem)_positionChangedParms[0];
                int newIndex = (int)_positionChangedParms[1];
                EventMapping mapping = (EventMapping)item.Data;
                
                _positionChangedParms = null;

                if (null != mapping)
                {
                    Adapter.EventMap.Reorder(mapping, newIndex);

                    _eventMappingList = new List<EventMapping>(Adapter.EventMap.Items);

                    _list.Data = _data;
                    DisplayData();
                }
            }

            if (null != _removedParms)
            {
                DraggableItem draggableItem = (DraggableItem)_removedParms[0];

                _removedParms = null;

                _data.RemoveItem(draggableItem);
                EventMapping mapping = (EventMapping)draggableItem.Data;
                if (null != mapping)
                {
                    //Debug.Log("Removing...");
                    _eventMappingList.Remove(mapping);

                    Adapter.EventMap.Remove(mapping);
                    _list.Data = _data;

                    DisplayData();
                }
            }
        }

// ReSharper disable UnusedMember.Local
        internal override void OnLostFocus()
// ReSharper restore UnusedMember.Local
        {
            foreach (DraggableItem item in _list.Items)
            {
                ((EventDisplayRow)item).Opening = false;
            }
        }

        #endregion

        #region Actions

        protected override void HandleSelectionChange()
        {
            if (null == Adapter)
                return;

            Refresh();
        }

        public void Refresh()
        {
            _eventMappingList = new List<EventMapping>(Adapter.EventMap.Items);
            DisplayData();
        }

        /// <summary>
        /// Changes the display of child descriptors
        /// </summary>
        private void DisplayData()
        {
            _data.Clear(false);

            var group = _data.Groups[0];

            foreach (EventMapping mapping in _eventMappingList)
            {
                var dataObject = new EventDisplayRow(mapping, new Rect(0, 0, 0, 0));
                group.Add(dataObject); // position is the Rect of EditorWindow!
            }

            _list.Data = _data;
        }

        #endregion

    }
}