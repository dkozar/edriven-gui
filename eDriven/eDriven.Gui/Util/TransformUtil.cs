using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// Transform util
    /// </summary>
    public class TransformUtil
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// Fetches top transforms (having no parent transform)
        /// </summary>
        /// <returns></returns>
        public static List<Transform> FetchTopTransforms()
        {
            GameObject[] arr = (GameObject[]) Object.FindObjectsOfType(typeof (GameObject));
            //(GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));

            List<GameObject> gameObjects = new List<GameObject>(arr);

            gameObjects.RemoveAll(delegate(GameObject gameObject)
            {
                // remove all non-root game objects
                return gameObject.transform.root != gameObject.transform;
            });

            #region _log

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

            #endregion

            /**
             * 2. Get transforms
             * */
            return GetTransforms(gameObjects);
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
    }
}