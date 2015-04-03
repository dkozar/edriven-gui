using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TComponent, TSkin"></typeparam>
    /// <typeparam name="TComponent"></typeparam>
    /// <typeparam name="TSkin"></typeparam>
// ReSharper disable UnusedMember.Global
    public class SkinnedItemRendererFactory<TComponent, TSkin> : IFactory where TComponent : SkinnableComponent
// ReSharper restore UnusedMember.Global
    {
        #region Implementation of IFactory

        /// <summary>
        /// Creates a new instance of the component and sets its skin
        /// </summary>
        /// <returns></returns>
        public object NewInstance()
        {
            TComponent cmp = (TComponent) Activator.CreateInstance(typeof(TComponent));
            cmp.SkinClass = typeof (TSkin);
            return cmp;
        }

        /// <summary>
        /// Additional entry point for a skin class
        /// </summary>
        //public Type SkinClass;

        #endregion
    }
}