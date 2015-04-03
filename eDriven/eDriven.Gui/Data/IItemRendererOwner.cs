using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    ///<summary>
    ///</summary>
    public interface IItemRendererOwner
    {
        ///<summary>
        /// Returns the String for display in an item renderer
        ///</summary>
        ///<param name="item"></param>
        ///<returns></returns>
        string ItemToLabel(object item);

        ///<summary>
        /// Updates the renderer for reuse
        ///</summary>
        ///<param name="renderer"></param>
        ///<param name="itemIndex"></param>
        ///<param name="data"></param>
        void UpdateRenderer(IVisualElement renderer, int itemIndex, object data);
    }
}
