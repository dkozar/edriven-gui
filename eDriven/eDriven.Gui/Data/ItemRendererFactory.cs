using System;
using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    ///<summary>
    ///</summary>
    ///<typeparam name="T"></typeparam>
// ReSharper disable UnusedMember.Global
    public class ItemRendererFactory<T> : IFactory
// ReSharper restore UnusedMember.Global
    {
        #region Implementation of IFactory

        public object NewInstance()
        {
            return Activator.CreateInstance(typeof(T));
        }

        #endregion
    }
}