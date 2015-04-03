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

using eDriven.Gui.Styles.MediaQueries;
using UnityEditor;

namespace eDriven.Gui.Editor.Dialogs.Commands
{
    internal static class AddStyleDeclarationCommand
    {
        private static SerializedObject _serializedObject;
        private static StyleDeclarationDataObject _data;

        /// <summary>
        /// Called to opan the window and add/edit the style declaration
        /// </summary>
        /// <param name="serializedObject"></param>
        internal static void Execute(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;

            var dialog = StyleDeclarationDialog.Instance;
            dialog.title = "Add Style Declaration";
            _data = new StyleDeclarationDataObject();

            dialog.Reset();
            /**
             * Passing an empty DTO
             * */
            dialog.Data = _data;
            //dialog.Initialize();
            //dialog.StyleSheet = (eDrivenStyleSheetBase)target;
            dialog.Callback = DialogCallback;
            dialog.ShowUtility();
        }

        private static void DialogCallback()
        {
            //Debug.Log("Adding style declaration for: " + className);

            // 1. reference declarations
            SerializedProperty declarations = GetDeclarations(_serializedObject);
            
            // 2. increase number of items
            declarations.arraySize += 1;

            // 3. get last (just created) item
            SerializedProperty declaration = declarations.GetArrayElementAtIndex(declarations.arraySize - 1);

            // 4. update serialized property
            _data.UpdateSerializedProperty(declaration);

            // 5. expand it
            declaration.isExpanded = true; // expand

            /*if (EditorSettings.LiveStyling)
                MediaQueryManager.Instance.Rescan();*/

            // process styles - live
            if (EditorSettings.LiveStyling)
                Gui.ProcessStyles();
        }

        #region Helper

        private static SerializedProperty GetDeclarations(SerializedObject serializedObject)
        {
            return serializedObject.FindProperty("StyleSheet").FindPropertyRelative("Declarations");
        }
        
        #endregion
    }
}
