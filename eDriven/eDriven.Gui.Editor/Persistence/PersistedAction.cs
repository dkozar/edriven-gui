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
using eDriven.Gui.Designer.Adapters;
using UnityEditor;
using UnityEngine;

namespace eDriven.Gui.Editor.Persistence
{
    /// <summary>
    /// Class used for persisting component's:
    /// 1. Type
    /// 2. Parent
    /// 3. Name
    /// Used when adding new components to the hierarchy or moving the existing ones
    /// 
    /// This class is constructed when about to stop the play mode
    /// All the properties are being read and saved
    /// 
    /// After the play mode is stopped, this action is re-played
    /// 
    /// Note: we are saving IDs rather than objects (ComponentInstanceId, TransformId, ParentTransformId)
    /// That's because during the play mode stop newly created transforms are removed, so we
    /// need to recreate them and they became new objects.
    /// 
    /// However, IDs recorded by this class don't change. That's because we are re-registering new transforms/adapters
    /// using the old IDs, so that PersistanceManager could find them and reapply saved values
    /// </summary>
    internal class PersistedAction : PersistedActionBase
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
        /// The type of the created component
        /// </summary>
        private readonly Type _componentType;

        /// <summary>
        /// Instance ID of the component
        /// </summary>
        private readonly int _componentInstanceId;

        /// <summary>
        /// Transform ID
        /// </summary>
        private readonly int _transformId;

        /// <summary>
        /// Parent transform ID
        /// </summary>
        private readonly int _parentTransformId;

        /// <summary>
        /// Name
        /// </summary>
        private readonly string _name;
        
        public PersistedAction(Component component)
        {
            if (null == component) 
                throw new Exception("Component is null");

            _componentInstanceId = component.GetInstanceID();
            _componentType = component.GetType();
            _transformId = component.transform.GetInstanceID();
            _parentTransformId = component.transform.parent.GetInstanceID();
            _name = component.gameObject.name;
        }

        public override string ToString()
        {
            return string.Format("ComponentInstanceId: [{0}]; TransformId: [{1}]; ParentTransformId: [{2}]; ComponentType: [{3}], Name: [{4}]",
                                 _componentInstanceId,
                                 _transformId,
                                 _parentTransformId,
                                 _componentType,
                                 _name);
        }

        /// <summary>
        /// Important: there is a false logic in recognizing the "old" adapters by 
        /// positive instance ID and "new" adapters by negative instance ID!!!!!
        /// That is because all the components instantiated in this Unity session (since UnityEditor
        /// is turned on) have the negative ID.
        /// </summary>
        internal override void Apply()
        {
            Transform parent = TransformRegistry.Instance.Get(_parentTransformId, true);
            if (null == parent)
            {
                throw new Exception(string.Format("Cannot locate parent transform [{0}]: ", _parentTransformId));
            }

            Transform transform;

            // we need to differentiate between old and new objects
            Component unityComponent = (ComponentAdapter)EditorUtility.InstanceIDToObject(_componentInstanceId);
            bool isAddition = null == unityComponent;
            
            /**
             * Note: it would be false to check if this is a new component by using "if (ComponentInstanceId less then 0)"
             * (see the explanation above)
             * */
            if (isAddition)
            {
                /**
                * 1.a. New adapter
                * Creating a new game object, giving it a name of component type
                * */
                GameObject go = new GameObject(_name); // /*_componentType.Name.Replace("Adapter", string.Empty)*/
                transform = go.transform;

                // creates a new adaptrer of a recorded type
                ComponentAdapter adapter = (ComponentAdapter)go.AddComponent(_componentType);

                #region Logging

#if DEBUG
                if (DebugMode)
                {
                    Debug.Log("Rerouting " + _componentInstanceId + " to " + adapter.GetInstanceID());
                }
#endif

                #endregion

                /**
                 * 1.b. Re-routing to a new transform/adapter   
                 * */
                ComponentRegistry.Instance.Register(_componentInstanceId, adapter);
                TransformRegistry.Instance.Register(_transformId, transform);
            }
            else
            {
                /**
                * 2. Old adapter
                * */
                ComponentAdapter component = (ComponentAdapter) unityComponent; //EditorUtility.InstanceIDToObject(ComponentInstanceId); // GuiLookup.GetObjectWithId(Id); // component ID
                Debug.Log("component: " + component);
                transform = component.transform;
            }

            /**
             * 3. Set the parent
             * */
            transform.parent = parent;
        }
    }
}