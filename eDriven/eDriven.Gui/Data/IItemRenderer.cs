using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    ///<summary>
    /// Item renderer interface
    ///</summary>
    public interface IItemRenderer : IDataRenderer, IVisualElement
    {
        ///<summary>
        /// Item index
        ///</summary>
        int ItemIndex { get; set;}
        
        ///<summary>
        ///</summary>
        bool Dragging {get; set;}

        ///<summary>
        /// The String to display in the item renderer
        ///</summary>
        string Text { get; set;}

        ///<summary>
        /// Contains <code>true</code> if the item renderer can show itself as selected.
        ///</summary>
        bool Selected { get; set;}

        ///<summary>
        /// Contains <code>true</code> if the item renderer can show itself as focused. 
        ///</summary>
        bool ShowsCaret { get; set;}
    }
}