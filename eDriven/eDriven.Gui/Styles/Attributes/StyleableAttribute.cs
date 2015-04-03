using System;

namespace eDriven.Gui.Styles
{
    /// <summary>
    /// The attribute mapping component to a style proxy
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StyleableAttribute : Attribute
    {
        /// <summary>
        /// The mode of calling methods on target object
        /// </summary>
        public StyleableProxyMode Mode = StyleableProxyMode.SendMessage;

        /// <summary>
        /// Reader type
        /// </summary>
        public Type Reader { get; set; }

        /// <summary>
        /// The name of the property for reading the ID
        /// </summary>
        public string IdProperty { get; set; }

        /// <summary>
        /// The name of the property for reading the Classname
        /// </summary>
        public string ClassnameProperty { get; set; }

        ///// <summary>
        ///// The name of the method for getting the ID
        ///// </summary>
        //public string ReadIdMethod { get; set; }

        ///// <summary>
        ///// The name of the method for getting the Classname
        ///// </summary>
        //public string ReadClassnameMethod { get; set; }

        /// <summary>
        /// The name of the method loading styles (could be null)
        /// </summary>
        public string LoadMethod { get; set; }

        private string _getMethod = "GetStyle";
        /// <summary>
        /// The name of the method for getting the style value
        /// </summary>
        public string GetMethod
        {
            get { return _getMethod; }
            set { _getMethod = value; }
        }

        private string _setMethod = "SetStyle";
        /// <summary>
        /// The name of the method for setting the style value
        /// </summary>
        public string SetMethod
        {
            get { return _setMethod; }
            set { _setMethod = value; }
        }

        private string _changedMethod = "StyleChanged";
        /// <summary>
        /// The name of the method for getting the style value
        /// </summary>
        public string ChangedMethod
        {
            get { return _changedMethod; }
            set { _changedMethod = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public StyleableAttribute()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="styleReaderType"></param>
        public StyleableAttribute(Type styleReaderType)
        {
            Reader = styleReaderType;
        }
    }
}