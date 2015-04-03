using System.Collections.Generic;
using eDriven.Core.Geom;
using eDriven.Core.Reflection;
using eDriven.Gui.Layout;

namespace eDriven.Gui.Components
{
    public partial class SkinnableComponent
    {
        // Private cache of instantiated dynamic parts. This is accessed through
        // the numDynamicParts() and getDynamicPartAt() methods.
        private Dictionary<string, List<object>> _dynamicPartsCache;
    
        /**
         *  Create an instance of a dynamic skin part. 
         *  Dynamic skin parts should always be instantiated by this method, 
         *  rather than directly by calling the <code>newInstance()</code> method on the factory.
         *  This method creates the part, but does not add it to the display list.
         *  The component must call the <code>Group.addElement()</code> method, or the appropriate 
         *  method to add the skin part to the display list. 
         *
         *  Param: partName The name of the part.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        protected object CreateDynamicPartInstance(string partName)
        {
            IFactory factory = (IFactory) CoreReflector.GetValue(Skin, partName);
            
            if (null != factory)
            {
                DisplayObject instance = (DisplayObject)factory.NewInstance();
            
                // Add to the dynamic parts cache
                if (null == _dynamicPartsCache)
                    _dynamicPartsCache = new Dictionary<string, List<object>>();
                
                if (!_dynamicPartsCache.ContainsKey(partName))
                    _dynamicPartsCache[partName] = new List<object>();
            
                _dynamicPartsCache[partName].Add(instance);
            
                // Send notification
                PartAdded(partName, instance);
            
                return instance;
            }
        
            return null;
        }

        /**
         *  Remove an instance of a dynamic part. 
         *  You must call this method  before a dynamic part is deleted.
         *  This method does not remove the part from the display list.
         *
         *  Param: partname The name of the part.
         *
         *  Param: instance The instance of the part.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="instance"></param>
        protected void RemoveDynamicPartInstance(string partName, object instance)
        {
            // Send notification
            PartRemoved(partName, instance);
        
            // Remove from the dynamic parts cache
            var cache = _dynamicPartsCache[partName];
            cache.Remove(instance);
        }

        /**
         *  Returns the number of instances of a dynamic part.
         *
         *  Param: partName The name of the dynamic part.
         *
         *  Returns: The number of instances of the dynamic part.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        protected int NumDynamicParts(string partName)
        {
            if (null != _dynamicPartsCache && _dynamicPartsCache.ContainsKey(partName))
                return _dynamicPartsCache[partName].Count;
        
            return 0;
        }

        /**
         *  Returns a specific instance of a dynamic part.
         *
         *  Param: partName The name of the dynamic part.
         *
         *  Param: index The index of the dynamic part.
         *
         *  Returns: The instance of the part, or null if it the part does not exist.
         */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="partName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected object GetDynamicPartAt(string partName, int index)
        {
            if (null != _dynamicPartsCache && _dynamicPartsCache.ContainsKey(partName))
                return _dynamicPartsCache[partName][index];
        
            return null;
        }

        //---------------------------------
        //  Utility methods for subclasses
        //---------------------------------

        /**
         * 
         * 
         * Utility method to calculate a skin part's position relative to our component.
         *
         * Param: part The skin part instance to obtain coordinates of.
         *
         * Returns: The component relative position of the part.
         */ 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        protected Point GetSkinPartPosition(IVisualElement part)
        {
            return (null == part || null == part.Parent) ? new Point(0, 0) :
                GlobalToLocal(part.Parent.LocalToGlobal(new Point(LayoutUtil.GetLayoutBoundsX((InvalidationManagerClient)part), LayoutUtil.GetLayoutBoundsY((InvalidationManagerClient)part))));
        }
    }
}