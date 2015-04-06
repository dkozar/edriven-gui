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
using eDriven.Gui.Components;
using eDriven.Gui.Designer.Adapters;
using eDriven.Gui.Managers;
using eDriven.Gui.Util;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;
using Object=UnityEngine.Object;

namespace eDriven.Gui.Designer
{
    /// <summary>
    /// A class responsible for various kinds of GUI lookups (queries)
    /// </summary>
    public static class GuiLookup
    {
        #region Get adapter

        /// <summary>
        /// Returns the component adapter atached to the specified transform
        /// </summary>
        /// <param name="transform">Transform to look for adapter</param>
        /// <returns></returns>
        public static ComponentAdapter GetAdapter(Transform transform)
        {
            //Debug.Log("GetAdapter for transform: " + transform);
            return transform.GetComponent<ComponentAdapter>();
        }

        /// <summary>
        /// Returns the component adapter atached to the specified game object
        /// </summary>
        /// <param name="gameObject">GameObject to look for adapter</param>
        /// <returns></returns>
        public static ComponentAdapter GetAdapter(GameObject gameObject)
        {
            ComponentAdapter componentAdapter =
                gameObject.GetComponent<ComponentAdapter>();

            return componentAdapter;
        }

        #endregion

        #region Find adapter (scene)

        /// <summary>
        /// Finds a ComponentAdapter globally. If such an adapter exists, returns it; if not, returns null<br/>
        /// This lookup is most often used for looking up component factories (of popup windows)
        /// </summary>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static ComponentAdapter FindAdapter(string id)
        {
            Object[] foundObjects = Object.FindObjectsOfType(typeof(ComponentAdapter));
            if (foundObjects.Length == 0)
                return null;

            foreach (Object o in foundObjects)
            {
                ComponentAdapter desc = o as ComponentAdapter;
                if (null != desc)
                {
                    if (desc.Id == id)
                        return desc;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a ComponentAdapter globally. If such an adapter exists, returns it; if not, returns null<br/>
        /// This lookup is most often used for looking up component factories (of popup windows)
        /// </summary>
        /// <typeparam name="T">Type of the adapter to find</typeparam>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static T FindAdapter<T>(string id) where T : ComponentAdapter
        {
            Object[] foundObjects = Object.FindObjectsOfType(typeof(T));
            if (foundObjects.Length == 0)
                return null;

            foreach (Object o in foundObjects)
            {
                ComponentAdapter desc = o as ComponentAdapter;
                if (null != desc)
                {
                    if (desc.Id == id)
                        return (T)desc;
                }
            }
            return null;
        }

        #endregion

        #region Find adapter (game object)

        /// <summary>
        /// Finds a gui element in children. If such an element exists, returns it; if not, returns null
        /// </summary>
        /// <param name="go">Game object on which to do the search</param>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static ComponentAdapter FindAdapter(GameObject go, string id) 
        {
            //Debug.Log("FindAdapter: " + go + ", " + id, go);
            
            ComponentAdapter adapter;

            ComponentAdapter a = go.GetComponent<ComponentAdapter>();
            if (null != a && a.Id == id)
            {
                adapter = a;
            }
            else
            {
                adapter = FindAdapterRecursive(go, id);    
            }

            return adapter;
        }

        private static ComponentAdapter FindAdapterRecursive(GameObject go, string id)
        {
            ComponentAdapter adapter = null;
            ComponentAdapter a = go.GetComponent<ComponentAdapter>();
            if (null != a && a.Id == id)
            {
                return a;
            }

            Transform transform = go.transform;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform childTransform = transform.GetChild(i);
                // Note: recursion
                a = FindAdapterRecursive(childTransform.gameObject, id);
                if (null != a && a.Id == id)
                {
                    adapter = a;
                    break;
                }
            }
            return adapter;
        }

        /// <summary>
        /// Finds a gui element in children. If such an element exists, returns it; if not, returns null
        /// </summary>
        /// <typeparam name="T">Type of the adapter to find</typeparam>
        /// <param name="go">Game object on which to do the search</param>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static T FindAdapter<T>(GameObject go, string id) where T : ComponentAdapter
        {
            ComponentAdapter adapter = go.GetComponent<ComponentAdapter>();

            if (adapter.Id == id && adapter.ComponentType == typeof(T))
                return adapter as T;
            
            int count = adapter.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform childTransform = adapter.transform.GetChild(i);
                // Note: recursion
                adapter = FindAdapter<T>(childTransform.gameObject, id);
                if (adapter.Id == id && adapter.ComponentType == typeof(T))
                    return adapter as T;
            }

            return adapter as T;
        }

        #endregion

        #region Find component

        /// <summary>
        /// Finds a gui element in children. If such an element exists, returns it; if not, returns null
        /// </summary>
        /// <param name="go">Game object on which to do the search</param>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static Component FindComponent(GameObject go, string id)
        {
            ComponentAdapter adapter = go.GetComponent<ComponentAdapter>();
            Component component = adapter.Component;

            if (adapter.Id == id)
                return component;

            GroupAdapter groupAdapter = adapter as GroupAdapter;

            if (null != groupAdapter)
            {
                int count = groupAdapter.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    Transform childTransform = groupAdapter.transform.GetChild(i);
                    adapter = childTransform.GetComponent<ComponentAdapter>();
                    if (null != adapter.Component && adapter.Component.Id == id)
                        break;
                }
            }

            return adapter.Component;
        }

        /// <summary>
        /// Finds a gui element in children. If such an element exists, returns it; if not, returns null
        /// </summary>
        /// <typeparam name="T">Type of the adapter to find</typeparam>
        /// <param name="go">Game object on which to do the search</param>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static T FindComponent<T>(GameObject go, string id) where T : Component
        {
            ComponentAdapter adapter = go.GetComponent<ComponentAdapter>();
            Component component = adapter.Component;

            if (adapter.Id == id && adapter.ComponentType == typeof(T))
                return component as T;

            GroupAdapter groupAdapter = adapter as GroupAdapter;

            if (null != groupAdapter)
            {
                int count = groupAdapter.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    Transform childTransform = groupAdapter.transform.GetChild(i);
                    adapter = childTransform.GetComponent<ComponentAdapter>();
                    if (adapter.ComponentType == typeof(T) && null != adapter.Component && adapter.Component.Id == id)
                        break;
                }
            }

            return adapter.Component as T;
        }

        #endregion

        #region Get component

        /// <summary>
        /// Gets a component from the adapter specified by ID
        /// </summary>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static Component GetComponent(string id)
        {
            ComponentAdapter adapter = FindAdapter(id);
            if (null == adapter)
                return null;

            return adapter.Component;
        }

        /// <summary>
        /// Gets a component from the adapter specified by ID
        /// </summary>
        /// <returns></returns>
        public static Component GetComponent(GameObject parentGameObject, string id)
        {
            ComponentAdapter adapter = GetAdapter(parentGameObject);
            if (null == adapter) {
                Debug.LogWarning("Cannot find component adapter on game object: " + parentGameObject);
                return null;
            }

            if (null == adapter.Component) {
                Debug.LogWarning("Component not instantiated. Adapter: " + adapter);
                return null;
            }

            Component parentComponent = adapter.Component;

            // TODO: create method ComponentManager.Instance.GetMultiple(id); and use it here!
            InteractiveComponent component = ComponentManager.Instance.Get(id);

            if (parentComponent.HasChild(component))
                return (Component) component;

            return null;
        }

        #endregion

        #region Produce

        /// <summary>
        /// Produces a component using the adapter/factory specified by ID
        /// </summary>
        /// <typeparam name="T">Adapter type</typeparam>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static Component Produce<T>(string id) where T : ComponentAdapter
        {
            var factory = FindAdapter<T>(id);
            if (null == factory)
            {
                Debug.LogError(string.Format(@"Couldn't find factory of type [{0}] with ID=""{1}"". Skipping.", typeof(T), id));
                return null;
            }
            var component = factory.Produce(true, false);
            return component;
        }

        /// <summary>
        /// Produces a component using the adapter/factory specified by ID
        /// </summary>
        /// <param name="id">Adapter ID</param>
        /// <param name="warn">Warn if not found</param>
        /// <returns></returns>
        public static Component Produce(string id, bool warn)
        {
            var factory = FindAdapter(id);
            if (null == factory)
            {
                if (warn)
                    Debug.LogWarning(string.Format(@"Produce: Couldn't find adapter with ID=""{0}"". Skipping.", id));
                return null;
            }
            var component = factory.Produce(true, false);
            return component;
        }

        /// <summary>
        /// Produces a component using the adapter/factory specified by ID
        /// </summary>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static Component Produce(string id)
        {
            return Produce(id, true);
        }

        /// <summary>
        /// Produces a component using the adapter/factory specified by ID
        /// </summary>
        /// <param name="go"></param>
        /// <param name="id">Adapter ID</param>
        /// <param name="warn">Warn if not found</param>
        /// <returns></returns>
        public static Component Produce(GameObject go, string id, bool warn)
        {
            var factory = FindAdapter(go, id);
            if (null == factory)
            {
                if (warn)
                    Debug.LogWarning(string.Format(@"Produce: {0} -> Couldn't find adapter with ID=""{1}"". Skipping.", go, id), go);
                return null;
            }
            var component = factory.Produce(true, false);
            return component;
        }

        /// <summary>
        /// Produces a component using the adapter/factory specified by ID
        /// </summary>
        /// <param name="go"></param>
        /// <param name="id">Adapter ID</param>
        /// <returns></returns>
        public static Component Produce(GameObject go, string id)
        {
            return Produce(go, id, true);
        }

        #endregion

        #region Parent

        /// <summary>
        /// Finds a parent of the component where the parent type is being T (bottom-up)<br/>
        /// Returns null if such a parent not found
        /// </summary>
        /// <typeparam name="T">Parent type</typeparam>
        /// <param name="component">Component</param>
        /// <returns></returns>
        public static T FindParent<T>(Component component) where T : Component
        {
            List<DisplayListMember> chain = ComponentUtil.GetParentChain(component, false);
            foreach (DisplayListMember c in chain)
            {
                if (c.GetType() == typeof(T) && c is T)
                    return (T) c;
            }
            return null;
        }

        #endregion

        #region Game object

        /// <summary>
        /// Gets the parent game object to which a component adapter is attached to<br/>
        /// If this component is not being attached directly, but as a part of the container,<br/>
        /// the container registered in component adapter collection is returned
        /// </summary>
        /// <param name="component">Component to get the game object for</param>
        /// <returns></returns>
        public static GameObject GetGameObject(Component component)
        {
            if (null == component)
                return null;

            GameObject gameObject = null;
            if (ComponentAdapter.Registry.ContainsKey(component))
            {
                gameObject = ComponentAdapter.Registry[component];
            }
            else
            {
                if (null != component.Parent)
                {
                    return GetGameObject(component.Parent as Component);
                }
            }

            if (null != gameObject) {
                return gameObject;
            }

            return null;
        }

        #endregion

        #region Path

        /// <summary>
        /// Returns the list of transforms specifying the component path
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static List<Transform> GetPath(Transform transform)
        {
            List<Transform> list = new List<Transform>();

            while (null != transform)
            {
                list.Add(transform);
                transform = transform.parent;
            }

            list.Reverse();

            return list;
        }

        /// <summary>
        /// Utility method for getting the string presentation of component path
        /// </summary>
        /// <param name="transform">Transform the component is attached to</param>
        /// <param name="delimiter">Delimiter used when building the string</param>
        /// <returns></returns>
        public static string PathToString(Transform transform, string delimiter)
        {
            List<Transform> list = GetPath(transform);
            if (string.IsNullOrEmpty(delimiter))
                delimiter = ".";

            string name = "";
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                name += list[i].gameObject.name;
                if (i < count - 1)
                    name += delimiter;
            }

            return name;
        }

        #endregion
    }
}