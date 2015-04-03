using eDriven.Core.Events;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using UnityEngine;
using Event=eDriven.Core.Events.Event;

namespace eDriven.Gui.Plugins
{
    public class DialogCloseOnEsc : IPlugin
    {
        public bool Enabled = true;

        private Dialog _dialog;

        private const EventPhase Phases = EventPhase.Capture | EventPhase.Target;

        private bool _initialized;

        public void Initialize(InvalidationManagerClient component)
        {
            if (_initialized)
                return;

            _initialized = true;
            //Debug.Log("DialogCloseOnEsc init");
            _dialog = (Dialog) component;
            _dialog.AddEventListener(KeyboardEvent.KEY_UP, KeyUpHandler, Phases);
        }

        public void Dispose()
        {
            _dialog.RemoveEventListener(KeyboardEvent.KEY_UP, KeyUpHandler, Phases);
        }

        private void KeyUpHandler(Event e)
        {
            KeyboardEvent ke = (KeyboardEvent)e;
            //Debug.Log("KeyUpHandler: " + ke.KeyCode);
            
            if (KeyCode.Escape == ke.KeyCode)
            {
                //Debug.Log("DialogCloseOnEsc->Escape");
                e.CancelAndStopPropagation();
                //PopupManager.Instance.RemovePopup(_dialog);
                if (Enabled)
                    _dialog.ExecCallback("cancel");
            }
        }
    }
}
