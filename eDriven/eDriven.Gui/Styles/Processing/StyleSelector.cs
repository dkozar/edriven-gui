using System;
using eDriven.Gui.Styles.Serialization;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// Style selector
    /// </summary>
    public class StyleSelector
    {
        /// <summary>
        /// Component type
        /// </summary>
        public string Type;

        /// <summary>
        /// Class name
        /// </summary>
        public string Class;

        /// <summary>
        /// Component ID
        /// </summary>
        public string Id;

        public StyleSelector(string type, string @class, string id)
        {
            Validate(type, @class, id);

            Type = type;
            Class = @class;
            Id = id;
        }

        ///// <summary>
        ///// Builds the style selector
        ///// </summary>
        ///// <param name="type"></param>
        ///// <param name="className"></param>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public static StyleSelector Build(string type, string className, string id)
        //{
        //    Validate(type, className, id);

        //    return new StyleSelector
        //    {
        //        Type = type,
        //        Class = className,
        //        Id = id
        //    };
        //}

        private static void Validate(string type, string className, string id)
        {
            if (string.IsNullOrEmpty(type) && string.IsNullOrEmpty(className) && string.IsNullOrEmpty(id))
                throw new Exception("Selector must have at least one of the 3 properties defined");
        }

        private static string _toString;
        
        /// <summary>
        /// Formats the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string FormatType(string type)
        {
            _toString = string.Empty;
            if (!string.IsNullOrEmpty(type))
                _toString += string.Format("[{0}]", type);
            return _toString;
        }

        /// <summary>
        /// Formats the type
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static string FormatClassname(string classname)
        {
            _toString = string.Empty;
            if (!string.IsNullOrEmpty(classname))
                _toString += string.Format(".{0}", classname);
            return _toString;
        }

        /// <summary>
        /// Formats the type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string FormatId(string id)
        {
            _toString = string.Empty;
            if (!string.IsNullOrEmpty(id))
                _toString += string.Format("#{0}", id);
            return _toString;
        }

        /// <summary>
        /// Builds the string version of the selector
        /// </summary>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string BuildString(string type, string className, string id)
        {
            Validate(type, className, id);

            _toString = string.Empty;
            if (!string.IsNullOrEmpty(type))
                _toString += string.Format("[{0}]", type);
            if (!string.IsNullOrEmpty(className))
                _toString += string.Format(".{0}", className);
            if (!string.IsNullOrEmpty(id))
                _toString += string.Format("#{0}", id);

            return _toString;
        }

        /// <summary>
        /// Builds the string version of the selector
        /// </summary>
        /// <param name="declaration"></param>
        /// <returns></returns>
        internal static string BuildString(Serialization.StyleDeclaration declaration)
        {
            return BuildString(declaration.Type, declaration.Class, declaration.Id);
        }

        public override string ToString()
        {
            return BuildString(Type, Class, Id);
        }

        public bool Equals(StyleSelector other)
        {
            //if (ReferenceEquals(null, other)) return false;
            //if (ReferenceEquals(this, other)) return true;
            //return Equals(other.Type, Type) && Equals(other.Class, Class) && Equals(other.Id, Id);
            return ToString().Equals(other.ToString());
        }

        public override bool Equals(object obj)
        {
            return Equals((StyleSelector) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Type != null ? Type.GetHashCode() : 0);
                result = (result*397) ^ (Class != null ? Class.GetHashCode() : 0);
                result = (result*397) ^ (Id != null ? Id.GetHashCode() : 0);
                return result;
            }
        }
    }
}