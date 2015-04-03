using eDriven.Gui.Components;

namespace eDriven.Gui.Data
{
    ///<summary>
    /// The item renderer function<br/>
    /// Returns the item renderer factory depending of the input (supplied item)<br/>
    /// Used for lists having variable renderers
    ///</summary>
    ///<param name="item"></param>
    public delegate IFactory ItemRendererFunction(object item);
}