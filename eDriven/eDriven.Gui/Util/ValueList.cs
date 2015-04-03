//using System.Collections.Generic;

//namespace eDriven.Gui.Util
//{
//    /// <summary>
//    /// A list having its Equals mettod overriden so it checks for equality of each item in sequence
//    /// rather then returning the equality by reference
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public class Values<T> : List<T>
//    {
//        public override bool Equals(object obj)
//        {
//            List<T> list = (List<T>) obj;
//            if (Count != list.Count)
//                return false;

//            for (int i = 0; i < Count; i++) {

//                if (Equals(this[i], default(T)) ^ Equals(list[i], default(T))) // one is null
//                {
//                    return false;
//                }

//                //// null and null is equal (we say ;-) 
//                //if (Equals(this[i], default(T)) && Equals(list[i], default(T)))
//                //{
//                //    continue;
//                //}
//                //// if both are not null, but one is => not equal!
//                //if (Equals(this[i], default(T)) || Equals(list[i], default(T)))
//                //{
//                //    return false;
//                //}

//                if (Equals(this[i], list[i]))
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        public override int GetHashCode()
//        {
//            return 0;
//        }
//    }
//}
