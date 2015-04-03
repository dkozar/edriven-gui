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

using System;
using System.Collections.Generic;
using System.Reflection;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Reflection;
using UnityEngine;
using Component = eDriven.Gui.Components.Component;

namespace eDriven.Gui.Designer.Adapters
{
    /// <summary>
    /// The adapter for Panel component
    /// </summary>
    [Obfuscation(Exclude = true)]
    [Toolbox(Icon = "eDriven/Editor/Controls/panel", Label = "Panel", Tooltip = "A panel")]

    public class PanelAdapter : SkinnableContainerAdapter
    {
        public override Type ComponentType
        {
            get { return typeof(Panel); }
        }

        public override Type DefaultSkinClass
        {
            get { return typeof(PanelSkin); }
        }

        /// <summary>
        /// Panel title
        /// </summary>
        [Saveable]
        public string Title = "Panel";

        /// <summary>
        /// Icon
        /// </summary>
        [Saveable]
        public Texture Icon;

        /// <summary>
        /// Control bar group children
        /// </summary>
        [Saveable]
        //[SerializeField]
        [ChildCollection(ShowHeader = true, TargetContainer = "ControlBarGroup", Label = "Control bar", Icon = "eDriven/Editor/Icons/group_control_bar")]
        public List<ComponentAdapter> ControlBarGroupChildren = new List<ComponentAdapter>();

        /// <summary>
        /// Tool group children
        /// </summary>
        [Saveable]
        //[SerializeField]
        [ChildCollection(ShowHeader = true, TargetContainer = "ToolGroup", Label = "Tools", Icon = "eDriven/Editor/Icons/group_tools")]
        public List<ComponentAdapter> ToolGroupChildren = new List<ComponentAdapter>();

        ///<summary>
        ///</summary>
        public PanelAdapter()
        {
            //ClipContent = true;
            MinWidth = 200;
            MinHeight = 100;
        }

        public override Component NewInstance()
        {
            return new Panel
            {
                //ClipContent = true,
                //ScrollContent = true,
                //LayoutDescriptor = eDriven.Gui.Layout.LayoutDescriptor.VerticalTopLeft
                Layout = new VerticalLayout
                {
                    //Direction = eDriven.Gui.Layout.LayoutDirection.Vertical,
                    VerticalAlign = VerticalAlign.Top,
                    HorizontalAlign = HorizontalAlign.Left
                }
            };
        }

        /// <summary>
        /// Applies changes
        /// </summary>
        /// <param name="component"></param>
        public override void Apply(Component component)
        {
            base.Apply(component);

            Panel panel = (Panel)component;
            panel.Title = Title;
            panel.Icon = Icon;
            //panel.ContentGroup.ClipAndEnableScrolling = ClipAndEnableScrolling; // TODO ? - no: in skin

            switch (Layout)
            {
                case LayoutEnum.Absolute:
                    panel.Layout = new AbsoluteLayout();
                    break;
                case LayoutEnum.Horizontal:
                    panel.Layout = new HorizontalLayout
                    {
                        Gap = Gap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        PaddingLeft = PaddingLeft,
                        PaddingRight = PaddingRight,
                        PaddingTop = PaddingTop,
                        PaddingBottom = PaddingBottom
                    };
                    break;
                case LayoutEnum.Vertical:
                    panel.Layout = new VerticalLayout
                    {
                        Gap = Gap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        PaddingLeft = PaddingLeft,
                        PaddingRight = PaddingRight,
                        PaddingTop = PaddingTop,
                        PaddingBottom = PaddingBottom
                    };
                    break;
                case LayoutEnum.Tile:
                    panel.Layout = new TileLayout
                    {
                        Orientation = TileOrientation,
                        HorizontalGap = Gap,
                        VerticalGap = Gap,
                        HorizontalAlign = HorizontalAlign,
                        VerticalAlign = VerticalAlign,
                        RowHeight = UseRowHeight ? RowHeight : (float?)null,
                        ColumnWidth = UseColumnWidth ? ColumnWidth : (float?)null,
                        RequestedRowCount = RequestedRowCount,
                        RequestedColumnCount = RequestedColumnCount,
                        RowAlign = RowAlign,
                        ColumnAlign = ColumnAlign
                    };
                    break;
                default:
                    panel.Layout = null;
                    break;
            }
        }
    }
}