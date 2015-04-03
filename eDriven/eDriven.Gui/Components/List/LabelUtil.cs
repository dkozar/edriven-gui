using System;
using eDriven.Core.Reflection;
using UnityEngine;

namespace eDriven.Gui.Components
{
    ///<summary>
    ///</summary>
    public static class LabelUtil
    {
        ///<summary>
        ///</summary>
        ///<param name="item"></param>
        ///<param name="labelField"></param>
        ///<param name="labelFunction"></param>
        ///<returns></returns>
        public static string ItemToLabel(object item, string labelField/*=null*/, LabelFunction labelFunction/*=null*/)
        {
            if (null != labelFunction)
                return labelFunction(item);

            // early check for Strings
            if (item is string)
                return (string)item;

            if (item != null)
            {
                try
                {
                    //if (item[labelField] != null)
                    //    item = item[labelField];
                    var val = CoreReflector.GetValue(item, labelField);
                    if (null != val)
                        item = val;
                }
                catch(Exception ex)
                {
                    Debug.LogError(string.Format(@"Cannot read value ""{0}"" from object {1}:
    {2}", labelField, item, ex));
                }
            }

            // late check for strings if item[labelField] was valid
            if (item is string)
                return (string)item;

            try
            {
                if (null != item)
                    return item.ToString();
            }
            catch(Exception ex)
            {
                Debug.LogError(string.Format(@"Cannot convert to string: ""{0}"":
    {1}", item, ex));
            }

            return " ";
        }
    }
}