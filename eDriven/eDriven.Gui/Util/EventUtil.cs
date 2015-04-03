/*using UnityEngine;

namespace eDriven.Gui.Util
{
    public class EventUtil
    {
        public static bool IsDrawingEvent(Event e)
        {
            if (null == Event.current)
                return false;

            EventType type = Event.current.type;
            
            return type == EventType.repaint ||
                   type == EventType.mouseDown ||
                   type == EventType.mouseUp ||
                   //type == EventType.mouseMove ||
                   type == EventType.scrollWheel ||
                   type == EventType.keyDown ||
                   type == EventType.keyUp ||
                   type == EventType.mouseDrag;

            //return true;
        }

        public static bool IsKeyReleased(KeyCode keyCode)
        {
            Event e = Event.current;

            if (null == e)
                return false;

            return e.rawType == EventType.keyUp && 
                   e.isKey &&
                   e.keyCode == keyCode;
        }
    }
}*/