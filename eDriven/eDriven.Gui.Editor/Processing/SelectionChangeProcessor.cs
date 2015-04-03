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

using eDriven.Gui.Designer.Rendering;
using eDriven.Gui.Editor.List;
using eDriven.Gui.Editor.Persistence;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Editor.Processing
{
    internal class SelectionChangeProcessor
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static SelectionChangeProcessor _instance;

        /// <summary>
        /// The flag indicated that by the time of the game object selection, the adapter hasn't yet instantiated the component
        /// if the adapter has just been created
        /// By setting this flag to true we are deffering the actual component selection (with yellow rectangle)
        /// until the delta processing is finished
        /// </summary>
        private bool _doDefferedSelection;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private SelectionChangeProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SelectionChangeProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating SelectionChangeProcessor instance"));
#endif
                    _instance = new SelectionChangeProcessor();
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

        }

        private static Component _selectedComponent;
        /// <summary>
        /// Selected component
        /// </summary>
        public static Component SelectedComponent
        {
            get { return _selectedComponent; }
        }

        public void SelectionChangeSlot(params object[] parameters)
        {
            DraggableList.StopDrag(); // solves the bug of still dragging when view changed
            
            Transform selectedTransform = (Transform) parameters[0];
            bool shouldDeselect = false;

            if (null != selectedTransform)
            {
                /**
                 * When the game object is selected (in the hierarchy window) we need to select the component on screen
                 * The component might not be created by this time, so we need to defer the component selection
                 * until the hierarchy change and delta processing
                 * */
                if (null != EditorState.Instance.Adapter && null != DesignerOverlay.Instance)
                {
                    var selectedComponent = EditorState.Instance.Adapter.Component;
                    if (null != selectedComponent)
                    {
                        //Debug.Log("Immediate selection: " + _selectedComponent);
                        DesignerOverlay.Instance.Select(selectedComponent);
                    }
                    else
                    {
                        shouldDeselect = true;
                        _doDefferedSelection = true;
                    }
                }
                else
                {
                    _selectedComponent = null;
                    shouldDeselect = true;
                }
            }
            else
            {
                shouldDeselect = true;
            }

            if (shouldDeselect)
            {
                if (null != DesignerOverlay.Instance)
                    DesignerOverlay.Instance.Deselect();
            }

            HierarchyViewDecorator.Instance.ReScan();

            PlayModeStateChangeEmitter.Instance.SelectionChangedSignal.Emit();
        }

        /// <summary>
        /// After the new component is created and immediatelly selected (creating with holding the CTRL key)
        /// we have to process it here (this is run after the delta processing, so components are already instantiated)
        /// </summary>
        public void SelectCreatedComponent()
        {
            if (_doDefferedSelection)
            {
                _doDefferedSelection = false;

                if (!EditorSettings.InspectorEnabled)
                    return;

                if (null != EditorState.Instance.Adapter)
                {
                    _selectedComponent = EditorState.Instance.Adapter.Component;
                    //Debug.Log("Deffered selection: " + _selectedComponent);
                    if (null != _selectedComponent && null != DesignerOverlay.Instance)
                        DesignerOverlay.Instance.Select(_selectedComponent);
                }
            }
        }
    }
}
