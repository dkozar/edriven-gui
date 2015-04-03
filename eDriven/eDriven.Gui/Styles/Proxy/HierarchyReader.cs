using System;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Classic style proxy
    /// </summary>
    internal class HierarchyReader : IStyleReader
    {
        #region Implementation of IStyleReader

        /// <summary>
        /// Gets the component ID
        /// </summary>
        /// <returns></returns>
        public string GetId(object current)
        {
            return ((MonoBehaviour)current).gameObject.name;
        }

        /// <summary>
        /// Gets the component classname
        /// </summary>
        /// <returns></returns>
        public string GetClassname(object current)
        {
            return null;
        }

        ///// <summary>
        ///// Gets the parent based on supplied component
        ///// </summary>
        ///// <param name="?"></param>
        ///// <returns></returns>
        //public object GetParent(MonoBehaviour current)
        //{
        //    return current.transform.parent;
        //}

        /// <summary>
        /// Gets the parent based on supplied component
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public object GetParent(object current)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}