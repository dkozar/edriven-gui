using System;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Core.Events;
using eDriven.Core.Geom;
using UnityEngine;
using Event=UnityEngine.Event;

namespace eDriven.Gui.Events
{
    public class MouseEventHelper
    {
        internal static void BuildAndDispatchMouseEvent(EventDispatcher dispatcher, DisplayListMember targetComponent, string type, Point position, Event unityEvent)
        {
            //Debug.Log("BuildAndDispatchMouseEvent");

            if (null == dispatcher)
                throw new Exception("dispatcher cannot be null");

            if (null == targetComponent)
                throw new Exception("targetComponent cannot be null");

            if (!dispatcher.HasEventListener(type) && !targetComponent.HasBubblingEventListener(type)) // optimization
                return; // don't bother to build an event

            //Debug.Log("unityEvent: " + unityEvent);
            //var ue = unityEvent ?? Event.current;
            //Debug.Log("ue: " + ue);
            //Debug.Log("ue.button: " + ue.button);

            /**
             * 1) InvalidateDrawingList the event
             * */
            MouseEvent me = new MouseEvent(type)
                                {
                                    Target = targetComponent,
                                    CurrentEvent = unityEvent,
                                    GlobalPosition = position,
                                    LocalPosition = targetComponent.GlobalToLocal(position)
                                };

            if (null != MouseProcessor.MouseDownEvent)
            {
                // this is not a mouse move event, but rather a mouse drag, because MouseProcessor holds the reference to a mousedown event
                me.ButtonDown = MouseProcessor.MouseDownEvent.button == 0;
                me.RightButtonDown = MouseProcessor.MouseDownEvent.button == 1;
                me.MiddleButtonDown = MouseProcessor.MouseDownEvent.button == 2;
            }

            /**
             * 2) Dispatch from manager
             * */
            dispatcher.DispatchEvent(me);

            /**
             * 3) If not canceled, dispatch from component
             * */
            if (!me.Canceled)
            {
                me.Bubbles = true; // added 28.1.2012.
                targetComponent.DispatchEvent(me);
            }
        }
    }
}