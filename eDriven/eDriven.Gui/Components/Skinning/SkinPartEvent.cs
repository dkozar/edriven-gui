namespace eDriven.Gui.Components
{
    public class SkinPartEvent : eDriven.Core.Events.Event
    {
        public const string PART_ADDED = "partAdded";
        public const string PART_REMOVED = "partRemoved";

        /// <summary>
        /// The skin part being added or removed
        /// </summary>
   
        public object Instance;

        /// <summary>
        /// The name of the skin part being added or removed
        /// </summary>
        public string partName;

        #region Constructors

        public SkinPartEvent(string type) : base(type)
        {
        }

        public SkinPartEvent(string type, object target) : base(type, target)
        {
        }

        public SkinPartEvent(string type, bool bubbles) : base(type, bubbles)
        {
        }

        public SkinPartEvent(string type, bool bubbles, bool cancelable) : base(type, bubbles, cancelable)
        {
        }

        public SkinPartEvent(string type, object target, bool bubbles, bool cancelable) : base(type, target, bubbles, cancelable)
        {
        }

        #endregion

    }
}