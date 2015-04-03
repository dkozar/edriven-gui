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

#if DEBUG
using UnityEngine;
#endif

namespace eDriven.Gui.Editor.Processing
{
    internal class Bootstrap
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static Bootstrap _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private Bootstrap()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Bootstrap Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating Bootstrap instance"));
#endif
                    _instance = new Bootstrap();
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
// ReSharper disable UnusedVariable
            var playModeStateChangeEmitter = PlayModeStateChangeEmitter.Instance; // init
            var overlay = PlayModeChangeProcessorHandlingDesignerOverlay.Instance;
            var persistence = PlayModeChangeProcessorHandlingPersistence.Instance;
            var keys = PlayModeChangeProcessorHandlingKeys.Instance;
            var hierarchy = HierarchyChangeProcessor.Instance;
            var designRemove = DesignerEventProcessor.Instance;
            var mouseClick = GameViewMouseClickProcessor.Instance;
// ReSharper restore UnusedVariable
        }
    }
}
