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
using eDriven.Gui.Editor.Styles;
using eDriven.Gui.Reflection;
using eDriven.Gui.Styles;
using eDriven.Gui.Styles.MediaQueries;
using eDriven.Gui.Styles.Serialization;
using UnityEditor;

namespace eDriven.Gui.Editor.Dialogs.Commands
{
    internal static class EditStyleDeclarationCommand
    {
        private static SerializedProperty _declaration;
        private static StyleDeclarationDataObject _data;

        /// <summary>
        /// Called to opan the window and add/edit the style declaration
        /// </summary>
        /// <param name="declaration"></param>
        internal static void Execute(SerializedProperty declaration)
        {
            _declaration = declaration;

            var dialog = StyleDeclarationDialog.Instance;
            dialog.title = "Edit style declaration";
            dialog.Reset();

            _data = StyleDeclarationDataObject.FromSerializedProperty(declaration);
            if (string.IsNullOrEmpty(_data.ModuleId))
            {
                var t = GlobalTypeDictionary.Instance.Get(_data.Type);
                _data.ModuleId = StyleModuleManager.Instance.GetOwnerModule(t).Id;
            }

            var propertyList = new List<StyleProperty>();
            var propertyArray = declaration.FindPropertyRelative("Properties");

            if (null != propertyArray)
            {
                var count = propertyArray.arraySize;
                //Debug.Log("count: " + count);

                for (int i = 0; i < count; i++)
                {
                    var p = propertyArray.GetArrayElementAtIndex(i);
                    var name = p.FindPropertyRelative("Name").stringValue;
                    var type = p.FindPropertyRelative("Type").stringValue;

                    // NOTE: Not passing the value here!
                    // This is because:
                    // 1. The value cannot be read from serialized style declaration
                    // 2. We don't need the value in this dialog anyway
                    propertyList.Add(new StyleProperty { Name = name, Type = type });
                }
            }

            _data.StyleProperties = propertyList;

            var mediaQueryList = new List<MediaQuery>();
            var mediaQueryArray = declaration.FindPropertyRelative("MediaQueries");

            if (null != mediaQueryArray)
            {
                var count = mediaQueryArray.arraySize;

                for (int i = 0; i < count; i++)
                {
                    var q = mediaQueryArray.GetArrayElementAtIndex(i);
                    var name = q.FindPropertyRelative("Name").stringValue;
                    var type = q.FindPropertyRelative("Type").stringValue;

                    // NOTE: Not passing the value here!
                    // This is because:
                    // 1. The value cannot be read from serialized style declaration
                    // 2. We don't need the value in this dialog anyway
                    mediaQueryList.Add(new MediaQuery { Name = name, Type = type });
                }
            }

            _data.MediaQueries = mediaQueryList;

            dialog.Data = _data;
            //dialog.Initialize();

            //dialog.StyleSheet = (eDrivenStyleSheetBase)target;
            dialog.Callback = DialogCallback;
            dialog.ShowUtility();
        }

        private static void DialogCallback()
        {
            _data.UpdateSerializedProperty(_declaration); // only this line is needed

            /*if (EditorSettings.LiveStyling)
                MediaQueryManager.Instance.Rescan();*/

            // process styles - live
            if (EditorSettings.LiveStyling)
                Gui.ProcessStyles();
            
        }
    }
}
