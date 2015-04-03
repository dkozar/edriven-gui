using System;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Indicates that a class is a style client adapter
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StyleClientAdapterAttribute : Attribute
    {
        /// <summary>
        /// Media query ID
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public StyleClientAdapterAttribute(Type type)
        {
            Type = type;
        }
    }
}