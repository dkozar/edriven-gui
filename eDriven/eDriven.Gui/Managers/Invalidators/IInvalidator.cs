using eDriven.Gui.Components;

namespace eDriven.Gui.Managers.Invalidators
{
    internal interface IInvalidator
    {
        bool Invalid { get; }

        void Invalidate(InvalidationManagerClient obj, bool invalidateClientFlag);

        void Validate(ref InvalidationManagerClient currentObject);

        void ValidateClient(InvalidationManagerClient target, ref InvalidationManagerClient currentObject);
    }
}