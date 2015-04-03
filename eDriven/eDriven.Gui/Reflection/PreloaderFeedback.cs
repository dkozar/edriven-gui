using eDriven.Core.Signals;
using eDriven.Gui.Managers;

#if DEBUG
using UnityEngine;
#endif

namespace eDriven.Gui.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PreloaderFeedback
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif
        
        // ReSharper disable InconsistentNaming
#pragma warning disable 1591
        public const string REFLECTING = "reflecting"; // "Loading default styles..."
        public const string LOADING_DEFAULT_STYLES = "loadingDefaultStyles"; // "Loading default styles..."
        public const string LOADING_STYLESHEETS = "loadingStyleSheets"; // "Loading stylesheets..."
        public const string INITIALIZING_STYLES = "initializingStyles"; // "Initializing..."
#pragma warning restore 1591
        // ReSharper restore InconsistentNaming

        #region Singleton

        private static PreloaderFeedback _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PreloaderFeedback()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PreloaderFeedback Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating InitializationFeedback instance"));
#endif
                    _instance = new PreloaderFeedback();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public readonly Signal StepStartSignal = new Signal();

        /// <summary>
        /// 
        /// </summary>
        public readonly Signal InitializationCompleteSignal = new Signal();

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {
            InvalidationManager.Instance.UpdateCompleteSignal.Connect(UpdateCompleteSlot);
        }

        private void UpdateCompleteSlot(object[] parameters)
        {
            InitializationCompleteSignal.Emit();

            // auto-disconnect signals
            StepStartSignal.DisconnectAll();
            InitializationCompleteSignal.DisconnectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void StepStart(string message)
        {
            StepStartSignal.Emit(message);
        }
    }
}
