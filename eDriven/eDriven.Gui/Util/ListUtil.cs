using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Util
{
    public static class ListUtil<T>
    {
        public static List<T> Clone(List<T> list)
        {
            //Debug.Log("Clonning list: " + list);
            List<T> listOut = new List<T>();
            foreach (var element in list)
            {
                listOut.Add(element);
            }
            return listOut;
        }

        public static bool ContainsNullReference(List<T> list)
        {
            //Debug.Log("Running ContainsNullReference:");
            int count = list.Count;

            //Debug.Log("count: " + count);

            for (int i = 0; i < count; i++)
            {
                //Debug.Log(string.Format(@"   - Comparing: {0} and {1}", list1[i], list2[i])); // beware of null

                // null == null
                if (Equals(list[i], default(T)))
                    return true;
            }

            return false;
        }

        public static bool Equals(List<T> list1, List<T> list2)
        {
            //Debug.Log("Equals...");

            if (list1.Count != list2.Count)
                return false;

            int count = list1.Count;

            //Debug.Log("count: " + count);

            for (int i = 0; i < count; i++)
            {
                //Debug.Log(string.Format(@"   - Comparing: {0} and {1}", list1[i], list2[i])); // beware of null

                // null == null
                if (Equals(list1[i], default(T)) && Equals(list2[i], default(T)))
                    continue;

                if (Equals(list1[i], default(T)) ^ Equals(list2[i], default(T)))
                { // one is null
                    //Debug.Log("    Equals returning false (one is null)");
                    return false;
                }

                if (!Equals(list1[i], list2[i]))
                {
                    //if (!Equals(list1[i], list2[i])) {
                    //Debug.Log("    Equals returning false");
                    return false;
                }

                //                if (!list2[i].Equals(list1[i]))
                //                    return false;
            }

            //Debug.Log("    Equals returning true");

            return true;
        }

        ///<summary>
        ///</summary>
        ///<param name="list"></param>
        ///<returns></returns>
        public static string Format(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T element in list)
            {
                sb.AppendLine(element.ToString());
            }
            return sb.ToString();
        }
    }
}