using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Core.Managers;
using eDriven.Core.Reflection;
using eDriven.Gui.Reflection;
using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Styles.MediaQueries
{
    public class MediaQueryManager
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static MediaQueryManager _instance;
        
        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private MediaQueryManager()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static MediaQueryManager Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating MediaQueryManager instance"));
#endif
                    _instance = new MediaQueryManager();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        private Dictionary<string, MethodInfo> _methodInfos;
        /// <summary>
        /// Gets media queries metgos infos
        /// </summary>
        public Dictionary<string, MethodInfo> MethodInfos
        {
            get { return _methodInfos; }
        }

        private Dictionary<string, MediaQuery> _queries;
        /// <summary>
        /// Gets media queries
        /// </summary>
        public Dictionary<string, MediaQuery> Queries
        {
            get { return _queries; }
        }

        private readonly Dictionary<string, bool> _results = new Dictionary<string, bool>();
        /// <summary>
        /// Gets results of media queries
        /// </summary>
        public Dictionary<string, bool> Results
        {
            get { return _results; }
        }

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            /*if (_loaded)
                return;*/

            Rescan();

            //_loaded = true;
            SystemManager.Instance.ResizeSignal.Connect(ResizeSlot);
        }

        /// <summary>
        /// Re-scanns all the media queries on demand
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Rescan()
        {
            _methodInfos = new Dictionary<string, MethodInfo>();
            _queries = new Dictionary<string, MediaQuery>();

            /*foreach (Type type in GlobalTypeDictionary.Instance.Values)
            {
                if (typeof(MediaQueryBase).IsAssignableFrom(type) && typeof(MediaQueryBase) != type)
                {
                    MediaQueryBase query = (MediaQueryBase)Activator.CreateInstance(type);
                    _queries[query.Id] = query;
                }
            }*/

            var queries = GuiReflector.GetMethodsInAllLoadedTypesDecoratedWith(typeof (MediaQueryAttribute));

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Found {0} media queries", queries.Count));
            }
#endif

            foreach (var methodInfo in queries)
            {
                if (null == methodInfo.ReturnParameter || methodInfo.ReturnType != typeof (bool))
                    throw new Exception(@"Method decorated with MediaQuery attribute should return a boolean value:
" + methodInfo);

                var p = methodInfo.GetParameters();
                var parameters = new List<Type>();
                foreach (var parameterInfo in p)
                {
                    parameters.Add(parameterInfo.ParameterType);
                }

                // get attribute
                var attributes = CoreReflector.GetMethodAttributes<MediaQueryAttribute>(methodInfo);
                if (attributes.Count == 0)
                    throw new Exception(@"Cannot find MediaQuery attribute:
" + methodInfo);

                var attribute = attributes[0];

                /**
                 * If there is already an existing query with a same ID, allow overriding
                 * only if this is an editor override. This way we'll get all the editor overrides
                 * override the Play mode media queries.
                 * */
                if (_methodInfos.ContainsKey(attribute.Id) && !attribute.EditorOverride)
                    continue;

                _methodInfos[attribute.Id] = methodInfo;

                MediaQuery query;
                if (parameters.Count > 0)
                {
                    var type = parameters[0];
                    query = (MediaQuery) NameValueBase.CreateProperty<MediaQuery>(attribute.Id, type);
                    //query.Value = type.
                }
                else
                {
                    query = new MediaQuery
                    {
                        Name = attribute.Id,
                        Parameters = parameters.ToArray()
                    };
                }
                _queries[attribute.Id] = query;
            }

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Number of queries after overrides: {0}", _queries.Keys.Count));
            }
#endif

#if DEBUG
            if (DebugMode)
            {
                Debug.Log(string.Format(@"Retrieved {0} media queries:
{1}", _queries.Count, DictionaryUtil<string, MediaQuery>.Format(_queries)));
            }
#endif
        }

        /// <summary>
        /// A flag indicating that the manager has never run
        /// </summary>
        /*private bool _runOnce;*/

        private void ResizeSlot(object[] parameters)
        {
            //Debug.Log("Resize detected: " + Screen.width + ", " + Screen.height + " ; queries: " + _queries.Count);
            //Debug.Log("Settings.Instance.LiveMediaQueries: " + Gui.LiveMediaQueries);
            if (!Gui.LiveMediaQueries)
                return;

            if (_queries.Count == 0)
                return; // no media queries used. quick return

            //Debug.Log(parameters[0]);

            Gui.ProcessStyles();
        }
        
        /// <summary>
        /// Evaluates the cached query, passing parameters
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool EvaluateQuery(string id, params object[] parameters)
        {
            var methodInfo = _methodInfos[id];
            var result = (bool) methodInfo.Invoke(null, parameters);
            //Debug.Log(string.Format("{0} => {1}", id, result));
            return result;
        }
    }
}