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

using eDriven.Gui.Editor.IO;
using UnityEditor;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// The class being called by the "Finish" action of the AddEventHandlerDialog wizard
    /// It processes the given persisted data and decides what it would do
    /// It makes choices of stopping the application and delaying the script addition and handler mapping
    /// </summary>
    internal static class PersistedDataProcessingLogic
    {
        public static void Process(AddEventHandlerPersistedData persistedData)
        {
            switch (persistedData.Action)
            {
                default:

                    /**
                     * 1. Mapping the existing handler
                     * Play mode could have be stopped - OPTIONAL
                     * */
                    if (persistedData.OpenScript && EditorApplication.isPlaying)
                    {
                        const string title = "Stop the play mode?";
                        const string msg = @"You have chosen to open a script after mapping the handler.

Do you want to stop the play mode?

NOTE: This is recommended if you are planning to edit the script.";

                        if (EditorUtility.DisplayDialog(title, msg, "Yes", "No"))
                        {
                            // opening script. Save and stop the app.
                            //Debug.Log("Stopping the play mode...");
                            persistedData.Save();
                            EditorApplication.isPlaying = false; // stop the play mode
                        }
                        else
                        {
                            // run now
                            PersistedDataProcessor.Instance.Process(persistedData);
                        }
                    }
                    else
                    {
                        // run now
                        //Debug.Log("Executing immediatelly");
                        PersistedDataProcessor.Instance.Process(persistedData);
                    }

                    break;

                case AddHandlerAction.AttachExistingScriptAndMapHandler:

                    /**
                     * 2. Adding the existing script
                     * Play mode should be stopped - OPTIONAL: if script not already attached
                     * */
                    if (EditorApplication.isPlaying && !persistedData.ScriptAlreadyAttached)
                    {
                        persistedData.Save();
                        DisplayStoppingMessage("because a script is being added");
                        EditorApplication.isPlaying = false; // stop the play mode
                    }
                    else
                    {
                        PersistedDataProcessor.Instance.Process(persistedData);
                    }
                    return;

                case AddHandlerAction.CreateNewHandlerInExistingScript:

                    /**
                     * 3. Creating a new handler
                     * Play mode should be stopped - MANDATORY
                     * */
                    CreateNewHandlerScriptFileExec.Run(AddEventHandlerDialog.Instance.Adapter, persistedData);
                    //Debug.Log("CreateNewHandlerInExistingScript!");

                    if (EditorApplication.isPlaying)
                    {
                        bool mandatoryStop = EditorApplication.isPlaying && !persistedData.ScriptAlreadyAttached;
                        if (mandatoryStop) // script not yet added
                        {
                            persistedData.Save();
                            DisplayStoppingMessage("because a script is being added");
                            EditorApplication.isPlaying = false; // stop the play mode
                        }
                        else
                        {
                            const string title = "Stop the play mode?";
                            const string msg = @"You have chosen to edit a script while in Play mode.

Do you want to stop the play mode?

NOTE: This is recommended if you are planning to edit the script.";

                            if (EditorUtility.DisplayDialog(title, msg, "Yes", "No")) {
                                persistedData.Save();
                                EditorApplication.isPlaying = false; // stop the play mode
                            }
                            else
                            {
                                PersistedDataProcessor.Instance.Process(persistedData);
                            }
                        }
                    }
                    else
                    {
                        PersistedDataProcessor.Instance.Process(persistedData);
                    }
                    break;

                case AddHandlerAction.CreateNewScriptAndHandler:
                    
                    /**
                     * 4. Creating new script
                     * Play mode should be stopped - MANDATORY
                     * */
                    CreateNewHandlerScriptFileExec.Run(AddEventHandlerDialog.Instance.Adapter, persistedData);
                    persistedData.Save();
                    if (EditorApplication.isPlaying)
                    {
                        DisplayStoppingMessage("to be recompiled, because a new script is being created");
                        EditorApplication.isPlaying = false; // stop the play mode
                    }
                    break;

                
            }
        }

        private static void DisplayStoppingMessage(string part)
        {
            const string title = "Stopping the play mode";
            string msg = string.Format(@"The application will be stopped {0}.", part);
            EditorUtility.DisplayDialog(title, msg, "OK");
        }
    }
}