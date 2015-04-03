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

using eDriven.Core.Events;
using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Rendering;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.List;
using eDriven.Gui.Editor.Rendering;
using UnityEditor;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Editor
{
    internal class GameViewMouseClickProcessor
    {

#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static GameViewMouseClickProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private GameViewMouseClickProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GameViewMouseClickProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating GameViewMouseClickProcessor instance"));
#endif
                    _instance = new GameViewMouseClickProcessor();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            /**
             * Subscribes to game-view component clicks
             * */
            DesignerOverlay.ClickSignal.Connect(ClickSlot);
        }

        /// <summary>
        /// Fires when in-game GUI element clicked
        /// </summary>
        /// <param name="parameters"></param>
        internal static void ClickSlot(object[] parameters)
        {
            if (!EditorSettings.InspectorEnabled)
                return;

            /**
             * 1. No component with adapter selected -> remove the hierarchy selection
             * */
            if (parameters.Length == 0)
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                if (DipSwitches.DeselectHierarchyOnNonAdaptedComponentSelection)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
#pragma warning disable 162
                    Selection.activeTransform = null;
#pragma warning restore 162
                return;
            }

            Component comp = parameters[0] as Component;

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("Component clicked in the game view: {0}", comp));
            }
#endif

            DraggableList.StopDrag(); // solves the bug of still dragging when view changed

            /**
             * 2. The adapted component selected
             * */
            if (null != comp)
            {
                /**
                 * 2.a) Get game object having adapter for this component attached<br/>
                 * If no adapter found, it means that this component is not bound to an adapter<br/>
                 * For instance Alert (created from code), or component created by an adapter working in factory mode
                 * */
                GameObject gameObject = GuiLookup.GetGameObject(comp);

                /**
                 * 2.b) Check if game object found
                 * If not, nullify the selection and return
                 * */
                if (null == gameObject)
                {
                    Selection.activeTransform = null;
                    return;
                }

                /**
                 * 2.c) If game object found, ping it (highlight it) and select it
                 * Selecting it will in turn execute all the mechanisms for processing the selection change
                 * */
                //EditorGUIUtility.PingObject(gameObject); // ping
                Selection.activeTransform = gameObject.transform; // select

                if (parameters.Length > 1)
                {
                    MouseEvent me = (MouseEvent)parameters[1];
                    ProcessMouseEvent(me);
                }
            }
        }

        private static void ProcessMouseEvent(InputEvent e)
        {
            //Debug.Log(string.Format("e: {0}", e));
            //Debug.Log(string.Format("e.CurrentEvent.button: {0}", e.CurrentEvent.button));

            //switch (e.CurrentEvent.button)
            switch (e.Type)
            {
                //case 0: // left button
                case MouseEvent.RIGHT_CLICK: // left button
                    MainTabBar.Instance.TabIndex = MainWindow.ORDER_DISPLAY; // order
                    break;

                //case 0: // left button
                case MouseEvent.RIGHT_DOUBLE_CLICK: // left button
                    MainTabBar.Instance.TabIndex = MainWindow.ORDER_DISPLAY; // order
                    ToolboxDialog.Instance.ShowUtility();
                    break;

                case MouseEvent.MIDDLE_CLICK: // middle button
                    MainTabBar.Instance.TabIndex = MainWindow.LAYOUT_DISPLAY; // layout
                    break;

                case MouseEvent.DOUBLE_CLICK: // double click
                    if (EditorSettings.MouseDoubleClickEnabled)
                    {
                        // show events dialog
                        MainTabBar.Instance.TabIndex = MainWindow.EVENTS_DISPLAY;

                        //Debug.Log("ProcessMouseEvent: " + @e.CurrentEvent.button);
                        var adapter = GuiLookup.GetAdapter(Selection.activeTransform);
                        if (null == adapter)
                        {
                            Debug.LogError("No adapter found");
                            return;
                        }

                        // bring up the Add Event Handler dialog
                        var dialog = AddEventHandlerDialog.Instance;
                        dialog.Reset();
                        dialog.Adapter = adapter;
                        dialog.ShowUtility();
                    }
                    break;
            }
        }
    }
}
