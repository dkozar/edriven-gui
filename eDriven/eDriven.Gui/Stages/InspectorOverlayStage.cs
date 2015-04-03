using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Util;

#if DEBUG
using UnityEngine;
#endif

namespace eDriven.Gui.Stages
{
    ///<summary>
    /// Gui inspector overlay stage
    ///</summary>
    public class InspectorOverlayStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        /// <summary>
        /// Debug mode
        /// </summary>
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static InspectorOverlayStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private InspectorOverlayStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static InspectorOverlayStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating InspectorOverlayStage instance"));
#endif
                    _instance = new InspectorOverlayStage();
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
            ZIndex = StageManager.GUI_INSPECTOR_STAGE_DEPTH;
            Id = "InspectorOverlayStage";
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            Enabled = true;

            Register();

            CoordinateProcessor.ExcludeStage(this); // NONO: it should be mouse-sensitive because of the toolbar!
        }

        #region Properties

        private bool _stickyChanged;
        private bool _sticky;
        /// <summary>
        /// The mode used with the CTRL key
        /// </summary>
        public bool Sticky
        {
            get { return _sticky; }
            set
            {
                if (value == _sticky)
                    return;
                _sticky = value;
                _stickyChanged = true;
                InvalidateProperties();
            }
        }

        #endregion

        private InspectorOverlay _overlay;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _overlay = new InspectorOverlay { Id="inspector_overlay", Visible = false };
            AddChild(_overlay);
        }

        #region Invalidation

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_stickyChanged)
            {
                _stickyChanged = false;
                if (!_sticky)
                    HideOverlay();
            }
        }

        #endregion
        
        internal void DoShowOverlay(DisplayObject target)
        {
            if (null == target)
                return;

            DisplayListMember dlm = target as DisplayListMember;
            // do not analyze anything from this stage
            if (null != dlm && !GuiInspector.IsInspectable(dlm))
                return;

            if (null != _overlay)
            {
                _overlay.Visible = true;
                _overlay.Redraw(target.Transform.GlobalBounds, Bounds, NamingUtil.DisplayListMemberToString((DisplayListMember)target));
            }
            //InspectorDetailsWindow.Instance.Inform();
        }

        /// <summary>
        /// Moves the overlay off-screen
        /// </summary>
        internal void HideOverlay()
        {
            _overlay.Visible = false;
            _overlay.Move(-100, -100);
            _overlay.SetActualSize(10, 10);
        }
    }
}