using System;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// The default event
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DefaultEvent : System.Attribute
    {
        /// <summary>
        /// Event name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The default event
        /// </summary>
        /// <param name="name"></param>
        public DefaultEvent(string name)
        {
            Name = name;
        }
    }
}