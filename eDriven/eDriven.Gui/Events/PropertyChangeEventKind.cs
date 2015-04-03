namespace eDriven.Gui.Events
{
    /// <summary>
    /// The kind of property change
    /// </summary>
    public class PropertyChangeEventKind
    {
        public const string UPDATE = "update";

        /**
		 *  Indicates that the property was deleted from the object.
		 */
        public const string DELETE = "delete";
    }
}