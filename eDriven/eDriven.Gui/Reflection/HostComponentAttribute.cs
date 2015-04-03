using System;

namespace eDriven.Gui.Reflection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HostComponentAttribute : System.Attribute
    {
        private Type _type;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public HostComponentAttribute(Type type)
        {
            _type = type;
        }

        public Type Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }
    }
}