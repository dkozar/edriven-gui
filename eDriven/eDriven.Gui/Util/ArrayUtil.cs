using System;
using System.Collections.Generic;
using UnityEngine;

namespace eDriven.Gui.Util
{
    public static class ArrayUtil<T>
    {
        /// <summary>
        /// Clones the array<br/>
        /// If elements cloneable, clones also the elements
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T[] Clone(T[] list)
        {
            //Debug.Log("Clonning list: " + list);
            List<T> listOut = new List<T>();
            foreach (var element in list)
            {
                if (element is ICloneable)
                    listOut.Add((T) (element as ICloneable).Clone());
                else
                {
                    throw new Exception("Not cloneable");
                }
                //else
                //    listOut.Add(element);
            }
            return listOut.ToArray();
        }

        /// <summary>
        /// Returns true if array contains null reference
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool ContainsNullReference(T[] array)
        {
            //Debug.Log("Running ContainsNullReference:");
            int count = array.Length;

            //Debug.Log("count: " + count);

            for (int i = 0; i < count; i++)
            {
                //Debug.Log(string.Format(@"   - Comparing: {0} and {1}", list1[i], list2[i])); // beware of null

                // null == null
                if (Equals(array[i], default(T)))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if two arrays are equal
        /// </summary>
        /// <param name="array1"></param>
        /// <param name="array2"></param>
        /// <returns></returns>
        public static bool Equals(T[] array1, T[] array2)
        {
            //Debug.Log("Equals called: " + array1.Length + ", " + array2.Length);
            if (array1.Length != array2.Length)
                return false;

            int count = array1.Length;

            for (int i = 0; i < count; i++)
            {
                if (Equals(array1[i], default(T)) && Equals(array2[i], default(T)))
                    continue;

                if (Equals(array1[i], default(T)) ^ Equals(array2[i], default(T)))
                { // one is null
                    return false;
                }

                //if (!Equals(array1[i], array2[i]))
                if (!array1[i].Equals(array2[i]))
                {
                    Debug.Log("Not equal: " + array1[i] + "; " + array2[i]);
                    return false;
                }
            }

            return true;
        }
    }
}