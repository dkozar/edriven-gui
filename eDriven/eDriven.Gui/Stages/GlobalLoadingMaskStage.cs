using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Plugins;
using UnityEngine;

namespace eDriven.Gui.Stages
{
    public class GlobalLoadingMaskStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static GlobalLoadingMaskStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private GlobalLoadingMaskStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static GlobalLoadingMaskStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating GlobalLoadingMaskStage instance"));
#endif
                    _instance = new GlobalLoadingMaskStage();
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
            ZIndex = StageManager.GLOBAL_LOADING_MASK_STAGE_DEPTH;
            Id = "GlobalLoadingMaskStage";
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            Enabled = true;

            Plugins.Add(new LoadingMaskPlugin());

            Register();
        }
    }
}