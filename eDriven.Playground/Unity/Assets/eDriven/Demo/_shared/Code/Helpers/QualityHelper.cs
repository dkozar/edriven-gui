using System.Collections.Generic;
using eDriven.Core.Data.Collections;

namespace Assets.eDriven.Demo.Helpers
{
    public static class QualityHelper
    {
        private static List<object> _list;

        public static List<object> GetQualityList()
        {
            if (null == _list) // go lazy here
            {
                _list = new List<object>
                            {
                                new ListItem(5, string.Format("Predivno")),
                                new ListItem(4, string.Format("Lijepo")),
                                new ListItem(3, string.Format("Dobro")),
                                new ListItem(2, string.Format("Jednostavno")),
                                new ListItem(1, string.Format("Brzo")),
                                new ListItem(0, string.Format("Najbrže"))
                            };
            }

            return _list;
        }
    }
}