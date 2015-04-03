using System;
using System.Runtime.Serialization;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// The exception that can be thrown by the Container
    /// </summary>
    public class DisplayObjectContainerException : Exception
    {
        public const string ChildIsNull = "Child is null";
        public const string IndexOutOfRange = "Child index out of range";
        
        public DisplayObjectContainerException()
        {
        }

        public DisplayObjectContainerException(string message)
            : base(message)
        {
        }

        public DisplayObjectContainerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DisplayObjectContainerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}