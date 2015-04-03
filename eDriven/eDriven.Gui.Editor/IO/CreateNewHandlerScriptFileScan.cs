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
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Dialogs;
using eDriven.Gui.Editor.Reflection;
using UnityEditor;

namespace eDriven.Gui.Editor.IO
{
    internal class CreateNewHandlerScriptFileScan
    {
        public ComponentAdapter Adapter;
        public AddHandlerDataObject Data;
        public string DefaultClassName;

        public bool Run()
        {
            if (string.IsNullOrEmpty(DefaultClassName))
                throw new Exception("DefaultClassName not defined");

            /**
             * 1. Get fixed class name
             * */
            var className = EditorReflector.CreateUniqueScriptName(DefaultClassName);

            /**
             * 2. Get path
             * */
            var path = GetFilePath(className);
            
            if (string.IsNullOrEmpty(path)) // canceled
                return false;

            /**
             * 3. Get chosen class name
             * */
            className = Util.ClassNameFromPath(path);

            bool isUnique = EditorReflector.IsUniqueScriptName(className);
            if (!isUnique)
            {
                string text = string.Format(@"Script of type ""{0}"" already exists in a project.

Please choose a different script name.", className);
                //Debug.LogWarning(text);
                EditorUtility.DisplayDialog("Duplicated script name", text, "OK");
                Run();
                return false;
            }

            /**
             * 4. Check if the component is already attached
             * */
            var component = Adapter.gameObject.GetComponent(className);

            if (null != component)
            {
                string text = string.Format(@"Script ""{0}"" is already attached to the selected game object.", className);
                //Debug.LogWarning(text);
                EditorUtility.DisplayDialog("Duplicated script", text, "OK");
                Run();
                return false;
            }

            Data.ScriptPath = path;
            Data.ClassName = className;
            //Data.AttachedScriptType = ReflectionUtil.GetTypeByClassName(className); // Type.GetType(className);
            //Debug.LogWarning("Data.AttachedScriptType: " + Data.AttachedScriptType);
            Data.Action = AddHandlerAction.CreateNewScriptAndHandler;

            return true;
        }

        private static string GetFilePath(string className)
        {
            //Debug.Log("GetFilePath. className: " + className);

            var path = EditorUtility.SaveFilePanelInProject(
                "Create new script",
                className,
                EditorSettings.ScriptExtension,
                "Please enter a file name to save the script to"
            );

            return path;
        }
    }
}