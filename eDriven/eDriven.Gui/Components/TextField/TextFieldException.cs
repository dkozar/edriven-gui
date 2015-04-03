using System;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// The exception that can be thrown by Component
    /// </summary>
    public class TextFieldException : Exception
    {
#pragma warning disable 1591
        public static string MultilineAndPasswordModeCantCoexist = "Multiline and password mode cannot coexist";
#pragma warning restore 1591

#pragma warning disable 1591
        public TextFieldException()
#pragma warning restore 1591
        {

        }

        /// <summary>
        /// Constructor
        ///</summary>
        ///<param name="message"></param>
        public TextFieldException(string message)
            : base(message)
        {

        }
    }
}