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

using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// The action data collected by the AddEventHandlerDialog
    /// </summary>
    internal class AddStyleDeclarationPersistedData
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        #region Keys

        private const string SavedKey = "eDriven_b9f165d0-0693-4042-8b23-175abb735df7";
        private const string PostCompileProcessingModeKey = "eDriven_efdb7a05-0368-45ab-a7b0-cf789059f6bb";
        private const string EventNameKey = "eDriven_686ec997-d8d6-4bef-8f87-e8b7a0f67abd";
        private const string ActionKey = "eDriven_cd4f3420-4722-4fa4-86f0-e1f4cac0587f";
        private const string ScriptPathKey = "eDriven_00fa0ea3-b460-491b-be26-0b30ceb72422";
        private const string ClassNameKey = "eDriven_4cf99366-bde8-4480-a858-17cb5bff9e53";
        private const string MethodNameKey = "eDriven_4a658d87-8f28-400b-a581-dacce0a950be";
        //private const string AttachedScriptTypeKey = "eDriven_edd12797-aed4-4358-b282-b8c9d6406838";
        private const string SnippetKey = "eDriven_3f5c6dd2-2666-48db-a71e-2145062d3791";
        private const string SnippetLineNumberKey = "eDriven_014f0b73-0add-43aa-be9d-7eb38fd1a6b4";
        private const string EventPhasesKey = "eDriven_654e588b-e570-4e24-bc69-a285d5f558af";
        private const string OpenScriptKey = "eDriven_17b38b4c-faac-40c5-8012-871219ace0e0";
        private const string TransformInstanceIdKey = "eDriven_50941152-d661-4887-b370-78fb90254191";
        private const string ScriptAlreadyAttachedKey = "eDriven_f75cf96c-9b03-4739-9d41-82e574915df7";

        #endregion

        #region Properties

        /// <summary>
        /// A flag read by the system after the reload/recompile
        /// </summary>
        public static bool Saved
        {
            get { return EditorPrefs.GetBool(SavedKey); }
            set
            {
                if (value == EditorPrefs.GetBool(SavedKey))
                    return;

                EditorPrefs.SetBool(SavedKey, value);
#if DEBUG
                if (DebugMode)
                {
                    Debug.Log(string.Format(@"SavedKey changed to " + value));
                }
#endif
            }
        }

        /// <summary>
        /// Script name
        /// </summary>
        public string ClassName;

        #endregion
        
        /// <summary>
        /// 1. Creates the persisted data from ordinary data object
        /// We are using the persisted data even if we are not in play mode
        /// This is because we want to have a single pipeline for both cases
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public static AddStyleDeclarationPersistedData FromDataObject(StyleDeclarationDataObject dataObject)
        {
            AddStyleDeclarationPersistedData persistedData = new AddStyleDeclarationPersistedData
            {
                ClassName = dataObject.Type
            };

            return persistedData;
        }

        /// <summary>
        /// 2. Saving data to EditorSettings
        /// Used when in play mode and the play mode has to be stopped
        /// </summary>
        public void Save()
        {
            EditorPrefs.SetString(ClassNameKey, ClassName);
            Saved = true;
        }

        /// <summary>
        /// 3. Loading data from EditorSettings
        /// Used if was in play mode and the play mode has been stopped
        /// </summary>
        public static AddStyleDeclarationPersistedData Load()
        {
            AddStyleDeclarationPersistedData persistedData = new AddStyleDeclarationPersistedData
            {
                ClassName = EditorPrefs.GetString(ClassNameKey)
            };

            return persistedData;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(@"Type: ""{0}""", ClassName);
        }
    }
}