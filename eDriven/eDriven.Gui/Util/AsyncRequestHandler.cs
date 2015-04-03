using System;
using System.Collections.Generic;
using eDriven.Core.Managers;
using UnityEngine;

namespace eDriven.Gui.Util
{
    /// <summary>
    /// A class that handles async calls and callbacks
    /// </summary>
    /// <remarks>Author: Danko Kozar</remarks>
    public class AsyncRequestHandler<TIdentifier, TResponse> : IDisposable
    {
        public static bool DebugMode;

        #region Delegate definition

        /// <summary>
        /// Callback signature
        /// </summary>
        public delegate void Callback(TResponse response);

        /// <summary>
        /// The signature of function to check status
        /// </summary>
        public delegate TIdentifier IdResolver(TResponse response);

        #endregion

        #region Members

        /// <summary>
        /// Dictionary [TIdentifier, Callback]
        /// </summary>
        private readonly Dictionary<TIdentifier, Callback> _dictId;

        /// <summary>
        /// Dictionary [TResponse, Callback]
        /// </summary>
        private readonly Dictionary<TResponse, Callback> _dictResponse;

        /// <summary>
        /// Finished IDs
        /// </summary>
        //private readonly List<TIdentifier> _finishedIds;

        /// <summary>
        /// Unprocessed responses
        /// </summary>
        private readonly List<TResponse> _responses;

        /// <summary>
        /// Finished responses
        /// </summary>
        //private readonly List<TResponse> _finishedResponses;

        private IdResolver _getId;
        // ReSharper disable MemberCanBePrivate.Global
        /// <summary>
        /// Delegate for getting ID from response
        /// </summary>
        public IdResolver GetId
            // ReSharper restore MemberCanBePrivate.Global
        {
            get
            {
                if (null == _getId)
                    throw new Exception("GetId function not defined");

                return _getId;
            } 
            set
            {
                _getId = value;
            }
        }
        
        #endregion

        #region Constructor

        public AsyncRequestHandler()
        {
            _dictId = new Dictionary<TIdentifier, Callback>();
            _dictResponse = new Dictionary<TResponse, Callback>();

            //_finishedIds = new List<TIdentifier>();
            
            _responses = new List<TResponse>();
            //_finishedResponses = new List<TResponse>();

            /**
             * Subscribe to system manager
             * */
            //SystemManager.Instance.AddEventListener(SystemManager.UPDATE, Process);
            SystemManager.Instance.UpdateSignal.Connect(UpdateSlot);
        }

        #endregion

        #region Methods

        /// <summary>
        /// A heartbeat function
        /// </summary>
        public void Process(/*Event e*/)
        {
            if (0 == _responses.Count)
                return; // early return if nothnig to process

            /**
             * Look up for unprocessed responses
             * */
            foreach (TResponse response in _responses)
            {
                TIdentifier id = GetId(response);

#if DEBUG
                if (DebugMode)
                    Debug.Log(string.Format("Processing response with ID[{0}]", id));
#endif

                if (_dictId.ContainsKey(id))
                {
                    Callback callback = _dictId[id];

                    _dictId.Remove(id);
                    _dictResponse.Add(response, callback);

#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Response [{0}] processed", id));
#endif

                }
            }

            /**
             * Fire callbacks with response as parameter
             * */
            foreach (KeyValuePair<TResponse, Callback> pair in _dictResponse)
            {
                pair.Value(pair.Key);
            }

            /**
             * Clear response dict
             * */
            _dictResponse.Clear();
            _responses.Clear();
        }

        // ReSharper disable MemberCanBeProtected.Global
        public virtual void Send(TIdentifier request, Callback callback)
            // ReSharper restore MemberCanBeProtected.Global
        {
            //Debug.Log("Send: " + request + ", " + callback);

            _dictId.Add(request, callback);
        }

        public void AddResponse(TResponse response){
            _responses.Add(response);
        }

        #endregion

        #region Implementation of ISlot

        public void UpdateSlot(params object[] parameters)
        {
            Process();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            SystemManager.Instance.UpdateSignal.Disconnect(UpdateSlot);
        }

        #endregion
    }
}