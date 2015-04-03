using System.Collections.Generic;
using eDriven.Core.Data.Collections;

namespace Assets.eDriven.Demo.Helpers
{
    public static class EasingHelper
    {
        private static List<object> _list;
        
        public static List<object> GetEasingList()
        {
            if (null == _list) // go lazy here
            {
                _list = new List<object>
                            {
                                new ListItem("Back", "Back"),
                                new ListItem("Bounce", "Bounce"),
                                new ListItem("Circ", "Circ"),
                                new ListItem("Cubic", "Cubic"),
                                new ListItem("Elastic", "Elastic"),
                                new ListItem("Expo", "Expo"),
                                new ListItem("Linear", "Linear"),
                                new ListItem("Quad", "Quad"),
                                new ListItem("Quart", "Quart"),
                                new ListItem("Quint", "Quint"),
                                new ListItem("Sine", "Sine")
                            };
            }

            return _list;
        }

        private static List<object> _list2;
        
        public static List<object> GetEasingInOutList()
        {
            if (null == _list2) // go lazy here
            {
                _list2 = new List<object>
                             {
                                 new ListItem("EaseIn", "EaseIn"),
                                 new ListItem("EaseOut", "EaseOut"),
                                 new ListItem("EaseInOut", "EaseInOut")
                             };
            }

            return _list2;
        }
    }
}