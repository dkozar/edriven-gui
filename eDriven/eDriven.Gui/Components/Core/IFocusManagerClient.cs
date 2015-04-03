using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    public interface IFocusManagerClient
    {
        void FocusInHandler(Event e);
        void FocusOutHandler(Event e);
    }
}