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

using System.Collections.Generic;
using eDriven.Core.Geom;
using eDriven.Gui.Containers;
using eDriven.Gui.Editor.GUIStyles;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Editor.Styles
{
    public class StylingOverlayStage : Stage
    {
#if DEBUG
        // ReSharper disable UnassignedField.Global
        public new static bool DebugMode;
        // ReSharper restore UnassignedField.Global
#endif

// ReSharper disable InconsistentNaming
        private const int STYLING_OVERLAY_STAGE_DEPTH = -1200;
// ReSharper restore InconsistentNaming

        #region Singleton

        private static StylingOverlayStage _instance;

        /// <summary>
        /// Singleton class for handling focus
        /// </summary>
        private StylingOverlayStage()
        {
            // Constructor is protected
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static StylingOverlayStage Instance
        {
            get
            {
                if (_instance == null)
                {
#if DEBUG
                    if (DebugMode)
                        Debug.Log(string.Format("Instantiating StylingOverlayStage instance"));
#endif
                    _instance = new StylingOverlayStage();
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
            ZIndex = STYLING_OVERLAY_STAGE_DEPTH;
            Id = "StylingOverlayStage";
            Layout = new AbsoluteLayout();
            FocusEnabled = false;
            Enabled = true;

            Register();

            CoordinateProcessor.ExcludeStage(this);
        }

        private bool _componentsSet;
        private List<Component> _components;
        public void Draw(List<Component> components)
        {
            _components = components;
            _componentsSet = true;
            //InvalidateProperties();
            InvalidateDisplayList();
        }

        private Component _shape;
        /*protected override void CommitProperties() // TODO: why this doesn't work???
        {
            base.CommitProperties();

            //Debug.Log("!");
            if (_componentsSet)
            {
                _componentsSet = false;
                foreach (Component component in _components)
                {
                    //Debug.Log("    component.Bounds: " + component.Bounds);
                    //Debug.Log("component.Transform.GlobalPosition: " + component.Transform.GlobalPosition);

                    Rectangle bounds = null != component.Parent ? component.Parent.LocalToGlobal(component.Bounds) : component.Bounds; // Bounds = stage only
                    //Debug.Log("    bounds: " + bounds);

                    _shape = new StylingOverlayRectShape
                    {
                        X = bounds.X,
                        Y = bounds.Y,
                        Width = bounds.Width,
                        Height = bounds.Height
                    };

                    AddChild(_shape);
                    //_shape.ValidateNow();
                }
                InvalidateDisplayList();
                ValidateDisplayList();
                //ValidateNow();
            }
        }*/

        protected override void UpdateDisplayList(float width, float height)
        {
            base.UpdateDisplayList(width, height);
            //Debug.Log("!");
            if (_componentsSet)
            {
                _componentsSet = false;
                foreach (Component component in _components)
                {
                    //Debug.Log("    component.Bounds: " + component.Bounds);
                    //Debug.Log("component.Transform.GlobalPosition: " + component.Transform.GlobalPosition);

                    Rectangle bounds = null != component.Parent ? component.Parent.LocalToGlobal(component.Bounds) : component.Bounds; // Bounds = stage only
                    //Debug.Log("    bounds: " + bounds);

                    _shape = new StylingOverlayRectShape
                    {
                        X = bounds.X,
                        Y = bounds.Y,
                        Width = bounds.Width,
                        Height = bounds.Height
                    };

                    AddChild(_shape);
                    //_shape.ValidateNow();
                }
                /*InvalidateDisplayList();
                ValidateDisplayList();*/
                //ValidateNow();
            }
        }

        public void Clear()
        {
            RemoveAllChildren();
        }
    }
}