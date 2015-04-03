using eDriven.Gui.Styles;

namespace Assets.eDriven.Demo.Gui.Code.ViewportDemo
{
    /// <summary>
    /// Just an override of the MyButton class
    /// </summary>
    [Style(Name = "someMyButton2Style", Type = typeof(float), Default = 0.5f)] // , ProxyMemberName = "RepeatDelay"

    public class MyButton2 : MyButton
    {
    }
}
