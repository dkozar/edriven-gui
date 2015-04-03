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
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Persistence
{
    /// <summary>
    /// The class used for persisting component ID to component relation<br/>
    /// Each persisted object reads the component InstanceID when created<br/>
    /// It uses the same instance ID after the play mode is stopped to get to the adapter its datas hould be applied to<br/>
    /// However, there's no guarantee that the adapter with this ID stillexists<br/>
    /// That is because if the adapter was added in play mode, after the play mode is stopped, id doesn't exist anymore<br/>
    /// In this case the additions list contains the change so this adapter is being resurrected, however it gets the new ID!<br/>
    /// To keep the relation alive, from within the additions processor we are calling the Register method of this class with a new ID<br/>
    /// Now the persisted changes are veing related to this new adapter
    /// </summary>
    internal sealed class ComponentRegistry
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static ComponentRegistry _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private ComponentRegistry()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static ComponentRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating ComponentRegistry instance"));
#endif
                    _instance = new ComponentRegistry();
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

        /// <summary>
        /// Static dictionary containing the instance ID to newly created adapter (in edit mode) relation
        /// </summary>
        private readonly Dictionary<int, Object> _dict = new Dictionary<int, Object>();

        public void Register(int id, ComponentAdapter adapter)
        {
            _dict[id] = adapter;
        }

        //public void Unregister(int id)
        //{
        //    _dict.Remove(id);
        //}

        public Object Get(int id)
        {
            if (!_dict.ContainsKey(id))
                return null;

            return _dict[id];
        }

        /// <summary>
        /// This override is used for getting the adapter reference after stopping the play mode
        /// IMPORTANT: component adapters present in the scene in edit mode keep their instance ID while
        /// in play mode, as well as the play mode is stopped
        /// We put the reference to the observed adapter while stopping the play mode
        /// However, these "old" adapters, when put into the dictionary while stopping, have the same 
        /// InstanceIDs as the "new" ones created after the play mode is stopped
        /// However, if getting them from dictionary (and not using the EditorUtility.InstanceIDToObject) we get 
        /// the "stale" reference, pointing to the old adapter which is still around (no error when appliying values)
        /// So, we have to make sure that we are first looking for adapters in the scene
        /// This way we always get the fresh one
        /// </summary>
        /// <param name="id">Instance ID</param>
        /// <param name="searchInTheSceneFirst">Should we use the editor utility search</param>
        /// <returns>Component adapter specified by supplied instance ID</returns>
        public Object Get(int id, bool searchInTheSceneFirst)
        {
            if (searchInTheSceneFirst)
            {
                var obj = EditorUtility.InstanceIDToObject(id);
                if (null != obj)
                    return obj;
            }

            return Get(id);
        }

        public void Clear()
        {
            _dict.Clear();
        }
    }
}
