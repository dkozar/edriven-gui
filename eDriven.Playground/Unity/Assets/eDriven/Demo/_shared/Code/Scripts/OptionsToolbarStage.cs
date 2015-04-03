using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using UnityEngine;

namespace Assets.eDriven.Demo.Scripts
{
    public class OptionsToolbarStage : Stage
    {
#if DEBUG
// ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool DebugMode;

// ReSharper restore UnassignedField.Global
#endif

        public static int OPTIONS_TOOLBAR_STAGE_DEPTH = -500;
        
        #region Singleton

        private static OptionsToolbarStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private OptionsToolbarStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static OptionsToolbarStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating OptionsToolbarStage instance"));
#endif
                    _instance = new OptionsToolbarStage();
                    _instance.Init();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes the Singleton instance
        /// </summary>
        private void Init()
        {
            ZIndex = OPTIONS_TOOLBAR_STAGE_DEPTH;
            Id = "OptionsToolbarStage";
            //LayoutDescriptor = LayoutDescriptor.Absolute;
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            //ClipContent = false;
            Enabled = true;

            Register();
        }
    }
}