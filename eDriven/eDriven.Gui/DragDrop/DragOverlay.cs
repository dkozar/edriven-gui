using eDriven.Gui.Shapes;

namespace eDriven.Gui.DragDrop
{
    /// <summary>
    /// Drag proxy
    /// </summary>
    public class DragOverlay : RectShape
    {
        public DragOverlay()
        {
            MouseEnabled = false;
            MouseChildren = false;
            ProcessKeys = false;
            ResizeWithStyleBackground = true;
            //ApplySkin(CoreSkinMapper.Instance.System/*StageManager.Instance.SystemSkin*/);
            //StyleName = "DragOverlay";
            Visible = false; // not visible by default
        }
    }
}