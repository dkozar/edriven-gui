using System;
using eDriven.Gui.Components;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Containers
{
    /// <summary>
    /// Stage is the root group<br/>
    /// The application can have multiple stages
    /// </summary>
    
    [Style(Name = "showBackground", Type = typeof(bool), Default = false)]

    public class Stage : Group
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Stage()
        {
            //LayoutDescriptor = LayoutDescriptor.Vertical; // NOTE: This was the bug of pushing all stage componentns to (0, 0)!!!
            // (This overrides the Layout setting in ScrollableContainer constructor)
            //MouseEnabled = false; // NOTE: This was the bug of disabling the mouse on stage so it didn't scroll
            Stage = this; // because of the Stage propagation
        }

        public Matrix4x4 Matrix = Matrix4x4.identity;

        override public int NestLevel
        {
            get
            {
                return 1; // _registered ? 1 : -1;
            }
            set
            {
                // do nothing, stage will always have nest level of 1
            }
        }

        private int _zIndex = -1;
        private bool _zIndexChanged;
        public int ZIndex
        {
            get
            {
                return _zIndex;
            }
            set
            {
                _zIndex = value;
                _zIndexChanged = true;
                InvalidateProperties();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            //Initialized = false;

            Unregister();
        }

        protected override void CommitProperties()
        {
            base.CommitProperties();

            if (_zIndexChanged)
            {
                _zIndexChanged = false;
                StageManager.Instance.Sort(); // TODO: on each stage addition, the list is being sorted twice. Think.
            }
        }

        private Matrix4x4 _matrix;

        protected override void PreRender()
        {
            // apply stage depth
            //GUI.depth = Depth; // NONO: One depth only per StageManager

            if (Matrix != Matrix4x4.identity) {
                _matrix = GUI.matrix;
                // GUI.matrix = Matrix; // TODO
            }

            base.PreRender();
        }

        protected override void PostRender()
        {
            base.PostRender();

            if (Matrix != Matrix4x4.identity)
            {
                GUI.matrix = _matrix;
            }
        }

        /// <summary>
        /// Descending depth comparison
        /// Used for drawing
        /// Stage with a greater Depth is drawn first
        /// </summary>
        internal static readonly Comparison<Stage> DescendingDepthComparison = delegate(Stage stage1, Stage stage2)
        {
            if (stage1.ZIndex > stage2.ZIndex)
                return -1;

            if (stage1.ZIndex < stage2.ZIndex)
                return 1;

            return 0;
        };

        /// <summary>
        /// Gets the stage at a supplied depth<br/>
        /// If not found, returns null
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static Stage Get(int depth)
        {
            return StageManager.Instance.GetStageAtDepth(depth);
        }

        #region Registration

// ReSharper disable UnaccessedField.Local
        //private bool _registered;
// ReSharper restore UnaccessedField.Local

        /// <summary>
        /// Registers stage to stage manager
        /// </summary>
        public void Register()
        {
            /* Important: we have to get the stage size right */
            StageManager.Instance.RegisterStage(this);
        }

        /// <summary>
        /// Unregisters stage from stage manager
        /// </summary>
        public void Unregister()
        {
            StageManager.Instance.UnregisterStage(this);
        }

        #endregion

    }
}