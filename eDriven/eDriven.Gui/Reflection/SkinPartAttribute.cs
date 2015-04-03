using System;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// Marks the child component as a skin part
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SkinPartAttribute : Attribute
    {
        private string _id;
        /// <summary>
        /// Skin part ID
        /// </summary>
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        private bool _required;
        /// <summary>
        /// Is the skin part required or optional
        /// </summary>
        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                _required = value;
            }
        }
    }
}