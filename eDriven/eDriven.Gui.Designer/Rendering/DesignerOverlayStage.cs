#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using eDriven.Core.Geom;
using eDriven.Core.Managers;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Designer.Styles;
using eDriven.Gui.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Rendering
{
    public class DesignerOverlayStage : Stage
    {
#if DEBUG
    // ReSharper disable UnassignedField.Global
    /// <summary>
    /// Debug mode
    /// </summary>
// ReSharper disable MemberCanBePrivate.Global
        public new static bool DebugMode;
// ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnassignedField.Global
#endif

// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
        public static int STAGE_DEPTH = -100;
// ReSharper restore MemberCanBePrivate.Global
// ReSharper restore InconsistentNaming

        #region Singleton

        private static DesignerOverlayStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private DesignerOverlayStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static DesignerOverlayStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating DesignerOverlayStage instance"));
#endif
                    _instance = new DesignerOverlayStage();
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
            ZIndex = STAGE_DEPTH;
            Id = "DesignerOverlayStage";
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            Enabled = true;

            Register();

            CoordinateProcessor.ExcludeStage(this);
        }

        private InspectorOverlay _selectionOverlay;
        private InspectorOverlay _hoverOverlay;

        protected override void CreateChildren()
        {
            base.CreateChildren();

            _selectionOverlay = new InspectorOverlay { Id="selection_overlay", Visible = false };
            _selectionOverlay.SetStyle("showLabel", false);
            _selectionOverlay.SetStyle("borderStyle", InspectorOverlay4PxStyle.Instance);
            _selectionOverlay.SetStyle("borderColor", Color.yellow);
            AddChild(_selectionOverlay);

            _hoverOverlay = new InspectorOverlay { Id = "hover_overlay", Visible = false };
            _hoverOverlay.SetStyle("borderColor", Color.red);
            AddChild(_hoverOverlay);
        }

        internal void Hover(DisplayObject target)
        {
            var dlm = target as DisplayListMember;
            // do not analyze anything from this stage
            if (null != dlm && !DesignerOverlay.IsInspectable(dlm))
                return; 
            
            var go = GuiLookup.GetGameObject((Component) target);

            if (null != go)
            {
                _hoverOverlay.Visible = true;
                var bounds = target.Transform.GlobalBounds;
                
                /**
                 * 1. Expand around component
                 * */
                bounds = bounds.Expand(PlayModeOverlayHoverBorderStyle.BorderWidth);

                /**
                 * 2. Constrain with stage bounds
                 * */
                bounds.ConstrainWithin(Rectangle.FromSize(SystemManager.Instance.ScreenSize));

                _hoverOverlay.Redraw(bounds, Bounds, GuiLookup.PathToString(go.transform, " -> "));
            }
            else
            {
                Unhover();
            }
        }

        internal void Select(DisplayObject target)
        {
            var dlm = target as DisplayListMember;
            // do not analyze anything from this stage
            if (null != dlm && !DesignerOverlay.IsInspectable(dlm))
                return; 
            
            var go = GuiLookup.GetGameObject((Component) target);
            
            if (null != go)
            {
                _selectionOverlay.Visible = true;
                var bounds = target.Transform.GlobalBounds;
                
                /**
                 * 1. Expand around component
                 * */
                bounds = bounds.Expand(PlayModeOverlaySelectionBorderStyle.BorderWidth);

                /**
                 * 2. Constrain with stage bounds
                 * */
                bounds.ConstrainWithin(Rectangle.FromSize(SystemManager.Instance.ScreenSize));

                _selectionOverlay.Redraw(bounds, Bounds, GuiLookup.PathToString(go.transform, " -> "));
                _selectionOverlay.ValidateNow();
            }
        }

        internal void Deselect()
        {
            if (null == _selectionOverlay)
                return;
            _selectionOverlay.Visible = false;
            _selectionOverlay.Move(-100, -100);
            _selectionOverlay.SetActualSize(10, 10);
        }

        internal void Unhover()
        {
            if (null == _hoverOverlay)
                return;
            _hoverOverlay.Visible = false;
            _hoverOverlay.Move(-100, -100);
            _hoverOverlay.SetActualSize(10, 10);
        }
    }
}