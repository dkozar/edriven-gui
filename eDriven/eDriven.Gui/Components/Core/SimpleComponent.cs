using eDriven.Gui.Util;
using UnityEngine;

namespace eDriven.Gui.Components
{
    public class SimpleComponent : Component
    {
        private bool _expandMinWidthToContent;
        /// <summary>
        /// Do not allow content cropping on X axis
        /// </summary>
        public virtual bool ExpandMinWidthToContent
        {
            get { return _expandMinWidthToContent; }
            set
            {
                if (value != _expandMinWidthToContent)
                {
                    _expandMinWidthToContent = value;
                    InvalidateSize();
                }
            }
        }

        /// <summary>
        /// Measures the component from Style+content<br/>
        /// Sets measured width and height<br/>
        /// This dimensions doesn't need to be respected from the parent, because they are only requested by the component
        /// Note: some Style options like WordWrap produce strange results with sizes
        /// </summary>
        protected override void Measure()
        {
            base.Measure();

            float width = 0;
            float height = 0;
            
            /**
             * There are 2 working modes:
             * 1) The component resizes it's height for all the content to fit
             * 2) The component resizes for both width and height
             * */

            float styleWidth = 0f;
            float styleHeight = 0f;

            float contentWidth = 0f;
            float contentHeight = 0f;

            // measure background
            if (ResizeWithStyleBackground && null != ActiveStyle)
            {
                //Debug.Log("Measuring background style for component: " + this);
                GUIStyleMeasureUtil.MeasureStyle(ActiveStyle, out styleWidth, out styleHeight);
            }

            // measure content
            if (ResizeWithContent && null != ActiveStyle)
            {
                //Debug.Log("Measuring content for component: " + this);
                GUIStyleMeasureUtil.MeasureContent(ActiveStyle, Content, out contentWidth, out contentHeight);
            }

            int pl = (int)(GetStyle("paddingLeft") ?? 0);
            int pr = (int)(GetStyle("paddingRight") ?? 0);
            int pt = (int)(GetStyle("paddingTop") ?? 0);
            int pb = (int)(GetStyle("paddingBottom") ?? 0);

            //Debug.Log("pb: " + pb);

            // get max of background and content + programatic paddings
            //float innerWidth = Mathf.Max(styleWidth, contentWidth + pl + pr);
            //float innerHeight = Mathf.Max(styleHeight, contentHeight + pt + pb);

            width = Mathf.Max(width, styleWidth, contentWidth + pl + pr);
            height = Mathf.Max(height, styleHeight, contentHeight + pt + pb);

            MeasuredMinWidth = MeasuredWidth = width; // messes scrollbars
            MeasuredMinHeight = MeasuredHeight = height;

            //if (Stage != TooltipManagerStage.Instance &&
            //        Stage != InspectorOverlayStage.Instance &&
            //         null != Stage && Stage.Id != "PlayModeOverlayStage")
            //    Debug.LogWarning("[Simple] " + this + " -> MeasuredWidth: " + MeasuredWidth + "; MeasuredHeight: " + MeasuredHeight + "; NestLevel: " + NestLevel + "; Stage: " + Stage);

            //Debug.Log(string.Format("{0} measured: {1}, {2}", this, width, height));
        }
    }
}