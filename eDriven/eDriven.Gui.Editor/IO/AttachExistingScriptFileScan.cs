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

using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Reflection;
using UnityEditor;

namespace eDriven.Gui.Editor.IO
{
    internal class AttachExistingScriptFileScan
    {
        public ComponentAdapter Adapter;
        public AddHandlerDataObject Data;
        public bool CreatingNewHandler;

        public bool Run()
        {
            var path = EditorUtility.OpenFilePanel(
                    "Choose the existing script",
                    "Assets",
                    EditorSettings.ScriptExtension);

            if (string.IsNullOrEmpty(path)) // canceled
                return false;

            //Debug.Log("path: " + path);

            /**
             * 1. Get class name
             * */
            var className = Util.ClassNameFromPath(path);
            //Debug.Log("className: " + className);

            //Debug.Log(string.Format(@"AddHandlerScript [adapter: {0}, className: {1}]", Adapter, className));
            
            var component = Adapter.gameObject.GetComponent(className);

            /**
             * 1. Check if the component is already attached, but only if not editing (adding new handler) - we'll handle that separately
             * */
            if (null != component)
            {
                /*if (!CreatingNewHandler) {
                    string text = string.Format(@"Script ""{0}"" is already attached to the selected game object.", className);
                    Debug.LogWarning(text);
                    EditorUtility.DisplayDialog("Duplicated script", text, "OK");
                    return false;
                }*/
                Data.ScriptAlreadyAttached = true;
            }

            Data.ScriptPath = path;
            Data.ClassName = className;
            Data.AttachedScriptType = EditorReflector.GetTypeByClassName(className); // Type.GetType(className);
            Data.Action = CreatingNewHandler ? AddHandlerAction.CreateNewHandlerInExistingScript : AddHandlerAction.AttachExistingScriptAndMapHandler;

            //Data.Snippet = AssetDatabase.LoadAssetAtPath(path, typeof(string)).ToString();
            if (CreatingNewHandler)
            {
                // load the old script
                Data.Snippet = Util.LoadFile(path);
                //Debug.LogWarning("Data.Snippet: " + Data.Snippet);
            }

            //Debug.LogWarning("Data.AttachedScriptType: " + Data.AttachedScriptType);
            
            return true;
        }
    }
}
