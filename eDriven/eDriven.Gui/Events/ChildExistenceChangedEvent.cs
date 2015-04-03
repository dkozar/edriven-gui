using eDriven.Gui.Components;

namespace eDriven.Gui.Events
{
    public class ChildExistenceChangedEvent : Core.Events.Event
    {
        public const string CHILD_ADDING = "childAdding";
        public const string CHILD_ADD = "childAdded";
        public const string CHILD_REMOVING = "childRemoving";
        public const string CHILD_REMOVE = "childRemoved";
        /**
	     *  Reference to the child object that was created or destroyed.
	     */
	    public DisplayListMember RelatedObject;

        public ChildExistenceChangedEvent(string type)
            : base(type)
        {

        }

        public ChildExistenceChangedEvent(string type, object target) : base(type, target)
        {
        }
    }
}
