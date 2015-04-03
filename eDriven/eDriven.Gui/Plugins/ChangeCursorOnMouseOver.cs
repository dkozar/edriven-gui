using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Cursor;
using Component=eDriven.Gui.Components.Component;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    public class ChangeCursorOnMouseOver : IPlugin
    {
        private Component _component;

        #region Implementation of IPlugin

        private bool _initialized;

        public void Initialize(InvalidationManagerClient component)
        {
            if (_initialized)
                return;

            _initialized = true;

            _component = (Component) component;

            // subscribe to CAPTURE PHASE component events

            // listening for mouse move over the component border
            _component.AddEventListener(MouseEvent.MOUSE_OVER, MouseOverHandler, EventPhase.Capture | EventPhase.Target); // NOTE: Target phase must be present because of the simple components
            
            // listening for mouse out to remove the overlay
            _component.AddEventListener(MouseEvent.MOUSE_OUT, MouseOutHandler, EventPhase.Capture | EventPhase.Target);
        }

        #endregion

        /// <summary>
        /// Mouse-over cursor
        /// </summary>
        public string MouseOverCursor = CursorType.Pointer;

        #region Implementation of IDisposable

        private int _cursorId;

        private void MouseOverHandler(Event e)
        {
            _cursorId = CursorManager.Instance.SetCursor(MouseOverCursor, CursorPriority.Low);
        }

        private void MouseOutHandler(Event e)
        {
            //CursorManager.Instance.RemoveAllCursors();
            CursorManager.Instance.RemoveCursor(_cursorId);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (null != _component)
            {
                _component.RemoveEventListener(MouseEvent.MOUSE_OVER, MouseOverHandler, EventPhase.Capture | EventPhase.Target);
                _component.RemoveEventListener(MouseEvent.MOUSE_OUT, MouseOutHandler, EventPhase.Capture | EventPhase.Target);
            }
        }
        
        #endregion
    }
}