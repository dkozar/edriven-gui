using eDriven.Core.Geom;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Tooltip;
using UnityEngine;

namespace eDriven.Gui.Stages
{
    public class TooltipManagerStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static TooltipManagerStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private TooltipManagerStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static TooltipManagerStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating TooltipManagerStage instance"));
#endif
                    _instance = new TooltipManagerStage();
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
            ZIndex = StageManager.TOOLTIP_STAGE_DEPTH;
            Id = "TooltipManagerStage";
            //LayoutDescriptor = LayoutDescriptor.Absolute;
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            //ScrollContent = false;
            Enabled = true;

            //StageManager.Instance.RegisterStage(this);
            Register();

            CoordinateProcessor.ExcludeStage(this);
        }

        private TooltipInstance _tooltip;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _tooltip = new TooltipInstance();
            //AddChild(_tooltip);
        }

        public void ShowTooltip(string text, Point position)
        {
            _tooltip.Text = text;
            _tooltip.Visible = true;
            _tooltip.Move(position);
            _tooltip.ValidateNow();
        }

        public void UpdateTooltip(string text)
        {
            _tooltip.Text = text;
            _tooltip.ValidateNow();
        }

        public void HideTooltip()
        {
            _tooltip.Visible = false;
        }

        public void MoveTooltip(Point position)
        {
            _tooltip.Move(position);
        }
    }
}