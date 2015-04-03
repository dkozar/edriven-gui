using System;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace eDriven.Gui.Util
{
    public class Logger
    {
        public static void DebugVariables(object target, params string[] list)
        {
            if (list.Length == 0)
            {
                throw new ArgumentException("One list parameter is required.");
            }


            StringBuilder sb = new StringBuilder(); 
            
            for (int i = 0; i < list.Length; i++)
            {
                Type tp = target.GetType();
//                PropertyInfo pi = tp.GetProperty(list[i], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                PropertyInfo pi = tp.GetProperty(list[i], BindingFlags.Default);
                if (null == pi)
                    throw new Exception(string.Format(@"Property ""{0}"" not found on {1}""", list[i], tp));

                Type t1 = pi.GetType();
                Type t2 = pi.PropertyType;
                object value = pi.GetValue(target, null);
                
                sb.AppendLine(string.Format(@"Type: {0}; Type: {1}; value: {2};", t1, t2, value));
            }

            Debug.Log(string.Format(@"### DEBUG VARIABLE START ###
{0}
### DEBUG VARIABLE END ###", sb));
        }
    }
}