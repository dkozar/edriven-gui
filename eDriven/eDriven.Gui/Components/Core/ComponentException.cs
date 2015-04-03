using System;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// The exception that can be thrown by Component
    /// </summary>
    public class ComponentException : Exception
    {
#pragma warning disable 1591
        public static string FontRequiredToCalculateSize = "Font required to calculate size";
        public static string CannotChangeUid = @"Component Uid can be set only once [Existing Uid=""{0}"", Desired Uid=""{1}""]";
        public static string CannotChangeId = @"Component Id can be set only once [Existing Id=""{0}"", Desired Id=""{1}""]";
#pragma warning restore 1591
        
#pragma warning disable 1591
        public ComponentException()
#pragma warning restore 1591
        {

        }

        /// <summary>
        /// Constructor
        ///</summary>
        ///<param name="message"></param>
        public ComponentException(string message)
            : base(message)
        {

        }
    }
}