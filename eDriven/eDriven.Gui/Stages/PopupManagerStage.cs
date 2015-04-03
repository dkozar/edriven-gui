using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using UnityEngine;

namespace eDriven.Gui.Stages
{
    public class PopupManagerStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static PopupManagerStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private PopupManagerStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static PopupManagerStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating PopupManagerStage instance"));
#endif
                    _instance = new PopupManagerStage();
                    _instance.Init();

                    /* Important: we have to get the stage size right */
                    _instance.ValidateNow();
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
            ZIndex = StageManager.POPUP_MANAGER_STAGE_DEPTH;
            Id = "PopupManagerStage";
            //LayoutDescriptor = LayoutDescriptor.Absolute;
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            MouseEnabled = false; // for wmouse wheeling the background
            //ScrollContent = false;
            Enabled = true;

            //StageManager.Instance.RegisterStage(this);
            Register();
        }
    }
}