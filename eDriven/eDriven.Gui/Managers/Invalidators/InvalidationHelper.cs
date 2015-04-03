using eDriven.Core.Util;
using eDriven.Gui.Components;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Managers.Invalidators
{
    internal class InvalidationHelper
    {
        public static void Log(string message, INestLevel obj)
        {
            Debug.Log(string.Format(@"{0}
{1}",
                                    LogUtil.Prepend(
                                        //string.Format("{0} [{1}]", obj.NestLevel, obj),
                                        string.Format("{0} {1}", obj.NestLevel, NamingUtil.DisplayListMemberToString((Components.Component) obj)),
                                        obj.NestLevel,
                                        "    " //"-> "
                                    ),

                                    LogUtil.Prepend(
                                        message,
                                        obj.NestLevel,
                                        "    " //"-> "
                                    )
                          ));
        }
    }
}
