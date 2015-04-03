using System;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// Representing the event dispatched by the component
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class EventAttribute : Attribute
    {
        /// <summary>
        /// Event name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Event type
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Set this to true if event bubbles
        /// </summary>
        public bool Bubbles { get; set; }

        public override string ToString()
        {
            //return string.Format(@"Name: ""{0}"", Type: {1}, Bubbles: {2}", Name, Type, Bubbles);
            return string.Format(@"""{0}"" [Type: {1}, Bubbles: {2}]", Name, Type, Bubbles);
        }
    }
}