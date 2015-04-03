using System.Collections.Generic;
using System.Text;

namespace eDriven.Gui.Styles
{
    ///<summary>
    ///</summary>
    public class OrderedObject<T> : List<OrderedObjectRow<T>>
    {
        ///<summary>
        ///</summary>
        public List<string> Keys
        {
            get { 
                List<string> names = new List<string>();
                foreach (OrderedObjectRow<T> row in this)
                {
                    names.Add(row.Name);
                }
                return names;
            }
        }

        ///<summary>
        ///</summary>
        public List<T> Values
        {
            get
            {
                List<T> values = new List<T>();
                foreach (OrderedObjectRow<T> row in this)
                {
                    values.Add(row.Value);
                }
                return values;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, T value)
        {
            Add(new OrderedObjectRow<T>(key, value));
        }

        ///<summary>
        ///</summary>
        ///<param name="key"></param>
        ///<returns></returns>
        public T Get(string key)
        {
            var value = Find(delegate(OrderedObjectRow<T> match)
                     {
                         return match.Name == key;
                     });

            if (null == value)
                return default(T);

            return value.Value;
        }

        public bool KeyExists(string key)
        {
            return Exists(delegate(OrderedObjectRow<T> match)
                              {
                                  return match.Name == key;
                              });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Count: {0}", Count));
            foreach (OrderedObjectRow<T> o in this)
            {
                sb.AppendLine(string.Format(@"{0}, {1}", o.Name, o.Value));
            }
            return sb.ToString();
        }
    }
}