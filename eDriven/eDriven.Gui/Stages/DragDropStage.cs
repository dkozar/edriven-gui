using eDriven.Gui.Containers;
using eDriven.Gui.DragDrop;
using eDriven.Gui.Layout;
using UnityEngine;

namespace eDriven.Gui.Stages
{
    public class DragDropStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

        #region Singleton

        private static DragDropStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DragDropStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DragDropStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DragDropStage instance"));
#endif
                    _instance = new DragDropStage();
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
            ZIndex = StageManager.DRAG_DROP_STAGE_DEPTH;
            Id = "DragDropStage";
            //LayoutDescriptor = LayoutDescriptor.Absolute;
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            //SupressEventPropagation = false;
            //ScrollContent = false;
            Enabled = true;
            //Skin = CoreSkinMapper.Instance.System;

            //StageManager.Instance.RegisterStage(this);
            Register();
        }

        protected override void CreateChildren()
        {
            base.CreateChildren();

            AddChild(DragDropManager.Overlay);

            // add drag & drop proxy
            //AddChild(DragDropManager.Proxy);
        }
    }
}