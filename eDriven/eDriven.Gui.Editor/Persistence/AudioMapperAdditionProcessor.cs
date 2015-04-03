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
    internal class AudioMapperAdditionProcessor
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

        private static AudioMapperAdditionProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private AudioMapperAdditionProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static AudioMapperAdditionProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Instantiating AudioMapperAdditionProcessor instance"));
                    }
#endif
                    _instance = new AudioMapperAdditionProcessor();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private const string AudioMapperPathKey = "eDrivenC43FA54563";

        internal static string AudioMapperPath
        {
            get { return EditorPrefs.GetString(AudioMapperPathKey); }
            set
            {
                if (value == EditorPrefs.GetString(AudioMapperPathKey))
                    return;

                EditorPrefs.SetString(AudioMapperPathKey, value);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(@"AudioMapperPath changed to " + value));
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
                Debug.Log("=== AudioMapperAdditionProcessor INITIALIZE ===");
            }
#endif
            //Process();
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void Process()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            if (!string.IsNullOrEmpty(AudioMapperPath))
            {
                Debug.Log("Creating audio mapper...");
                var command = new CreateAudioMapperCommand { AudioMapperPath = AudioMapperPath };
                command.Run();

                AudioMapperPath = null;
            }
        }

    }
}