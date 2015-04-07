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

#if TRIAL

using System.Reflection;
using eDriven.Core.Geom;
using eDriven.Gui.Editor.Rendering;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    [Obfuscation(Exclude = true)]
    internal class PurchaseDialog : EDrivenDialogBase
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

#region Singleton

        private static PurchaseDialog _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PurchaseDialog()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PurchaseDialog Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PurchaseDialog instance"));
#endif
                    //_instance = new PurchaseDialog();
                    _instance = (PurchaseDialog)CreateInstance(typeof(PurchaseDialog));
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
            title = "Purchase";
            Rectangle screenRectangle = Rectangle.FromSize(new Point(Screen.currentResolution.width, Screen.currentResolution.height));
            Rectangle winRectangle = Rectangle.FromSize(new Point(430, 400));
            winRectangle = winRectangle.CenterInside(screenRectangle);
            position = winRectangle.ToRect();
            //Debug.Log("position: " + position);
            minSize = maxSize = winRectangle.Size.ToVector2();
            wantsMouseMove = true;
        }   

// ReSharper disable UnusedMember.Local
        void OnGUI() {
// ReSharper restore UnusedMember.Local
            DialogContentWrapper.Start();

            PurchaseBox.Instance.Render();

            DialogContentWrapper.End();

            if (Event.current.type == EventType.MouseMove)
                Repaint();
        }
    }
}

#endif