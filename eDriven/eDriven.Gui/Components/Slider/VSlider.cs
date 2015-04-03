using eDriven.Core.Geom;
using eDriven.Gui.Layout;
using eDriven.Gui.Styles;
using UnityEngine;

namespace eDriven.Gui.Components
{
    /// <summary>
    /// Vertical slider
    /// </summary>
    [Style(Name = "skinClass", Default = typeof(VSliderSkin))]
    public class VSlider : SliderBase
    {
        protected override float PointToValue(float x, float y)
        {
            if (null == Thumb || null == Track)
                return 0;

            var range = Maximum - Minimum;
            var thumbRange = LayoutUtil.GetLayoutBoundsHeight(Track) - LayoutUtil.GetLayoutBoundsHeight(Thumb);
            return Minimum + ((thumbRange != 0) ? ((thumbRange - y) / thumbRange) * range : 0); 
        }

        protected override void UpdateSkinDisplayList()
        {
            if (null == Thumb || null == Track)
                return;

            var thumbRange = LayoutUtil.GetLayoutBoundsHeight(Track) - LayoutUtil.GetLayoutBoundsHeight(Thumb);
            var range = Maximum - Minimum;
            
            // calculate new thumb position.
            var thumbPosTrackY = (range > 0) ? thumbRange - ((PendingValue - Minimum) / range) * thumbRange : 0;
            
            // convert to parent's coordinates.
            var thumbPos = Track.LocalToGlobal(new Point(0, thumbPosTrackY));
            var thumbPosParentY = Thumb.Parent.GlobalToLocal(thumbPos).Y;

            Thumb.SetLayoutBoundsPosition(LayoutUtil.GetLayoutBoundsX(Thumb), Mathf.Round(thumbPosParentY));
        }

        protected override void UpdateDataTip(object dataTipInstance, Point initialPosition)
        {
            //var tipAsDisplayObject:DisplayObject = dataTipInstance as DisplayObject;
        
            //if (tipAsDisplayObject && thumb)
            //{
            //    var relX:Number = thumb.getLayoutBoundsX() - 
            //                        (tipAsDisplayObject.width - thumb.getLayoutBoundsWidth()) / 2;
            //    var o:Point = new Point(relX, initialPosition.y);
            //    var r:Point = thumb.parent.localToGlobal(o);     
                
            //    // Get the screen bounds
            //    var screenBounds:Rectangle = systemManager.getVisibleApplicationRect();
            //    // Get the tips bounds. We only care about the dimensions.
            //    var tipBounds:Rectangle = tipAsDisplayObject.getBounds(tipAsDisplayObject.parent);
                
            //        // Make sure the tip doesn't exceed the bounds of the screen
            //        r.x = Math.floor( Math.max(screenBounds.left, 
            //                            Math.min(screenBounds.right - tipBounds.width, r.x)));
            //        r.y = Math.floor( Math.max(screenBounds.top, 
            //                            Math.min(screenBounds.bottom - tipBounds.height, r.y)));
                
            //    r = tipAsDisplayObject.parent.globalToLocal(r);
                
            //    tipAsDisplayObject.x = r.x;
            //    tipAsDisplayObject.y = r.y;
            //}
        }
    }
}
