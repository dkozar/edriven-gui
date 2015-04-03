using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    public class FrameworkEvent : Event
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Dispatched when component ready to have children ^_^
        /// </summary>
        public const string PREINITIALIZE = "preinitialize";

        /// <summary>
        /// Dispatched when the children are born and set up
        /// </summary>
        public const string INITIALIZE = "initialize";

        /// <summary>
        /// The event that fires just before the first show and the creation complete
        /// The component has already been measured at this stage, sou you can use measured data for tweening etc.
        /// </summary>
        //public const string PRE_CREATION_COMPLETE = "PRE_CREATION_COMPLETE";

        /// <summary>
        /// Dispatched after the first layout pass (properties, size, position display list)
        /// </summary>
        public const string CREATION_COMPLETE = "creationComplete";

        public const string UPDATE_COMPLETE = "updateComplete";

        public const string FIRST_SHOW = "firstShow";
        public const string SHOW = "show";
        public const string HIDE = "hide";

        public const string SHOWING = "showing";
        public const string HIDING = "hiding";

        public const string ADDING = "adding";

        public const string ADD = "add";
        public const string REMOVE = "remove";

        public const string X_CHANGED = "xChanged";
        public const string Y_CHANGED = "yChanged";

        public const string WIDTH_CHANGED = "widthChanged";
        public const string HEIGHT_CHANGED = "heightChanged";
        public const string EXPLICIT_WIDTH_CHANGED = "explicitWidthChanged";
        public const string EXPLICIT_HEIGHT_CHANGED = "explicitHeightChanged";
        public const string MIN_WIDTH_CHANGED = "minWidthChanged";
        public const string MIN_HEIGHT_CHANGED = "minHeightChanged";
        public const string EXPLICIT_MIN_WIDTH_CHANGED = "explicitMinWidthChanged";
        public const string EXPLICIT_MIN_HEIGHT_CHANGED = "explicitMinHeightChanged";
        public const string MAX_WIDTH_CHANGED = "maxWidthChanged";
        public const string MAX_HEIGHT_CHANGED = "maxHeightChanged";
        public const string EXPLICIT_MAX_WIDTH_CHANGED = "explicitMaxWidthChanged";
        public const string EXPLICIT_MAX_HEIGHT_CHANGED = "explicitMaxHeightChanged";

        public const string LAYOUT_CHANGED = "layoutChanged";

        public const string VALUE_COMMIT = "valueCommit";

        public const string CHANGE_START = "changeStart";
        public const string CHANGE_END = "changeEnd";
    
        public const string DATA_CHANGE = "dataChange";

        public const string ENTER_STATE = "enterState";

        public const string EXIT_STATE = "exitState";

        public const string ADDED_TO_STAGE = "addedToStage";
        public const string REMOVED_FROM_STAGE = "removedFromStage";

// ReSharper restore InconsistentNaming

        public FrameworkEvent(string type) : base(type)
        {
        }

        public FrameworkEvent(string type, object target) : base(type, target)
        {
        }

        public FrameworkEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public FrameworkEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }
    }
}
