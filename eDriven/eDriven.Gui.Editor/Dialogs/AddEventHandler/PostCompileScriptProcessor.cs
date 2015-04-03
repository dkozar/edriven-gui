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
using System.Text;
using eDriven.Gui.Editor.IO;
using eDriven.Gui.Reflection;
using UnityEngine;
using System.Collections.Generic;

namespace eDriven.Gui.Editor.Dialogs
{
    /// <summary>
    /// A class handling the loading of the scripts created before the assembly reload to the specified game object
    /// </summary>
    internal class PostCompileScriptProcessor
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

        private static PostCompileScriptProcessor _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PostCompileScriptProcessor()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        internal static PostCompileScriptProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                    {
                        Debug.Log(string.Format("Instantiating PostCompileScriptProcessor instance"));
                    }
#endif
                    _instance = new PostCompileScriptProcessor();
                    Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private static void Initialize()
        {
#if DEBUG
            if (DebugMode)
            {
                Debug.Log("=== PostCompileScriptProcessor INITIALIZE ===");
            }
#endif
        }

// ReSharper disable MemberCanBeMadeStatic.Global
        public void Process()
// ReSharper restore MemberCanBeMadeStatic.Global
        {
            // reflect all types

            /*var snippet = AllTypesSnipetCreator.Create();
            CreateTypeListExec.Run(snippet);*/ // TODO

            if (!AddEventHandlerPersistedData.PostCompileProcessingMode)
                return;

            if (AddEventHandlerPersistedData.Saved)
            {
                AddEventHandlerPersistedData.Saved = false;
                
                //Debug.Log("Loading persisted data...");

                // process
                var persistedData = AddEventHandlerPersistedData.Load();

//                Debug.Log(@"persistedData: 
//" + persistedData);

                PersistedDataProcessor.Instance.Process(persistedData);
            }
        }
    }

    internal class AllTypesSnipetCreator
    {
        public static string Create()
        {
            var namespaces = new List<string>()
            {
                "System",
                "System.Collections.Generic",
                "UnityEngine"
            };

            StringBuilder sb = new StringBuilder();
            var allTypes = TypeReflector.GetAllLoadedTypes();
            //foreach (var type in allTypes)
            for (int i = 0; i < allTypes.Count; i++)
            {
                var type = allTypes[i];
                if (!namespaces.Contains(type.Namespace) && !string.IsNullOrEmpty(type.Namespace))
                    namespaces.Add(type.Namespace);

                if (i >= allTypes.Count - 1)
                    sb.Append(string.Format("typeof({0})", type.Name));
                else
                    sb.AppendLine(string.Format("typeof({0}),", type.Name));
            }

            StringBuilder ns = new StringBuilder();
            foreach (string s in namespaces)
            {
                ns.AppendLine(string.Format("using {0};", s));
            }

            return string.Format(@"{0}
internal static class TypeReflector
{{
    internal static List<Type> GetList()
    {{
        var list = new List<Type>() {{
{1}
        }};
        return list;
    }}
}}", ns, sb);
        }
    }
}