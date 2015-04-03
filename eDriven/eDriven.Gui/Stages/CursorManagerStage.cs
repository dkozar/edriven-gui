using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using UnityEngine;

namespace eDriven.Gui.Stages
{
    public class CursorManagerStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static CursorManagerStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private CursorManagerStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static CursorManagerStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating CursorManagerStage instance"));
#endif
                    _instance = new CursorManagerStage();
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
            ZIndex = StageManager.CURSOR_STAGE_DEPTH;
            Id = "CursorManagerStage";
            //LayoutDescriptor = LayoutDescriptor.Absolute;
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            //ScrollContent = false;
            Enabled = true;
            Visible = false;

            //StageManager.Instance.RegisterStage(this);
            Register();

            CoordinateProcessor.ExcludeStage(this);

            /* Important: in some circumstances we have to get the stage size right
             * (for instance the popup manager creares the stage just before the first popup is created
             * and the stage has to be sized right for centering popup */
            ValidateNow(); // TODO: not needed for this stage for size is irrelevant?
        }
    }
}