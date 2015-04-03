using System;

namespace eDriven.Gui.Managers
{
    public class FocusManagerException : Exception
    {

        public const string ComponentNotInFocus = "Component not in focus, so cannot be blurred";
        
        public FocusManagerException()
        {

        }

        public FocusManagerException(string message)
            : base(message)
        {

        }
    }
}