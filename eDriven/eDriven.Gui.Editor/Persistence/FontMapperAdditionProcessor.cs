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

using eDriven.Gui.Editor.Commands;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Persistence
{
    /// <summary>
    /// A class handling loading of the scripts created before the assembly reload to the specified game object
    /// 
    /// </summary>
    internal class FontMapperAdditionProcessor
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;
// ReSharper restore UnassignedField.Global
#endif
        #region Singleton

        private static FontMapperAdditionProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private FontMapperAdditionProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static FontMapperAdditionProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Instantiating FontMapperAdditionProcessor instance"));
                    }
#endif
                    _instance = new FontMapperAdditionProcessor();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private const string FontMapperPathKey = "eDrivenC0AAC456";

        internal static string FontMapperPath
        {
            get { return EditorPrefs.GetString(FontMapperPathKey); }
            set
            {
                if (value == EditorPrefs.GetString(FontMapperPathKey))
                    return;

                EditorPrefs.SetString(FontMapperPathKey, value);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(@"FontMapperPath changed to " + value));
                }
#endif
            }
        }

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("=== FontMapperAdditionProcessor INITIALIZE ===");
            }
#endif
            //Process();
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void Process()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            if (!string.IsNullOrEmpty(FontMapperPath))
            {
                Debug.Log("Creating font mapper...");
                var command = new CreateFontMapperCommand {FontMapperPath = FontMapperPath};
                command.Run();

                FontMapperPath = null;

                ///**
                // * 3. Re-scan the hierarchy
                // * */
                //HierarchyViewDecorator.Instance.ReScan(/*adapter.GetInstanceID()*/);
            }
        }
    }
}