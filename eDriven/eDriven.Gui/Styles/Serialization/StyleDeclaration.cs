using System;
using System.Reflection;
using eDriven.Gui.Util;

namespace eDriven.Gui.Styles.Serialization
{
    [Obfuscation(Exclude = true)]
    [Serializable]
    public class StyleDeclaration : ICloneable
    {
        ///<summary>
        /// The style module ID, used for lookup<br/>
        /// This ID is a reference to the provider which handles the actual application of styles<br/>
        /// Once serialized, it cannot be changed
        ///</summary>
        public string Module;

        ///<summary>
        /// CSS component type
        ///</summary>
        public string Type;

        ///<summary>
        /// CSS class
        ///</summary>
        public string Class;

        ///<summary>
        /// CSS ID
        ///</summary>
        public string Id;

        ///<summary>
        /// The list of properties
        ///</summary>
        public StyleProperty[] Properties;

        ///<summary>
        /// The list of media queries applied to this style declaration
        ///</summary>
        public MediaQuery[] MediaQueries;

        /// <summary>
        /// Returns the dictionary of name/value pairs<br/>
        /// Used by component traversers
        /// </summary>
        /// <returns></returns>
        public StyleTable ToStyleTable()
        {
            StyleTable styleTable = new StyleTable();
            foreach (StyleProperty property in Properties)
            {
                styleTable.Add(property.Name, property.Value);
            }
            return styleTable;
        } 

        #region Implementation of ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object Clone()
        {
            StyleDeclaration clone = new StyleDeclaration
                                         {
                                             Module = Module,
                                             Type = Type,
                                             Class = Class,
                                             Id = Id
                                         };
            if (null != Properties)
                clone.Properties = ArrayUtil<StyleProperty>.Clone(Properties);

            if (null != MediaQueries)
                clone.MediaQueries = ArrayUtil<MediaQuery>.Clone(MediaQueries);

            return clone;
        }

        #endregion

        #region Equals

        public bool Equals(StyleDeclaration other)
        {
            if (ReferenceEquals(null, other)) return false;
            return Equals(other.Type, Type) && 
                ArrayUtil<StyleProperty>.Equals(other.Properties, Properties) &&
                ArrayUtil<string>.Equals(other.MediaQueries, MediaQueries);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(StyleDeclaration)) return false;
            return Equals((StyleDeclaration)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ 
                    (Properties != null ? Properties.GetHashCode() : 0) ^
                    (MediaQueries != null ? MediaQueries.GetHashCode() : 0);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}, Properties: {1}, MediaQueries: {2}", StyleSelector.BuildString(Type, Class, Id), Properties.Length, MediaQueries.Length);
        }
    }
}