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

namespace eDriven.Gui.Editor
{
    internal static class Menu
    {

        /// <summary>
        /// API
        /// </summary>
        //[MenuItem("Help/eDriven/About")]
        //public static void About()
        //{
        //    EditorSettings.ShowAbout = true;
        //}

        /// <summary>
        /// API
        /// </summary>
        [MenuItem("Help/eDriven/eDriven.Gui Homepage")]
        public static void LounchHomepage()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.HOMEPAGE);
        }

        /// <summary>
        /// API
        /// </summary>
        [MenuItem("Help/eDriven/eDriven Forum")]
        public static void LounchForum()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.FORUM);
        }

        /// <summary>
        /// Manual
        /// </summary>
        [MenuItem("Help/eDriven/Documentation/Manual")]
        public static void LounchManual()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.MANUAL);
        }

        /// <summary>
        /// API
        /// </summary>
        [MenuItem("Help/eDriven/Documentation/API")]
        public static void LounchApi()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.API);
        }

        /// <summary>
        /// Core source @ Github
        /// </summary>
        [MenuItem("Help/eDriven/Documentation/eDriven.Core at GitHub")]
        public static void LounchGithub()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.GITHUB);
        }

        /// <summary>
        /// Video channel
        /// </summary>
        [MenuItem("Help/eDriven/Documentation/Video Channel")]
        public static void LounchVideo()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.VIDEO);
        }

        /// <summary>
        /// Video channel
        /// </summary>
        [MenuItem("Help/eDriven/Documentation/Demo Site")]
        public static void LounchDemo()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.DEMO);
        }
        
        /// <summary>
        /// Q&A thread
        /// </summary>
        [MenuItem("Help/eDriven/Unity Forum/eDriven.Core Thread")]
        public static void LounchForumCore()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.UNITY_FORUM_CORE);
        }

        /// <summary>
        /// Q&A thread
        /// </summary>
        [MenuItem("Help/eDriven/Unity Forum/eDriven.Gui Thread")]
        public static void LounchForumGui()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.UNITY_FORUM_GUI);
        }

        /// <summary>
        /// Q&A thread
        /// </summary>
        [MenuItem("Help/eDriven/Unity Forum/eDriven.Gui Thread at Assets and Asset Store")]
        public static void LounchForumAssetStore()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.UNITY_FORUM_ASSET_STORE);
        }

        /// <summary>
        /// Q&A thread
        /// </summary>
        [MenuItem("Help/eDriven/Unity Forum/eDriven Questions and Answers")]
// ReSharper disable InconsistentNaming
        public static void LounchForumQA()
// ReSharper restore InconsistentNaming
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.UNITY_FORUM_QA);
        }

        /// <summary>
        /// eDriven.Gui Free Edition
        /// </summary>
        [MenuItem("Help/eDriven/Unity Forum/eDriven Free Edition")]
        public static void LounchForumFree()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.UNITY_FORUM_FREE);
        }

        /// <summary>
        /// Author's homepage
        /// </summary>
        [MenuItem("Help/eDriven/Contact/Author @Twitter")]
        public static void LounchTwitter()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.TWITTER);
        }

        /// <summary>
        /// Author's homepage
        /// </summary>
        [MenuItem("Help/eDriven/Contact/Author's homepage")]
        public static void LounchAuthorsHomepage()
        {
            UnityEditor.Help.BrowseURL(eDrivenLinks.AUTHORS_HOMEPAGE);
        }
    }
}