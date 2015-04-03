using eDriven.Core.Events;

namespace eDriven.Gui.Components
{
    ///<summary>
    /// The ability of a component to be validated
    ///</summary>
    public interface IInvalidationManagerClient : IEventDispatcher, INestLevel
    {
        //string Name { get;  }

        bool Initialized { get; set; }

        //bool UpdateFlag { get; set; }

        /// <summary>
        /// A lifecycle method
        /// Validates all (properties, position, size and layout)
        /// </summary>
        //void ValidateNow();

        /// <summary>
        /// A lifecycle method
        /// Validates properties
        /// Top-down
        /// </summary>
        void ValidateProperties();

        /// <summary>
        /// Validates position
        /// A lifecycle method
        /// Top-down
        /// </summary>
        void ValidateTransform();

        /// <summary>
        /// Validates size layout
        /// A lifecycle method
        /// <b>Bottom-up</b>
        /// </summary>
        void ValidateSize(bool recursive);

        /// <summary>
        /// Validates layout
        /// A lifecycle method
        /// Top-down
        /// </summary>
        void ValidateDisplayList();

        /// <summary>
        /// Fires queued events
        /// </summary>
        void ProcessQueue();
    }
}