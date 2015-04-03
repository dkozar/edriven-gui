using eDriven.Gui.Components;
using eDriven.Gui.Events;

namespace eDriven.Gui.DragDrop
{
    public class DragEvent : GuiEvent
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Dispatched when the component drag is started
        /// </summary>
        public const string DRAG_START = "dragStart";

        /// <summary>
        /// Dispatched when the component is dragged over another component
        /// </summary>
        public const string DRAG_ENTER = "dragEnter";

        /// <summary>
        /// Dispatched when the component is dragged out of current component
        /// </summary>
        public const string DRAG_OVER = "dragOver";

        /// <summary>
        /// Dispatched when the component is dragged out of current component
        /// </summary>
        public const string DRAG_EXIT = "dragExit";

        /// <summary>
        /// Dispatched when component is dropped
        /// </summary>
        public const string DRAG_DROP = "dragDrop";
        
        /// <summary>
        /// Dispatched when the component is dropped and drop is processed by developer
        /// </summary>
        public const string DRAG_COMPLETE = "dragComplete";

        // ReSharper restore InconsistentNaming

        public DragDropManager.Action Action;

        /// <summary>
        /// Not used for now
        /// </summary>
        public object DraggedItem;

        /// <summary>
        /// A reference to dragged component
        /// </summary>
        public Component DragInitiator;

        /// <summary>
        /// Data that should be dragged
        /// </summary>
        public DragSource DragSource;

        #region Constructor

        public DragEvent(string type) : base(type)
        {
        }

        public DragEvent(string type, object target) : base(type, target)
        {
        }

        #endregion

    }
}
