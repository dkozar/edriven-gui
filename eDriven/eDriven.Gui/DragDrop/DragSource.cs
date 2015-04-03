using System;
using System.Collections.Generic;

namespace eDriven.Gui.DragDrop
{
    public class DragSource
    {
        /// <summary>
        /// Callback definition
        /// </summary>
        /// <returns></returns>
        public delegate object DataLookupHandler();

        /// <summary>
        /// Formats dictionary
        /// </summary>
        private readonly Dictionary<string, object> _formats = new Dictionary<string, object>();
        public Dictionary<string, object> Formats
        {
            get
            {
                return _formats;
            }
        }

        #region Constructor

        public DragSource()
        {
        }

        public DragSource(object data, string format)
        {
            if (data is DataLookupHandler)
            {
                AddHandler((DataLookupHandler) data, format);
            }
            else
            {
                AddData(data, format);
            }
        }

        #endregion

        /// <summary>
        /// Adds data and a corresponding format String to the drag source
        /// </summary>
        /// <param name="data"></param>
        /// <param name="format"></param>
        public void AddData(object data, string format)
        {
            _formats.Add(format, data);
        } 

        /// <summary>
        /// Adds a handler that is called when data for the specified format is requested
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="format"></param>
        public void AddHandler(DataLookupHandler handler, string format)
        {
            _formats.Add(format, handler);
        }

        /// <summary>
        /// Retrieves the data for the specified format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public object DataForFormat(string format)
        {
            object d = _formats[format];

            if (null == d)
            {
                throw new Exception(string.Format(@"Data for format ""{0}"" not found", format));
            }

            // if it's a handler
            if (d is DataLookupHandler)
            {
                return ((DataLookupHandler) d)();
            }
            
            // else
            return d;
        }

        /// <summary>
        /// Returns true if the data source contains the requested format; otherwise, it returns false
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public bool HasFormat(string format)
        {
            return _formats.ContainsKey(format);
        }
    }
}