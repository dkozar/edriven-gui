using System;
using System.Reflection;
using UnityEngine;

namespace eDriven.Gui.Styles
{
    ///<summary>
    ///</summary>
    public class StyleableProxy
    {
        #region Private

        private StyleableProxyMode _mode = StyleableProxyMode.SendMessage;

        /////<summary>
        /////</summary>
        //private MethodInfo _getIdMethod;

        /////<summary>
        /////</summary>
        //private MethodInfo _getClassnameMethod;

        /// <summary>
        /// 
        /// </summary>
        private MethodInfo _getStyleMethod;

        /// <summary>
        /// 
        /// </summary>
        private MethodInfo _setStyleMethod;

        /// <summary>
        /// 
        /// </summary>
        private MethodInfo _styleChangedMethod;

        /// <summary>
        /// 
        /// </summary>
        private string _getStyleMethodName;

        /// <summary>
        /// 
        /// </summary>
        private string _setStyleMethodName;

        /// <summary>
        /// 
        /// </summary>
        private string _styleChangedMethodName;

        /// <summary>
        /// 
        /// </summary>
        private string _idProperty;

        /// <summary>
        /// 
        /// </summary>
        private string _classnameProperty;

        private MethodInfo _loadStylesMethod;

        private MonoBehaviour _monoBehaviour;

        ///<summary>
        ///</summary>
        public delegate string ReaderMethod(object current);

        private ReaderMethod _readIdMethod;
        private ReaderMethod _readClassnameMethod;

        #endregion

        #region Methods

        /// <summary>
        /// Reads the component ID
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public string ReadId(object component)
        {
            // TODO: IdProperty variant
            //Debug.Log("_getIdMethod: " + _getIdMethod);
            //return (string)_getIdMethod.Invoke(null, new[] { component }); // call static
            return _readIdMethod(component);
        }

        /// <summary>
        /// Reads the component classname
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public string ReadClassname(object component)
        {
            // TODO: ClassnameProperty variant
            //Debug.Log("_getClassnameMethod: " + _getClassnameMethod);
            //return (string)_getClassnameMethod.Invoke(null, new[] { component }); // call static
            return _readClassnameMethod(component);
        }

        #endregion

        /// <summary>
        /// Styleable proxy
        /// </summary>
        public StyleableProxy()
        {
        }

        //public object Get(string styleName)
        //{
        //    if (_mode == StyleableProxyMode.SendMessage) {
        //        MonoBehaviour.SendMessage(_getStyleMethodName, styleName);
        //        return 
        //    }

        //    return _getStyleMethod.Invoke();
        //}

        //public void Set(object value)
        //{
        //    return _setStyleMethod.Invoke(object value);
        //}

        /// <summary>
        /// Invokes the style changed method on component
        /// </summary>
        /// <param name="component"></param>
        /// <param name="styleName"></param>
        public void InvokeStyleChanged(object component, string styleName)
        {
            if (_mode == StyleableProxyMode.SendMessage) {
                _monoBehaviour.SendMessage(_styleChangedMethodName, styleName);
                return;
            }
            _styleChangedMethod.Invoke(component, new object[] { styleName });
        }

        ///<summary>
        ///</summary>
        ///<param name="component"></param>
        ///<param name="attribute"></param>
        ///<returns></returns>
        ///<exception cref="Exception"></exception>
        public static StyleableProxy CreateProxy(object component, StyleableAttribute attribute)
        {
            StyleableProxy proxy = new StyleableProxy
                                       {
                                           _mode = attribute.Mode,
                                           _classnameProperty = attribute.ClassnameProperty,
                                           _idProperty = attribute.IdProperty
                                       };

            Type readerType = attribute.Reader;
            if (null == readerType)
            {
                // default reader (for MonoBehavour)
                readerType = typeof(HierarchyReader);
                //throw new Exception("No reader found");
            }

            Debug.Log("readerType: " + readerType);
            IStyleReader reader = (IStyleReader) Activator.CreateInstance(readerType);

            proxy._readIdMethod = reader.GetId;
            proxy._readClassnameMethod = reader.GetClassname;

            Type type = component.GetType();
            //Debug.Log("type: " + type);

            //try
            //{
            //    //if (!string.IsNullOrEmpty(attribute.ReadIdMethod))
            //    //var reader = Activator.CreateInstance(readerType);
            //    //Debug.Log("attribute.ReadIdMethod: " + attribute.ReadIdMethod);
                
            //    proxy._getIdMethod = readerType.GetMethod(attribute.ReadIdMethod); // GetMethodInfo(component, attribute.ReadIdMethod);

            //    //Debug.Log("!!!");
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(string.Format("Couldn't get the method specified with ID [{0}]: ", attribute.ReadIdMethod), ex);
            //}

            
            //if (!string.IsNullOrEmpty(attribute.ReadClassnameMethod))
            //{
            //    try
            //    {
            //        //Debug.Log("attribute.ReadClassnameMethod: " + attribute.ReadClassnameMethod);
            //        //Debug.Log("type: " + type);

            //        proxy._getClassnameMethod = readerType.GetMethod(attribute.ReadClassnameMethod); // GetMethodInfo(component, attribute.ReadClassnameMethod);

            //        //Debug.Log("!!!");
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("Couldn't get the method specified with ReadClassnameMethod [{0}]: ", attribute.ReadClassnameMethod), ex);
            //    }
            //}

            if (proxy._mode == StyleableProxyMode.SendMessage)
            {
                if (!(component is MonoBehaviour))
                {
                    throw new Exception("SendMessage mode could only be used with MonoBehaviour subclasses");
                }
                proxy._monoBehaviour = (MonoBehaviour) component;
                return proxy; // do not process anything via reflection; return
            }

            if (!string.IsNullOrEmpty(attribute.LoadMethod))
            {
                try
                {
                    proxy._loadStylesMethod = type.GetMethod(attribute.LoadMethod, new Type[]{}); //GetMethodInfo(component, attribute.LoadMethod);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Couldn't get the method specified with LoadMethod: [{0}]: ", attribute.LoadMethod), ex);
                }
            }

            //Debug.Log(3);
            //if (!string.IsNullOrEmpty(attribute.GetMethod))
            //{
            //    try
            //    {
                    
            //        proxy._getStyleMethod = type.GetMethod(attribute.GetMethod, new[]{typeof(string)});
            //        //proxy._getStyleMethod = GetMethodInfo(component, attribute.GetMethod);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("Couldn't get the method specified with GetMethod: [{0}]: ", attribute.ReadClassnameMethod), ex);
            //    }
            //}

            //Debug.Log(4);
            //if (!string.IsNullOrEmpty(attribute.SetMethod))
            //{
            //    try
            //    {
            //        proxy._setStyleMethod = type.GetMethod(attribute.SetMethod, new[] { typeof(string), typeof(object) }); //GetMethodInfo(component, attribute.SetMethod);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(string.Format("Couldn't get the method specified with SetMethod:  [{0}]: ", attribute.ReadClassnameMethod), ex);
            //    }
            //}
            //Debug.Log(5);
            if (!string.IsNullOrEmpty(attribute.ChangedMethod))
            {
                try
                {
                    proxy._styleChangedMethod = type.GetMethod(attribute.ChangedMethod, new[] { typeof(string), typeof(object) }); // GetMethodInfo(component, attribute.ChangedMethod);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Couldn't get the method specified with ChangedMethod:  [{0}]: ", attribute.ChangedMethod), ex);
                }
            }
            //Debug.Log(6);

            return proxy;
        }

        //private static MethodInfo GetMethodInfo(object component, string methodName)
        //{
        //    Type type = component.GetType();
        //    return type.GetMethod(methodName);
        //}
    }
}