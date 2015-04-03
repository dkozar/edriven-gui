using eDriven.Gui.Data;

namespace eDriven.Gui.Components
{
    ///<summary>
    ///</summary>
    public interface ISelectableList : IList
    {
        ///<summary>
        ///</summary>
        int SelectedIndex { get; set; }
    }
}
