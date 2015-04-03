using eDriven.Gui.Components;

namespace eDriven.Gui.Containers
{
    public interface IChildManager
    {
        void AddChild(Component child);
        void RemoveChild(Component child);
        void RemoveAllChildren();
    }
}