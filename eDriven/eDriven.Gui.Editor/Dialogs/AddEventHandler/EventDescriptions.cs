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
using eDriven.Gui.Components;
using UnityEngine;
using Event=UnityEngine.Event;

namespace eDriven.Gui.Editor.Dialogs
{
    public class EventDescriptions : Dictionary<string, string>
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static EventDescriptions _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private EventDescriptions()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static EventDescriptions Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating EventDescriptions instance"));
#endif
                    _instance = new EventDescriptions();
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
            Add(FrameworkEvent.PREINITIALIZE, "fires after the components constructor has been run");
            Add(FrameworkEvent.INITIALIZE, "fires after components children have been created");
            Add(FrameworkEvent.CREATION_COMPLETE, "fires after the component and its children have been created and layed out");
            Add(FrameworkEvent.SHOWING, "fires before the component is shown");
            Add(FrameworkEvent.HIDING, "fires before the component is hidden");
            Add(FrameworkEvent.SHOW, "fires after the component is shown");
            Add(FrameworkEvent.ADDING, "fires before the component is being added to a parent");
            Add(FrameworkEvent.ADD, "fires after the component is added to a parent");

            // mouse events
            Add(MouseEvent.MOUSE_MOVE, "fires when the mouse is moved over the component");
        }
    }
}