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

using System.IO;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using UnityEditor;

namespace eDriven.Gui.Editor.IO
{
    internal static class CreateNewHandlerScriptFileExec
    {
        private static ComponentAdapter _adapter;
        private static string _className;

        public static void Run(ComponentAdapter adapter, AddEventHandlerPersistedData persistedData)
        {
            //if (EditorApplication.isPlaying)
            //{
            //    //Debug.Log("Stopping...");
            //    EditorApplication.isPlaying = false;
            //}

            _adapter = adapter;

            //Debug.Log("*** persistedData.ScriptPath: " + persistedData.ScriptPath);
            //Debug.Log("*** persistedData.Snippet: " + persistedData.Snippet);

            /**
             * 4. Save file
             * */
            SaveFile(persistedData.ScriptPath, persistedData.Snippet);
        }

        private static void SaveFile(string path, string content)
        {
            //Debug.Log("SaveFile. path: " + path);
            File.WriteAllBytes(path, Util.GetBytes(content));
            // As we are saving to the asset folder, tell Unity to scan for modified or new assets
            AssetDatabase.Refresh();
        }
    }
}