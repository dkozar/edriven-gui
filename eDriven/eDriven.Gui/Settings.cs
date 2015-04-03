/*using UnityEngine;

namespace eDriven.Gui
{
    public class Settings
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static Settings _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private Settings()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating Settings instance"));
#endif
                    _instance = new Settings();
                    _instance.Initialize();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Initialize()
        {

        }

        /// <summary>
        /// Media queries are being run by default
        /// </summary>
        public bool LiveMediaQueries = false; // TRUE!!!!

        /// <summary>
        /// Set this flag to true if in need to reset the declaration cache
        /// For instance, the editor should set this flag to true if any of style declarations added/removed/changed,
        /// just before reloading the style sheet collection (calling the Load() method)
        /// </summary>
        internal bool StyleCacheDirty;
    }
}*/