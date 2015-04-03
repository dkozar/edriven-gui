using System;
using eDriven.Core.Reflection;

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// Compares properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyComparer<T> : System.Collections.Generic.IComparer<T>
    {
        private bool _descending;
        /// <summary>
        /// Descending
        /// </summary>
        public bool Descending
        {
            get { return _descending; }
            set { _descending = value; }
        }

        private string _sortField;
        /// <summary>
        /// Sort field
        /// </summary>
        public string SortField
        {
            get { return _sortField; }
            set { _sortField = value; }
        }

        private bool _numeric;
        /// <summary>
        /// Numeric
        /// </summary>
        public bool Numeric
        {
            get { return _numeric; }
            set { _numeric = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyComparer()
        {
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertyComparer(string sortField, bool descending)
        {
            _sortField = sortField;
            _descending = descending;
        }

        public int Compare(T xWord, T yWord)
        {
            // GetEaser property values
            object xValue = CoreReflector.GetValue(xWord, _sortField);
            object yValue = CoreReflector.GetValue(yWord, _sortField);

            // Determine sort order
            if (_descending)
                return CompareDescending(xValue, yValue);
            
            return CompareAscending(xValue, yValue);
        }

        // Compare two property values of any type
        private static int CompareAscending(object xValue, object yValue)
        {
            int result;

            // If values implement IComparer
            if (xValue is IComparable)
            {
                result = ((IComparable)xValue).CompareTo(yValue);
            }
                
            // If values don't implement IComparer but are equivalent
            else if (xValue.Equals(yValue))
            {
                result = 0;
            }
            
            // Values don't implement IComparer and are not equivalent, so compare as string values
            else result = xValue.ToString().CompareTo(yValue.ToString());

            // Return result
            return result;
        }

        private static int CompareDescending(object xValue, object yValue)
        {
            // Return result adjusted for ascending or descending sort order ie
            // multiplied by 1 for ascending or -1 for descending
            return CompareAscending(xValue, yValue) * -1;
        }
    }
}