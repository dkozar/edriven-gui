using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    /// <summary>
    /// </summary>
// ReSharper disable UnusedMember.Global
    public class SkinnedItemRendererFactory : IFactory
// ReSharper restore UnusedMember.Global
    {
        /// <summary>
        /// Additional entry point for a skin class
        /// </summary>
        private readonly Type _componentType;

        /// <summary>
        /// Additional entry point for a skin class
        /// </summary>
        private readonly Type _skinClass;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="skinClass"></param>
        public SkinnedItemRendererFactory(Type componentType, Type skinClass)
        {
            _componentType = componentType;
            _skinClass = skinClass;
        }

        #region Implementation of IFactory

        /// <summary>
        /// Creates a new instance of the component and sets its skin
        /// </summary>
        /// <returns></returns>
        public object NewInstance()
        {
            if (null == _componentType)
            {
                throw new Exception("Component type not defined");
            }

            object obj = Activator.CreateInstance(_componentType);

            SkinnableComponent cmp = obj as SkinnableComponent;
            if (null == cmp)
            {
                throw new Exception("The instance is not a skinnable component");
            }

            if (null != _skinClass)
                cmp.SkinClass = _skinClass;
            
            return cmp;
        }

        #endregion
    }
}