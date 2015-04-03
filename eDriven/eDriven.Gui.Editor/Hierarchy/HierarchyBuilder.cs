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
using UnityEngine;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Editor.Hierarchy
{
    internal class HierarchyBuilder
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static HierarchyBuilder _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private HierarchyBuilder()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static HierarchyBuilder Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating HierarchyBuilder instance"));
#endif
                    _instance = new HierarchyBuilder();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize() {}

        /// <summary>
        /// Root node
        /// </summary>
        private Node _rootNode;

        /// <summary>
        /// Fetch mode<br/>
        /// GuiStagesOnly mode is more optimized // TODO: Fix stage mode
        /// </summary>
        public LevelLookupMode ClassMode = LevelLookupMode.GuiStagesOnly;


        /// <summary>
        /// Fetch mode<br/>
        /// EnabledOnly mode is more optimized
        /// </summary>
        public EnabledLookupMode EnabledMode = EnabledLookupMode.EnabledOnly;

        
        /// <summary>
        /// Fetches all transforms having the StageAdapter attached
        /// </summary>
        /// <returns></returns>
        private List<Transform> FetchStages()
        {
            StageAdapter[] arr = EnabledMode == EnabledLookupMode.EnabledOnly ?
                (StageAdapter[])Object.FindObjectsOfType(typeof(StageAdapter)):
                (StageAdapter[])Resources.FindObjectsOfTypeAll(typeof(StageAdapter));

            //Debug.Log(arr.Length + " adapters.");
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (StageAdapter adapter in arr)
            {
                gameObjects.Add(adapter.gameObject);
            }
            //Debug.Log(gameObjects.Count + " gameObjects.");

#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder(@"Stages:
");

                foreach (GameObject go in gameObjects)
                {
                    sb.AppendLine(string.Format("    {0}", go.name));
                }

                Debug.Log(sb);
            }
#endif
            /**
             * 2. Get transforms
             * */
            return GetTransforms(gameObjects);
        }

        /// <summary>
        /// Fetches top transforms (having no parent transform)
        /// </summary>
        /// <returns></returns>
        private List<Transform> FetchTopTransforms()
        {
            GameObject[] arr = EnabledMode == EnabledLookupMode.EnabledOnly ?
                (GameObject[])Object.FindObjectsOfType(typeof(GameObject)) :
                (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

            List<GameObject> gameObjects = new List<GameObject>(arr);

            gameObjects.RemoveAll(delegate(GameObject gameObject)
                                      {
                                          // remove all non-root game objects
                                          return gameObject.transform.root != gameObject.transform;
                                      });
#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder(@"Top transforms:
");

                foreach (GameObject go in gameObjects)
                {
                    sb.AppendLine(string.Format("    {0}", go.name));
                }

                Debug.Log(sb);
            }
#endif

            /**
             * 2. Get transforms
             * */
            return GetTransforms(gameObjects);
        }

        public Node BuildHierarchy()
        {
            /**
             * 1. Fetch all objects
             * */
            List<Transform> topTransforms = 
                (ClassMode == LevelLookupMode.GuiStagesOnly) ? FetchStages() : FetchTopTransforms();
#if DEBUG
            if (DebugMode)
            {
                StringBuilder sb = new StringBuilder(@"Root game objects in the hierarchy:
"); foreach (Transform go in topTransforms)
                {
                    sb.AppendLine(string.Format("    {0}", go.name));
                }

                Debug.Log(sb);
            }
#endif

            /**
             * 3. Create node for each root transform
             * Build each node and add the to root nodes collection
             * */
            _rootNode = new Node(null);

            foreach (Transform transform in topTransforms)
            {
                var node = new Node(transform);
                ComponentAdapter adapter = node.Transform.gameObject.GetComponent<ComponentAdapter>();

                if (null != adapter)
                {
                    node.Build();
                    _rootNode.Add(node);
                }
            }

            return _rootNode;
        }

        #region Helper

        private static List<Transform> GetTransforms(IEnumerable<GameObject> allObjects)
        {
            List<Transform> transforms = new List<Transform>();
            foreach (GameObject go in allObjects)
            {
                transforms.Add(go.transform);
            }
            //Debug.Log(transforms.Count + " transforms.");
            return transforms;
        }

        #endregion

        public enum LevelLookupMode
        {
            TopTransforms, GuiStagesOnly
        }

        public enum EnabledLookupMode
        {
            All, EnabledOnly
        }
    }
}