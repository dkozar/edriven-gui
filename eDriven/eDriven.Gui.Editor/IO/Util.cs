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

using System;
using System.IO;
using System.Text;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.IO
{
    internal static class Util
    {
        public static string ClassNameFromPath(string path)
        {
            string filename = string.Empty;
            var parts = path.Split('/');
            if (parts.Length > 0)
            {
                filename = parts[parts.Length - 1];
            }
            //Debug.Log("filename: " + filename);
            var className = filename.Replace(string.Format(".{0}", EditorSettings.ScriptExtension), string.Empty);
            //Debug.Log("ClassNameFromPath. className: " + className);
            return className;
        }

        public static Component AddHandlerScript(ComponentAdapter adapter, AddEventHandlerPersistedData data)
        {
            //Debug.Log(string.Format(@"AddHandlerScript [adapter: {0}, data: {1}]", adapter, data));
            var component = adapter.gameObject.GetComponent(data.ClassName);

            Component addedComponent = null;

            /**
             * 1. Check if the component is already attached
             * */
            if (null != component)
            {
                string text = string.Format(@"Script ""{0}"" is already attached to the selected game object.", data.ClassName);
                Debug.LogWarning(text);
                EditorUtility.DisplayDialog("Duplicated script", text, "OK");
            }
            else
            {
                /**
                 * 2. Add component
                 * */
                addedComponent = adapter.gameObject.AddComponent(data.ClassName);
            }

            return addedComponent;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }

        public static string LoadFile(string path)
        {
            StreamReader reader = null;

            //Debug.Log("Loading script from path: " + path);

            FileInfo fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
                reader = fileInfo.OpenText();

            if (reader == null)
            {
                throw new Exception(string.Format(@"Cannot load file ""{0}""", fileInfo));
            }

            StringBuilder sb = new StringBuilder();

            // Read each line from the file
            string txt;
            while ((txt = reader.ReadLine()) != null)
            {
                sb.AppendLine(txt);
            }

            //Debug.Log(sb.ToString());

            return sb.ToString();
        }
    }
}
