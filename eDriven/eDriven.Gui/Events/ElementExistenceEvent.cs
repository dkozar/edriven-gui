using eDriven.Core.Events;
using eDriven.Gui.Components;

namespace eDriven.Gui.Events
{
    public class ElementExistenceEvent : Event
    {
        public const string ELEMENT_ADD = "elementAdd";

        public const string ELEMENT_REMOVE = "elementRemove";

        public int Index;

        //----------------------------------
        //  element
        //----------------------------------

        public IVisualElement Element;

        public ElementExistenceEvent(string type) : base(type)
        {
            
        }
    }
}
