using System;
using System.Reflection;
using eDriven.Gui.Util;

namespace eDriven.Gui.Styles.Serialization
{
    [Obfuscation(Exclude = true)]
    [Serializable]
    public class StyleSheet : ICloneable
    {
        /// <summary>
        /// Style declarations
        /// </summary>
        //[List(ShowSize = false, ShowListLabel = false, ShowElementLabels = false, ShowButtons = true)]
        public StyleDeclaration[] Declarations;
        
        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            var clone = new StyleSheet();
            if (null != Declarations)
                clone.Declarations = ArrayUtil<StyleDeclaration>.Clone(Declarations);

            return clone;
        }

        #endregion

        #region Equals

        public bool Equals(StyleSheet other)
        {
            //Debug.Log("Equals1 called");
            if (ReferenceEquals(null, other)) return false;
            return /*Equals(other.Name, Name) && */ArrayUtil<StyleSheetValuesFactory>.Equals(other.Declarations, Declarations);
        }

        public override bool Equals(object obj)
        {
            //Debug.Log("Equals2 called");
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof (StyleSheet)) return false;
            return Equals((StyleSheet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (/*(Name != null ? Name.GetHashCode() : 0)*397) ^ (*/Declarations != null ? Declarations.GetHashCode() : 0);
            }
        }

        #endregion
    }
}