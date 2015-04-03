using eDriven.Gui.Components;

namespace eDriven.Gui.States
{
    /// <summary>
    /// State override
    /// </summary>
    public interface IOverride
    {
        /// <summary>
        /// Initializes the override
        /// </summary>
        void Initialize();

        /// <summary>
        /// Applies the override
        /// </summary>
        /// <param name="parent"></param>
        void Apply(Component parent);

        /// <summary>
        /// Removes the override
        /// </summary>
        /// <param name="parent"></param>
        void Remove(Component parent);
    }
}