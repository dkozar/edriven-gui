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
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.IO
{
    internal static class CreateTypeListExec
    {
// ReSharper disable once InconsistentNaming
        public const string FILE_PATH = "/eDriven/Resources/Reflection/AllTypes.cs";

        public static void Run(string snippet)
        {
            Debug.Log(@"Writing snippet:

" + snippet);
            /**
             * 4. Save file
             * */
            SaveFile(Application.dataPath + FILE_PATH, snippet);
        }

        private static void SaveFile(string path, string content)
        {
            Debug.Log("CreateTypeListExec SaveFile. path: " + path);
            File.WriteAllBytes(path, Util.GetBytes(content));
            // As we are saving to the asset folder, tell Unity to scan for modified or new assets
            AssetDatabase.Refresh();
        }
    }
}