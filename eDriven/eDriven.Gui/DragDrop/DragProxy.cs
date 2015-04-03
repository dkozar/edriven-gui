using eDriven.Gui.Components;
using eDriven.Gui.Shapes;

namespace eDriven.Gui.DragDrop
{
    /// <summary>
    /// Drag proxy
    /// </summary>
    public class DragProxy : RectShape
    {
        public DragProxy()
        {
            //Proxify(this);
            //ApplySkin(CoreSkinMapper.Instance.System/*StageManager.Instance.SystemSkin*/);
            //StyleName = "DragProxy";
            Visible = false; // not visible by default
        }

        /// <summary>
        /// Makes the component transparent for mouse clicks, so it could serve as a proxy
        /// </summary>
        /// <param name="component"></param>
        public static void Proxify(Component component)
        {
            component.MouseEnabled = false;
            component.ProcessKeys = false;
            component.ResizeWithStyleBackground = true;
        }
    }
}