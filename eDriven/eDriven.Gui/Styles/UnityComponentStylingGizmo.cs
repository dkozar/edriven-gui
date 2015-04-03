using System.Collections.Generic;
using eDriven.Core.Managers;
using eDriven.Core.Util;
using UnityEngine;
using Event = eDriven.Core.Events.Event;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Shows or hides styling gizmos for supplied components
    /// </summary>
    public class UnityComponentStylingGizmo
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
// ReSharper disable once CSharpWarnings::CS1591
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        /// <summary>
        /// The time gizmos will be visible on screen before dissapearing
        /// </summary>
        public static float Duration = 3;

        private static Timer _gizmoTimer;
        private static List<Component> _components;

        /// <summary>
        /// Processes components and draws gizmos
        /// </summary>
        /// <param name="components"></param>
        public static void Show(List<Component> components)
        {
            _components = components;

            SystemManager.Instance.GizmoSignal.Connect(GizmosSlot);

            if (null == _gizmoTimer)
            {
                _gizmoTimer = new Timer(Duration, 1); // 3 seconds, single time
            }
            else
            {
                _gizmoTimer.Delay = Duration;
            }
            
            _gizmoTimer.Reset();
            _gizmoTimer.Start();
            _gizmoTimer.AddEventListener(Timer.COMPLETE, OnGizmoTimerComplete);
        }

        /// <summary>
        /// Hides all gizmos
        /// </summary>
        public static void Hide()
        {
            SystemManager.Instance.GizmoSignal.Disconnect(GizmosSlot);
            _gizmoTimer.RemoveEventListener(Timer.COMPLETE, OnGizmoTimerComplete);
        }

        private static void OnGizmoTimerComplete(Event e)
        {
            Hide();
        }

        private static void GizmosSlot(object[] parameters)
        {
            foreach (Component component in _components)
            {
                Gizmos.DrawIcon(component.transform.position, "gizmo_style.png", false); // do not scale the gizmo
            }
        }
    }
}
