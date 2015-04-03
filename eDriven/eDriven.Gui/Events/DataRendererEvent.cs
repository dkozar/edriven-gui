namespace eDriven.Gui.Events
{
    /// <summary>
    /// The event that has old and new value
    /// </summary>
    public class DataRendererEvent : GuiEvent
    {
        // ReSharper disable InconsistentNaming
        public const string EVENT_REMOVE = "remove";
        public const string EVENT_CLEAR = "clear";
        public const string EVENT_EDIT = "edit";
        // ReSharper restore InconsistentNaming

        public object Data;
        public string DataField;
        public object OldValue;
        public object NewValue;

        public int ColumnIndex = -1;
        public int RowIndex = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Event type</param>
        public DataRendererEvent(string type)
            : base(type)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Event type</param>
        /// <param name="target">Event target</param>
        public DataRendererEvent(string type, object target)
            : base(type, target)
        {
        }

        public override string ToString()
        {
            return string.Format(@"DataRendererEvent: 
Item: {0}
Property: {1}
OldValue: {2}
NewValue: {3}
ColumnIndex: {4}
RowIndex: {5}", Data, DataField, OldValue, NewValue, ColumnIndex, RowIndex);
        }
    }
}