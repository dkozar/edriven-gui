//using System;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;
//using Object=UnityEngine.Object;

//namespace eDriven.Gui.Editor.Persistence
//{
//    /// <summary>
//    /// Class representing the hierarchy change action
//    /// Used when adding new components to the hierarchy or changing the existing ones
//    /// When adapter added through designer interface, this action is also created and saved within PersistenceManager
//    /// After the play mode is stopped, this action is re-played, but depending of Id (InstanceID):
//    /// 1. If Id >= 0, referencing the existing component using GuiLookup.GetObjectWithId
//    /// 2. If Id < 0, creating a new adapter of type ComponentType
//    /// </summary>
//    internal class PersistedReorder : PersistedActionBase
//    {
//        //public Transform ParentTransform;
//        //public Transform Transform;

//        //public int AdapterId;
//        public int TransformId;
        
//        public List<List<int>> ChildNodeOrder;

//        public PersistedReorder(Transform transform, Object adapter)
//        {
//            //Debug.Log(transform);
//            //Transform = transform;
//            //if (null != adapter)
//            //{
//            //    // 20130318
//            //    AdapterId = adapter.GetInstanceID();
//            //}

//            Debug.Log("Creating reorder for " + transform); //transform.GetComponent<ContainerAdapter>());

//            //ParentTransform = transform.parent;
//            TransformId = transform.GetInstanceID();
//            //Debug.Log("* TransformId: " + TransformId);
//        }

//        public override string ToString()
//        {
//            return string.Format("Id: [{0}];", TransformId);
//        }

//        internal override void Apply()
//        {
//            return; // TEMP! TODO

//            //Debug.Log("ParentTransformId: " + ParentTransformId);

//            Transform thisTransform;

//            Dictionaries.OldTransformInstanceIdToObjectDictionary.TryGetValue(TransformId, out thisTransform);

//            if (null == thisTransform)
//            {
//                thisTransform = (Transform) EditorUtility.InstanceIDToObject(TransformId);
//            }

//            //Debug.Log("parent: " + parent);

//            if (null == thisTransform)
//                throw new Exception("Cannot find transform with ID=" + TransformId);

//            ComponentAdapter componentAdapter = thisTransform.GetComponent<ComponentAdapter>();
//            ContainerAdapter containerAdapter = componentAdapter as ContainerAdapter;
//            if (null == containerAdapter)
//                return;

//            var processedAdapters = new List<ComponentAdapter>();

//            List<List<ComponentAdapter>> outerAdapters = new List<List<ComponentAdapter>>();

//            foreach (List<int> outer in ChildNodeOrder)
//            {
//                List<ComponentAdapter> innerAdapters = new List<ComponentAdapter>();
//                foreach (int id in outer)
//                {
//                    var adapter = GetChildTransformWithId(thisTransform, id);
//                    Debug.Log(string.Format("Adapter retrieved [{0}] [{1}]", id, null == adapter ? "-" : adapter.ToString()));
//                    if (null != adapter) {
//                        innerAdapters.Add(adapter);
//                        processedAdapters.Add(adapter);
//                    }
//                }
//                outerAdapters.Add(innerAdapters);
//            }

//            if (outerAdapters.Count > 0)
//            {
//                // find orphans
//                foreach (Transform childTransform in thisTransform)
//                {
//                    ComponentAdapter adapter = childTransform.GetComponent<ComponentAdapter>() as ComponentAdapter;
//                    if (null != adapter && !processedAdapters.Contains(adapter))
//                    {
//                        Debug.Log("Adding orphan adapter: " + adapter);
//                        outerAdapters[0].Add(adapter);
//                    }
//                }
//            }

//            containerAdapter.SetOrder(outerAdapters);
//        }

//        private static ComponentAdapter GetChildTransformWithId(Transform transform, int guid)
//        {
//            foreach (Transform childTransform in transform)
//            {
//                if (childTransform.GetInstanceID() == guid)
//                {
//                    ComponentAdapter ca = childTransform.GetComponent<ComponentAdapter>();
//                    if (null != ca/* && ca.Guid == guid*/)
//                    {
//                        return ca;
//                    }
//                }

               
//            }
//            return null;
//        }
//    }
//}