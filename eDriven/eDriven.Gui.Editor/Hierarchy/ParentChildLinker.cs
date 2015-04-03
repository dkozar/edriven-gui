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
using System.Text;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Editor.Persistence;
using UnityEngine;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class ParentChildLinker
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static ParentChildLinker _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private ParentChildLinker()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ParentChildLinker Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating ParentChildLinker instance"));
#endif
                    _instance = new ParentChildLinker();
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

        }

        readonly Dictionary<int, SaveablePack> _changes = new Dictionary<int, SaveablePack>();
        public Dictionary<int, SaveablePack> Changes
        {
            get { return _changes; }
        }

        public bool IsChanged(int instanceId)
        {
            return _changes.ContainsKey(instanceId);
        }

        public void Update(GroupAdapter parentAdapter)
        {
            Update(parentAdapter, null);
        }

        /// <summary>
        /// Scans the supplied parent adapter
        /// Converts ChildGroupPacks to SaveablePacks and saves them to dictionary
        /// These packs are used later (in "Process" method) to apply the changes after the play mode is stopped
        /// </summary>
        /// <param name="parentAdapter"></param>
        /// <param name="pack"></param>
        public void Update(GroupAdapter parentAdapter, ChildGroupPack pack)
        {
            //Debug.Log("Update: " + parentAdapter);
#if DEBUG
            if (DebugMode)
            {
                //Debug.Log(string.Format("Linking {0} -> {1}", parentAdapter, childAdapter));
                Debug.Log(string.Format("ParentChildLinker: Updating {0}", parentAdapter));
            }
#endif
            //Debug.Log("parentAdapter: " + parentAdapter);
            // 1. we have to monitor parentAdapter for later (if not yet monitored)
            if (Application.isPlaying)
                PersistenceManager.Instance.Watch(parentAdapter);

            // 2. if no pack supplied, read it now
            if (null == pack)
            {
                //Debug.Log("ParentChildLinker: Pack not defined. Reading now.");
                pack = ChildGroupPack.Read(parentAdapter);
            }

            // 3. register adapters for later
            pack.RegisterAdapters();

            // 4. get relation object
            var guidPack = pack.ToSaveablePack();

            // 5. cache it (overwrite existing!)
            _changes[guidPack.ParentInstanceId] = guidPack;
        }
        
        /// <summary>
        /// Describes changes
        /// </summary>
        /// <returns></returns>
        public string DescribeChanges()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, SaveablePack> pack in _changes)
            {
//                sb.Append(string.Format(@"[{0}]
//{1}", pack.Key, pack.Value));
                sb.Append(pack.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns true if having changes
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return _changes.Count > 0;
        }

        /// <summary>
        /// Resets all changes
        /// </summary>
        public void Reset()
        {
            _changes.Clear();
        }

        /// <summary>
        /// Processes changes (after the play mode is stopped)
        /// It has to go through the created hierarchy and fix the children collections
        /// If these collections are not fixed, the children not present in the collection will not be instantiated
        /// (although their transforms are present and properly parented in the hierarchy)
        /// </summary>
        public void Process()
        {

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format("ParentChildLinker: Processing {0} changes", _changes.Count));
            }
#endif
            foreach (SaveablePack pack in _changes.Values)
            {
                pack.Write();
            }
        }
    }
}
