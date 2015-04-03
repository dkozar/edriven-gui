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
using eDriven.Gui.Editor.IO;
using UnityEngine;

namespace eDriven.Gui.Editor.Dialogs
{
    internal static class SnippetCreator
    {
        public static void CreateSnippet(AddHandlerDataObject data)
        {
            ISnippetGenerator gene;

            switch (EditorSettings.ScriptExtension)
            {
                    //case ScriptExtensions.JAVASCRIPT:
                default:
                    gene = new JavascriptSnippetGenerator(data);
                    break;
                case ScriptExtensions.CSHARP:
                    gene = new CSharpSnippetGenerator(data);
                    break;
                case ScriptExtensions.BOO:
                    gene = new BooSnippetGenerator(data);
                    break;
            }

            string snippet = gene.Generate();

            if (data.OpenScript)
            {
                int index = snippet.IndexOf(PersistedDataProcessor.CursorMarker);

                string[] parts = snippet.Substring(0, index).Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // set line number
                data.SnippetLineNumber = parts.Length; // +1;

                // remove market
                snippet = snippet.Replace(PersistedDataProcessor.CursorMarker, string.Empty);
            }

            data.Snippet = snippet;
        }

        public static void InsertHandler(AddHandlerDataObject data)
        {
            ISnippetGenerator gene;

            switch (EditorSettings.ScriptExtension)
            {
                    //case ScriptExtensions.JAVASCRIPT:
                default:
                    gene = new JavascriptSnippetGenerator(data);
                    break;
                case ScriptExtensions.CSHARP:
                    gene = new CSharpSnippetGenerator(data);
                    break;
                case ScriptExtensions.BOO:
                    gene = new BooSnippetGenerator(data);
                    break;
            }

            string origSnippet = data.Snippet;

            if (string.IsNullOrEmpty(origSnippet))
            {
                Debug.LogError("Problem with file content");
                return;
            }

            string handlerString = gene.CreateEventHandler();

            string[] parts = origSnippet.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>(parts);
            int index = Math.Max(0, parts.Length - 2);
            list.Insert(index, handlerString);
            string snippet = String.Join(Environment.NewLine, list.ToArray());

            if (data.OpenScript)
            {
                index = snippet.IndexOf(PersistedDataProcessor.CursorMarker);
                if (index == -1)
                {
                    Debug.LogError("Cannot find cursor marker");
                    return;
                }

                parts = snippet.Substring(0, index).Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // set line number
                data.SnippetLineNumber = parts.Length; // +1;

                // remove marker
                data.Snippet = snippet.Replace(PersistedDataProcessor.CursorMarker, string.Empty);

                // set line number
                data.SnippetLineNumber = index;
            }
        }
    }
}