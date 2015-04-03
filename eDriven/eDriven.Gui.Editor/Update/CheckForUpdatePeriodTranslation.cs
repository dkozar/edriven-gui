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
using UnityEngine;

namespace eDriven.Gui.Editor.Update
{
    /// <summary>
    /// Translates the update check period index to hours
    /// </summary>
    internal class CheckForUpdatePeriodTranslation : List<float>
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static CheckForUpdatePeriodTranslation _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private CheckForUpdatePeriodTranslation()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CheckForUpdatePeriodTranslation Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating CheckForUpdatePeriod instance"));
#endif
                    _instance = new CheckForUpdatePeriodTranslation();
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
            Add(1); // 1h
            Add(3); // 3h
            Add(6); // 6h
            Add(12); // 11h
            Add(24); // 1 day
            Add(24 * 3); // 3 days
            Add(24 * 7); // 1 week
            Add(24 * 14); // 2 weeks
            Add(24 * 30); // 1 month
            Add(24 * 30 * 3); // 3 months
        }
    }
}
