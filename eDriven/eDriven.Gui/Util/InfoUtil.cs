using System.Collections.Generic;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using Component = eDriven.Gui.Components;

namespace eDriven.Gui.Util
{
    internal sealed class InfoUtil
    {
        public enum Mode
        {
            Layout, Spacing, Mouse, Data
        }

        public static string GetInfo(Mode mode)
        {
            switch (mode)
            {
                case Mode.Layout:
                    return BuildLayoutInfo();
                case Mode.Spacing:
                    return BuildSpacingInfo();
                case Mode.Mouse:
                    return BuildMouseInfo();
                case Mode.Data:
                    return BuildDataInfo();
            }
            return "";
        }

        private static string BuildLayoutInfo()
        {
            Component.Component component = GuiInspector.Instance.GetCurrentComponent();
            Group container = component as Group;

            string text = string.Empty; // sticky ? "Sticky\n" : string.Empty;

            if (null != component)
            {
                float? x = component.X;
                float? y = component.Y;
                float? ew = component.ExplicitWidth;
                float? pw = component.PercentWidth;
                float? eh = component.ExplicitHeight;
                float? ph = component.PercentHeight;
                float? emw = component.ExplicitMinWidth;
                float? mmw = component.MeasuredMinWidth;
                float? mw = component.MinWidth;
                float? emh = component.ExplicitMinHeight;
                float? mmh = component.MeasuredMinHeight;
                float? mh = component.MinHeight;
                float? mxw = component.MaxWidth;
                float? mxh = component.MaxHeight;

                //string sd = string.Empty;
                //var declarations = ReflectionUtil.GetStyleDeclarations(currentComponent);
                //int count = 0;
                //int totalCount = declarations.Count;
                //declarations.ForEach(delegate (object declaration)
                //                         {
                //                             sd += declaration;
                //                             if (count < totalCount - 1)
                //                                 sd += "; ";
                //                             count++;
                //                         });

                text += string.Format(@"Uid: {0},
StyleName: {30}
X: {1}; Y: {2}
Constrains: Left:{24}; Right:{25}; Top:{26}; Bottom:{27}
Width: Explicit: {4}; Percent: {6}; Measured: {5} 
  --> [Width: {3}]
Height: Explicit: {8}; Percent: {10}; Measured: {9}
  --> [Height: {7}]
MinWidth: Explicit: {16}; Measured: {17}; 
  --> [MinWidth: {18}]
MinHeight: Explicit: {19}; Measured: {20} 
  --> [MinHeight: {21}]
MaxWidth: {28}; MaxHeight: {29}
ActiveStyleName: {11}
Depth: {12}
ScrollContent: {14}
ClipContent: {15}
HotControlId: {13},
Data: {31}", // OS: {11};

                                      // ContentRect: {22},
                                      // RenderingRect: {23}
           
                                      string.IsNullOrEmpty(component.Uid) ? "-" : component.Uid,
                                      x,
                                      y,
                                      component.Width,
                                      null == ew ? "-" : ew.ToString(),
                                      component.MeasuredWidth,
                                      null == pw ? "-" : pw + "%",
                                      component.Height,
                                      null == eh ? "-" : eh.ToString(),
                                      component.MeasuredHeight,
                                      null == ph ? "-" : ph + "%",
                                      component.ActiveStyleName ?? "-",
                                      component.Depth,
                                      component.HotControlId,
                                      null != container, // && container.ClipContent, //ScrollContent,
                                      null != container, // && container.ClipContent,
                                      null == emw ? "-" : emw.ToString(),
                                      mmw,
                                      mw,
                                      null == emh ? "-" : emh.ToString(),
                                      mmh,
                                      mh,
                                      "-", //"null == container ? "-": container.ContentRect.ToString(),
                                      //null == container ? "-": container.RenderingRect.ToString(),
                                      "-", //component.RenderingRect, // 20121212
                                      null == component.Left ? "-" : component.Left.ToString(),
                                      null == component.Right ? "-" : component.Right.ToString(),
                                      null == component.Top ? "-" : component.Top.ToString(),
                                      null == component.Bottom ? "-" : component.Bottom.ToString(),
                                      null == mxw ? "-" : mxw.ToString(), // 30
                                      null == mxh ? "-" : mxh.ToString(), // 31
                                      component.StyleName ?? "-", // 32
                                      component.Data ?? "-" // 33
                    );
            }

            if (null != FocusManager.Instance.FocusedComponent)
            {
                text += string.Format("\nFocusedComponent: {0} [ID:{1}]; ", FocusManager.Instance.FocusedComponent, FocusManager.Instance.FocusedComponentUid);
            }

            return text;
        }

        private static string BuildMouseInfo()
        {
            Component.Component mouseTarget = MouseEventDispatcher.MouseTarget;
            List<Components.Component> mouseWheelTargets = MouseEventDispatcher.MouseWheelTargets;
            Component.Component inspectorTarget = MouseEventDispatcher.InspectorTarget;
            
            string text = string.Empty;
            text += null == mouseTarget ? string.Empty : "Mouse target: " + mouseTarget + "\n";
            text += mouseWheelTargets.Count > 0 ? string.Empty : "Mouse wheel target: " + mouseWheelTargets[0] + "\n";
            text += null == inspectorTarget ? string.Empty : "Inspector target: " + inspectorTarget + "\n";

            bool sticky = GuiInspector.GetSticky();
            text += sticky ? " [sticky]" : string.Empty;

            return text;
        }

        private static string BuildSpacingInfo()
        {
            Component.Component component = GuiInspector.Instance.GetCurrentComponent();
            Group container = component as Group;

            if (null == component)
                return string.Empty;

            string text = "paddingLeft: " + component.GetStyle("paddingLeft") + "\n";
            text += "paddingRight: " + component.GetStyle("paddingRight") + "\n";
            text += "paddingTop: " + component.GetStyle("paddingTop") + "\n";
            text += "paddingBottom: " + component.GetStyle("paddingBottom") + "\n";
//            text += "marginLeft: " + component.GetStyle("marginLeft") + "\n";
//            text += "marginRight: " + component.GetStyle("marginRight") + "\n";
//            text += "marginTop: " + component.GetStyle("marginTop") + "\n";
//            text += "marginBottom: " + component.GetStyle("marginBottom") + "\n";
            text += "Position: " + component.Transform.Position + "\n";
            text += "GlobalPosition: " + component.Transform.GlobalPosition + "\n";
            text += "RenderingPosition: " + component.Transform.RenderingPosition + "\n";
            text += "Bounds:\n" + component.Bounds + "\n";
            text += "GlobalBounds:\n" + component.Transform.GlobalBounds + "\n";
            text += "RenderingRect:\n" + component.RenderingRect + "\n";
            
            if (null != container && container.Layout is VerticalLayout)
            {
                //text += "ContentRect:\n" + container.ContentRect + "\n";

                VerticalLayout bl = container.Layout as VerticalLayout;
                if (null != bl) // TODO: Make layout ToString() on each layout with displaying important data
                {
                    //text += "HorizontalSpacing: " + bl.HorizontalSpacing + "\n";
                    text += "VerticalSpacing: " + bl.Gap;
                }
            }

            return text;
        }

        private static string BuildDataInfo()
        {
            Component.Component component = GuiInspector.Instance.GetCurrentComponent();
            if (null == component)
                return null;

            if (null == component.Data)
            {
                return "- No data -";
            }

            return component.Data.ToString();
        }
    }
}