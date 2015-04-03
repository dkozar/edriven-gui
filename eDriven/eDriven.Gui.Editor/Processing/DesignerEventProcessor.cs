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

using eDriven.Gui.Designer;
using eDriven.Gui.Designer.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.Processing
{
    /// <summary>
    /// Takes care of showing/hiding the designer overlay
    /// </summary>
    internal class DesignerEventProcessor
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static DesignerEventProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DesignerEventProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DesignerEventProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DesignerEventProcessor instance"));
#endif
                    _instance = new DesignerEventProcessor();
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
            DesignerState.Instance.ComponentRemovedSignal.Connect(ComponentRemovedSlot);
        }

        /// <summary>
        /// We are subscribing to designer directly, to have the immediate info on component being removed
        /// This way we can react werly and remove the selection rectangle
        /// </summary>
        /// <param name="parameters"></param>
        private void ComponentRemovedSlot(object[] parameters)
        {
            //Debug.Log("Deselecting via designer");
            if (null != DesignerOverlay.Instance)
            {
                DesignerOverlay.Instance.Unhover();
                DesignerOverlay.Instance.Deselect();
            }
        }
    }
}