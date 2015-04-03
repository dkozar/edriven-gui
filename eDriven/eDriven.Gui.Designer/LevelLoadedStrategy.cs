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

using eDriven.Core.Managers;
using eDriven.Gui.Designer.Util;
using UnityEngine;

namespace eDriven.Gui.Designer
{
    internal static class LevelLoadedStrategy
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
    /// <summary>
    /// Debug mode
    /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        private static bool _started;
        public static bool Started
        {
            get { return _started; }
        }

        public static void Start()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("=== DesignerModeStrategy Start ===");
            }
#endif
            var signal = SystemManager.Instance.LevelLoadedSignal;
            if (!signal.HasSlot(LevelLoadedSlot))
                signal.Connect(LevelLoadedSlot);

            _started = true;
        }

        public static void Stop()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("=== DesignerModeStrategy Stop ===");
            }
#endif
            SystemManager.Instance.LevelLoadedSignal.Disconnect(LevelLoadedSlot);

            _started = false;
        }

        private static void LevelLoadedSlot(object[] parameters)
        {
            //Debug.Log("Scene changed. Clearing all GUIDs");
            //GuidUtil.ClearAllGuids();
        }
    }
}